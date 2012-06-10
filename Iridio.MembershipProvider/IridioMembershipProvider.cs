using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.Security;
using Iridio.Infrastructure;
using Iridio.Messages.Commands;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Dtos;
using Autofac;
using System.Web.Mvc;

namespace Iridio.MembershipProvider
{
  public class IridioMembershipProvider : System.Web.Security.MembershipProvider
  {
    internal enum UpdateFailures { Password, PasswordAnswer };
    private int newPasswordLength = 8;
    private string eventSource = "IridioMembershipProvider";
    private string eventLog = "Application";
    private string exceptionMessage = "An exception occurred. Please check the Event Log.";
    private IUsersRepository repository;
    private IServiceBus serviceBus;
    private MachineKeySection machineKey;
    public bool WriteExceptionsToEventLog { get; set; }

    public IridioMembershipProvider()
    {
    }

    public IridioMembershipProvider(IServiceBus serviceBus, IUsersRepository repository)
    {
      this.repository = repository;
      this.serviceBus = serviceBus;
    }

    public override void Initialize(string name, NameValueCollection config)
    {
      if (config == null)
        throw new ArgumentNullException("config");
      if (name == null || name.Length == 0)
        name = "IridioMembershipProvider";
      if (String.IsNullOrEmpty(config["description"]))
      {
        config.Remove("description");
        config.Add("description", "Evoluzione Telematica Membership provider");
      }
      base.Initialize(name, config);
      ValidatingPassword += IridioMembershipProvider_ValidatingPassword;
      pApplicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
      pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
      pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
      pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
      pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
      pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
      pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
      pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
      pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
      pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
      WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));
      string temp_format = config["passwordFormat"];
      if (temp_format == null)
        temp_format = "Clear";
      temp_format = temp_format.ToUpper();
      switch (temp_format)
      {
        case "HASHED":
          pPasswordFormat = MembershipPasswordFormat.Hashed;
          break;
        case "ENCRYPTED":
          pPasswordFormat = MembershipPasswordFormat.Encrypted;
          break;
        case "CLEAR":
          pPasswordFormat = MembershipPasswordFormat.Clear;
          break;
        default:
          throw new ProviderException("Formato della password non supportato");
      }
      machineKey = GetMachineKeySection();
      if (machineKey.ValidationKey.Contains("AutoGenerate"))
        if (PasswordFormat != MembershipPasswordFormat.Clear)
          throw new ProviderException("Le password Hashed od Encrypted non sono supportate con chiavi auto generate.");
      SetUserRepository();
    }

    private void SetUserRepository()
    {
      if (repository == null)
      {
        //I know I'm usign the service locator anti-pattern, but I don't see other ways around atm. Suggestions are more than welcome
        repository = DependencyResolver.Current.GetService<IUsersRepository>();
        serviceBus = DependencyResolver.Current.GetService<IServiceBus>();
      }
    }

    private string GetConfigValue(string configValue, string defaultValue)
    {
      if (String.IsNullOrEmpty(configValue))
        return defaultValue;
      return configValue;
    }

    //mi serve nella sezione di test. virtual per ovverride dai test
    protected virtual MachineKeySection GetMachineKeySection()
    {
      System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
      return (MachineKeySection)cfg.GetSection("system.web/machineKey");
    }

    #region Properties
    private string pApplicationName;
    private bool pEnablePasswordReset;
    private bool pEnablePasswordRetrieval;
    private bool pRequiresQuestionAndAnswer;
    private bool pRequiresUniqueEmail;
    private int pMaxInvalidPasswordAttempts;
    private int pPasswordAttemptWindow;
    private int pMinRequiredNonAlphanumericCharacters;
    private int pMinRequiredPasswordLength;
    private string pPasswordStrengthRegularExpression;
    private MembershipPasswordFormat pPasswordFormat;

    public override string ApplicationName
    {
      get { return pApplicationName; }
      set { pApplicationName = value; }
    }

    public override bool EnablePasswordReset
    {
      get { return pEnablePasswordReset; }
    }

    public override bool EnablePasswordRetrieval
    {
      get { return pEnablePasswordRetrieval; }
    }

    public override bool RequiresQuestionAndAnswer
    {
      get { return pRequiresQuestionAndAnswer; }
    }

    public override bool RequiresUniqueEmail
    {
      get { return pRequiresUniqueEmail; }
    }

    public override int MaxInvalidPasswordAttempts
    {
      get { return pMaxInvalidPasswordAttempts; }
    }

    public override int PasswordAttemptWindow
    {
      get { return pPasswordAttemptWindow; }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
      get { return pPasswordFormat; }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
      get { return pMinRequiredNonAlphanumericCharacters; }
    }
    public override int MinRequiredPasswordLength
    {
      get { return pMinRequiredPasswordLength; }
    }
    public override string PasswordStrengthRegularExpression
    {
      get { return pPasswordStrengthRegularExpression; }
    }
    #endregion

    private void IridioMembershipProvider_ValidatingPassword(object sender, ValidatePasswordEventArgs e)
    {
      //Enforce our criteria
      var errorMessage = "";
      if (String.IsNullOrEmpty(e.Password))
      {
        errorMessage += "[Password missing]";
        e.Cancel = true;
        return;
      }
      var pwChar = e.Password.ToCharArray();
      //Check Length
      if (e.Password.Length < MinRequiredPasswordLength)
      {
        errorMessage += "[Minimum length: " + MinRequiredPasswordLength + "]";
        e.Cancel = true;
      }
      //Check Strength
      if (PasswordStrengthRegularExpression != string.Empty)
      {
        Regex r = new Regex(PasswordStrengthRegularExpression);
        if (!r.IsMatch(e.Password))
        {
          errorMessage += "[Insufficient Password Strength]";
          e.Cancel = true;
        }
      }
      //Check Non-alpha characters
      int iNumNonAlpha = 0;
      Regex rAlpha = new Regex(@"\w");
      foreach (char c in pwChar)
      {
        if (!char.IsLetterOrDigit(c)) iNumNonAlpha++;
      }
      if (iNumNonAlpha < MinRequiredNonAlphanumericCharacters)
      {
        errorMessage += "[Insufficient Non-Alpha Characters]";
        e.Cancel = true;
      }
      e.FailureInformation = new MembershipPasswordException(errorMessage);
    }

    public override bool ChangePassword(string username, string oldPwd, string newPwd)
    {
      if (!ValidateUser(username, oldPwd))
        return false;
      var result = ChangePassword(username, newPwd);
      return result;
    }

    public bool ChangePassword(string username, string newPwd)
    {
      ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPwd, true);
      OnValidatingPassword(args);
      if (args.Cancel)
        if (args.FailureInformation != null)
          throw args.FailureInformation;
        else
          throw new MembershipPasswordException("Cambio password annullato a causa di un errore durante la validazione della password");
      bool result = false;
      try
      {
        var user = repository.GetUserByName(username, pApplicationName);
        if (user != null)
        {
          serviceBus.Send(new ChangeUserPassword(user.Id, EncodePassword(newPwd), DateTime.Now));
          result = true;
        }
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "CambioPassword");
          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      return result;
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPwdQuestion, string newPwdAnswer)
    {
      if (!ValidateUser(username, password))
        return false;

      bool result = false;
      try
      {
        var user = repository.GetUserByName(username, pApplicationName);
        if (user != null)
        {
          serviceBus.Send(new ChangeUserPasswordQuestionAndAnswer(user.Id, newPwdQuestion, EncodePassword(newPwdAnswer)));
          result = true;
        }
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "CambioPasswordQuestionAndAnswer");
          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      return result;
    }

    public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
      bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      return this.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, null, out status);
    }

    public IridioMembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
      bool isApproved, object providerUserKey, UserProfile profile, out MembershipCreateStatus status)
    {
      ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
      OnValidatingPassword(args);
      if (args.Cancel)
      {
        status = MembershipCreateStatus.InvalidPassword;
        return null;
      }

      if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
      {
        status = MembershipCreateStatus.DuplicateEmail;
        return null;
      }

      System.Web.Security.MembershipUser user = GetUser(username, false);
      if (user == null)
      {
        DateTime createDate = DateTime.Now;
        try
        {
          var userId = Guid.NewGuid();
          serviceBus.Send(new CreateUser(userId, username, EncodePassword(password), email, isApproved, pApplicationName));
          if ((passwordQuestion != null) && (passwordAnswer != null))
            serviceBus.Send(new ChangeUserPasswordQuestionAndAnswer(userId, passwordQuestion, EncodePassword(passwordAnswer)));
          status = MembershipCreateStatus.Success;
          return (IridioMembershipUser)GetUser(username, false);
        }
        catch (Exception e)
        {
          if (WriteExceptionsToEventLog)
          {
            WriteToEventLog(e, "CreateUser");
          }
          status = MembershipCreateStatus.ProviderError;
        }
      }
      else
      {
        status = MembershipCreateStatus.DuplicateUserName;
      }
      return null;
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      bool result = false;
      try
      {
        var ut = repository.GetUserByName(username, pApplicationName);
        if (ut != null)
        {
          if (deleteAllRelatedData)
          {
            //Process commands to delete all data for the user in the database.
            //Prima, ovviamente, deve lanciare gli eventuali comandi di eliminazione dei figli.
          }
          serviceBus.Send(new DeleteUser(ut.Id));
        }
        result = true;
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "DeleteUser");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      return result;
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      totalRecords = 0;
      MembershipUserCollection users = new MembershipUserCollection();
      try
      {
        var muList = repository.GetUsers(pageIndex, pageSize, pApplicationName, out totalRecords);
        if (totalRecords <= 0)
          return users;
        foreach (var mu in muList)
          users.Add(GetUserFromMembershipUser(mu));
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetAllUsers");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      return users;
    }

    public override int GetNumberOfUsersOnline()
    {
      //This don't work anymore, because I remove the update of LastActivityDate
      TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
      DateTime compareTime = DateTime.Now.Subtract(onlineSpan);
      int numOnline = 0;
      try
      {
        numOnline = repository.GetNumberOfUsersOnline(compareTime, pApplicationName);
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetNumberOfUsersOnline");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      return numOnline;
    }

    public override string GetPassword(string username, string answer)
    {
      if (!EnablePasswordRetrieval)
        throw new ProviderException("Password Retrieval Not Enabled.");
      if (PasswordFormat == MembershipPasswordFormat.Hashed)
        throw new ProviderException("Cannot retrieve Hashed passwords.");
      string password = "";
      string passwordAnswer = "";
      try
      {
        var ut = repository.GetUserByName(username, pApplicationName);
        if (ut != null)
        {
          if (ut.IsLockedOut)
            throw new MembershipPasswordException("The supplied user is locked out.");
          password = ut.Password;
          passwordAnswer = ut.PasswordAnswer;
        }
        else
        {
          throw new MembershipPasswordException("The supplied user name is not found.");
        }
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetPassword");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }

      if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
      {
        UpdateFailureCount(username, UpdateFailures.PasswordAnswer);
        throw new MembershipPasswordException("Incorrect password answer.");
      }

      if (PasswordFormat == MembershipPasswordFormat.Encrypted)
      {
        password = UnEncodePassword(password);
      }
      return password;
    }

    public override System.Web.Security.MembershipUser GetUser(string userName, bool userIsOnline)
    {
      IridioMembershipUser u = null;
      // Don't accept empty user names.
      if (string.IsNullOrEmpty(userName))
        return u;
      //throw new ArgumentNullException("Username");
      try
      {
        var mu = repository.GetUserByName(userName, pApplicationName);
        if (mu != null)
        {
          //Removed LastActivityDate to bypass the continue updates to the server 
          //if (userIsOnline)
          //{
          //  mu.LastActivityDate = DateTime.Now;
          //  repository.Save(mu);
          //}
          u = GetUserFromMembershipUser(mu);
        }
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetUser(String, Boolean)");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      return u;
    }

    public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      IridioMembershipUser u = null;

      // Ensure the provider key is valid.
      if (null == providerUserKey)
        throw (new ArgumentNullException("providerUserKey"));

      try
      {
        var mu = repository.GetBy((Guid)providerUserKey);
        if (mu != null)
        {
          //Removed to bypass the continue updates to the db
          //if (userIsOnline)
          //{
          //mu.LastActivityDate = DateTime.Now;
          //repository.Save(mu);
          //}
          u = GetUserFromMembershipUser(mu);
        }
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetUser(Object, Boolean)");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      return u;
    }

    private IridioMembershipUser GetUserFromMembershipUser(User membershipUser)
    {
      return new IridioMembershipUser(this.Name,
                                  membershipUser.UserName,
                                  membershipUser.Id,
                                  membershipUser.Email,
                                  membershipUser.PasswordQuestion,
                                  membershipUser.Comment,
                                  membershipUser.IsApproved,
                                  membershipUser.IsLockedOut,
                                  membershipUser.CreationDate ?? new DateTime(),
                                  membershipUser.LastLoginDate ?? new DateTime(),
                                  membershipUser.LastActivityDate ?? new DateTime(),
                                  membershipUser.LastPasswordChangedDate ?? new DateTime(),
                                  membershipUser.LastLockedOutDate ?? new DateTime(),
                                  membershipUser.Profile);
    }

    public override bool UnlockUser(string username)
    {
      try
      {
        var ut = repository.GetUserByName(username, pApplicationName);
        if (ut != null)
        {
          serviceBus.Send(new UnlockUser(ut.Id, DateTime.Now));
          return true;
        }
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "UnlockUser");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw new MemberAccessException(e.Message);
        }
      }
      return false;
    }

    public override string GetUserNameByEmail(string email)
    {
      string username = "";
      // Don't accept empty emails.
      if (string.IsNullOrEmpty(email))
        throw new ArgumentNullException("email");
      try
      {
        username = repository.GetUserNameByEmail(email, pApplicationName);
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetUserNameByEmail");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      if (username == null)
        username = "";
      return username;
    }

    public override string ResetPassword(string username, string answer)
    {
      if (!EnablePasswordReset)
        throw new NotSupportedException("Password reset is not enabled.");
      if (answer == null && RequiresQuestionAndAnswer)
      {
        UpdateFailureCount(username, UpdateFailures.PasswordAnswer);
        throw new ProviderException("Password answer required for password reset.");
      }
      string newPassword = Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);
      ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);
      OnValidatingPassword(args);
      if (args.Cancel)
        if (args.FailureInformation != null)
          throw args.FailureInformation;
        else
          throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
      bool result = false;
      string passwordAnswer = "";
      try
      {
        var ut = repository.GetUserByName(username, pApplicationName);
        if (ut != null)
        {
          if (ut.IsLockedOut)
            throw new MembershipPasswordException("The supplied user is locked out.");
          passwordAnswer = ut.PasswordAnswer;
          if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
          {
            UpdateFailureCount(username, UpdateFailures.PasswordAnswer);
            throw new MembershipPasswordException("Incorrect password answer.");
          }
          serviceBus.Send(new ResetUserPassword(ut.Id, EncodePassword(newPassword), DateTime.Now));
          result = true;
        }
        else
          throw new MembershipPasswordException("The supplied user name is not found.");
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "ResetPassword");
          throw new MembershipPasswordException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      if (result)
        return newPassword;
      else
        throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
    }

    public override void UpdateUser(System.Web.Security.MembershipUser user)
    {
      IridioMembershipUser u = (IridioMembershipUser)user;
      try
      {
        //mu.Profile = u.Profile;
        serviceBus.Send(new UpdateUser((Guid)u.ProviderUserKey, u.Email, u.IsApproved, u.Comment));
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "UpdateUser");
          throw new MemberAccessException(exceptionMessage);
        }
        else
          throw e;
      }
    }

    public override bool ValidateUser(string username, string password)
    {
      bool result = false;
      try
      {
        var ut = repository.GetUserByName(username, pApplicationName);
        if (ut != null)
        {
          if (!ut.IsLockedOut)
          {
            if (CheckPassword(password, ut.Password))
            {
              if (ut.IsApproved)
              {
                result = true;
                serviceBus.Send(new UpdateUserLastLoginDate(ut.Id, DateTime.Now));
              }
            }
            else
              UpdateFailureCount(username, UpdateFailures.Password);
          }
        }
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "ValidateUser");
          throw new MemberAccessException(exceptionMessage);
        }
        else
          throw e;
      }
      return result;
    }

    internal void UpdateFailureCount(string username, UpdateFailures failureType)
    {
      DateTime windowStart = new DateTime();
      int failureCount = 0;
      try
      {
        var ut = repository.GetUserByName(username, pApplicationName);
        if (ut != null)
        {
          if (failureType == UpdateFailures.Password)
          {
            failureCount = ut.FailedPasswordAttemptCount;
            windowStart = ut.FailedPasswordAttemptWindowStart ?? DateTime.Now;
          }
          if (failureType == UpdateFailures.PasswordAnswer)
          {
            failureCount = ut.FailedPasswordAnswerAttemptCount;
            windowStart = ut.FailedPasswordAnswerAttemptWindowStart ?? DateTime.Now;
          }
          DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);
          if (failureCount == 0 || DateTime.Now > windowEnd)
          {
            // First password failure or outside of PasswordAttemptWindow. 
            // Start a new password failure count from 1 and a new window starting now.
            if (failureType == UpdateFailures.Password)
              serviceBus.Send(new SetUserFailedPasswordAttempt(ut.Id, 1, DateTime.Now));
            if (failureType == UpdateFailures.PasswordAnswer)
              serviceBus.Send(new SetUserFailedPasswordAnswerAttempt(ut.Id, 1, DateTime.Now));
          }
          else
          {
            if (failureCount++ >= MaxInvalidPasswordAttempts)
            {
              // Password attempts have exceeded the failure threshold. Lock out the user.
              serviceBus.Send(new LockUser(ut.Id, DateTime.Now));
            }
            else
            {
              // Password attempts have not exceeded the failure threshold. 
              // Update the failure counts. Leave the window the same.
              if (failureType == UpdateFailures.Password)
                serviceBus.Send(new SetUserFailedPasswordAttempt(ut.Id, failureCount, windowStart));
              if (failureType == UpdateFailures.PasswordAnswer)
                serviceBus.Send(new SetUserFailedPasswordAnswerAttempt(ut.Id, failureCount, windowStart));
            }
          }
        }
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "UpdateFailureCount");
          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
    }

    public bool CheckPassword(string password, string dbpassword)
    {
      string pass1 = password;
      string pass2 = dbpassword;

      switch (PasswordFormat)
      {
        case MembershipPasswordFormat.Encrypted:
          pass2 = UnEncodePassword(dbpassword);
          break;
        case MembershipPasswordFormat.Hashed:
          pass1 = EncodePassword(password);
          break;
        default:
          break;
      }
      return pass1 == pass2;
    }

    private string EncodePassword(string password)
    {
      string encodedPassword = password;
      switch (PasswordFormat)
      {
        case MembershipPasswordFormat.Clear:
          break;
        case MembershipPasswordFormat.Encrypted:
          encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
          break;
        case MembershipPasswordFormat.Hashed:
          HMACSHA1 hash = new HMACSHA1();
          hash.Key = HexToByte(machineKey.ValidationKey);
          encodedPassword =
          Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
          break;
        default:
          throw new ProviderException("Unsupported password format.");
      }
      return encodedPassword;
    }

    private string UnEncodePassword(string encodedPassword)
    {
      string password = encodedPassword;
      switch (PasswordFormat)
      {
        case MembershipPasswordFormat.Clear:
          break;
        case MembershipPasswordFormat.Encrypted:
          password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
          break;
        case MembershipPasswordFormat.Hashed:
          throw new ProviderException("Cannot unencode a hashed password.");
        default:
          throw new ProviderException("Unsupported password format.");
      }
      return password;
    }

    // HexToByte
    // Converts a hexadecimal string to a byte array. Used to convert encryption
    // key values from the configuration.
    private byte[] HexToByte(string hexString)
    {
      byte[] returnBytes = new byte[hexString.Length / 2];
      for (int i = 0; i < returnBytes.Length; i++)
        returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
      return returnBytes;
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      totalRecords = 0;
      MembershipUserCollection users = new MembershipUserCollection();
      try
      {
        var muList = repository.FindUsersByName(usernameToMatch, pageIndex, pageSize, pApplicationName);
        totalRecords = muList.Count;
        foreach (var mu in muList)
          users.Add(GetUserFromMembershipUser(mu));
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "FindUsersByName");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      return users;
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      MembershipUserCollection users = new MembershipUserCollection();
      totalRecords = 0;
      try
      {
        var muList = repository.FindUsersByEmail(emailToMatch, pageIndex, pageSize, pApplicationName);
        totalRecords = muList.Count;
        foreach (var mu in muList)
          users.Add(GetUserFromMembershipUser(mu));
      }
      catch (Exception e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "FindUsersByEmail");
          throw new MemberAccessException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      return users;
    }

    private void WriteToEventLog(Exception e, string action)
    {
      EventLog log = new EventLog();
      log.Source = eventSource;
      log.Log = eventLog;
      string message = "An exception occurred communicating with the data source.\n\nAction: " + action + "\n\nException: " + e.ToString();
      log.WriteEntry(message);
    }
  }
}
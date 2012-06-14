using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using Iridio.MembershipProvider;
using Iridio.ReadModel.Abstracts;

namespace Iridio.SampleWebSite.Models
{
  #region Models
  [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessageResourceType = typeof(Resources.WebSite.ViewModels), ErrorMessageResourceName = "PasswordsMustMatch")]
  public class MembershipUserViewModel
  {
    public int Id { get; set; }
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [Display(Name = "UserName", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string UserName { get; set; }
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string Email { get; set; }
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [ValidatePasswordLength]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string Password { get; set; }
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string ConfirmPassword { get; set; }
    [Display(Name = "IsLockedOut", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public bool IsLockedOut { get; set; }
    public SelectList Languages { get; set; }
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [Display(Name = "Language", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public int LanguageId { get; set; }
  }

  [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessageResourceType = typeof(Resources.WebSite.ViewModels), ErrorMessageResourceName = "PasswordsMustMatch")]
  public class ChangePasswordModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [DataType(DataType.Password)]
    [Display(Name = "OldPassword", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string OldPassword { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [ValidatePasswordLength]
    [DataType(DataType.Password)]
    [Display(Name = "NewPassword", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string NewPassword { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string ConfirmPassword { get; set; }
  }

  public class ResetPasswordModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [Display(Name = "UserName", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string UserName { get; set; }
    public string NewPassword { get { return Membership.GeneratePassword(8, 2); } }
  }

  public class LogOnModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [Display(Name = "UserName", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string UserName { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string Password { get; set; }

    [Display(Name = "RememberMe", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public bool RememberMe { get; set; }
  }

  [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessageResourceType = typeof(Resources.WebSite.ViewModels), ErrorMessageResourceName = "PasswordsMustMatch")]
  public class RegisterModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [Display(Name = "UserName", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string UserName { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string Email { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [ValidatePasswordLength]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string Password { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public string ConfirmPassword { get; set; }
  }

  public class ProfileModel
  {
    public string CognomeNome { get; private set; }
    public string Email { get; private set; }
    public string Ruolo { get; private set; }
    public SelectList Languages { get; set; }
    [Required(ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "Required")]
    [Display(Name = "Language", ResourceType = typeof(Resources.WebSite.ViewModels))]
    public int LanguageId { get; set; }
  }

  #endregion

  #region Services
  // The FormsAuthentication type is sealed and contains static members, so it is difficult to
  // unit test code that calls its members. The interface and helper class below demonstrate
  // how to create an abstract wrapper around such a type in order to make the AccountController
  // code unit testable.

  public interface IMembershipService
  {
    int MinPasswordLength { get; }
    bool ValidateUser(string userName, string password);
    MembershipCreateStatus CreateUser(string userName, string password, string email);
    bool ChangePassword(string userName, string oldPassword, string newPassword);
    bool ResetPassword(string userName, string newPassword);
  }

  public class AccountMembershipService : IMembershipService
  {
    private readonly IUsersService usersService;
    private readonly System.Web.Security.MembershipProvider _provider;

    public AccountMembershipService(IUsersService usersService)
      : this(null, usersService)
    {
    }

    public AccountMembershipService(System.Web.Security.MembershipProvider provider, IUsersService usersService)
    {
      this.usersService = usersService;
      _provider = provider ?? Membership.Provider;
    }

    public int MinPasswordLength
    {
      get
      {
        return _provider.MinRequiredPasswordLength;
      }
    }

    public bool ValidateUser(string userName, string password)
    {
      if (String.IsNullOrEmpty(userName)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "userName");
      if (String.IsNullOrEmpty(password)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "password");

      return _provider.ValidateUser(userName, password);
    }

    public MembershipCreateStatus CreateUser(string userName, string password, string email)
    {
      if (String.IsNullOrEmpty(userName)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "userName");
      if (String.IsNullOrEmpty(password)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "password");
      if (String.IsNullOrEmpty(email)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "email");

      MembershipCreateStatus status;
      _provider.CreateUser(userName, password, email, null, null, true, null, out status);
      if (status == MembershipCreateStatus.Success)
      {
        var user = usersService.GetUserByName(userName, _provider.ApplicationName);
        if (user == null) throw new ArgumentException(Resources.WebSite.Controllers.UserNullAfterSave);
		
		//TODO: Refactor and move this logic in the EventHandler
        usersService.SendRegisterCongratulationsToEmail(user);
      }
      return status;
    }

    public bool ChangePassword(string userName, string oldPassword, string newPassword)
    {
      if (String.IsNullOrEmpty(userName)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "userName");
      if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "oldPassword");
      if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "newPassword");

      // The underlying ChangePassword() will throw an exception rather than return false in certain failure scenarios.
      try
      {
        System.Web.Security.MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
        return currentUser.ChangePassword(oldPassword, newPassword);
      }
      catch (ArgumentException)
      {
        return false;
      }
      catch (MembershipPasswordException)
      {
        return false;
      }
    }

    public bool ResetPassword(string userName, string newPassword)
    {
      if (String.IsNullOrEmpty(userName)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "userName");
      if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "newPassword");
      try
      {
        System.Web.Security.MembershipUser currentUser = _provider.GetUser(userName, false); //non mi serve l'aggiornamento dell'attività online
        if (currentUser == null)
          return false;
        string oldPassword = currentUser.GetPassword();
		//TODO: Refactor and move the sendmail logic to the EventHandler
        if (currentUser.ChangePassword(oldPassword, newPassword))
          return usersService.SendResetPasswordToEmail(currentUser.Email, newPassword);
        else
          return false;
      }
      catch (ArgumentException)
      {
        return false;
      }
      catch (MembershipPasswordException)
      {
        return false;
      }
    }
  }

  public interface IFormsAuthenticationService
  {
    void SignIn(string userName, bool createPersistentCookie);
    void SignOut();
  }

  public class FormsAuthenticationService : IFormsAuthenticationService
  {
    public void SignIn(string userName, bool createPersistentCookie)
    {
      if (String.IsNullOrEmpty(userName)) throw new ArgumentException(Resources.WebSite.Controllers.ValueCannotBeNull, "userName");
      FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
    }

    public void SignOut()
    {
      FormsAuthentication.SignOut();
    }
  }
  #endregion

  #region Validation
  public static class AccountValidation
  {
    public static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://go.microsoft.com/fwlink/?LinkID=177550 for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return Resources.WebSite.Controllers.DuplicateUserName;

        case MembershipCreateStatus.DuplicateEmail:
          return Resources.WebSite.Controllers.DuplicateEmail; ;

        case MembershipCreateStatus.InvalidPassword:
          return Resources.WebSite.Controllers.InvalidPassword;

        case MembershipCreateStatus.InvalidEmail:
          return Resources.WebSite.Controllers.InvalidEmail;

        case MembershipCreateStatus.InvalidAnswer:
          return Resources.WebSite.Controllers.InvalidAnswer;

        case MembershipCreateStatus.InvalidQuestion:
          return Resources.WebSite.Controllers.InvalidQuestion;

        case MembershipCreateStatus.InvalidUserName:
          return Resources.WebSite.Controllers.InvalidUserName;

        case MembershipCreateStatus.ProviderError:
          return Resources.WebSite.Controllers.ProviderError;

        case MembershipCreateStatus.UserRejected:
          return Resources.WebSite.Controllers.UserRejected;

        default:
          return Resources.WebSite.Controllers.UnknownError;
      }
    }
  }

  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class PropertiesMustMatchAttribute : ValidationAttribute
  {
    private static string _defaultErrorMessage = Resources.WebSite.ViewModels.PropertiesMustMatch;
    private readonly object _typeId = new object();

    public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
      : base(_defaultErrorMessage)
    {
      OriginalProperty = originalProperty;
      ConfirmProperty = confirmProperty;
    }

    public string ConfirmProperty { get; private set; }
    public string OriginalProperty { get; private set; }

    public override object TypeId
    {
      get
      {
        return _typeId;
      }
    }

    public override string FormatErrorMessage(string name)
    {
      return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString, OriginalProperty, ConfirmProperty);
    }

    public override bool IsValid(object value)
    {
      PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
      object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
      object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
      return Object.Equals(originalValue, confirmValue);
    }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
  {
    private static string _defaultErrorMessage = Resources.WebSite.ViewModels.ValidatePasswordLength;
    private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

    public ValidatePasswordLengthAttribute()
      : base(_defaultErrorMessage)
    {
    }

    public override string FormatErrorMessage(string name)
    {
      return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, _minCharacters);
    }

    public override bool IsValid(object value)
    {
      string valueAsString = value as string;
      return (valueAsString != null && valueAsString.Length >= _minCharacters);
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      return new[] { new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minCharacters, int.MaxValue) };
    }
  }
  #endregion
}

using System;
using CommonDomain.Core;
using Iridio.Messages.Events;

namespace Iridio.DomainModel.Entities
{
  public class User : AggregateBase
  {
    public string UserName { get; private set; }
    public string ApplicationName { get; private set; }
    public string Email { get; private set; }
    public string Comment { get; private set; }
    public string Password { get; private set; }
    public string PasswordQuestion { get; private set; }
    public string PasswordAnswer { get; private set; }
    public bool IsApproved { get; private set; }
    public bool IsOnline { get; private set; }
    public bool IsLockedOut { get; private set; }
    public DateTime? LastActivityDate { get; private set; }
    public DateTime? LastLoginDate { get; private set; }
    public DateTime? LastPasswordChangedDate { get; private set; }
    public DateTime? CreationDate { get; private set; }
    public DateTime? LastLockedOutDate { get; private set; }
    public DateTime? FailedPasswordAttemptWindowStart { get; private set; }
    public DateTime? FailedPasswordAnswerAttemptWindowStart { get; private set; }
    public DateTime? PrevLoginDate { get; private set; }
    public int FailedPasswordAttemptCount { get; private set; }
    public int FailedPasswordAnswerAttemptCount { get; private set; }
    public UserProfile Profile { get; private set; }

    #region ChangePassword
    public void ChangePassword(string newPassword, DateTime? lastPasswordChangedDate)
    {
      RaiseEvent(new UserPasswordChanged(Id, newPassword, lastPasswordChangedDate));
    }

    private void Apply(UserPasswordChanged @event)
    {
      //TODO: I can omit this piece of code?. If we need to do some business validation while reconstructing the events I need to ste all the values
      Password = @event.NewPassword;
      LastPasswordChangedDate = @event.LastPasswordChangedDate;
    }
    #endregion

    #region ChangePasswordQuestionAndAnswer
    public void ChangePasswordQuestionAndAnswer(string newPasswordQuestion, string newPasswordAnswer)
    {
      RaiseEvent(new UserPasswordQuestionAndAnswerChanged(Id, newPasswordQuestion, newPasswordAnswer));
    }

    private void Apply(UserPasswordQuestionAndAnswerChanged @event)
    {
      PasswordQuestion = @event.NewPasswordQuestion;
      PasswordAnswer = @event.NewPasswordAnswer;
    }
    #endregion

    #region UpdateLastLoginDate
    public void UpdateLastLoginDate(DateTime? lastLoginDate)
    {
      RaiseEvent(new UserLastLoginDateUpdated(Id, lastLoginDate));
    }

    private void Apply(UserLastLoginDateUpdated @event)
    {
      LastLoginDate = @event.LastLoginDate;
    }
    #endregion

    #region ctor/CreateUser
    public User()
    {
      Profile = new UserProfile();
      //Roles = new List<MembershipRole>();
    }

    public User(Guid userId, string userName, string password, string email, bool isApproved, string applicationName,
      DateTime? creationDate, DateTime? lastPasswordChangedDate, DateTime? lastActivityDate, bool isLockedOut, DateTime? lastLockedOutDate,
      int failedPasswordAttemptCount, DateTime? failedPasswordAttemptWindowStart, int failedPasswordAnswerAttemptCount,
      DateTime? failedPasswordAnswerAttemptWindowStart)
    {
      RaiseEvent(new UserCreated(userId, userName, password, email, isApproved, applicationName, creationDate, lastPasswordChangedDate, lastActivityDate, isLockedOut, lastLockedOutDate,
        failedPasswordAttemptCount, failedPasswordAttemptWindowStart, failedPasswordAnswerAttemptCount, failedPasswordAnswerAttemptWindowStart));
    }

    private void Apply(UserCreated @event)
    {
      Id = @event.AggregateId;
      UserName = @event.UserName;
      Password = @event.Password;
      Email = @event.Email;
      IsApproved = @event.IsApproved;
      CreationDate = @event.CreationDate;
      LastPasswordChangedDate = @event.LastPasswordChangedDate;
      LastActivityDate = @event.LastActivityDate;
      ApplicationName = @event.ApplicationName;
      IsLockedOut = @event.IsLockedOut;
      LastLockedOutDate = @event.LastLockedOutDate;
      FailedPasswordAttemptCount = @event.FailedPasswordAttemptCount;
      FailedPasswordAttemptWindowStart = @event.FailedPasswordAttemptWindowStart;
      FailedPasswordAnswerAttemptCount = @event.FailedPasswordAnswerAttemptCount;
      FailedPasswordAnswerAttemptWindowStart = @event.FailedPasswordAnswerAttemptWindowStart;
      Profile = new UserProfile();
    }
    #endregion

    #region DeleteUser
    public void Delete()
    {
      RaiseEvent(new UserDeleted(Id));
    }

    private void Apply(UserDeleted @event)
    {
      //Do nothing
    }
    #endregion

    #region ResetPassword
    public void ResetPassword(string newPassword, DateTime? lastPasswordChangedDate)
    {
      RaiseEvent(new UserPasswordResetted(Id, newPassword, lastPasswordChangedDate));
    }

    private void Apply(UserPasswordResetted @event)
    {
      Password = @event.NewPassword;
      LastPasswordChangedDate = @event.LastPasswordChangedDate;
    }
    #endregion

    #region Lock/Unlock User
    public void UnlockUser(DateTime? lastLockedOutDate)
    {
      RaiseEvent(new UserUnlocked(Id, lastLockedOutDate));
    }

    private void Apply(UserUnlocked @event)
    {
      IsLockedOut = @event.IsLockedOut;
      LastLockedOutDate = @event.LastLockedOutDate;
    }

    public void LockUser(DateTime? lastLockedOutDate)
    {
      RaiseEvent(new UserLocked(Id, lastLockedOutDate));
    }

    private void Apply(UserLocked @event)
    {
      IsLockedOut = @event.IsLockedOut;
      LastLockedOutDate = @event.LastLockedOutDate;
    }
    #endregion

    #region UpdateUser
    public void UpdateUser(string email, bool isApproved, string comment)
    {
      RaiseEvent(new UserUpdated(Id, email, isApproved, comment));
    }

    private void Apply(UserUpdated @event)
    {
      Email = @event.Email;
      Comment = @event.Comment;
      IsApproved = @event.IsApproved;
    }
    #endregion

    #region SetFailedPasswordAttempt
    public void SetFailedPasswordAttempt(int failedPasswordAttemptCount, DateTime? failedPasswordAttemptWindowStart)
    {
      RaiseEvent(new UserFailedPasswordAttemptSetted(Id, failedPasswordAttemptCount, failedPasswordAttemptWindowStart));
    }

    private void Apply(UserFailedPasswordAttemptSetted @event)
    {
      FailedPasswordAttemptCount = @event.FailedPasswordAttemptCount;
      FailedPasswordAttemptWindowStart = @event.FailedPasswordAttemptWindowStart;
    }
    #endregion

    #region SetFailedPasswordAnswerAttempt
    public void SetFailedPasswordAnswerAttempt(int failedPasswordAnswerAttemptCount, DateTime? failedPasswordAnswerAttemptWindowStart)
    {
      RaiseEvent(new UserFailedPasswordAnswerAttemptSetted(Id, failedPasswordAnswerAttemptCount, failedPasswordAnswerAttemptWindowStart));
    }

    private void Apply(UserFailedPasswordAnswerAttemptSetted @event)
    {
      FailedPasswordAnswerAttemptCount = @event.FailedPasswordAnswerAttemptCount;
      FailedPasswordAnswerAttemptWindowStart = @event.FailedPasswordAnswerAttemptWindowStart;
    }
    #endregion

    #region Roles
    //TODO: To implemet the RoleProvider
    //public  ICollection<UserRole> Roles { get; private set; }
    #endregion
  }
}
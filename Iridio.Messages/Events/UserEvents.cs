using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridio.Messages.Events
{
  public class UserPasswordChanged : Event
  {
    public DateTime? LastPasswordChangedDate { get; private set; }
    public string NewPassword { get; private set; }

    public UserPasswordChanged(Guid id, string newPassword, DateTime? lastPasswordChangedDate)
    {
      AggregateId = id;
      LastPasswordChangedDate = lastPasswordChangedDate;
      NewPassword = newPassword;
    }
  }

  public class UserPasswordQuestionAndAnswerChanged : Event
  {
    public string NewPasswordQuestion { get; private set; }
    public string NewPasswordAnswer { get; private set; }

    public UserPasswordQuestionAndAnswerChanged(Guid id, string newPasswordQuestion, string newPasswordAnswer)
    {
      AggregateId = id;
      NewPasswordQuestion = newPasswordQuestion;
      NewPasswordAnswer = newPasswordAnswer;
    }
  }

  public class UserLastLoginDateUpdated : Event
  {
    public DateTime? LastLoginDate { get; private set; }

    public UserLastLoginDateUpdated(Guid id, DateTime? lastLoginDate)
    {
      AggregateId = id;
      LastLoginDate = lastLoginDate;
    }
  }

  public class UserCreated : Event
  {
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public string Email { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? CreationDate { get; private set; }
    public DateTime? LastPasswordChangedDate { get; private set; }
    public DateTime? LastActivityDate { get; private set; }
    public string ApplicationName { get; private set; }
    public bool IsLockedOut { get; private set; }
    public DateTime? LastLockedOutDate { get; private set; }
    public int FailedPasswordAttemptCount { get; private set; }
    public DateTime? FailedPasswordAttemptWindowStart { get; private set; }
    public int FailedPasswordAnswerAttemptCount { get; private set; }
    public DateTime? FailedPasswordAnswerAttemptWindowStart { get; private set; }

    public UserCreated(Guid id, string userName, string password, string email, bool isApproved, string applicationName,
      DateTime? creationDate, DateTime? lastPasswordChangedDate, DateTime? lastActivityDate, bool isLockedOut, DateTime? lastLockedOutDate,
      int failedPasswordAttemptCount, DateTime? failedPasswordAttemptWindowStart, int failedPasswordAnswerAttemptCount,
      DateTime? failedPasswordAnswerAttemptWindowStart)
    {
      AggregateId = id;
      UserName = userName;
      Password = password;
      Email = email;
      IsApproved = isApproved;
      CreationDate = creationDate;
      LastPasswordChangedDate = lastPasswordChangedDate;
      LastActivityDate = lastActivityDate;
      ApplicationName = applicationName;
      IsLockedOut = isLockedOut;
      LastLockedOutDate = lastLockedOutDate;
      FailedPasswordAttemptCount = failedPasswordAttemptCount;
      FailedPasswordAttemptWindowStart = failedPasswordAttemptWindowStart;
      FailedPasswordAnswerAttemptCount = failedPasswordAnswerAttemptCount;
      FailedPasswordAnswerAttemptWindowStart = failedPasswordAnswerAttemptWindowStart;
    }
  }

  public class UserDeleted : Event
  {
    public UserDeleted(Guid id)
    {
      AggregateId = id;
    }
  }

  public class UserPasswordResetted : Event
  {
    public string NewPassword { get; set; }
    public DateTime? LastPasswordChangedDate { get; set; }

    public UserPasswordResetted(Guid id, string newPassword, DateTime? lastPasswordChangedDate)
    {
      AggregateId = id;
      NewPassword = newPassword;
      LastPasswordChangedDate = lastPasswordChangedDate;
    }
  }

  public class UserUnlocked : Event
  {
    public DateTime? LastLockedOutDate { get; set; }
    public bool IsLockedOut { get; set; }

    public UserUnlocked(Guid id, DateTime? lastLockedOutDate)
    {
      AggregateId = id;
      IsLockedOut = false;
      LastLockedOutDate = lastLockedOutDate;
    }
  }

  public class UserUpdated : Event
  {
    public string Email { get; set; }
    public string Comment { get; set; }
    public bool IsApproved { get; set; }

    public UserUpdated(Guid id, string email, bool isApproved, string comment)
    {
      AggregateId = id;
      Email = email;
      IsApproved = isApproved;
      Comment = comment;
    }
  }

  public class UserFailedPasswordAttemptSetted : Event
  {
    public int FailedPasswordAttemptCount { get; set; }
    public DateTime? FailedPasswordAttemptWindowStart { get; set; }

    public UserFailedPasswordAttemptSetted(Guid id, int failedPasswordAttemptCount, DateTime? failedPasswordAttemptWindowStart)
    {
      AggregateId = id;
      FailedPasswordAttemptCount = failedPasswordAttemptCount;
      FailedPasswordAttemptWindowStart = failedPasswordAttemptWindowStart;
    }
  }

  public class UserFailedPasswordAnswerAttemptSetted : Event
  {
    public int FailedPasswordAnswerAttemptCount { get; set; }
    public DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }

    public UserFailedPasswordAnswerAttemptSetted(Guid id, int failedPasswordAnswerAttemptCount, DateTime? failedPasswordAnswerAttemptWindowStart)
    {
      AggregateId = id;
      FailedPasswordAnswerAttemptCount = failedPasswordAnswerAttemptCount;
      FailedPasswordAnswerAttemptWindowStart = failedPasswordAnswerAttemptWindowStart;
    }
  }

  public class UserLocked : Event
  {
    public DateTime? LastLockedOutDate { get; set; }
    public bool IsLockedOut { get; set; }

    public UserLocked(Guid id, DateTime? lastLockedOutDate)
    {
      AggregateId = id;
      IsLockedOut = true;
      LastLockedOutDate = lastLockedOutDate;
    }
  }
}

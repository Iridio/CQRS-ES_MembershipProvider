using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridio.Messages.Commands
{
  public class ChangeUserPassword : Command
  {
    public DateTime? LastPasswordChangedDate { get; private set; }
    public string NewPassword { get; private set; }

    public ChangeUserPassword(Guid userId, string newPassword, DateTime? lastPasswordChangedDate)
    {
      AggregateId = userId;
      LastPasswordChangedDate = lastPasswordChangedDate;
      NewPassword = newPassword;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public ChangeUserPassword() { }
  }

  public class ChangeUserPasswordQuestionAndAnswer : Command
  {
    public string NewPasswordQuestion { get; private set; }
    public string NewPasswordAnswer { get; private set; }

    public ChangeUserPasswordQuestionAndAnswer(Guid userId, string newPasswordQuestion, string newPasswordAnswer)
    {
      AggregateId = userId;
      NewPasswordQuestion = newPasswordQuestion;
      NewPasswordAnswer = newPasswordAnswer;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public ChangeUserPasswordQuestionAndAnswer() { }
  }

  public class UpdateUserLastLoginDate : Command
  {
    public DateTime? LastLoginDate { get; private set; }

    public UpdateUserLastLoginDate(Guid userId, DateTime? lastLoginDate)
    {
      AggregateId = userId;
      LastLoginDate = lastLoginDate;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public UpdateUserLastLoginDate() { }
  }

  public class CreateUser : Command
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

    public CreateUser(Guid userId, string userName, string password, string email, bool isApproved, string applicationName)
    {
      DateTime createDate = DateTime.Now;
      AggregateId = userId;
      UserName = userName;
      Password = password;
      Email = email;
      IsApproved = isApproved;
      CreationDate = createDate;
      LastPasswordChangedDate = createDate;
      LastActivityDate = createDate;
      ApplicationName = applicationName;
      IsLockedOut = false;
      LastLockedOutDate = createDate;
      FailedPasswordAttemptCount = 0;
      FailedPasswordAttemptWindowStart = createDate;
      FailedPasswordAnswerAttemptCount = 0;
      FailedPasswordAnswerAttemptWindowStart = createDate;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public CreateUser() { }
  }

  public class DeleteUser : Command
  {
    public DeleteUser(Guid userId)
    {
      AggregateId = userId;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public DeleteUser() { }
  }

  public class ResetUserPassword : Command
  {
    public string NewPassword { get; set; }
    public DateTime? LastPasswordChangedDate { get; set; }

    public ResetUserPassword(Guid userId, string newPassword, DateTime? lastPasswordChangedDate)
    {
      AggregateId = userId;
      NewPassword = newPassword;
      LastPasswordChangedDate = lastPasswordChangedDate;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public ResetUserPassword() { }
  }

  public class UnlockUser : Command
  {
    public DateTime? LastLockedOutDate { get; set; }
    public bool IsLockedOut { get; set; }

    public UnlockUser(Guid userId, DateTime? lastLockedOutDate)
    {
      AggregateId = userId;
      IsLockedOut = false;
      LastLockedOutDate = lastLockedOutDate;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public UnlockUser() { }
  }

  public class UpdateUser : Command
  {
    public string Email { get; set; }
    public string Comment { get; set; }
    public bool IsApproved { get; set; }

    public UpdateUser(Guid userId, string email, bool isApproved, string comment)
    {
      AggregateId = userId;
      Email = email;
      IsApproved = isApproved;
      Comment = comment;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public UpdateUser() { }
  }

  public class SetUserFailedPasswordAttempt : Command
  {
    public int FailedPasswordAttemptCount { get; set; }
    public DateTime? FailedPasswordAttemptWindowStart { get; set; }

    public SetUserFailedPasswordAttempt(Guid userId, int failedPasswordAttemptCount, DateTime? failedPasswordAttemptWindowStart)
    {
      AggregateId = userId;
      FailedPasswordAttemptCount = failedPasswordAttemptCount;
      FailedPasswordAttemptWindowStart = failedPasswordAttemptWindowStart;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public SetUserFailedPasswordAttempt() { }
  }

  public class SetUserFailedPasswordAnswerAttempt : Command
  {
    public int FailedPasswordAnswerAttemptCount { get; set; }
    public DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }

    public SetUserFailedPasswordAnswerAttempt(Guid userId, int failedPasswordAnswerAttemptCount, DateTime? failedPasswordAnswerAttemptWindowStart)
    {
      AggregateId = userId;
      FailedPasswordAnswerAttemptCount = failedPasswordAnswerAttemptCount;
      FailedPasswordAnswerAttemptWindowStart = failedPasswordAnswerAttemptWindowStart;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public SetUserFailedPasswordAnswerAttempt() { }
  }

  public class LockUser : Command
  {
    public DateTime? LastLockedOutDate { get; set; }
    public bool IsLockedOut { get; set; }

    public LockUser(Guid userId, DateTime? lastLockedOutDate)
    {
      AggregateId = userId;
      IsLockedOut = true;
      LastLockedOutDate = lastLockedOutDate;
    }

    [Obsolete("Needed for serialization. Do not use.")]
    public LockUser() { }
  }
}

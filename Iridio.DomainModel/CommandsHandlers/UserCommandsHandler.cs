using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Iridio.Messages.Commands;
using Iridio.DomainModel.Entities;

namespace Iridio.DomainModel.CommandHandlers
{
  public class UserCommandsHandler : CommandsHandler, IHandleCommand<ChangeUserPassword>, IHandleCommand<ChangeUserPasswordQuestionAndAnswer>, IHandleCommand<UpdateUserLastLoginDate>,
    IHandleCommand<CreateUser>, IHandleCommand<DeleteUser>, IHandleCommand<ResetUserPassword>, IHandleCommand<UnlockUser>, IHandleCommand<UpdateUser>, IHandleCommand<SetUserFailedPasswordAttempt>,
    IHandleCommand<SetUserFailedPasswordAnswerAttempt>, IHandleCommand<LockUser>
  {
    public UserCommandsHandler(Func<IRepository> repository)
      : base(repository)
    {
    }

    public void Handle(ChangeUserPassword command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.ChangePassword(command.NewPassword, command.LastPasswordChangedDate);
      repo.Save(user, Guid.NewGuid());
    }

    public void Handle(ChangeUserPasswordQuestionAndAnswer command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.ChangePasswordQuestionAndAnswer(command.NewPasswordQuestion, command.NewPasswordAnswer);
      repo.Save(user, Guid.NewGuid());
    }

    public void Handle(UpdateUserLastLoginDate command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.UpdateLastLoginDate(command.LastLoginDate);
      repo.Save(user, Guid.NewGuid());
    }

    public void Handle(CreateUser command)
    {
      var user = new User(command.AggregateId, command.UserName, command.Password, command.Email, command.IsApproved, command.ApplicationName, command.CreationDate, command.LastPasswordChangedDate,
        command.LastActivityDate, command.IsLockedOut, command.LastLockedOutDate, command.FailedPasswordAttemptCount, command.FailedPasswordAttemptWindowStart, command.FailedPasswordAnswerAttemptCount,
        command.FailedPasswordAnswerAttemptWindowStart);
      repository().Save(user, Guid.NewGuid());
    }

    public void Handle(DeleteUser command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.Delete();
      repo.Save(user, Guid.NewGuid());
    }

    public void Handle(ResetUserPassword command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.ResetPassword(command.NewPassword, command.LastPasswordChangedDate);
      repo.Save(user, Guid.NewGuid());
    }

    public void Handle(UnlockUser command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.UnlockUser(command.LastLockedOutDate);
      repo.Save(user, Guid.NewGuid());
    }

    public void Handle(UpdateUser command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.UpdateUser(command.Email, command.IsApproved, command.Comment);
      repo.Save(user, Guid.NewGuid());
    }

    public void Handle(SetUserFailedPasswordAttempt command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.SetFailedPasswordAttempt(command.FailedPasswordAttemptCount, command.FailedPasswordAttemptWindowStart);
      repo.Save(user, Guid.NewGuid());
    }

    public void Handle(SetUserFailedPasswordAnswerAttempt command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.SetFailedPasswordAnswerAttempt(command.FailedPasswordAnswerAttemptCount, command.FailedPasswordAnswerAttemptWindowStart);
      repo.Save(user, Guid.NewGuid());
    }

    public void Handle(LockUser command)
    {
      var repo = repository();
      var user = Get<User>(command.AggregateId, repo);
      user.LockUser(command.LastLockedOutDate);
      repo.Save(user, Guid.NewGuid());
    }
  }
}

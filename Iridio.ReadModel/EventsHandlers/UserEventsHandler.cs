using System;
using Iridio.Messages.Events;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Dtos;

namespace Iridio.ReadModel.EventsHandlers
{
  public class UserEventsHandler : EventsHandler, IHandleEvent<UserPasswordChanged>, IHandleEvent<UserPasswordQuestionAndAnswerChanged>, IHandleEvent<UserLastLoginDateUpdated>,
    IHandleEvent<UserCreated>, IHandleEvent<UserDeleted>, IHandleEvent<UserPasswordResetted>, IHandleEvent<UserUnlocked>, IHandleEvent<UserUpdated>, IHandleEvent<UserFailedPasswordAttemptSetted>,
    IHandleEvent<UserFailedPasswordAnswerAttemptSetted>, IHandleEvent<UserLocked>
  {
    public UserEventsHandler(Func<IPersistor> persistor)
      : base(persistor)
    {
    }

    public void Handle(UserPasswordChanged @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
      {
        user.Password = @event.NewPassword;
        user.LastPasswordChangedDate = @event.LastPasswordChangedDate;
        persi.Update(user);
      }
    }

    public void Handle(UserPasswordQuestionAndAnswerChanged @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
      {
        user.PasswordQuestion = @event.NewPasswordQuestion;
        user.PasswordAnswer = @event.NewPasswordAnswer;
        persi.Update(user);
      }
    }

    public void Handle(UserLastLoginDateUpdated @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
      {
        user.LastLoginDate = @event.LastLoginDate;
        persi.Update(user);
      }
    }

    public void Handle(UserCreated @event)
    {
      var user = new User()
      {
        Id = @event.AggregateId,
        UserName = @event.UserName,
        Password = @event.Password,
        Email = @event.Email,
        IsApproved = @event.IsApproved,
        CreationDate = @event.CreationDate,
        LastPasswordChangedDate = @event.LastPasswordChangedDate,
        LastActivityDate = @event.LastActivityDate,
        ApplicationName = @event.ApplicationName,
        IsLockedOut = @event.IsLockedOut,
        LastLockedOutDate = @event.LastLockedOutDate,
        FailedPasswordAttemptCount = @event.FailedPasswordAttemptCount,
        FailedPasswordAttemptWindowStart = @event.FailedPasswordAttemptWindowStart,
        FailedPasswordAnswerAttemptCount = @event.FailedPasswordAnswerAttemptCount,
        FailedPasswordAnswerAttemptWindowStart = @event.FailedPasswordAnswerAttemptWindowStart
      };
      persistor().Create(user);
    }

    public void Handle(UserDeleted @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
        persi.Delete(user);
    }

    public void Handle(UserPasswordResetted @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
      {
        user.Password = @event.NewPassword;
        user.LastPasswordChangedDate = @event.LastPasswordChangedDate;
        persi.Update(user);
      }
    }

    public void Handle(UserUnlocked @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
      {
        user.IsLockedOut = false;
        user.LastLockedOutDate = @event.LastLockedOutDate;
        persi.Update(user);
      }
    }

    public void Handle(UserUpdated @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
      {
        user.IsApproved = @event.IsApproved;
        user.Email = @event.Email;
        user.Comment = @event.Comment;
        persi.Update(user);
      }
    }

    public void Handle(UserFailedPasswordAttemptSetted @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
      {
        user.FailedPasswordAttemptCount = @event.FailedPasswordAttemptCount;
        user.FailedPasswordAttemptWindowStart = @event.FailedPasswordAttemptWindowStart;
        persi.Update(user);
      }
    }

    public void Handle(UserFailedPasswordAnswerAttemptSetted @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
      {
        user.FailedPasswordAnswerAttemptCount = @event.FailedPasswordAnswerAttemptCount;
        user.FailedPasswordAnswerAttemptWindowStart = @event.FailedPasswordAnswerAttemptWindowStart;
        persi.Update(user);
      }
    }

    public void Handle(UserLocked @event)
    {
      var persi = persistor();
      var user = persi.GetById<User>(@event.AggregateId);
      if (user != null)
      {
        user.IsLockedOut = true;
        user.LastLockedOutDate = @event.LastLockedOutDate;
        persi.Update(user);
      }
    }
  }
}

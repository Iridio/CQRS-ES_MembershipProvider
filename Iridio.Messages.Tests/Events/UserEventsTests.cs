using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Iridio.Messages.Events;

namespace Iridio.Messages.Tests.Events
{
  [TestFixture]
  public class UserEventsTests
  {
    [Test]
    public void ChageUserPassword()
    {
      var guid = Guid.NewGuid();
      var date = DateTime.Now;
      var @event = new UserPasswordChanged(guid, "abc", date);
      Assert.AreEqual(guid, @event.AggregateId);
      Assert.AreEqual("abc", @event.NewPassword);
      Assert.AreEqual(date, @event.LastPasswordChangedDate);
    }

    [Test]
    public void ChageUserPasswordQuestionAndAnswer()
    {
      var guid = Guid.NewGuid();
      var @event = new UserPasswordQuestionAndAnswerChanged(guid, "abc", "123");
      Assert.AreEqual(guid, @event.AggregateId);
      Assert.AreEqual("abc", @event.NewPasswordQuestion);
      Assert.AreEqual("123", @event.NewPasswordAnswer);
    }

    [Test]
    public void UpdateUserLastLoginDate()
    {
      var guid = Guid.NewGuid();
      var date = DateTime.Now;
      var @event = new UserLastLoginDateUpdated(guid, date);
      Assert.AreEqual(guid, @event.AggregateId);
      Assert.AreEqual(date, @event.LastLoginDate);
    }

    [Test]
    public void UserCreated()
    {
      var userId = Guid.NewGuid();
      var createDate = DateTime.Now;
      var @event = new UserCreated(userId, "username", "password", "email", true, "pApplicationName", createDate, createDate, createDate, false,
        createDate, 2, createDate, 4, createDate);
      Assert.AreEqual(userId, @event.AggregateId);
      Assert.AreEqual("username", @event.UserName);
      Assert.AreEqual("password", @event.Password);
      Assert.AreEqual("email", @event.Email);
      Assert.AreEqual(true, @event.IsApproved);
      Assert.AreEqual(createDate, @event.CreationDate);
      Assert.AreEqual(createDate, @event.LastPasswordChangedDate);
      Assert.AreEqual(createDate, @event.LastActivityDate);
      Assert.AreEqual("pApplicationName", @event.ApplicationName);
      Assert.AreEqual(false, @event.IsLockedOut);
      Assert.AreEqual(createDate, @event.LastLockedOutDate);
      Assert.AreEqual(2, @event.FailedPasswordAttemptCount);
      Assert.AreEqual(createDate, @event.FailedPasswordAttemptWindowStart);
      Assert.AreEqual(4, @event.FailedPasswordAnswerAttemptCount);
      Assert.AreEqual(createDate, @event.FailedPasswordAnswerAttemptWindowStart);
    }

    [Test]
    public void UserDeleted()
    {
      var userId = Guid.NewGuid();
      var @event = new UserDeleted(userId);
      Assert.AreEqual(userId, @event.AggregateId);
    }

    [Test]
    public void UserPasswordResetted()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var @event = new UserPasswordResetted(userId, "123abc", resetDate);
      Assert.AreEqual(userId, @event.AggregateId);
      Assert.AreEqual("123abc", @event.NewPassword);
      Assert.AreEqual(resetDate, @event.LastPasswordChangedDate);
    }

    [Test]
    public void UserUnlocked()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var @event = new UserUnlocked(userId, resetDate);
      Assert.AreEqual(userId, @event.AggregateId);
      Assert.IsFalse(@event.IsLockedOut);
      Assert.AreEqual(resetDate, @event.LastLockedOutDate);
    }

    [Test]
    public void UserUpdated()
    {
      var userId = Guid.NewGuid();
      var @event = new UserUpdated(userId, "email", true, "comment");
      Assert.AreEqual(userId, @event.AggregateId);
      Assert.AreEqual("email", @event.Email);
      Assert.IsTrue(@event.IsApproved);
      Assert.AreEqual("comment", @event.Comment);
    }

    [Test]
    public void UserFailedPasswordAttemptSetted()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var @event = new UserFailedPasswordAttemptSetted(userId, 10, resetDate);
      Assert.AreEqual(userId, @event.AggregateId);
      Assert.AreEqual(10, @event.FailedPasswordAttemptCount);
      Assert.AreEqual(resetDate, @event.FailedPasswordAttemptWindowStart);
    }

    [Test]
    public void UserFailedPasswordAnswerAttemptSetted()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var @event = new UserFailedPasswordAnswerAttemptSetted(userId, 10, resetDate);
      Assert.AreEqual(userId, @event.AggregateId);
      Assert.AreEqual(10, @event.FailedPasswordAnswerAttemptCount);
      Assert.AreEqual(resetDate, @event.FailedPasswordAnswerAttemptWindowStart);
    }

    [Test]
    public void UserLocked()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var @event = new UserLocked(userId, resetDate);
      Assert.AreEqual(userId, @event.AggregateId);
      Assert.IsTrue(@event.IsLockedOut);
      Assert.AreEqual(resetDate, @event.LastLockedOutDate);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Iridio.Messages.Commands;

namespace Iridio.Messages.Tests.Commands
{
  [TestFixture]
  public class UserCommandsTests
  {
    [Test]
    public void ChageUserPassword()
    {
      var guid = Guid.NewGuid();
      var date = DateTime.Now;
      var command = new ChangeUserPassword(guid, "abc", date);
      Assert.AreEqual(guid, command.AggregateId);
      Assert.AreEqual("abc", command.NewPassword);
      Assert.AreEqual(date, command.LastPasswordChangedDate);
    }

    [Test]
    public void ChageUserPasswordQuestionAndAnswer()
    {
      var guid = Guid.NewGuid();
      var command = new ChangeUserPasswordQuestionAndAnswer(guid, "abc", "123");
      Assert.AreEqual(guid, command.AggregateId);
      Assert.AreEqual("abc", command.NewPasswordQuestion);
      Assert.AreEqual("123", command.NewPasswordAnswer);
    }

    [Test]
    public void UpdateUserLastLoginDate()
    {
      var guid = Guid.NewGuid();
      var date = DateTime.Now;
      var command = new UpdateUserLastLoginDate(guid, date);
      Assert.AreEqual(guid, command.AggregateId);
      Assert.AreEqual(date, command.LastLoginDate);
    }

    [Test]
    public void CreateUser()
    {
      var userId = Guid.NewGuid();
      var command = new CreateUser(userId, "username", "password", "email", true, "pApplicationName");
      Assert.AreEqual(userId, command.AggregateId);
      Assert.AreEqual("username", command.UserName);
      Assert.AreEqual("password", command.Password);
      Assert.AreEqual("email", command.Email);
      Assert.AreEqual(true, command.IsApproved);
      Assert.Less(DateTime.MinValue, command.CreationDate);
      Assert.AreEqual(command.CreationDate, command.LastPasswordChangedDate);
      Assert.AreEqual(command.CreationDate, command.LastActivityDate);
      Assert.AreEqual("pApplicationName", command.ApplicationName);
      Assert.AreEqual(false, command.IsLockedOut);
      Assert.AreEqual(command.CreationDate, command.LastLockedOutDate);
      Assert.AreEqual(0, command.FailedPasswordAttemptCount);
      Assert.AreEqual(command.CreationDate, command.FailedPasswordAttemptWindowStart);
      Assert.AreEqual(0, command.FailedPasswordAnswerAttemptCount);
      Assert.AreEqual(command.CreationDate, command.FailedPasswordAnswerAttemptWindowStart);
    }

    [Test]
    public void DeleteUser()
    {
      var userId = Guid.NewGuid();
      var command = new DeleteUser(userId);
      Assert.AreEqual(userId, command.AggregateId);
    }

    [Test]
    public void ResetUserPassword()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var command = new ResetUserPassword(userId, "123abc", resetDate);
      Assert.AreEqual(userId, command.AggregateId);
      Assert.AreEqual("123abc", command.NewPassword);
      Assert.AreEqual(resetDate, command.LastPasswordChangedDate);
    }

    [Test]
    public void UnlockUser()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var command = new UnlockUser(userId, resetDate);
      Assert.AreEqual(userId, command.AggregateId);
      Assert.IsFalse(command.IsLockedOut);
      Assert.AreEqual(resetDate, command.LastLockedOutDate);
    }

    [Test]
    public void UpdateUser()
    {
      var userId = Guid.NewGuid();
      var command = new UpdateUser(userId, "email", true, "comment");
      Assert.AreEqual(userId, command.AggregateId);
      Assert.AreEqual("email", command.Email);
      Assert.IsTrue(command.IsApproved);
      Assert.AreEqual("comment", command.Comment);
    }

    [Test]
    public void SetUserFailedPasswordAttempt()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var command = new SetUserFailedPasswordAttempt(userId, 10, resetDate);
      Assert.AreEqual(userId, command.AggregateId);
      Assert.AreEqual(10, command.FailedPasswordAttemptCount);
      Assert.AreEqual(resetDate, command.FailedPasswordAttemptWindowStart);
    }

    [Test]
    public void SetUserFailedPasswordAnswerAttempt()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var command = new SetUserFailedPasswordAnswerAttempt(userId, 10, resetDate);
      Assert.AreEqual(userId, command.AggregateId);
      Assert.AreEqual(10, command.FailedPasswordAnswerAttemptCount);
      Assert.AreEqual(resetDate, command.FailedPasswordAnswerAttemptWindowStart);
    }

    [Test]
    public void LockUser()
    {
      var userId = Guid.NewGuid();
      var resetDate = DateTime.Now;
      var command = new LockUser(userId, resetDate);
      Assert.AreEqual(userId, command.AggregateId);
      Assert.IsTrue(command.IsLockedOut);
      Assert.AreEqual(resetDate, command.LastLockedOutDate);
    }

  }
}

using Iridio.DomainModel.Entities;
using NUnit.Framework;
using System;

namespace Iridio.DomainModel.Tests.Entities
{
  [TestFixture]
  public class UserTests
  {
    [Test]
    public void Create_Initialize_Profile()
    {
      User user = new User();
      Assert.IsNotNull(user.Profile);
    }

    [Test]
    public void ChangePassword_ChangeUserPassword()
    {
      var user = new User();
      var date = DateTime.Now;
      user.ChangePassword("abc", date);
      Assert.AreEqual(date, user.LastPasswordChangedDate);
      Assert.AreEqual("abc", user.Password);
    }

    [Test]
    public void ChangePasswordQuestionAndAnswer_ChangeQuestionAndAnswer()
    {
      var user = new User();
      var date = DateTime.Now;
      user.ChangePasswordQuestionAndAnswer("abc", "123");
      Assert.IsNull(user.Password);
      Assert.AreEqual("abc", user.PasswordQuestion);
      Assert.AreEqual("123", user.PasswordAnswer);
    }

    [Test]
    public void UpdateLastLoginDate_ChangeLastLoginDate()
    {
      var user = new User();
      var date = DateTime.Now;
      user.UpdateLastLoginDate(date);
      Assert.AreEqual(date, user.LastLoginDate);
    }

    [Test]
    public void CreateUser()
    {
      var userId = Guid.NewGuid();
      var createDate = DateTime.Now;
      User user = new User(userId, "username", "password", "email", true, "pApplicationName", createDate, createDate, createDate, false,
        createDate, 2, createDate, 4, createDate);
      Assert.IsNotNull(user.Profile);
      Assert.AreEqual(userId, user.Id);
      Assert.AreEqual("username", user.UserName);
      Assert.AreEqual("password", user.Password);
      Assert.AreEqual("email", user.Email);
      Assert.AreEqual(true, user.IsApproved);
      Assert.AreEqual(createDate, user.CreationDate);
      Assert.AreEqual(createDate, user.LastPasswordChangedDate);
      Assert.AreEqual(createDate, user.LastActivityDate);
      Assert.AreEqual("pApplicationName", user.ApplicationName);
      Assert.AreEqual(false, user.IsLockedOut);
      Assert.AreEqual(createDate, user.LastLockedOutDate);
      Assert.AreEqual(2, user.FailedPasswordAttemptCount);
      Assert.AreEqual(createDate, user.FailedPasswordAttemptWindowStart);
      Assert.AreEqual(4, user.FailedPasswordAnswerAttemptCount);
      Assert.AreEqual(createDate, user.FailedPasswordAnswerAttemptWindowStart);
    }

    [Test]
    public void DeleteUser()
    {
      var user = new User();
      user.Delete();
      Assert.Pass("Il codice lo esegue IHandle<UserDeleted>. Qui non fa nulla");
    }

    [Test]
    public void ResetPassword()
    {
      var user = new User();
      var resetDate = DateTime.Now;
      user.ResetPassword("123abc", resetDate);
      Assert.AreEqual("123abc", user.Password);
      Assert.AreEqual(resetDate, user.LastPasswordChangedDate);
    }

    [Test]
    public void UnlockUser()
    {
      var user = new User(Guid.NewGuid(), "2", "2", "", true, "", null, null, null, true, null, 0, null, 0, null);
      var resetDate = DateTime.Now;
      user.UnlockUser(resetDate);
      Assert.IsFalse(user.IsLockedOut);
      Assert.AreEqual(resetDate, user.LastLockedOutDate);
    }

    [Test]
    public void UpdateUser()
    {
      var user = new User(Guid.NewGuid(), "2", "2", "", true, "", null, null, null, true, null, 0, null, 0, null);
      user.UpdateUser("email", false, "comment");
      Assert.IsFalse(user.IsApproved);
      Assert.AreEqual("email", user.Email);
      Assert.AreEqual("comment", user.Comment);
    }

    [Test]
    public void SetFailedPasswordAttempt()
    {
      var resetDate = DateTime.Now;
      var user = new User(Guid.NewGuid(), "2", "2", "", true, "", null, null, null, true, null, 0, null, 0, null);
      user.SetFailedPasswordAttempt(20, resetDate);
      Assert.AreEqual(20, user.FailedPasswordAttemptCount);
      Assert.AreEqual(resetDate, user.FailedPasswordAttemptWindowStart);
    }

    [Test]
    public void SetFailedPasswordAnswerAttempt()
    {
      var resetDate = DateTime.Now;
      var user = new User(Guid.NewGuid(), "2", "2", "", true, "", null, null, null, true, null, 0, null, 0, null);
      user.SetFailedPasswordAnswerAttempt(20, resetDate);
      Assert.AreEqual(20, user.FailedPasswordAnswerAttemptCount);
      Assert.AreEqual(resetDate, user.FailedPasswordAnswerAttemptWindowStart);
    }

    [Test]
    public void LockUser()
    {
      var user = new User(Guid.NewGuid(), "2", "2", "", true, "", null, null, null, false, null, 0, null, 0, null);
      var resetDate = DateTime.Now;
      user.LockUser(resetDate);
      Assert.IsTrue(user.IsLockedOut);
      Assert.AreEqual(resetDate, user.LastLockedOutDate);
    }
  }
}

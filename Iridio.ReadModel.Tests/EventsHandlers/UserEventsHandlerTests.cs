using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Iridio.ReadModel.Abstracts;
using Moq;
using Iridio.ReadModel.Dtos;
using Iridio.ReadModel.EventsHandlers;
using Iridio.Messages.Events;

namespace Iridio.ReadModel.Tests.EventsHandlers
{
  [TestFixture]
  public class UserEventsHandlerTests
  {
    private Mock<IPersistor> persistor;
    private User user;
    private Guid guid = Guid.NewGuid();

    [SetUp]
    public void Setup()
    {
      user = new User() { Id = guid, UserName = "user1", Password = "password" };
      persistor = new Mock<IPersistor>();
      persistor.Setup(x => x.GetById<User>(guid)).Returns(user);
    }

    public UserEventsHandler GetHandler()
    {
      return new UserEventsHandler(() => persistor.Object);
    }

    [Test]
    public void UserPasswordChanged()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      handler.Handle(new UserPasswordChanged(guid, "123abc", dateRef));
      persistor.Verify(x => x.Update(It.Is<User>(u => u.Id == guid && u.Password == "123abc" && u.LastPasswordChangedDate == dateRef)), Times.Once());
    }

    [Test]
    public void UserPasswordQuestionAndAnswerChanged()
    {
      var handler = GetHandler();
      handler.Handle(new UserPasswordQuestionAndAnswerChanged(guid, "question", "answer"));
      persistor.Verify(x => x.Update(It.Is<User>(u => u.Id == guid && u.PasswordQuestion == "question" && u.PasswordAnswer == "answer")), Times.Once());
    }

    [Test]
    public void UserLastLoginDateUpdated()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      handler.Handle(new UserLastLoginDateUpdated(guid, dateRef));
      persistor.Verify(x => x.Update(It.Is<User>(u => u.Id == guid && u.LastLoginDate == dateRef)), Times.Once());
    }

    [Test]
    public void UserCreated()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      handler.Handle(new UserCreated(guid, "newUsername", "newPassword", "newEmail", true, "TestApp", dateRef, dateRef.AddDays(1), dateRef.AddDays(2), false, dateRef.AddDays(3), 1, dateRef.AddDays(4), 2, dateRef.AddDays(5)));
      persistor.Verify(x => x.Create(It.Is<User>(u => u.Id == guid && u.UserName == "newUsername" && u.Password == "newPassword" && u.Email == "newEmail" && u.IsApproved
        && u.ApplicationName == "TestApp" && u.CreationDate == dateRef && u.LastPasswordChangedDate == dateRef.AddDays(1) && u.LastActivityDate == dateRef.AddDays(2) && !u.IsLockedOut
        && u.LastLockedOutDate == dateRef.AddDays(3) && u.FailedPasswordAttemptCount == 1 && u.FailedPasswordAttemptWindowStart == dateRef.AddDays(4) && u.FailedPasswordAnswerAttemptCount == 2
        && u.FailedPasswordAnswerAttemptWindowStart == dateRef.AddDays(5))), Times.Once());
    }

    [Test]
    public void UserDeleted()
    {
      var handler = GetHandler();
      handler.Handle(new UserDeleted(guid));
      persistor.Verify(x => x.Delete(It.Is<User>(u => u.Id == guid)), Times.Once());
    }

    [Test]
    public void UserPasswordResetted()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      handler.Handle(new UserPasswordResetted(guid, "newPassword", dateRef));
      persistor.Verify(x => x.Update(It.Is<User>(u => u.Id == guid && u.Password == "newPassword" && u.LastPasswordChangedDate == dateRef)), Times.Once());
    }

    [Test]
    public void UserUnlocked()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      handler.Handle(new UserUnlocked(guid, dateRef));
      persistor.Verify(x => x.Update(It.Is<User>(u => u.Id == guid && !u.IsLockedOut && u.LastLockedOutDate == dateRef)), Times.Once());
    }

    [Test]
    public void UserUpdated()
    {
      var handler = GetHandler();
      handler.Handle(new UserUpdated(guid, "email", true, "comment"));
      persistor.Verify(x => x.Update(It.Is<User>(u => u.Id == guid && u.Email == "email" && u.IsApproved && u.Comment == "comment")), Times.Once());
    }

    [Test]
    public void UserFailedPasswordAttemptSetted()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      handler.Handle(new UserFailedPasswordAttemptSetted(guid, 2, dateRef));
      persistor.Verify(x => x.Update(It.Is<User>(u => u.Id == guid && u.FailedPasswordAttemptCount == 2 && u.FailedPasswordAttemptWindowStart == dateRef)), Times.Once());
    }

    [Test]
    public void UserFailedPasswordAnswerAttemptSetted()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      handler.Handle(new UserFailedPasswordAnswerAttemptSetted(guid, 3, dateRef));
      persistor.Verify(x => x.Update(It.Is<User>(u => u.Id == guid && u.FailedPasswordAnswerAttemptCount == 3 && u.FailedPasswordAnswerAttemptWindowStart == dateRef)), Times.Once());
    }
    [Test]
    public void UserLocked()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      handler.Handle(new UserLocked(guid, dateRef));
      persistor.Verify(x => x.Update(It.Is<User>(u => u.Id == guid && u.IsLockedOut && u.LastLockedOutDate == dateRef)), Times.Once());
    }

  }
}

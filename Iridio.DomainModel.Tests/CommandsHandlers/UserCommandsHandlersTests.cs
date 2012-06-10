using System;
using System.Collections.Generic;
using CommonDomain.Persistence;
using Iridio.DomainModel.CommandHandlers;
using Iridio.DomainModel.Entities;
using Moq;
using NUnit.Framework;
using Iridio.Messages.Commands;

namespace Iridio.DomainModel.Tests.CommandsHandlers
{
  [TestFixture]
  public class UserCommandsHandlersTests
  {
    private Mock<IRepository> repository;
    private User user;
    private Guid guid = Guid.NewGuid();

    [SetUp]
    public void Setup()
    {
      user = new User(guid, "user1", "123", "", true, "TestApp", null, null, null, false, null, 0, null, 0, null);
      repository = new Mock<IRepository>();
      repository.Setup(x => x.GetById<User>(guid)).Returns(user);
      repository.Setup(x => x.Save(It.IsAny<User>(), It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()));
    }

    public UserCommandsHandler GetHandler()
    {
      return new UserCommandsHandler(() => repository.Object);
    }

    //Devo testare le proprietà dell'utente cambiate in quanto non riesco a creare dei mock per la classe utente (non ci sono virtual nei metodi e nelle proprietà)
    [Test]
    public void HandleChangeUserPassword()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      var command = new ChangeUserPassword(guid, "newpwd", dateRef);
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      Assert.AreEqual("newpwd", user.Password);
      Assert.AreEqual(dateRef, user.LastPasswordChangedDate);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleChangeUserPasswordAnswerAndQuestion()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      var command = new ChangeUserPasswordQuestionAndAnswer(guid, "newQuestion", "newAnswer");
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      Assert.AreEqual("newQuestion", user.PasswordQuestion);
      Assert.AreEqual("newAnswer", user.PasswordAnswer);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleUpdateLastLoginDate()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      var command = new UpdateUserLastLoginDate(guid, dateRef);
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      Assert.AreEqual(dateRef, user.LastLoginDate);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleCreateUser()
    {
      var handler = GetHandler();
      var newGuid = Guid.NewGuid();
      var command = new CreateUser(newGuid, "user2", "123abc", "email@domain.com", true, "TestApp"); //devo usare la 
      handler.Handle(command);
      repository.Verify(x => x.Save(It.Is<User>(u => u.Id == newGuid && u.UserName == "user2" && u.Password == "123abc"), It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleDeleteUser()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      var command = new DeleteUser(guid);
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleResetUserPassword()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      var command = new ResetUserPassword(guid, "newpwd", dateRef);
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      Assert.AreEqual("newpwd", user.Password);
      Assert.AreEqual(dateRef, user.LastPasswordChangedDate);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleUnlockUser()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      var command = new UnlockUser(guid, dateRef);
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      Assert.IsFalse(user.IsLockedOut);
      Assert.AreEqual(dateRef, user.LastLockedOutDate);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleUpdateUser()
    {
      var handler = GetHandler();
      var command = new UpdateUser(guid, "email2", true, "comment");
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      Assert.AreEqual("email2", user.Email);
      Assert.IsTrue(user.IsApproved);
      Assert.AreEqual("comment", user.Comment);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleSetUserFailedPasswordAttempt()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      var command = new SetUserFailedPasswordAttempt(guid, 3, dateRef);
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      Assert.AreEqual(3, user.FailedPasswordAttemptCount);
      Assert.AreEqual(dateRef, user.FailedPasswordAttemptWindowStart);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleSetUserFailedPasswordAnswerAttempt()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      var command = new SetUserFailedPasswordAnswerAttempt(guid, 5, dateRef);
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      Assert.AreEqual(5, user.FailedPasswordAnswerAttemptCount);
      Assert.AreEqual(dateRef, user.FailedPasswordAnswerAttemptWindowStart);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }

    [Test]
    public void HandleLockUser()
    {
      var handler = GetHandler();
      var dateRef = DateTime.Now;
      var command = new LockUser(guid, dateRef);
      handler.Handle(command);
      Assert.AreEqual(guid, user.Id);
      Assert.AreEqual(dateRef, user.LastLockedOutDate);
      Assert.IsTrue(user.IsLockedOut);
      repository.Verify(x => x.Save(user, It.IsAny<Guid>(), It.IsAny<Action<IDictionary<string, object>>>()), Times.Once());
    }
  }
}

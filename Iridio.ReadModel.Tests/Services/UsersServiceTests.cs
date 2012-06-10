using System;
using Iridio.ReadModel.Abstracts;
using Moq;
using NUnit.Framework;
using Iridio.ReadModel.Dtos;

namespace Iridio.ReadModel.Services.Tests
{
  [TestFixture]
  public class UsersServiceTests
  {
    private Mock<IEmailService> emailService;
    private Mock<IUsersRepository> mockUsersRepository;
    private Guid goodId = Guid.NewGuid();
    private Guid badId = Guid.NewGuid();

    [SetUp]
    public void SetUp()
    {
      emailService = new Mock<IEmailService>();
      mockUsersRepository = new Mock<IUsersRepository>();
      mockUsersRepository.Setup(x => x.GetUserByName("GoodUser", It.IsAny<string>())).Returns(new User());
      mockUsersRepository.Setup(x => x.GetUserByName("BadUser", It.IsAny<string>())).Returns((User)null);
      mockUsersRepository.Setup(x => x.GetBy(goodId)).Returns(new User() { Id = goodId });
      mockUsersRepository.Setup(x => x.GetBy(badId)).Returns((User)null);
    }

    [Test]
    public void SendCongratulationsEmail_ShouldSendEmail()
    {
      var user = FakeUser();
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      emailService.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(true);
      var ret = service.SendRegisterCongratulationsToEmail(user);
      emailService.Verify(x => x.Send("usermail@domain.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
      Assert.IsTrue(ret);
    }

    [Test]
    public void SendCongratulationsEmail_ShouldReturnFalse_IfAProblem()
    {
      var user = FakeUser();
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      emailService.Setup(x => x.Send(user.Email, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(false);
      var ret = service.SendRegisterCongratulationsToEmail(user);
      emailService.Verify(x => x.Send("usermail@domain.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
      Assert.IsFalse(ret);
    }

    [Test]
    public void GetUserByName_returnUser()
    {
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      var ret = service.GetUserByName("GoodUser", "appTest");
      mockUsersRepository.Verify(x => x.GetUserByName("GoodUser", It.IsAny<string>()), Times.Once());
      Assert.IsNotNull(ret);
    }

    [Test]
    public void GetUserByName_ReturnNullOnWrongUserName()
    {
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      var ret = service.GetUserByName("BadUser", "appTest");
      mockUsersRepository.Verify(x => x.GetUserByName("BadUser", It.IsAny<string>()), Times.Once());
      Assert.IsNull(ret);
    }

    [Test]
    public void GetUserByProviderUserKeyReturnUser()
    {
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      var result = service.GetUserByProviderUserKey(goodId, "appTest");
      Assert.IsNotNull(result);
      Assert.AreEqual(goodId, result.Id);
    }

    [Test]
    public void GetUserByIdReturnNullIfWrong()
    {
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      var result = service.GetUserByProviderUserKey(null, "appTest");
      Assert.IsNull(result);
    }

    [Test]
    public void GetUserByIdReturnNullIfIdNotExists()
    {
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      var result = service.GetUserByProviderUserKey(badId, "appTest");
      Assert.IsNull(result);
    }

    [Test]
    public void SendResetPasswordEmail_ShouldSendEmail()
    {
      var user = FakeUser();
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      emailService.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(true);
      var ret = service.SendResetPasswordToEmail(user.Email, "12345678");
      emailService.Verify(x => x.Send("usermail@domain.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
      Assert.IsTrue(ret);
    }

    [Test]
    public void SendResetPasswordEmail_ShouldReturnFalse_IfAProblem()
    {
      var user = FakeUser();
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      emailService.Setup(x => x.Send(user.Email, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(false);
      var ret = service.SendResetPasswordToEmail(user.Email, "12345678");
      emailService.Verify(x => x.Send("usermail@domain.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
      Assert.IsFalse(ret);
    }

    [Test]
    public void SendResetPasswordEmail_ShouldReturnFalse_IfEmailNull()
    {
      var user = FakeUser();
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      emailService.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(true);
      var ret = service.SendResetPasswordToEmail(null, "12345678");
      emailService.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
      Assert.IsFalse(ret);
    }

    [Test]
    public void SendResetPasswordEmail_ShouldReturnFalse_IfNewPasswordNull()
    {
      var user = FakeUser();
      var service = new UsersService(emailService.Object, mockUsersRepository.Object);
      emailService.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(true);
      var ret = service.SendResetPasswordToEmail("asd@asd.com", null);
      emailService.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
      Assert.IsFalse(ret);
    }

    private User FakeUser()
    {
      return new User { Id = goodId, ApplicationName = "prova", UserName = "user1", Email = "usermail@domain.com", Profile = new UserProfile { } };
    }
  }
}

using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Web.Security;
using NUnit.Framework;
using Moq;
using Iridio.Infrastructure;

namespace Iridio.MembershipProvider.Tests
{
  [TestFixture]
  public class IridioMembershipProviderTestsEncrypted
  {
    private EncryptionProvider _nhProv;

    [SetUp]
    public void InitializeTest()
    {
      var tmp = (MembershipSection)ConfigurationManager.GetSection("system.web/membership");
      var config = tmp.Providers["IridioMembershipProvider"].Parameters;
      var repo = Utilities.GetMockUsersRepository(config["applicationName"]).Object;
      config["passwordFormat"] = "Encrypted";
      config["enablePasswordReset"] = "true";
      config["enablePasswordRetrieval"] = "true";
      config["requiresQuestionAndAnswer"] = "true";
      Mock<IServiceBus> serviceBus = new Mock<IServiceBus>();
      _nhProv = new EncryptionProvider(repo, serviceBus.Object);
      _nhProv.Initialize("", config);
    }

    [TearDown]
    public void CleanupTest()
    {
      _nhProv = null;
    }

    [Test]
    [ExpectedException(typeof(MembershipPasswordException))]
    public void GetPassword_GivenGoodUserAndBadAnswer_WithRequireAnswer_ThrowsException()
    {
      var name = "EncryptUser";
      var answer = "BadAnswer";
      _nhProv.GetPassword(name, answer);
    }

    [Test]
    public void ChangePassword_GoodUserGoodPass_ReturnsTrue()
    {
      var user = "EncryptUser";
      var oldpass = "GoodPass";
      var newpass = "ABC123!?";
      var actual = _nhProv.ChangePassword(user, oldpass, newpass);
      Assert.IsTrue(actual);
    }

    [Test]
    public void GetPassword_GivenGoodUserAndGoodAnswer_WithRequireAnswer_ReturnsPassword()
    {
      var name = "EncryptUser";
      var answer = "GoodAnswer";
      var expected = "GoodPass";
      var actual = _nhProv.GetPassword(name, answer);
      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void GetPassword_GivenGoodUserAndGoodAnswer_ReturnsPassword()
    {
      var name = "EncryptUser";
      var answer = "GoodAnswer";
      var expected = "GoodPass";
      var actual = _nhProv.GetPassword(name, answer);
      Assert.AreEqual(expected, actual);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void ResetPassword_NullAnswer_QandARequired_ThrowsException()
    {
      var name = "GoodUser";
      _nhProv.ResetPassword(name, null);
    }

    [Test]
    [ExpectedException(typeof(MembershipPasswordException))]
    public void ResetPassword_BadAnswer_QandARequired_ThrowsException()
    {
      var name = "EncryptUser";
      var answer = "BadAnswer";
      _nhProv.ResetPassword(name, answer);
    }

    [Test]
    public void ResetPassword_GoodUser_QandARequired_ReturnsNewPassword()
    {
      var name = "EncryptUser";
      var answer = "GoodAnswer";
      var actual = _nhProv.ResetPassword(name, answer);
      Assert.AreNotEqual("", actual);
    }
  }
}
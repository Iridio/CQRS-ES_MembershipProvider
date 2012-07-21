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
      _nhProv.GetPassword("EncryptUser", "BadAnswer");
    }

    [Test]
    public void ChangePassword_GoodUserGoodPass_ReturnsTrue()
    {
      var actual = _nhProv.ChangePassword("EncryptUser", "GoodPass", "ABC123!?");
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
      var actual = _nhProv.GetPassword("EncryptUser", "GoodAnswer");
      Assert.AreEqual("GoodPass", actual);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void ResetPassword_NullAnswer_QandARequired_ThrowsException()
    {
      _nhProv.ResetPassword("GoodUser", null);
    }

    [Test]
    [ExpectedException(typeof(MembershipPasswordException))]
    public void ResetPassword_BadAnswer_QandARequired_ThrowsException()
    {
      _nhProv.ResetPassword("EncryptUser", "BadAnswer");
    }

    [Test]
    public void ResetPassword_GoodUser_QandARequired_ReturnsNewPassword()
    {
      var actual = _nhProv.ResetPassword("EncryptUser", "GoodAnswer");
      Assert.AreNotEqual("", actual);
    }
  }
}
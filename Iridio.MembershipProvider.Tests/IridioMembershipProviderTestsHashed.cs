using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using NUnit.Framework;
using Moq;
using Iridio.Infrastructure;

namespace Iridio.MembershipProvider.Tests
{
  [TestFixture]
  public class IridioMembershipProviderTestsHashed
  {
    private EncryptionProvider _nhProv;

    [SetUp]
    public void InitializeTest()
    {
      var tmp = (MembershipSection)ConfigurationManager.GetSection("system.web/membership");
      var config = tmp.Providers["IridioMembershipProvider"].Parameters;
      var repo = Utilities.GetMockUsersRepository(config["applicationName"]).Object;
      config["passwordFormat"] = "Hashed";
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
    [ExpectedException(typeof(ProviderException))]
    public void GetPassword_GivenGoodUserAndGoodAnswer_ThrowsException()
    {
      _nhProv.GetPassword("GoodUser", "GoodAnswer");
    }

    [Test]
    public void ResetPassword_GoodUser_QandARequired_ReturnsNewPassword()
    {
      var actual = _nhProv.ResetPassword("HashUser", "GoodAnswer");
      Assert.AreNotEqual("", actual);
    }

    [Test]
    public void ValidateUser_GivenGoodUserGoodPassword_ReturnsTrue()
    {
      var actual = _nhProv.ValidateUser("HashUser", "GoodPass");
      Assert.IsTrue(actual);
    }
  }
}
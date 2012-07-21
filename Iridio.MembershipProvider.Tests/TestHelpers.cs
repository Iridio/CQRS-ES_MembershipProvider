using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;
using Iridio.Infrastructure;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Dtos;
using Moq;

namespace Iridio.MembershipProvider.Tests
{
  public struct PasswordQandA
  {
    public string Question { get; set; }
    public string Answer { get; set; }
  }

  public class UserParameters
  {
    public string username { get; set; }
    public string password { get; set; }
    public string email { get; set; }
    public string passwordQuestion { get; set; }
    public string passwordAnswer { get; set; }
    public bool isApproved { get; set; }
    public object providerUserKey { get; set; }
  }

  public static class TestUtils
  {
    public static UserParameters GetValidUser()
    {
      var u = new UserParameters();

      u.username = "TestUserName";
      u.password = "!Password?123";
      u.email = "user@domain.com";
      u.passwordQuestion = "TestPasswordQuestion";
      u.passwordAnswer = "TestPasswordAnswer";
      u.isApproved = false;
      u.providerUserKey = null;

      return u;
    }

    public static List<UserParameters> GetTestUsers(int numUsers, string prefix)
    {
      List<UserParameters> t = new List<UserParameters>();
      for (int i = 0; i < numUsers; i++)
      {
        var u = new UserParameters();

        u.username = prefix + "TestUser" + i;
        u.password = prefix + "!Password?" + i;
        u.email = u.username + "@testdomain.com";
        u.passwordQuestion = prefix + "TestPasswordQuestion" + i;
        u.passwordAnswer = prefix + "TestPasswordAnswer" + i;
        u.isApproved = true;
        u.providerUserKey = null;

        t.Add(u);
      }
      return t;
    }

    public static NameValueCollection GetComplexConfig()
    {
      var tmp = (MembershipSection)ConfigurationManager.GetSection("system.web/membership");
      var config = tmp.Providers["IridioMembershipProvider"].Parameters;
      config["maxInvalidPasswordAttempts"] = "3";
      config["passwordAttemptWindow"] = "10";
      config["minRequiredNonAlphanumericCharacters"] = "1";
      config["minRequiredPasswordLength"] = "7";
      config["passwordStrengthRegularExpression"] = "^.*(?=.{6,})(?=.*[a-z])(?=.*[A-Z]).*$";
      config["enablePasswordReset"] = "true";
      config["enablePasswordRetrieval"] = "true";
      config["requiresQuestionAndAnswer"] = "true";
      config["requiresUniqueEmail"] = "true";
      return config;
    }

    public static NameValueCollection GetNoPasswordConfig()
    {
      var tmp = (MembershipSection)ConfigurationManager.GetSection("system.web/membership");
      var config = tmp.Providers["IridioMembershipProvider"].Parameters;
      config["maxInvalidPasswordAttempts"] = "5";
      config["passwordAttemptWindow"] = "10";
      config["minRequiredNonAlphanumericCharacters"] = "1";
      config["minRequiredPasswordLength"] = "7";
      config["passwordStrengthRegularExpression"] = "";
      config["enablePasswordReset"] = "true";
      config["enablePasswordRetrieval"] = "false";
      config["requiresQuestionAndAnswer"] = "true";
      config["requiresUniqueEmail"] = "true";
      return config;
    }
  }

  public class EncryptionProvider : IridioMembershipProvider
  {
    public EncryptionProvider(IUsersRepository repository, IServiceBus serviceBus)
      : base(serviceBus, repository)
    {

    }

    protected override MachineKeySection GetMachineKeySection()
    {
      var mk = new MachineKeySection();
      mk.DecryptionKey = "0A5D40CA5C48726556180200D9DBE44A8FE58A8E6A3E8CC153BFC631833BA0FE";
      mk.ValidationKey = "7D30287B722BF7141915476F0609FFD604CBB5243D8574F85BA5B496FA58D3EE49A8CE1E07E958F145967495A56E5B6298082070C0488F7B4FC42EDE9956422E";
      mk.Validation = MachineKeyValidation.SHA1;
      mk.Decryption = "AES";
      return mk;
    }
  }

  public class EncryptionErrorProvider : IridioMembershipProvider
  {
    public EncryptionErrorProvider(IUsersRepository repository, IServiceBus serviceBus)
      : base(serviceBus, repository)
    {

    }

    protected override MachineKeySection GetMachineKeySection()
    {
      var mk = new MachineKeySection();
      mk.ValidationKey = "AutoGenerate";
      return mk;
    }
  }

  public static class Utilities
  {
    public static Guid Guid1 = Guid.NewGuid();
    public static Guid Guid2 = Guid.NewGuid();
    public static Guid Guid3 = Guid.NewGuid();
    public static Guid Guid4 = Guid.NewGuid();
    public static Guid Guid999 = Guid.NewGuid();

    public static Mock<IUsersRepository> GetMockUsersRepository(string appName)
    {
      var goodUser = new User()
      {
        Id = Guid1,
        Password = "GoodPass",
        IsApproved = true,
        CreationDate = DateTime.Now,
        PasswordAnswer = "GoodAnswer",
        LastLockedOutDate = DateTime.Now,
        LastPasswordChangedDate = DateTime.Now,
        LastLoginDate = DateTime.Now,
        UserName = "GoodUser"
      };

      var mockRepo = new Mock<IUsersRepository>();
      mockRepo.Setup(v => v.GetUserByName("GoodUser", appName)).Returns(goodUser);
      mockRepo.Setup(v => v.GetUserByName("LockedUser", appName)).Returns(new User() { IsLockedOut = true });
      mockRepo.Setup(v => v.GetUserByName("BadAnswerUser", appName)).Returns(new User() { PasswordAnswer = "GoodAnswer", Password = "GoodPass" });
      mockRepo.Setup(v => v.GetUserByName("PasswordAttemptCount5", appName)).Returns(new User() { Id = Utilities.Guid3, FailedPasswordAttemptCount = 5 });
      mockRepo.Setup(v => v.GetUserByName("PasswordAttemptCount4", appName)).Returns(new User() { Id = Utilities.Guid3, FailedPasswordAttemptCount = 4 });
      mockRepo.Setup(v => v.GetUserByName("PasswordAnswerAttemptCount5", appName)).Returns(new User() { Id = Utilities.Guid3, FailedPasswordAnswerAttemptCount = 5 });
      mockRepo.Setup(v => v.GetUserByName("PasswordAnswerAttemptCount4", appName)).Returns(new User() { Id = Utilities.Guid3, FailedPasswordAnswerAttemptCount = 4 });
      mockRepo.Setup(v => v.GetUserByName("EncryptUser", appName)).Returns(new User() { PasswordAnswer = "SerLEVf28XZ/mBLKLgqulBDfUK05rOsefCL0gd+WRDE=", Password = "Hei77AsDaWtwcvWYAJFawnB0X7BiukYVd+62O6lthNY=", IsApproved = true });
      mockRepo.Setup(v => v.GetUserByName("HashUser", appName)).Returns(new User() { PasswordAnswer = "/jGKx1DvdLPnZk1ZuQaz2fSFdws=", Password = "UAFsjFEtDHxMGwlRE/ICHnUehCs=", IsApproved = true });
      mockRepo.Setup(v => v.GetUserByName("NewUser", appName)).Returns((User)null);
      mockRepo.Setup(v => v.GetUserByName("ExceptionUser", appName)).Throws(new Exception());
      mockRepo.Setup(v => v.GetBy(Guid1)).Returns(goodUser);
      mockRepo.Setup(v => v.GetBy(Guid999)).Throws(new Exception()); //Serve per simulare eccezione eventuale nel get del repository
      mockRepo.Setup(v => v.FindUsersByEmail("GoodEmail", 0, 99, appName)).Returns(GetStubUsers(1));
      mockRepo.Setup(v => v.FindUsersByEmail("BadEmail", 0, 99, appName)).Returns(new List<User>());
      mockRepo.Setup(v => v.FindUsersByEmail("DupEmail", 0, 99, appName)).Returns(GetStubUsers(2));
      mockRepo.Setup(v => v.FindUsersByEmail("ExceptionMail", 0, 99, appName)).Throws(new Exception());
      mockRepo.Setup(v => v.FindUsersByName("GoodName", 0, 99, appName)).Returns(GetStubUsers(1));
      mockRepo.Setup(v => v.FindUsersByName("BadName", 0, 99, appName)).Returns(new List<User>());
      mockRepo.Setup(v => v.FindUsersByName("DupName", 0, 99, appName)).Returns(GetStubUsers(2));
      mockRepo.Setup(v => v.FindUsersByName("ExceptionUser", 0, 99, appName)).Throws(new Exception());
      mockRepo.Setup(v => v.GetUserNameByEmail("NewEmail", appName)).Returns("");
      mockRepo.Setup(v => v.GetUserNameByEmail("DupEmail", appName)).Returns("DupUser");
      mockRepo.Setup(v => v.GetUserNameByEmail("ExceptionEmail", appName)).Throws(new Exception());
      int totale = 10;
      mockRepo.Setup(v => v.GetUsers(0, 99, appName, out totale)).Returns(GetStubUsers(2));
      mockRepo.Setup(v => v.GetUsers(1, 99, appName, out totale)).Returns(new List<User>());
      mockRepo.Setup(v => v.GetUsers(2, 99, appName, out totale)).Throws(new Exception());

      var users = new List<User>() { 
        new User() {Id = Guid1, UserName = "User01" }, 
        new User() {Id = Guid2, UserName = "User02" },
        new User() {Id = Guid3, UserName = "User03" },
        new User() {Id = Guid4, UserName = "User04" },
      };
      return mockRepo;
    }

    public static List<User> GetStubUsers(int numUsers)
    {
      var uList = new List<User>();
      for (int i = 0; i < numUsers; i++)
        uList.Add(new User() { Id = Guid.NewGuid(), CreationDate = DateTime.Now, UserName = "SampleUser" + i });
      return uList;
    }

    public static IridioMembershipProvider GetProviderWithNoPasswordRetrievalOrReset(Mock<IServiceBus> serviceBus)
    {
      var tmp = (MembershipSection)ConfigurationManager.GetSection("system.web/membership");
      var config = tmp.Providers["IridioMembershipProvider"].Parameters;
      config["enablePasswordRetrieval"] = "false";
      config["enablePasswordReset"] = "false";
      var prov = new IridioMembershipProvider(serviceBus.Object, Utilities.GetMockUsersRepository(config["applicationName"]).Object);
      prov.Initialize("", config);
      return prov;
    }

    public static IridioMembershipProvider GetProviderWithPasswordRetrievalOrReset(Mock<IServiceBus> serviceBus)
    {
      var tmp = (MembershipSection)ConfigurationManager.GetSection("system.web/membership");
      var config = tmp.Providers["IridioMembershipProvider"].Parameters;
      config["enablePasswordRetrieval"] = "true";
      config["enablePasswordReset"] = "true";
      var prov = new IridioMembershipProvider(serviceBus.Object, Utilities.GetMockUsersRepository(config["applicationName"]).Object);
      prov.Initialize("", config);
      return prov;
    }
  }
}

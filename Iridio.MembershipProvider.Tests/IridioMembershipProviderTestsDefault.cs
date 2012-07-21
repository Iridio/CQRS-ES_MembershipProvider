using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Web.Security;
using Moq;
using NUnit.Framework;
using Iridio.Infrastructure;
using Iridio.Messages.Commands;

namespace Iridio.MembershipProvider.Tests
{
  [TestFixture]
  public class IridioMembershipProviderTestsDefault
  {
    private IridioMembershipProvider _nhProv;
    private Mock<IServiceBus> serviceBus;

    [SetUp]
    public void InitializeTest()
    {
      serviceBus = new Mock<IServiceBus>();
      serviceBus.Setup(x => x.Send(It.IsAny<CreateUser>()));
      serviceBus.Setup(x => x.Send(It.IsAny<ChangeUserPasswordQuestionAndAnswer>()));
      serviceBus.Setup(x => x.Send(It.IsAny<ChangeUserPassword>()));
      serviceBus.Setup(x => x.Send(It.IsAny<DeleteUser>()));
      serviceBus.Setup(x => x.Send(It.IsAny<ResetUserPassword>()));
      serviceBus.Setup(x => x.Send(It.IsAny<UnlockUser>()));
      serviceBus.Setup(x => x.Send(It.IsAny<SetUserFailedPasswordAttempt>()));
      serviceBus.Setup(x => x.Send(It.IsAny<SetUserFailedPasswordAnswerAttempt>()));

      var tmp = (MembershipSection)ConfigurationManager.GetSection("system.web/membership");
      var config = tmp.Providers["IridioMembershipProvider"].Parameters;
      var repo = Utilities.GetMockUsersRepository(config["applicationName"]).Object;
      _nhProv = new IridioMembershipProvider(serviceBus.Object, repo);
      _nhProv.Initialize("", config);
    }

    [TearDown]
    public void CleanupTest()
    {
      _nhProv = null;
    }

    #region Test Properties
    [Test]
    public void Exercise_Public_Properties()
    {
      var applicationName = _nhProv.ApplicationName;
      _nhProv.ApplicationName = applicationName;
      var maxInvalidPasswordAttempts = _nhProv.MaxInvalidPasswordAttempts;
      var minRequiredNonAlphanumericCharacters = _nhProv.MinRequiredNonAlphanumericCharacters;
      var minRequiredPasswordLength = _nhProv.MinRequiredPasswordLength;
      var passwordAttemptWindow = _nhProv.PasswordAttemptWindow;
      var passwordStrengthRegularExpression = _nhProv.PasswordStrengthRegularExpression;
      Assert.AreEqual(_nhProv.ApplicationName, applicationName);
      Assert.AreEqual(_nhProv.MaxInvalidPasswordAttempts, maxInvalidPasswordAttempts);
      Assert.AreEqual(_nhProv.MinRequiredNonAlphanumericCharacters, minRequiredNonAlphanumericCharacters);
      Assert.AreEqual(_nhProv.MinRequiredPasswordLength, minRequiredPasswordLength);
      Assert.AreEqual(_nhProv.PasswordAttemptWindow, passwordAttemptWindow);
      Assert.AreEqual(_nhProv.PasswordStrengthRegularExpression, passwordStrengthRegularExpression);
    }
    #endregion

    #region Test ChangePassword Method
    [Test]
    public void ChangePassword_GoodUserGoodPass_ReturnsTrue()
    {
      var actual = _nhProv.ChangePassword("GoodUser", "GoodPass", "ABC123!?");
      Assert.IsTrue(actual);
      serviceBus.Verify(x => x.Send(It.Is<ChangeUserPassword>(cmd => cmd.AggregateId == Utilities.Guid1 && cmd.NewPassword == "ABC123!?")), Times.Once());
    }

    [Test]
    [ExpectedException(typeof(MembershipPasswordException))]
    public void ChangePassword_GoodUserBadPass_ThrowsException()
    {
      _nhProv.ChangePassword("GoodUser", "GoodPass", "Bad");
      serviceBus.Verify(x => x.Send(It.IsAny<ChangeUserPassword>()), Times.Never());
    }

    [Test]
    public void ChangePassword_BadUserBadPass_ReturnsFalse()
    {
      var actual = _nhProv.ChangePassword("GoodUser", "BadPass", "Bad");
      Assert.IsFalse(actual);
      serviceBus.Verify(x => x.Send(It.IsAny<ChangeUserPassword>()), Times.Never());
    }
    #endregion

    #region Test ChangePasswordQuestionAndAnswer Method
    [Test]
    public void ChangePasswordQuestionAndAnswer_GoodUser_ReturnsTrue()
    {
      var actual = _nhProv.ChangePasswordQuestionAndAnswer("GoodUser", "GoodPass", "Good", "Answer");
      Assert.IsTrue(actual);
      serviceBus.Verify(x => x.Send(It.Is<ChangeUserPasswordQuestionAndAnswer>(cmd => cmd.AggregateId == Utilities.Guid1 && cmd.NewPasswordQuestion == "Good" && cmd.NewPasswordAnswer == "Answer")), Times.Once());
    }

    [Test]
    public void ChangePasswordQuestionAndAnswer_BadUser_ReturnsFalse()
    {
      var actual = _nhProv.ChangePasswordQuestionAndAnswer("BadUser", "BadPass", "Good", "Answer");
      Assert.IsFalse(actual);
      serviceBus.Verify(x => x.Send(It.IsAny<ChangeUserPasswordQuestionAndAnswer>()), Times.Never());
    }
    #endregion

    #region Test CreateUser Method
    [Test]
    public void CreateUser_ValidData_ReturnsSuccess()
    {
      var User = "NewUser";
      var pass = "ABC123!?";
      var email = "NewEmail";
      var approved = true;
      var question = "Question";
      var answer = "Answer";
      var key = 1;
      var result = new MembershipCreateStatus();
      _nhProv.CreateUser(User, pass, email, question, answer, approved, key, out result);
      Assert.AreEqual(MembershipCreateStatus.Success, result);
      serviceBus.Verify(x => x.Send(It.Is<CreateUser>(cmd => cmd.UserName == "NewUser" && cmd.Password == "ABC123!?" && cmd.Email == "NewEmail" && cmd.IsApproved && cmd.ApplicationName == "TestApp")), Times.Once());
      serviceBus.Verify(x => x.Send(It.Is<ChangeUserPasswordQuestionAndAnswer>(cmd => cmd.NewPasswordQuestion == "Question" && cmd.NewPasswordAnswer == "Answer")), Times.Once());
    }

    [Test]
    public void CreateUser_ValidData_With_No_Answer_Or_Question_ReturnsSuccess()
    {
      var User = "NewUser";
      var pass = "ABC123!?";
      var email = "NewEmail";
      var approved = true;
      string question = null;
      string answer = null;
      var key = 1;
      var result = new MembershipCreateStatus();
      _nhProv.CreateUser(User, pass, email, question, answer, approved, key, out result);
      Assert.AreEqual(MembershipCreateStatus.Success, result);
      serviceBus.Verify(x => x.Send(It.Is<CreateUser>(cmd => cmd.UserName == "NewUser" && cmd.Password == "ABC123!?" && cmd.Email == "NewEmail" && cmd.IsApproved && cmd.ApplicationName == "TestApp")), Times.Once());
      serviceBus.Verify(x => x.Send(It.IsAny<ChangeUserPasswordQuestionAndAnswer>()), Times.Never());
    }

    [Test]
    public void CreateUser_InvalidPassword_ReturnsInvalidPassword()
    {
      var User = "NewUser";
      var pass = "Bad";
      var email = "NewEmail";
      var approved = true;
      var question = "Question";
      var answer = "Answer";
      var key = 1;
      var result = new MembershipCreateStatus();
      var actual = _nhProv.CreateUser(User, pass, email, question, answer, approved, key, out result);
      Assert.AreEqual(MembershipCreateStatus.InvalidPassword, result);
      serviceBus.Verify(x => x.Send(It.IsAny<CreateUser>()), Times.Never());
      serviceBus.Verify(x => x.Send(It.IsAny<ChangeUserPasswordQuestionAndAnswer>()), Times.Never());
    }

    [Test]
    public void CreateUser_NullPassword_ReturnsInvalidPassword()
    {
      var User = "NewUser";
      var pass = "";
      var email = "NewEmail";
      var approved = true;
      var question = "Question";
      var answer = "Answer";
      var key = 1;
      var result = new MembershipCreateStatus();
      var actual = _nhProv.CreateUser(User, pass, email, question, answer, approved, key, out result);
      Assert.AreEqual(MembershipCreateStatus.InvalidPassword, result);
      serviceBus.Verify(x => x.Send(It.IsAny<CreateUser>()), Times.Never());
      serviceBus.Verify(x => x.Send(It.IsAny<ChangeUserPasswordQuestionAndAnswer>()), Times.Never());
    }

    [Test]
    public void CreateUser_DupUser_ReturnsDupUser()
    {
      var User = "GoodUser";
      var pass = "ABC123!?";
      var email = "NewEmail";
      var approved = true;
      var question = "Question";
      var answer = "Answer";
      var key = 1;
      var result = new MembershipCreateStatus();
      var actual = _nhProv.CreateUser(User, pass, email, question, answer, approved, key, out result);
      Assert.AreEqual(MembershipCreateStatus.DuplicateUserName, result);
      serviceBus.Verify(x => x.Send(It.IsAny<CreateUser>()), Times.Never());
      serviceBus.Verify(x => x.Send(It.IsAny<ChangeUserPasswordQuestionAndAnswer>()), Times.Never());
    }

    [Test]
    public void CreateUser_DupEMail_ReturnsDupEMail()
    {
      var User = "GoodUser";
      var pass = "ABC123!?";
      var email = "DupEmail";
      var approved = true;
      var question = "Question";
      var answer = "Answer";
      var key = 1;
      var result = new MembershipCreateStatus();
      var actual = _nhProv.CreateUser(User, pass, email, question, answer, approved, key, out result);
      Assert.AreEqual(MembershipCreateStatus.DuplicateEmail, result);
      serviceBus.Verify(x => x.Send(It.IsAny<CreateUser>()), Times.Never());
      serviceBus.Verify(x => x.Send(It.IsAny<ChangeUserPasswordQuestionAndAnswer>()), Times.Never());
    }
    #endregion

    #region Test DeleteUser Method
    [Test]
    public void DeleteUser_GoodUser_ReturnsTrue()
    {
      var Username = "GoodUser";
      var actual = _nhProv.DeleteUser(Username, true);
      Assert.IsTrue(actual);
      serviceBus.Verify(x => x.Send(It.Is<DeleteUser>(cmd => cmd.AggregateId == Utilities.Guid1)), Times.Once());
    }

    [Test]
    public void DeleteUser_BadUser_ReturnsTrue()
    {
      var Username = "BadUser";
      var actual = _nhProv.DeleteUser(Username, true);
      Assert.IsTrue(actual);
      serviceBus.Verify(x => x.Send(It.IsAny<DeleteUser>()), Times.Never());
    }

    [Test]
    [ExpectedException(typeof(Exception))]
    public void DeleteUser_ExceptionUser_ThrowsException()
    {
      _nhProv.DeleteUser("ExceptionUser", true);
    }
    #endregion

    #region Test FindUserByEmail Method
    [Test]
    public void FindUserByEmail_GivenGoodEmail_ReturnsOneRecord()
    {
      var email = "GoodEmail";
      var recs = -1;
      var expectedRecs = 1;
      var actual = _nhProv.FindUsersByEmail(email, 0, 99, out recs);
      Assert.AreEqual(expectedRecs, recs);
      Assert.AreEqual(expectedRecs, actual.Count);
    }

    [Test]
    public void FindUserByEmail_GivenDuplicateEmail_ReturnsTwoRecords()
    {
      var email = "DupEmail";
      var recs = -1;
      var expectedRecs = 2;
      var actual = _nhProv.FindUsersByEmail(email, 0, 99, out recs);
      Assert.AreEqual(expectedRecs, recs);
      Assert.AreEqual(expectedRecs, actual.Count);
    }

    [Test]
    public void FindUserByEmail_GivenBadEmail_ReturnsZeroRecords()
    {
      var email = "BadEmail";
      var recs = -1;
      var expectedRecs = 0;
      var actual = _nhProv.FindUsersByEmail(email, 0, 99, out recs);
      Assert.AreEqual(expectedRecs, recs);
      Assert.AreEqual(expectedRecs, actual.Count);
    }

    [Test]
    [ExpectedException(typeof(NullReferenceException))]
    public void FindUserByEmail_GivenException_ThrowsMemberAccessException()
    {
      var email = "ExceptionUser";
      var recs = -1;
      _nhProv.FindUsersByEmail(email, 0, 99, out recs);
    }
    #endregion

    #region Test FindUserByName Method
    [Test]
    public void FindUserByName_GivenGoodName_ReturnsOneRecord()
    {
      var Name = "GoodName";
      var recs = -1;
      var expectedRecs = 1;
      var actual = _nhProv.FindUsersByName(Name, 0, 99, out recs);
      Assert.AreEqual(expectedRecs, recs);
      Assert.AreEqual(expectedRecs, actual.Count);
    }

    [Test]
    public void FindUserByName_GivenDuplicateName_ReturnsTwoRecords()
    {
      var Name = "DupName";
      var recs = -1;
      var expectedRecs = 2;
      var actual = _nhProv.FindUsersByName(Name, 0, 99, out recs);
      Assert.AreEqual(expectedRecs, recs);
      Assert.AreEqual(expectedRecs, actual.Count);
    }

    [Test]
    public void FindUserByName_GivenBadName_ReturnsZeroRecords()
    {
      var Name = "BadName";
      var recs = -1;
      var expectedRecs = 0;
      var actual = _nhProv.FindUsersByName(Name, 0, 99, out recs);
      Assert.AreEqual(expectedRecs, recs);
      Assert.AreEqual(expectedRecs, actual.Count);
    }

    [Test]
    [ExpectedException(typeof(Exception))]
    public void FindUserByName_GivenException_ThrowsMemberAccessException()
    {
      var Name = "ExceptionUser";
      var recs = -1;
      _nhProv.FindUsersByName(Name, 0, 99, out recs);
    }

    [Test]
    public void GetUserByNameNullReturnsNull()
    {
      var result = _nhProv.GetUser(null, false);
      Assert.IsNull(result);
    }
    #endregion

    #region Test GetAllUsers Method
    [Test]
    public void GetAllUsers_GivenTwoUsers_ReturnsTwoUsers()
    {
      var tot = -1;
      var actual = _nhProv.GetAllUsers(0, 99, out tot);
      Assert.AreEqual(2, actual.Count);
      Assert.AreEqual(10, tot);
    }

    [Test]
    public void GetAllUsers_GivenZeroUsers_ReturnsZeroUsers()
    {
      var tot = -1;
      var actual = _nhProv.GetAllUsers(1, 99, out tot);
      Assert.AreEqual(0, actual.Count);
      Assert.AreEqual(10, tot);
    }

    [Test]
    [ExpectedException(typeof(Exception))]
    public void GetAllUsers_GivenExceptionUser_ThrowsException()
    {
      var tot = -1;
      _nhProv.GetAllUsers(2, 99, out tot);
    }
    #endregion

    #region Test GetNumberOfUsersOnline Method
    [Test]
    public void GetNumberOfUsersOnline_GivenTwoUsers_ReturnsTwoUsers()
    {
      var tmp = (MembershipSection)ConfigurationManager.GetSection("system.web/membership");
      var config = tmp.Providers["IridioMembershipProvider"].Parameters;
      var repo = Utilities.GetMockUsersRepository(config["applicationName"]);
      repo.Setup(v => v.GetNumberOfUsersOnline(It.IsAny<DateTime>(), config["applicationName"])).Returns(2);
      _nhProv = new IridioMembershipProvider(serviceBus.Object, repo.Object);
      _nhProv.Initialize("", config);
      var expected = 2;
      var actual = _nhProv.GetNumberOfUsersOnline();
      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void GetNumberOfUsersOnline_GivenZeroUsers_ReturnsZeroUsers()
    {
      var repo = Utilities.GetMockUsersRepository(null);
      repo.Setup(v => v.GetNumberOfUsersOnline(DateTime.Now, null)).Returns(0);
      _nhProv = new IridioMembershipProvider(serviceBus.Object, repo.Object);
      var expected = 0;
      var actual = _nhProv.GetNumberOfUsersOnline();
      Assert.AreEqual(expected, actual);
    }

    [Test]
    [ExpectedException(typeof(Exception))]
    public void GetNumberOfUsersOnline_GivenExceptionUser_ThrowsException()
    {
      var tmp = (MembershipSection)ConfigurationManager.GetSection("system.web/membership");
      var config = tmp.Providers["IridioMembershipProvider"].Parameters;
      var repo = Utilities.GetMockUsersRepository(config["applicationName"]);
      repo.Setup(v => v.GetNumberOfUsersOnline(It.IsAny<DateTime>(), config["applicationName"])).Throws(new Exception());
      _nhProv = new IridioMembershipProvider(serviceBus.Object, repo.Object);
      _nhProv.Initialize("", config);
      _nhProv.GetNumberOfUsersOnline();
    }
    #endregion

    #region Test GetPassword Method
    [Test]
    public void GetPassword_GivenGoodUserAndGoodAnswer_ReturnsPassword()
    {
      var name = "GoodUser";
      var answer = "GoodAnswer";
      var expected = "GoodPass";
      var actual = _nhProv.GetPassword(name, answer);
      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void GetPassword_GivenGoodUserAndBadAnswer_WithoutRequireAnswer_ReturnsPassword()
    {
      var name = "BadAnswerUser";
      var answer = "BadAnswer";
      var expected = "GoodPass";
      var actual = _nhProv.GetPassword(name, answer);
      Assert.AreSame(expected, actual);
    }

    [Test]
    [ExpectedException(typeof(MembershipPasswordException), ExpectedMessage = "The supplied user name is not found.")]
    public void GetPassword_GivenBadUser_ThrowsException()
    {
      var name = "BadUser";
      var answer = "BadAnswer";
      _nhProv.GetPassword(name, answer);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void GetPassword_WhenRetrievalDisabled_ThrowsException()
    {
      var noPassProv = Utilities.GetProviderWithNoPasswordRetrievalOrReset(serviceBus);
      var name = "BadUser";
      var answer = "BadAnswer";
      noPassProv.GetPassword(name, answer);
    }
    #endregion

    #region Test GetUser Method
    [Test]
    public void GetUser_GoodUserOnline_ReturnsUser()
    {
      var name = "GoodUser";
      var actual = _nhProv.GetUser(name, true);
      Assert.IsNotNull(actual);
    }

    [Test]
    [ExpectedException(typeof(Exception))]
    public void GetUser_BadUser_ThrowsException()
    {
      var name = "ExceptionUser";
      _nhProv.GetUser(name, true);
    }

    [Test]
    public void GetUser_GoodUserIdOnline_ReturnsUser()
    {
      var actual = _nhProv.GetUser(Utilities.Guid1, true);
      Assert.IsNotNull(actual);
    }

    [Test]
    [ExpectedException(typeof(InvalidCastException))]
    public void GetUser_BadUserId_ThrowsException()
    {
      _nhProv.GetUser(123, true);//esplode in quanto non Guid
    }
    #endregion

    #region Test GetUserNameByEmail Method
    [Test, ExpectedException(typeof(Exception))]
    public void GetUserNameByEmail_ExceptionUser_ThrowsException()
    {
      var email = "ExceptionEmail";
      _nhProv.GetUserNameByEmail(email);
    }
    #endregion

    #region Test Initialize Method
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Initialize_NullConfig_ThrowsArgumentNullException()
    {
      var repo = Utilities.GetMockUsersRepository(null).Object;
      _nhProv = new IridioMembershipProvider(serviceBus.Object, repo);
      _nhProv.Initialize("", null);
    }

    [Test]
    public void Initialize_NullName_SetsDefaultName()
    {
      var repo = Utilities.GetMockUsersRepository(null).Object;
      _nhProv = new IridioMembershipProvider(serviceBus.Object, repo);
      var expected = "IridioMembershipProvider";
      _nhProv.Initialize("", new NameValueCollection());
      var actual = _nhProv.Name;
      Assert.AreEqual(expected, actual);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void Initialize_CheckEncryptionKeyFails_ThrowsProviderException()
    {
      var repo = Utilities.GetMockUsersRepository(null).Object;
      var tmpProv = new EncryptionErrorProvider(repo, serviceBus.Object);
      var config = new NameValueCollection();
      config.Add("passwordFormat", "Hashed");
      tmpProv.Initialize("", config);
    }
    #endregion

    #region Test ResetPassword Method
    [Test]
    [ExpectedException(typeof(NotSupportedException))]
    public void ResetPassword_WhenRetrievalDisabled_ThrowsException()
    {
      var noPassProv = Utilities.GetProviderWithNoPasswordRetrievalOrReset(serviceBus);
      var name = "BadUser";
      var answer = "BadAnswer";
      noPassProv.ResetPassword(name, answer);
    }

    [Test]
    public void ResetPassword_GoodUserNoAnswer_ReturnsNewPassword()
    {
      var passProv = Utilities.GetProviderWithPasswordRetrievalOrReset(serviceBus);
      var name = "GoodUser";
      var actual = passProv.ResetPassword(name, null);
      Assert.AreNotEqual("", actual);
      serviceBus.Verify(x => x.Send(It.Is<ResetUserPassword>(cmd => cmd.AggregateId == Utilities.Guid1 && !String.IsNullOrEmpty(cmd.NewPassword))), Times.Once());
    }

    [Test]
    [ExpectedException(typeof(MembershipPasswordException))]
    public void ResetPassword_LockedUser_ThrowsException()
    {
      var name = "LockedUser";
      var passProv = Utilities.GetProviderWithPasswordRetrievalOrReset(serviceBus);
      var actual = passProv.ResetPassword(name, null);
    }
    #endregion

    #region Test UnlockUser Method
    [Test]
    public void UnlockUser_GoodUser_ReturnsTrue()
    {
      var name = "GoodUser";
      var actual = _nhProv.UnlockUser(name);
      Assert.IsTrue(actual);
      serviceBus.Verify(x => x.Send(It.Is<UnlockUser>(cmd => cmd.AggregateId == Utilities.Guid1 && cmd.LastLockedOutDate > DateTime.MinValue)), Times.Once());
    }

    [Test]
    [ExpectedException(typeof(Exception))]
    public void UnlockUser_ExceptionUser_ThrowsException()
    {
      _nhProv.UnlockUser("ExceptionUser");
    }
    #endregion

    #region Test UpdateUser Method
    [Test]
    public void UpdateUser_GoodUser_DoesNotThrowError()
    {
      var m = _nhProv.GetUser("GoodUser", true);
      _nhProv.UpdateUser(m);
      Assert.IsTrue(true);
      serviceBus.Verify(x => x.Send(It.Is<UpdateUser>(cmd => cmd.AggregateId == Utilities.Guid1 && cmd.Email == null && cmd.Comment == null)), Times.Once());
    }

    [Test]
    [ExpectedException(typeof(MemberAccessException), ExpectedMessage = "User can not be null")]
    public void UpdateUser_BadUser_ThrowsException()
    {
      _nhProv.UpdateUser(null);
    }
    #endregion

    #region Test ValidateUser Method
    [Test]
    public void ValidateUser_GivenGoodUserGoodPassword_ReturnsTrue()
    {
      var UserName = "GoodUser";
      var UserPass = "GoodPass";
      var actual = _nhProv.ValidateUser(UserName, UserPass);
      Assert.IsTrue(actual);
      serviceBus.Verify(x => x.Send(It.Is<UpdateUserLastLoginDate>(cmd => cmd.AggregateId == Utilities.Guid1 && cmd.LastLoginDate > DateTime.MinValue)), Times.Once());
    }

    [Test]
    public void ValidateUser_GivenGoodUserBadPassword_ReturnsFalse()
    {
      var UserName = "GoodUser";
      var UserPass = "BadPass";
      var actual = _nhProv.ValidateUser(UserName, UserPass);
      Assert.IsFalse(actual);
      serviceBus.Verify(x => x.Send(It.IsAny<UpdateUserLastLoginDate>()), Times.Never());
    }

    [Test]
    public void ValidateUser_GivenBadUserBadPassword_ReturnsFalse()
    {
      var UserName = "BadUser";
      var UserPass = "BadPass";
      var actual = _nhProv.ValidateUser(UserName, UserPass);
      Assert.IsFalse(actual);
      serviceBus.Verify(x => x.Send(It.IsAny<UpdateUserLastLoginDate>()), Times.Never());
    }

    [Test]
    [ExpectedException(typeof(Exception))]
    public void ValidateUser_GivenException_ThrowsMemberAccessException()
    {
      _nhProv.ValidateUser("ExceptionUser", "BadPass");
    }
    #endregion

    #region UpdateFailureCount
    [Test]
    public void UpdateFailureCount_First_Bad_Password_Set_First_Counter()
    {
      _nhProv.UpdateFailureCount("GoodUser", Iridio.MembershipProvider.IridioMembershipProvider.UpdateFailures.Password);
      serviceBus.Verify(x => x.Send(It.Is<SetUserFailedPasswordAttempt>(cmd => cmd.AggregateId == Utilities.Guid1 && cmd.FailedPasswordAttemptCount == 1 && cmd.FailedPasswordAttemptWindowStart > DateTime.MinValue)), Times.Once());
      serviceBus.Verify(x => x.Send(It.IsAny<SetUserFailedPasswordAnswerAttempt>()), Times.Never());
    }

    [Test]
    public void UpdateFailureCount_First_Bad_PasswordAnswer_Set_First_Counter()
    {
      _nhProv.UpdateFailureCount("GoodUser", Iridio.MembershipProvider.IridioMembershipProvider.UpdateFailures.PasswordAnswer);
      serviceBus.Verify(x => x.Send(It.Is<SetUserFailedPasswordAnswerAttempt>(cmd => cmd.AggregateId == Utilities.Guid1 && cmd.FailedPasswordAnswerAttemptCount == 1 && cmd.FailedPasswordAnswerAttemptWindowStart > DateTime.MinValue)), Times.Once());
      serviceBus.Verify(x => x.Send(It.IsAny<SetUserFailedPasswordAttempt>()), Times.Never());
    }

    [Test]
    public void UpdateFailureCount_PAsswordAttempt_Exceed_Max_Attempts_Lock_User()
    {
      _nhProv.UpdateFailureCount("PasswordAttemptCount5", Iridio.MembershipProvider.IridioMembershipProvider.UpdateFailures.Password);
      serviceBus.Verify(x => x.Send(It.IsAny<SetUserFailedPasswordAnswerAttempt>()), Times.Never());
      serviceBus.Verify(x => x.Send(It.IsAny<SetUserFailedPasswordAttempt>()), Times.Never());
      serviceBus.Verify(x => x.Send(It.Is<LockUser>(cmd => cmd.AggregateId == Utilities.Guid3 && cmd.LastLockedOutDate > DateTime.MinValue)), Times.Once());
    }

    [Test]
    public void UpdateFailureCount_Bad_Password_Increment_Counter()
    {
      _nhProv.UpdateFailureCount("PasswordAttemptCount4", Iridio.MembershipProvider.IridioMembershipProvider.UpdateFailures.Password);
      serviceBus.Verify(x => x.Send(It.Is<SetUserFailedPasswordAttempt>(cmd => cmd.AggregateId == Utilities.Guid3 && cmd.FailedPasswordAttemptCount == 5 && cmd.FailedPasswordAttemptWindowStart > DateTime.MinValue)), Times.Once());
      serviceBus.Verify(x => x.Send(It.IsAny<SetUserFailedPasswordAnswerAttempt>()), Times.Never());
    }

    [Test]
    public void UpdateFailureCount_Bad_PasswordAnswer_Increment_Counter()
    {
      _nhProv.UpdateFailureCount("PasswordAnswerAttemptCount4", Iridio.MembershipProvider.IridioMembershipProvider.UpdateFailures.PasswordAnswer);
      serviceBus.Verify(x => x.Send(It.Is<SetUserFailedPasswordAnswerAttempt>(cmd => cmd.AggregateId == Utilities.Guid3 && cmd.FailedPasswordAnswerAttemptCount == 5 && cmd.FailedPasswordAnswerAttemptWindowStart > DateTime.MinValue)), Times.Once());
      serviceBus.Verify(x => x.Send(It.IsAny<SetUserFailedPasswordAttempt>()), Times.Never());
    }

    #endregion
  }
}
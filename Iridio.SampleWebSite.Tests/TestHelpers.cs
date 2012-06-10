using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Iridio.Infrastructure;
using Iridio.MembershipProvider;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Dtos;
using Moq;


namespace Iridio.Web.Management.Tests
{
  //Ritorno sempre un utente valido in quanto qui devo essere autenticato
  public static class FakeProfileHelpers
  {
    public static Guid fakeProfileGuid = Guid.NewGuid();
  }

  public class FakeServiceBus : IServiceBus
  {
    public void Send<T>(T command) where T : Iridio.Messages.Commands.Command
    {
      //Do nothing
    }

    public void RegisterHandler<T>(Action<T> handler) where T : Iridio.Messages.Events.Event
    {
      //Do nothing
    }
  }

  public class MockMembershipProvider : IridioMembershipProvider
  {
    public UserProfile fakeProfile;

    public MockMembershipProvider()
      : base(new FakeServiceBus(), Utilities.GetMockRepository(((MembershipSection)ConfigurationManager.GetSection("system.web/membership")).Providers["MockMembershipProvider"].Parameters["applicationName"]).Object)
    {
    }

    public override void Initialize(string name, NameValueCollection config)
    {
      base.Initialize("MockMembershipProvider", config);
      fakeProfile = new UserProfile();
    }

    public override System.Web.Security.MembershipUser GetUser(string userName, bool userIsOnline)
    {
      return new IridioMembershipUser("MockMembershipProvider", "FakeLoggedIn", 1, "aaa@aa.it", "", "", true, false, new DateTime(), new DateTime(), new DateTime(), new DateTime(), new DateTime(), fakeProfile);
    }
  }

  public static class Utilities
  {
    public static string BasePath = "c:\\temp\\";
    public static Guid Guid1 = Guid.NewGuid();
    public static Guid Guid999 = Guid.NewGuid();

    public static Mock<IUsersRepository> GetMockRepository(string appName)
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
      mockRepo.Setup(v => v.GetUserByName("EncryptUser", appName)).Returns(new User() { PasswordAnswer = "SerLEVf28XZ/mBLKLgqulBDfUK05rOsefCL0gd+WRDE=", Password = "Hei77AsDaWtwcvWYAJFawnB0X7BiukYVd+62O6lthNY=", IsApproved = true });
      mockRepo.Setup(v => v.GetUserByName("HashUser", appName)).Returns(new User() { PasswordAnswer = "/jGKx1DvdLPnZk1ZuQaz2fSFdws=", Password = "UAFsjFEtDHxMGwlRE/ICHnUehCs=", IsApproved = true });
      mockRepo.Setup(v => v.GetUserByName("NewUser", appName)).Returns((User)null);
      mockRepo.Setup(v => v.GetUserByName("ExceptionUser", appName)).Throws(new Exception());
      mockRepo.Setup(v => v.GetBy(Guid1)).Returns(goodUser);
      mockRepo.Setup(v => v.GetBy(Guid999)).Throws(new Exception());
      mockRepo.Setup(v => v.FindUsersByEmail("GoodEmail", 0, 99, appName)).Returns(GetStubUsers(1));
      mockRepo.Setup(v => v.FindUsersByEmail("BadEmail", 0, 99, appName)).Returns(new List<User>());
      mockRepo.Setup(v => v.FindUsersByEmail("DupEmail", 0, 99, appName)).Returns(GetStubUsers(2));
      mockRepo.Setup(v => v.FindUsersByEmail("ExceptionMail", 0, 99, appName)).Throws(new Exception());
      mockRepo.Setup(v => v.FindUsersByName("GoodName", 0, 99, appName)).Returns(GetStubUsers(1));
      mockRepo.Setup(v => v.FindUsersByName("BadName", 0, 99, appName)).Returns(new List<User>());
      mockRepo.Setup(v => v.FindUsersByName("DupName", 0, 99, appName)).Returns(GetStubUsers(2));
      mockRepo.Setup(v => v.FindUsersByName("ExceptionMail", 0, 99, appName)).Throws(new Exception());
      mockRepo.Setup(v => v.GetUserNameByEmail("NewEmail", appName)).Returns("");
      mockRepo.Setup(v => v.GetUserNameByEmail("DupEmail", appName)).Returns("DupUser");
      mockRepo.Setup(v => v.GetUserNameByEmail("ExceptionEmail", appName)).Throws(new Exception());
      int total = 10;
      mockRepo.Setup(v => v.GetUsers(0, 99, appName, out total)).Returns(GetStubUsers(2));
      mockRepo.Setup(v => v.GetUsers(1, 99, appName, out total)).Returns(new List<User>());
      mockRepo.Setup(v => v.GetUsers(2, 99, appName, out total)).Throws(new Exception());

      return mockRepo;
    }

    public static List<User> GetStubUsers(int numUsers)
    {
      var uList = new List<User>();
      for (int i = 0; i < numUsers; i++)
      {
        var u = new User();
        u.CreationDate = DateTime.Now;
        u.UserName = "SampleUser" + i;
        u.Id = Guid.NewGuid();
        uList.Add(u);
      }
      return uList;
    }

    public static Mock<HttpContextBase> MockControllerContext(bool authenticated, bool isAjaxRequest)
    {
      var request = new Mock<HttpRequestBase>();
      request.SetupGet(r => r.HttpMethod).Returns("GET");
      request.SetupGet(r => r.IsAuthenticated).Returns(authenticated);
      request.SetupGet(r => r.ApplicationPath).Returns("/");
      request.SetupGet(r => r.ServerVariables).Returns((NameValueCollection)null);
      request.SetupGet(r => r.Url).Returns(new Uri("http://localhost/app", UriKind.Absolute));
      if (isAjaxRequest)
        request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });

      var server = new Mock<HttpServerUtilityBase>();
      server.Setup(x => x.MapPath(It.IsAny<string>())).Returns(BasePath);

      var response = new Mock<HttpResponseBase>();
      response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns((String url) => url);

      var session = new MockHttpSession();

      var mockHttpContext = new Mock<HttpContextBase>();
      mockHttpContext.Setup(c => c.Request).Returns(request.Object);
      mockHttpContext.Setup(c => c.Response).Returns(response.Object);
      mockHttpContext.Setup(c => c.Server).Returns(server.Object);
      mockHttpContext.Setup(x => x.Session).Returns(session);

      return mockHttpContext;
    }

    public static Mock<HttpContextBase> MockControllerContextWithFiles(bool authenticated, bool isAjaxRequest, string fileName)
    {
      var request = new Mock<HttpRequestBase>();
      request.SetupGet(r => r.HttpMethod).Returns("GET");
      request.SetupGet(r => r.IsAuthenticated).Returns(authenticated);
      request.SetupGet(r => r.ApplicationPath).Returns("/");
      request.SetupGet(r => r.ServerVariables).Returns((NameValueCollection)null);
      request.SetupGet(r => r.Url).Returns(new Uri("http://localhost/app", UriKind.Absolute));
      if (isAjaxRequest)
        request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });

      UTF8Encoding enc = new UTF8Encoding();
      Mock<HttpPostedFileBase> file = new Mock<HttpPostedFileBase>();
      file.Setup(d => d.FileName).Returns(fileName);
      file.Setup(d => d.InputStream).Returns(GetImageStream(ImageFormat.Jpeg));
      file.Setup(d => d.ContentLength).Returns(10);
      file.Setup(d => d.ContentType).Returns("multipart");

      var postedfilesKeyCollection = new Mock<HttpFileCollectionBase>();
      var fakeFileKeys = new List<string>() { "file" };
      postedfilesKeyCollection.Setup(keys => keys.GetEnumerator()).Returns(fakeFileKeys.GetEnumerator());
      postedfilesKeyCollection.Setup(keys => keys["file"]).Returns(file.Object);
      request.Setup(d => d.Files).Returns(postedfilesKeyCollection.Object);
      request.Setup(d => d.Files.Count).Returns(1);
      request.Setup(d => d.Files[0]).Returns(file.Object);
      var server = new Mock<HttpServerUtilityBase>();
      server.Setup(x => x.MapPath(It.IsAny<string>())).Returns(BasePath);

      var response = new Mock<HttpResponseBase>();
      response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns((String url) => url);

      var session = new MockHttpSession();

      var mockHttpContext = new Mock<HttpContextBase>();
      mockHttpContext.Setup(c => c.Request).Returns(request.Object);
      mockHttpContext.Setup(c => c.Response).Returns(response.Object);
      mockHttpContext.Setup(c => c.Server).Returns(server.Object);
      mockHttpContext.Setup(x => x.Session).Returns(session);

      return mockHttpContext;
    }

    public static Mock<HttpContextBase> MockControllerContextWithFilesEmpty(bool authenticated, bool isAjaxRequest)
    {
      var request = new Mock<HttpRequestBase>();
      request.SetupGet(r => r.HttpMethod).Returns("GET");
      request.SetupGet(r => r.IsAuthenticated).Returns(authenticated);
      request.SetupGet(r => r.ApplicationPath).Returns("/");
      request.SetupGet(r => r.ServerVariables).Returns((NameValueCollection)null);
      request.SetupGet(r => r.Url).Returns(new Uri("http://localhost/app", UriKind.Absolute));
      if (isAjaxRequest)
        request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
      //questo per accesso con foreach
      var postedfilesKeyCollection = new Mock<HttpFileCollectionBase>();
      var fakeFileKeys = new List<string>();
      postedfilesKeyCollection.Setup(keys => keys.GetEnumerator()).Returns(fakeFileKeys.GetEnumerator());
      //postedfilesKeyCollection.Setup(keys => keys["file"]).Returns(file.Object);
      request.Setup(d => d.Files).Returns(postedfilesKeyCollection.Object);
      //questo per accesso con index
      request.Setup(d => d.Files.Count).Returns(0);
      request.Setup(d => d.Files[0]).Returns((HttpPostedFileBase)null);
      var server = new Mock<HttpServerUtilityBase>();
      server.Setup(x => x.MapPath(It.IsAny<string>())).Returns(BasePath);

      var response = new Mock<HttpResponseBase>();
      response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns((String url) => url);

      var session = new MockHttpSession();

      var mockHttpContext = new Mock<HttpContextBase>();
      mockHttpContext.Setup(c => c.Request).Returns(request.Object);
      mockHttpContext.Setup(c => c.Response).Returns(response.Object);
      mockHttpContext.Setup(c => c.Server).Returns(server.Object);
      mockHttpContext.Setup(c => c.Session).Returns(session);

      return mockHttpContext;
    }

    public static Stream GetImageStream(ImageFormat format)
    {
      Image img = new Bitmap(1000, 1000, PixelFormat.Format32bppRgb);
      MemoryStream ms = new MemoryStream();
      img.Save(ms, format);
      return ms;
    }

    public static Image GetImage()
    {
      Image canvas = new Bitmap(50, 50);
      Graphics artist = Graphics.FromImage(canvas);
      artist.Clear(Color.Lime);
      artist.FillEllipse(Brushes.Red, 3, 30, 30, 30);
      artist.Dispose();
      return canvas;
    }

    public class MockHttpSession : HttpSessionStateBase
    {
      Dictionary<string, object> sessionStorage = new Dictionary<string, object>();

      public override object this[string name]
      {
        get { return sessionStorage.ContainsKey(name) ? sessionStorage[name] : null; }
        set { sessionStorage[name] = value; }
      }
      public override void Remove(string name)
      {
        sessionStorage.Remove(name);
        //base.Remove(name);
      }
    }

  }
}
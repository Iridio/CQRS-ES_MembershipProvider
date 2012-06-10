using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Iridio.ReadModel.Abstracts;
using Iridio.SampleWebSite.Controllers;
using Moq;
using NUnit.Framework;
using Iridio.SampleWebSite.Models;

namespace Iridio.Web.Management.Tests.Controllers
{
  [TestFixture]
  public class AccountControllerTests
  {
    private static Mock<IUsersService> mockUsersService;

    [SetUp]
    public void Setup()
    {
      mockUsersService = new Mock<IUsersService>();
    }

    private static AccountController GetAccountController()
    {
      RequestContext requestContext = new RequestContext(new MockHttpContext(), new RouteData());
      AccountController controller = new AccountController(mockUsersService.Object, new MockFormsAuthenticationService(), new MockMembershipService())
      {
        Url = new UrlHelper(requestContext)
      };
      controller.ControllerContext = new ControllerContext()
      {
        Controller = controller,
        RequestContext = requestContext
      };
      return controller;
    }

    [Test]
    public void ChangePassword_Get_ReturnsView()
    {
      AccountController controller = GetAccountController();
      ActionResult result = controller.ChangePassword();
      Assert.IsInstanceOf(typeof(ViewResult), result);
      Assert.AreEqual(10, ((ViewResult)result).ViewData["PasswordLength"]);
    }

    [Test]
    public void ChangePassword_Post_HaveValidateAntiForgeryTokenAttribute()
    {
      AccountController controller = GetAccountController();
      var type = controller.GetType();
      var methodInfo = type.GetMethod("ChangePassword", new Type[1] { typeof(ChangePasswordModel) });
      var attributes = methodInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true);
      Assert.IsTrue(attributes.Any());
    }

    [Test]
    public void ChangePassword_Post_ReturnsRedirectOnSuccess()
    {
      AccountController controller = GetAccountController();
      ChangePasswordModel model = new ChangePasswordModel()
      {
        OldPassword = "goodOldPassword",
        NewPassword = "goodNewPassword",
        ConfirmPassword = "goodNewPassword"
      };
      ActionResult result = controller.ChangePassword(model);
      Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
      RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
      Assert.AreEqual("ChangePasswordSuccess", redirectResult.RouteValues["action"]);
    }

    [Test]
    public void ChangePassword_Post_ReturnsViewIfChangePasswordFails()
    {
      AccountController controller = GetAccountController();
      ChangePasswordModel model = new ChangePasswordModel()
      {
        OldPassword = "goodOldPassword",
        NewPassword = "badNewPassword",
        ConfirmPassword = "badNewPassword"
      };
      ActionResult result = controller.ChangePassword(model);
      Assert.IsInstanceOf(typeof(ViewResult), result);
      ViewResult viewResult = (ViewResult)result;
      Assert.AreEqual(model, viewResult.ViewData.Model);
      Assert.AreEqual(Resources.WebSite.Controllers.IncorrectPassword, controller.ModelState[""].Errors[0].ErrorMessage);
      Assert.AreEqual(10, viewResult.ViewData["PasswordLength"]);
    }

    [Test]
    public void ChangePassword_Post_ReturnsViewIfModelStateIsInvalid()
    {
      AccountController controller = GetAccountController();
      ChangePasswordModel model = new ChangePasswordModel()
      {
        OldPassword = "goodOldPassword",
        NewPassword = "goodNewPassword",
        ConfirmPassword = "goodNewPassword"
      };
      controller.ModelState.AddModelError("", "Dummy error message.");
      ActionResult result = controller.ChangePassword(model);
      Assert.IsInstanceOf(typeof(ViewResult), result);
      ViewResult viewResult = (ViewResult)result;
      Assert.AreEqual(model, viewResult.ViewData.Model);
      Assert.AreEqual(10, viewResult.ViewData["PasswordLength"]);
    }

    [Test]
    public void ChangePasswordSuccess_ReturnsView()
    {
      AccountController controller = GetAccountController();
      ActionResult result = controller.ChangePasswordSuccess();
      Assert.IsInstanceOf(typeof(ViewResult), result);
    }

    [Test]
    public void LogOff_LogsOutAndRedirects()
    {
      AccountController controller = GetAccountController();
      ActionResult result = controller.LogOff();
      Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
      RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
      Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
      Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
      Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignOut_WasCalled);
    }

    [Test]
    public void LogOn_Get_ReturnsView()
    {
      AccountController controller = GetAccountController();
      ActionResult result = controller.LogOn();
      Assert.IsInstanceOf(typeof(ViewResult), result);
    }

    [Test]
    public void LogOn_Post_HaveValidateAntiForgeryTokenAttribute()
    {
      AccountController controller = GetAccountController();
      var type = controller.GetType();
      var methodInfo = type.GetMethod("LogOn", new Type[2] { typeof(LogOnModel), typeof(string) });
      var attributes = methodInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true);
      Assert.IsTrue(attributes.Any());
    }

    [Test]
    public void LogOn_Post_ReturnsRedirectOnSuccess_WithoutReturnUrl()
    {
      AccountController controller = GetAccountController();
      var httpContext = Utilities.MockControllerContext(false, false).Object;
      controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);
      LogOnModel model = new LogOnModel()
      {
        UserName = "someUser",
        Password = "goodPassword",
        RememberMe = false
      };
      ActionResult result = controller.LogOn(model, null);
      Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
      RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
      Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
      Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
      Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignIn_WasCalled);
    }

    [Test]
    public void LogOn_Post_ReturnsRedirectOnSuccess_WithLocalReturnUrl()
    {
      AccountController controller = GetAccountController();
      var httpContext = Utilities.MockControllerContext(false, false).Object;
      controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);
      LogOnModel model = new LogOnModel()
      {
        UserName = "someUser",
        Password = "goodPassword",
        RememberMe = false
      };
      ActionResult result = controller.LogOn(model, "/someUrl");
      Assert.IsInstanceOf(typeof(RedirectResult), result);
      RedirectResult redirectResult = (RedirectResult)result;
      Assert.AreEqual("/someUrl", redirectResult.Url);
      Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignIn_WasCalled);
    }

    [Test]
    public void LogOn_Post_ReturnsRedirectToHomeOnSuccess_WithExternalReturnUrl()
    {
      AccountController controller = GetAccountController();
      var httpContext = Utilities.MockControllerContext(false, false).Object;
      controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);
      LogOnModel model = new LogOnModel()
      {
        UserName = "someUser",
        Password = "goodPassword",
        RememberMe = false
      };
      ActionResult result = controller.LogOn(model, "http://malicious.example.net");
      Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
      RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
      Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
      Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
      Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignIn_WasCalled);
    }

    [Test]
    public void LogOn_Post_ReturnsViewIfModelStateIsInvalid()
    {
      AccountController controller = GetAccountController();
      LogOnModel model = new LogOnModel()
      {
        UserName = "someUser",
        Password = "goodPassword",
        RememberMe = false
      };
      controller.ModelState.AddModelError("", "Dummy error message.");
      ActionResult result = controller.LogOn(model, null);
      Assert.IsInstanceOf(typeof(ViewResult), result);
      ViewResult viewResult = (ViewResult)result;
      Assert.AreEqual(model, viewResult.ViewData.Model);
    }

    [Test]
    public void LogOn_Post_ReturnsViewIfValidateUserFails()
    {
      AccountController controller = GetAccountController();
      LogOnModel model = new LogOnModel()
      {
        UserName = "someUser",
        Password = "badPassword",
        RememberMe = false
      };
      ActionResult result = controller.LogOn(model, null);
      Assert.IsInstanceOf(typeof(ViewResult), result);
      ViewResult viewResult = (ViewResult)result;
      Assert.AreEqual(model, viewResult.ViewData.Model);
      Assert.AreEqual(Resources.WebSite.Controllers.IncorrectUserOrPassword, controller.ModelState[""].Errors[0].ErrorMessage);
    }

    [Test]
    public void ResetPassword_Post_HaveValidateAntiForgeryTokenAttribute()
    {
      AccountController controller = GetAccountController();
      var type = controller.GetType();
      var methodInfo = type.GetMethod("ResetPassword", new Type[1] { typeof(ResetPasswordModel) });
      var attributes = methodInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true);
      Assert.IsTrue(attributes.Any());
    }

    [Test]
    public void ResetPassword_Post_ErrorOnUserNameNotFound()
    {
      AccountController controller = GetAccountController();
      ResetPasswordModel model = new ResetPasswordModel() { UserName = "badEmail" };
      ActionResult result = controller.ResetPassword(model);
      Assert.IsInstanceOf(typeof(ViewResult), result);
      ViewResult viewResult = (ViewResult)result;
      Assert.AreEqual(model, viewResult.ViewData.Model);
      Assert.AreEqual(1, controller.ModelState.Values.Count);
    }

    [Test]
    public void ResetPassword_Post_GenerateNewPasswordAndReturnsRedirectOnSuccess()
    {
      //Non posso verificare l'invio della mail da qui
      //mockUsersService.Setup(x => x.SendResetPasswordToEmail(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
      AccountController controller = GetAccountController();
      ResetPasswordModel model = new ResetPasswordModel() { UserName = "goodEmail" };
      ActionResult result = controller.ResetPassword(model);
      Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
      RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
      Assert.AreEqual("ResetPasswordSuccess", redirectResult.RouteValues["action"]);
    }

    [Test]
    public void ResetPasswordSuccess_ReturnsView()
    {
      AccountController controller = GetAccountController();
      ActionResult result = controller.ResetPasswordSuccess();
      Assert.IsInstanceOf(typeof(ViewResult), result);
    }

    private class MockFormsAuthenticationService : IFormsAuthenticationService
    {
      public bool SignIn_WasCalled;
      public bool SignOut_WasCalled;

      public void SignIn(string userName, bool createPersistentCookie)
      {
        // verify that the arguments are what we expected
        Assert.AreEqual("someUser", userName);
        Assert.IsFalse(createPersistentCookie);
        SignIn_WasCalled = true;
      }

      public void SignOut()
      {
        SignOut_WasCalled = true;
      }
    }

    private class MockHttpContext : HttpContextBase
    {
      private readonly IPrincipal _user = new GenericPrincipal(new GenericIdentity("someUser"), null /* roles */);
      private readonly HttpRequestBase _request = new MockHttpRequest();

      public override IPrincipal User
      {
        get
        {
          return _user;
        }
        set
        {
          base.User = value;
        }
      }

      public override HttpRequestBase Request
      {
        get
        {
          return _request;
        }
      }
    }

    private class MockHttpRequest : HttpRequestBase
    {
      private readonly Uri _url = new Uri("http://mysite.example.com/");

      public override Uri Url
      {
        get
        {
          return _url;
        }
      }
    }

    private class MockMembershipService : IMembershipService
    {
      public int MinPasswordLength
      {
        get { return 10; }
      }

      public bool ValidateUser(string userName, string password)
      {
        return (userName == "someUser" && password == "goodPassword");
      }

      public MembershipCreateStatus CreateUser(string userName, string password, string email)
      {
        if (userName == "duplicateUser")
        {
          return MembershipCreateStatus.DuplicateUserName;
        }

        // verify that values are what we expected
        Assert.AreEqual("goodPassword", password);
        Assert.AreEqual("goodEmail", email);

        return MembershipCreateStatus.Success;
      }

      public bool ChangePassword(string userName, string oldPassword, string newPassword)
      {
        return (userName == "someUser" && oldPassword == "goodOldPassword" && newPassword == "goodNewPassword");
      }

      public bool ResetPassword(string userName, string newPassword)
      {
        return (userName == "goodEmail" && !String.IsNullOrEmpty(newPassword));
      }
    }
  }
}

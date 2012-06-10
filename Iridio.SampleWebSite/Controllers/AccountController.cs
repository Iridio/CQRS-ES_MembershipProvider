using System;
using System.Web.Mvc;
using System.Web.Security;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Dtos;
using Iridio.SampleWebSite.Models;

namespace Iridio.SampleWebSite.Controllers
{
  public class AccountController : BaseController
  {
    private readonly IUsersService usersService;
    public readonly IFormsAuthenticationService FormsService;
    public readonly IMembershipService MembershipService;

    //TODO: Remove the bastard injection and implement only Constructor injection
    public AccountController(IUsersService usersService)
      : this(usersService, new FormsAuthenticationService(), new AccountMembershipService(usersService))
    {
    }

    public AccountController(IUsersService usersService, IFormsAuthenticationService formsService, IMembershipService membershipService)
    {
      this.usersService = usersService;
      this.FormsService = formsService;
      this.MembershipService = membershipService;
    }

    public ActionResult LogOn()
    {
      return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public ActionResult LogOn(LogOnModel model, string returnUrl)
    {
      if (ModelState.IsValid)
      {
        if (MembershipService.ValidateUser(model.UserName, model.Password))
        {
          FormsService.SignIn(model.UserName, model.RememberMe);
          if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            return Redirect(returnUrl);
          else
            return RedirectToAction("Index", "Home");
        }
        else
        {
          ModelState.AddModelError("", Resources.WebSite.Controllers.IncorrectUserOrPassword);
        }
      }
      return View(model);
    }

    public ActionResult LogOff()
    {
      FormsService.SignOut();
      return RedirectToAction("Index", "Home");
    }

    public ActionResult Register()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);
        if (createStatus == MembershipCreateStatus.Success)
        {
          FormsService.SignIn(model.UserName, false);
          return RedirectToAction("Index", "Home");
        }
        else
        {
          ModelState.AddModelError("", ErrorCodeToString(createStatus));
        }
      }
      return View(model);
    }

    [Authorize]
    public ActionResult ChangePassword()
    {
      ViewBag.PasswordLength = MembershipService.MinPasswordLength;
      return View();
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public ActionResult ChangePassword(ChangePasswordModel model)
    {
      if (ModelState.IsValid)
      {
        bool changePasswordSucceeded;
        try
        {
          changePasswordSucceeded = MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
        }
        catch (Exception)
        {
          changePasswordSucceeded = false;
        }
        if (changePasswordSucceeded)
        {
          return RedirectToAction("ChangePasswordSuccess");
        }
        else
        {
          ModelState.AddModelError("", Resources.WebSite.Controllers.IncorrectPassword);
        }
      }
      ViewBag.PasswordLength = MembershipService.MinPasswordLength;
      return View(model);
    }

    public ActionResult ChangePasswordSuccess()
    {
      return View();
    }

    public ActionResult ResetPassword()
    {
      return View(new ResetPasswordModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public ActionResult ResetPassword(ResetPasswordModel model)
    {
      if (ModelState.IsValid)
      {
        if (MembershipService.ResetPassword(model.UserName, model.NewPassword))
          return RedirectToAction("ResetPasswordSuccess");
        else
          ModelState.AddModelError("", Resources.WebSite.Controllers.UserNotFound);
      }
      return View(model);
    }

    public ViewResult ResetPasswordSuccess()
    {
      return View();
    }

    #region Status Codes
    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://go.microsoft.com/fwlink/?LinkID=177550 for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "User name already exists. Please enter a different user name.";

        case MembershipCreateStatus.DuplicateEmail:
          return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
    #endregion
  }
}

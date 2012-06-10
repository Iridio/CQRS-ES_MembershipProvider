using System.Web.Mvc;
using NUnit.Framework;
using Iridio.SampleWebSite.Controllers;

namespace Iridio.Web.Management.Tests.Controllers
{
  [TestFixture]
  public class HomeControllerTest
  {
    [Test]
    public void Index()
    {
      HomeController controller = new HomeController();
      ViewResult result = controller.Index() as ViewResult;
      Assert.IsNotNull(result);
    }
  }
}

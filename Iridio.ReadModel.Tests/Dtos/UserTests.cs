using NUnit.Framework;
using Iridio.ReadModel.Dtos;

namespace Iridio.DomainModel.Tests.Entities
{
  [TestFixture]
  public class UserTests
  {
    [Test]
    public void Create_Initialize_Profile()
    {
      User user = new User();
      Assert.IsNotNull(user.Profile);
    }
  }
}

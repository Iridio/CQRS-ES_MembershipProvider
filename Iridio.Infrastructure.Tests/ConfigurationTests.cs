using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Iridio.Infrastructure.Tests
{
  [TestFixture]
  public class ConfigurationTests
  {
    [Test]
    public void Configuration_Get_ReadModelConnection()
    {
      Assert.AreEqual("Data Source=.\\SqlExpress;Initial Catalog=Iridio;Integrated Security=SSPI;", Configuration.ReadModelConnection);
    }

    [Test]
    public void Configuration_Get_SmtpSection()
    {
      Assert.AreEqual("mysmtp", Configuration.Smtp.Host);
      Assert.AreEqual(25, Configuration.Smtp.Port);
    }

    [Test]
    public void Configuration_Get_EmailSection()
    {
      Assert.AreEqual("no-reply@samplewebsite.com", Configuration.Email.ServiceFrom);
      Assert.AreEqual("sample wbsite", Configuration.Email.ServiceAlias);
    }
  }
}

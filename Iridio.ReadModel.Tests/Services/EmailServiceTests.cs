using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Moq;
using NUnit.Framework;
using Iridio.ReadModel.Abstracts;

namespace Iridio.ReadModel.Services.Tests
{
  /// <summary>
  /// Summary description for EmailServiceTests
  /// </summary>
  [TestFixture]
  public class EmailServiceTests
  {
    private ISmtpClient smtpClient;

    [SetUp]
    public void SetUp()
    {
      var mock = new Mock<ISmtpClient>();
      mock.Setup(x => x.Send(It.IsAny<MailMessage>()));
      smtpClient = mock.Object;
    }

    [Test]
    public void ShouldSendEmailOnlyText()
    {
      var service = new EmailService(smtpClient, "info@nohost.doh", "Alias");
      var result = service.Send("pippo@nohost.doh", "prova", "body of message", false);
      Assert.AreEqual(true, result);
    }

    [Test]
    public void ShouldSendEmailWithAttachemnt()
    {
      var service = new EmailService(smtpClient, "info@nohost.doh", "Alias");
      var result = service.Send("pippo@nohost.doh", "prova", "body of message", false, new List<Attachment> { new Attachment(new MemoryStream(), "file1") });
      Assert.AreEqual(true, result);
    }

    [Test]
    public void ShouldReturnFalseOnError()
    {
      var mock = new Mock<ISmtpClient>();
      mock.Setup(x => x.Send(It.IsAny<MailMessage>())).Throws(new Exception());
      var smtpException = mock.Object;
      var service = new EmailService(smtpException, "info@nohost.doh", "Alias");
      var result = service.Send("pippo@nohost.doh", "prova", "body of message", false, new List<Attachment> { new Attachment(new MemoryStream(), "file1") });
      Assert.AreEqual(false, result);
    }
  }
}

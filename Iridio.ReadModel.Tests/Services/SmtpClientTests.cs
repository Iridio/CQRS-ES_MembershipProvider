using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using Iridio.ReadModel.Abstracts;
using NUnit.Framework;

namespace Iridio.ReadModel.Services.Tests
{
  [TestFixture]
  public class SmtpClientTests
  {
    //Salva su c:\temp la mail. Vedere app.config di questo progetto se si vuole cambiare

    [Test]
    public void ShouldSendEmailOnlyText()
    {
      ISmtpClient service = new Iridio.ReadModel.Abstracts.SmtpClient("localhost", 25);
      var message = new MailMessage("sender@nohost.doh", "receiver@nohost.doh", "prova", "body of message");
      service.Send(message);
    }

    [Test]
    public void ShouldSendEmailWithAttachemnt()
    {
      ISmtpClient service = new Iridio.ReadModel.Abstracts.SmtpClient("localhost", 25);
      var valore = "prima riga" + System.Environment.NewLine + "seconda riga";
      var array = Encoding.UTF8.GetBytes(valore);
      using (MemoryStream ms = new MemoryStream(array))
      {
        var message = new MailMessage("sender@nohost.doh", "receiver@nohost.doh", "segnalazione", "body");
        message.Attachments.Add(new Attachment(ms, "segnalazione_test.txt"));
        service.Send(message);
      }
    }

    [Test, ExpectedException(typeof(ArgumentException))]
    public void EmailEmptyDoNotSend()
    {
      ISmtpClient service = new Iridio.ReadModel.Abstracts.SmtpClient("localhost", 25);
      var message = new MailMessage("sender@nohost.doh", "", "prova", "body of message");
      service.Send(message);
    }
  }
}

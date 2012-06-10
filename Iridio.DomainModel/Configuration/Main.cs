using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;

namespace Iridio.DomainModel.Configuration
{
  public static class Main
  {
    private static readonly NameValueCollection config = (NameValueCollection)ConfigurationManager.GetSection("Iridio/Main");

    private static string eventStoreConnectionName;
    public static string EventStoreConnectionName
    {
      get
      {
        if (string.IsNullOrEmpty(eventStoreConnectionName))
          eventStoreConnectionName = config["EventStoreConnectionName"];
        return eventStoreConnectionName;
      }
    }
    private static string smtpHost;
    public static string SmtpHost
    {
      get
      {
        if (string.IsNullOrEmpty(smtpHost))
          smtpHost = config["SmtpHost"];
        return smtpHost;
      }
    }
    private static int smtpPort;
    public static int SmtpPort
    {
      get
      {
        if (smtpPort == 0)
          smtpPort = int.Parse(config["SmtpPort"]);
        return smtpPort;
      }
    }

    private static string emailServiceFrom;
    public static string EmailServiceFrom
    {
      get
      {
        if (string.IsNullOrEmpty(emailServiceFrom))
          emailServiceFrom = config["EmailServiceFrom"];
        return emailServiceFrom;
      }
    }

    private static string emailServiceAlias;
    public static string EmailServiceAlias
    {
      get
      {
        if (string.IsNullOrEmpty(emailServiceAlias))
          emailServiceAlias = config["EmailServiceAlias"];
        return emailServiceAlias;
      }
    }
  }
}

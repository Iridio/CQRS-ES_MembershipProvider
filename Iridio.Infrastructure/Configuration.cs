using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

namespace Iridio.Infrastructure
{
  public static partial class Configuration
  {
    private const string GroupName = "Iridio/";

    private static string readModelConnection;
    public static string ReadModelConnection
    {
      get
      {
        if (string.IsNullOrEmpty(readModelConnection))
          readModelConnection = ConfigurationManager.ConnectionStrings["ReadModel"].ConnectionString;
        return readModelConnection;
      }
    }

    private static SmtpSectionHandler smtp;
    public static SmtpSectionHandler Smtp
    {
      get
      {
        if (smtp == null)
          smtp = ConfigurationManager.GetSection(GroupName + "Smtp") as SmtpSectionHandler;
        return smtp;
      }
    }

    private static EmailSectionHandler email; //TODO: GetSection should already use static, double check and then remove this eventually
    public static EmailSectionHandler Email
    {
      get
      {
        if (email == null)
          email = ConfigurationManager.GetSection(GroupName + "Email") as EmailSectionHandler;
        return email;
      }
    }
  }

  public class SmtpSectionHandler : ConfigurationSection
  {
    [ConfigurationProperty("host", DefaultValue = "mail.smtpserver.it", IsRequired = false)]
    public string Host
    {
      get
      {
        return (string)this["host"];
      }
      set
      {
        this["host"] = value;
      }
    }

    [ConfigurationProperty("port", DefaultValue = "25", IsRequired = true)]
    [IntegerValidator(ExcludeRange = false, MaxValue = 9999, MinValue = 1)]
    public int Port
    {
      get
      {
        return (int)this["port"];
      }
      set
      {
        this["port"] = value;
      }
    }
  }

  public class EmailSectionHandler : ConfigurationSection
  {
    [ConfigurationProperty("serviceFrom", DefaultValue = "noreply@samplewebsite.it", IsRequired = true)]
    public string ServiceFrom
    {
      get
      {
        return (string)this["serviceFrom"];
      }
      set
      {
        this["serviceFrom"] = value;
      }
    }

    [ConfigurationProperty("serviceAlias", DefaultValue = "noreply", IsRequired = true)]
    public string ServiceAlias
    {
      get
      {
        return (string)this["serviceAlias"];
      }
      set
      {
        this["serviceAlias"] = value;
      }
    }
  }
}

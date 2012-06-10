using System;
using System.IO;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Iridio.ReadModel.NHibernate
{
  public abstract class NHSessionBuilder : INHSessionBuilder
  {
    private static ISessionFactory _sessionFactory;
    private static ISession _currentSession;
    private static Configuration _configuration;

    public void BuildSchema(ISession session, bool createDB, string path, string prefix)
    {
      SchemaExport schemaExport = new SchemaExport(_configuration);
      if (!path.EndsWith("\\"))
        path += "\\";
      FileInfo t = new FileInfo(path + prefix + "_" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + ".sql");
      StreamWriter writer = t.CreateText();
      schemaExport.Execute(true, createDB, false, session.Connection, writer);
      writer.Close();
    }

    public ISession GetSession()
    {
      ISessionFactory factory = getSessionFactory();
      ISession session = getExistingOrNewSession(factory);
      return session;
    }

    private ISessionFactory getSessionFactory()
    {
      if (_sessionFactory == null)
      {
        _configuration = GetConfiguration();
        _sessionFactory = _configuration.BuildSessionFactory();
      }
      return _sessionFactory;
    }

    public abstract Configuration GetConfiguration();

    private ISession getExistingOrNewSession(ISessionFactory factory)
    {
      if (HttpContext.Current != null)
      {
        ISession session = GetExistingWebSession();
        if (session == null)
        {
          session = openSessionAndAddToContext(factory);
        }
        else if (!session.IsOpen)
        {
          session = openSessionAndAddToContext(factory);
        }
        session.FlushMode = FlushMode.Commit;
        return session;
      }

      if (_currentSession == null)
      {
        _currentSession = factory.OpenSession();
      }
      else if (!_currentSession.IsOpen)
      {
        _currentSession = factory.OpenSession();
      }
      _currentSession.FlushMode = FlushMode.Commit;
      return _currentSession;
    }

    public ISession GetExistingWebSession()
    {
      return HttpContext.Current.Items[GetType().FullName] as ISession;
    }

    private ISession openSessionAndAddToContext(ISessionFactory factory)
    {
      ISession session = factory.OpenSession();
      HttpContext.Current.Items.Remove(GetType().FullName);
      HttpContext.Current.Items.Add(GetType().FullName, session);
      return session;
    }
  }
}
using System;
using NH = NHibernate;
using NHCfg = NHibernate.Cfg;
using System.Web;
using Iridio.ReadModel.NHibernate.Mappings;
using NHibernate.Mapping.ByCode;


namespace Iridio.ReadModel.NHibernate
{
  public class NHSqlServer : NHSessionBuilder, IDisposable
  {
    private readonly string connectionString;

    public NHSqlServer(string connectionString)
    {
      this.connectionString = connectionString;
    }

    public NHSqlServer()
      : this("")
    {
    }

    public override NHCfg.Configuration GetConfiguration()
    {
      var config = new NHCfg.Configuration();
      NHCfg.Environment.UseReflectionOptimizer = true; //TODO: Something like Utility.GetTrustLevel() > System.Web.AspNetHostingPermissionLevel.Medium would be nice
      config.Properties[NHCfg.Environment.Dialect] = typeof(NH.Dialect.MsSql2008Dialect).AssemblyQualifiedName;
      config.Properties[NHCfg.Environment.ConnectionDriver] = typeof(NH.Driver.SqlClientDriver).AssemblyQualifiedName;
      config.Properties[NHCfg.Environment.Isolation] = "ReadCommitted";
      config.Properties[NHCfg.Environment.ConnectionString] = connectionString;
      config.Properties[NHCfg.Environment.FormatSql] = "true";
      config.Properties[NHCfg.Environment.QuerySubstitutions] = "true 1, false 0, yes 'Y', no 'N'";
      var mapper = new ModelMapper();
      mapper.AddMappings(typeof(UserMap).Assembly.GetTypes());
      config.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
      //config.Properties[NHCfg.Environment.UseSecondLevelCache] = "false"; 
      //config.Properties[NHCfg.Environment.UseQueryCache] = "false";
      //config.Properties[NHCfg.Environment.CacheProvider] = "";
      return config;
    }

    public void Dispose()
    {
      var session = GetSession();
      if (session != null)
      {
        try
        {
          if (session.IsOpen)
            session.Close();
        }
        catch
        {
          //noop
        }
      }
      if (HttpContext.Current != null)
        HttpContext.Current.Items.Remove(GetType().FullName);
    }
  }
}
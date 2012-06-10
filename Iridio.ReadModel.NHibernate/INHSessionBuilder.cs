using NHibernate;
using NHibernate.Cfg;

namespace Iridio.ReadModel.NHibernate
{
  public interface INHSessionBuilder
  {
    ISession GetSession();
    Configuration GetConfiguration();
    void BuildSchema(ISession session, bool createDB, string path, string prefix);
  }
}
using Autofac;
using Autofac.Integration.Mvc;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Concrete;
using Iridio.ReadModel.Dtos;
using Iridio.ReadModel.NHibernate;
using Iridio.ReadModel.NHibernate.Persistors;
using NHibernate;

namespace Iridio.Infrastructure.Modules
{
  public class ReadModelModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      var factory = new NHSqlServer(Configuration.ReadModelConnection);
      builder.RegisterInstance(factory);
      builder.Register(c => factory.GetSession()).As<ISession>().InstancePerDependency();
      builder.Register(c => new NHPersistorBase(factory)).As<IPersistor>().SingleInstance();
      builder.Register(c => new NHUsersPersistor(factory)).As<IUsersPersistor>().SingleInstance();
      builder.RegisterType<UsersRepository>().As<IUsersRepository>().SingleInstance();//TODO: put the correct lifetimes
      builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();
    }
  }
}

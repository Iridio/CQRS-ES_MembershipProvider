using System.Web.Mvc;
using Autofac;
using Iridio.Infrastructure.Modules;
using Autofac.Integration.Mvc;
using Iridio.Infrastructure;

namespace Iridio.SampleWebSite
{
  public static class AutofacBootstrapper
  {
    public static void Bootstrap()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterControllers(typeof(MvcApplication).Assembly);
      builder.RegisterModule(new SqlServerEventStoreModule());
      builder.RegisterModule(new ReadModelModule());
      builder.RegisterModule(new CommandHandlersModule());
      builder.RegisterModule(new EventHandlersModule());
      builder.RegisterModule(new BusModule());
      builder.RegisterModule(new ServicesModule());

      var container = builder.Build();
      RegisterEventHandlerInBus.Boot(container);
      DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
    }
  }
}
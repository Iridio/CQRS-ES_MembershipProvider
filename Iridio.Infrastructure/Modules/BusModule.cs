using Autofac;
using EventStore.Dispatcher;

namespace Iridio.Infrastructure.Modules
{
  public class BusModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.Register(c => new InProcessBus(c.Resolve<IComponentContext>())).As<IServiceBus>().As<IDispatchCommits>().SingleInstance();
    }
  }
}

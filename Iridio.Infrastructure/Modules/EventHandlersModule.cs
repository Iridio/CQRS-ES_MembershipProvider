using Autofac;
using Iridio.ReadModel.EventsHandlers;

namespace Iridio.Infrastructure.Modules
{
  public class EventHandlersModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterAssemblyTypes(typeof(EventsHandler).Assembly).AsClosedTypesOf(typeof(IHandleEvent<>)).SingleInstance();
    }
  }
}

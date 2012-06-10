using Autofac;
using Iridio.DomainModel.CommandHandlers;

namespace Iridio.Infrastructure.Modules
{
  public class CommandHandlersModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterAssemblyTypes(typeof(CommandsHandler).Assembly).AsClosedTypesOf(typeof(IHandleCommand<>)).SingleInstance();
    }
  }
}

using Autofac;
using Autofac.Integration.Mvc;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Services;

namespace Iridio.Infrastructure.Modules
{
  public class ServicesModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.Register(c => new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port)).As<ISmtpClient>().InstancePerHttpRequest();
      builder.Register(c => new EmailService(c.Resolve<ISmtpClient>(), Configuration.Email.ServiceFrom, Configuration.Email.ServiceAlias)).As<IEmailService>().InstancePerHttpRequest();
      builder.Register(c => new UsersService(c.Resolve<IEmailService>(), c.Resolve<IUsersRepository>())).As<IUsersService>().InstancePerHttpRequest();
    }
  }
}

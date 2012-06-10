using Iridio.Messages.Commands;

namespace Iridio.DomainModel.CommandHandlers
{
  public interface IHandleCommand<T> where T : Command
  {
    void Handle(T command);
  }
}

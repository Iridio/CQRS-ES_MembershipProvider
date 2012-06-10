using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iridio.Messages.Commands;
using Iridio.Messages.Events;

namespace Iridio.Infrastructure
{
  public interface IServiceBus
  {
    void Send<T>(T command) where T : Command;
    void RegisterHandler<T>(Action<T> handler) where T : Event;
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iridio.Messages.Events;

namespace Iridio.ReadModel.EventsHandlers
{
  public interface IHandleEvent<T> where T : Event
  {
    void Handle(T @event);
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iridio.ReadModel.Dtos;
using Iridio.ReadModel.Abstracts;

namespace Iridio.ReadModel.EventsHandlers
{
  public abstract class EventsHandler
  {
    protected readonly Func<IPersistor> persistor;

    public EventsHandler(Func<IPersistor> persistor)
    {
      this.persistor = persistor;
    }
  }
}

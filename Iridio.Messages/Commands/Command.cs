using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridio.Messages.Commands
{
  [Serializable]
  public abstract class Command : IMessage
  {
    public Guid AggregateId { get; set; }
    public int Version { get; set; }
  }
}

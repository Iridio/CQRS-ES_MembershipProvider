using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridio.Messages
{
  public interface IMessage
  {
    Guid AggregateId { get; set; }
    int Version { get; set; }
  }
}

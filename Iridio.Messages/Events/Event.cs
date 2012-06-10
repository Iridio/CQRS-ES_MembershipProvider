using System;

namespace Iridio.Messages.Events
{
  public abstract class Event : IMessage
  {
    public Guid AggregateId { get; set; }
    public int Version { get; set; }
  }
}

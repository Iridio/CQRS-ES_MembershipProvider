using System;

namespace Iridio.Messages
{
  public interface IMessage
  {
    Guid AggregateId { get; set; }
    int Version { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain;
using CommonDomain.Persistence;

namespace Iridio.Infrastructure.Domain
{
  public class AggregateFactory : IConstructAggregates
  {
    public IAggregate Build(Type type, Guid id, IMemento snapshot)
    {
      //Here we could provide an AOP support
      return Activator.CreateInstance(type) as IAggregate;
    }
  }
}

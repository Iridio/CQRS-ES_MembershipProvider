using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.ByCode.Conformist;
using Iridio.ReadModel.Dtos;
using NHibernate.Mapping.ByCode;

namespace Iridio.ReadModel.NHibernate.Mappings
{
  public abstract class BaseMap<T> : ClassMapping<T> where T : Dto
  {
    protected BaseMap(string tableName)
    {
      Table(tableName);
      Id(x => x.Id, m => m.Generator(Generators.Assigned));
      DynamicUpdate(true);
      Lazy(true);
    }
  }
}

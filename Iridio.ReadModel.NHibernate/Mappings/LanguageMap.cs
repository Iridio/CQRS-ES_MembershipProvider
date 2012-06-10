using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iridio.ReadModel.Dtos;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Iridio.ReadModel.NHibernate.Mappings
{
  public class LanguageMap : BaseMap<Language>
  {
    public LanguageMap()
      : base("Languages")
    {
      Property(x => x.Name, m => m.Length(100));
      Property(x => x.CultureName, m => m.Length(10));
    }
  }
}

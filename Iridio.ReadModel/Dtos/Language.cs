using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Iridio.ReadModel.Dtos
{
  public class Language : Dto
  {
    public Language()
    {
      CultureName = "it-IT";
    }

    [StringLength(100, ErrorMessageResourceType = typeof(Resources.Validations), ErrorMessageResourceName = "StringLength")]
    public virtual string Name { get; set; }
    public virtual string CultureName { get; set; }
    //public virtual CultureInfo CultureInfo { get { return new CultureInfo(CultureName); } }
  }
}

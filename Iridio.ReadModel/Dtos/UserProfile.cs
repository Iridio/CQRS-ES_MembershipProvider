using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridio.ReadModel.Dtos
{
  public class UserProfile //è un value object di user anche in nhibernate è mappato come component : Dto
  {
    public Language Language { get; set; }
  }
}

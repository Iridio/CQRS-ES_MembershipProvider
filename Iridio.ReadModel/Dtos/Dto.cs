using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridio.ReadModel.Dtos
{
  public abstract class Dto
  {
    public virtual Guid Id { get; set; }

    public override int GetHashCode()
    {
      return this.Id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as Dto);
    }

    public virtual bool Equals(Dto other)
    {
      return (null != other) && (this.GetType() == other.GetType()) && (other.Id == this.Id);
    }

    public static bool operator ==(Dto Dto1, Dto Dto2)
    {
      if ((object)Dto1 == null && (object)Dto2 == null)
        return true;
      if ((object)Dto1 == null || (object)Dto2 == null)
        return false;
      return ((Dto1.GetType() == Dto2.GetType()) && (Dto1.Id == Dto2.Id));
    }

    public static bool operator !=(Dto Dto1, Dto Dto2)
    {
      return (!(Dto1 == Dto2));
    }
  }
}

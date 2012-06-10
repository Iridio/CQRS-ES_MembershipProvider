using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridio.DomainModel.Entities
{
  public abstract class Entity
  {
    public Guid Id { get; private set; }

    public Entity()
    {
    }

    public Entity(Guid id)
    {
      Id = id;
    }

    public override int GetHashCode()
    {
      return this.Id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as Entity);
    }

    public virtual bool Equals(Entity other)
    {
      return (null != other) && (this.GetType() == other.GetType()) && (other.Id == this.Id);
    }

    public static bool operator ==(Entity entity1, Entity entity2)
    {
      if ((object)entity1 == null && (object)entity2 == null)
        return true;
      if ((object)entity1 == null || (object)entity2 == null)
        return false;
      return ((entity1.GetType() == entity2.GetType()) && (entity1.Id == entity2.Id));
    }

    public static bool operator !=(Entity entity1, Entity entity2)
    {
      return (!(entity1 == entity2));
    }
  }
}

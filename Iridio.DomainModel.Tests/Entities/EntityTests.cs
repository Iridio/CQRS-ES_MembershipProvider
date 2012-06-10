using Iridio.DomainModel.Entities;
using NUnit.Framework;
using System;

namespace Iridio.DomainModel.Tests.Entities
{
  [TestFixture]
  public class EntityTests
  {

    public class Fake1 : Entity
    {
      public Fake1() { }
      public Fake1(Guid id) : base(id) { }
    }

    public class Fake2 : Entity
    {
      public Fake2() { }
      public Fake2(Guid id) : base(id) { }
    }

    [Test]
    public void Equals_IfNull_ReturnFalse()
    {
      var entity = new Fake1();
      var result = entity.Equals(null);
      Assert.IsFalse(result);
    }

    [Test]
    public void Equals_IfNotSameType_ReturnFalse()
    {
      var entity1 = new Fake1();
      var entity2 = new Fake2();
      var result = entity1.Equals(entity2);
      Assert.IsFalse(result);
    }

    [Test]
    public void Equals_IfDifferentIds_ReturnFalse()
    {
      var entity1 = new Fake1(Guid.NewGuid());
      var entity2 = new Fake1(Guid.NewGuid());
      var result = entity1.Equals(entity2);
      Assert.IsFalse(result);
    }

    [Test]
    public void Equalse_IfsameIds_ReturnTrue()
    {
      var id = Guid.NewGuid();
      var entity1 = new Fake1(id);
      var entity2 = new Fake1(id);
      var result = entity1.Equals(entity2);
      Assert.IsTrue(result);
    }

    [Test]
    public void Operator_IfNull_ReturnFalse()
    {
      var entity1 = new Fake1(Guid.NewGuid());
      var result = entity1 == null;
      Assert.IsFalse(result);
    }

    [Test]
    public void Operator_IfNotSameType_ReturnFalse()
    {
      var id = Guid.NewGuid();
      var entity1 = new Fake1(id);
      var entity2 = new Fake2(id);
      var result = entity1 == entity2;
      Assert.IsFalse(result);
    }

    [Test]
    public void Operator_IfDifferentIds_ReturnFalse()
    {
      var entity1 = new Fake1(Guid.NewGuid());
      var entity2 = new Fake1(Guid.NewGuid());
      var result = entity1 == entity2;
      Assert.IsFalse(result);
    }

    [Test]
    public void Operator_IfSameIds_ReturnTrue()
    {
      var id = Guid.NewGuid();
      var entity1 = new Fake1(id);
      var entity2 = new Fake1(id);
      var result = entity1 == entity2;
      Assert.IsTrue(result);
    }
  }
}

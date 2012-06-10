using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iridio.ReadModel.Dtos;
using NUnit.Framework;

namespace Iridio.ReadModel.Tests.Dtos
{
  [TestFixture]
  public class DtoTests
  {

    public class Fake1 : Dto
    {
      public Fake1() { }
      public Fake1(Guid id) { Id = id; }
    }

    public class Fake2 : Dto
    {
      public Fake2() { }
      public Fake2(Guid id) { Id = id; }
    }

    [Test]
    public void Equals_IfNull_ReturnFalse()
    {
      var Dto = new Fake1();
      var result = Dto.Equals(null);
      Assert.IsFalse(result);
    }

    [Test]
    public void Equals_IfNotSameType_ReturnFalse()
    {
      var Dto1 = new Fake1();
      var Dto2 = new Fake2();
      var result = Dto1.Equals(Dto2);
      Assert.IsFalse(result);
    }

    [Test]
    public void Equals_IfDifferentIds_ReturnFalse()
    {
      var Dto1 = new Fake1(Guid.NewGuid());
      var Dto2 = new Fake1(Guid.NewGuid());
      var result = Dto1.Equals(Dto2);
      Assert.IsFalse(result);
    }

    [Test]
    public void Equalse_IfsameIds_ReturnTrue()
    {
      var id = Guid.NewGuid();
      var Dto1 = new Fake1(id);
      var Dto2 = new Fake1(id);
      var result = Dto1.Equals(Dto2);
      Assert.IsTrue(result);
    }

    [Test]
    public void Operator_IfNull_ReturnFalse()
    {
      var Dto1 = new Fake1(Guid.NewGuid());
      var result = Dto1 == null;
      Assert.IsFalse(result);
    }

    [Test]
    public void Operator_IfNotSameType_ReturnFalse()
    {
      var id = Guid.NewGuid();
      var Dto1 = new Fake1(id);
      var Dto2 = new Fake2(id);
      var result = Dto1 == Dto2;
      Assert.IsFalse(result);
    }

    [Test]
    public void Operator_IfDifferentIds_ReturnFalse()
    {
      var Dto1 = new Fake1(Guid.NewGuid());
      var Dto2 = new Fake1(Guid.NewGuid());
      var result = Dto1 == Dto2;
      Assert.IsFalse(result);
    }

    [Test]
    public void Operator_IfSameIds_ReturnTrue()
    {
      var id = Guid.NewGuid();
      var Dto1 = new Fake1(id);
      var Dto2 = new Fake1(id);
      var result = Dto1 == Dto2;
      Assert.IsTrue(result);
    }
  }
}

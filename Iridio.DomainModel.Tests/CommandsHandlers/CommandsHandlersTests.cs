using System;
using CommonDomain.Core;
using CommonDomain.Persistence;
using Iridio.DomainModel.CommandHandlers;
using Moq;
using NUnit.Framework;

namespace Iridio.DomainModel.Tests.CommandsHandlers
{
  [TestFixture]
  public class CommandsHandlersTests
  {
    private Mock<IRepository> repository;
    private Guid goodId = Guid.NewGuid();
    private Guid badId = Guid.NewGuid();

    public class TestAggregate : AggregateBase
    {
      public TestAggregate(Guid id)
      {
        Id = id;
      }
      public string Value { get; set; }
    }

    public class TestHandler : CommandsHandler
    {
      public TestHandler(Func<IRepository> repository)
        : base(repository)
      {
      }
    }

    [SetUp]
    public void Setup()
    {
      repository = new Mock<IRepository>();
      repository.Setup(x => x.GetById<TestAggregate>(goodId)).Returns((Guid x) => new TestAggregate(x));
      repository.Setup(x => x.GetById<TestAggregate>(badId)).Returns((TestAggregate)null);
    }

    public TestHandler GetHandler()
    {
      return new TestHandler(() => repository.Object);
    }

    [Test]
    public void Get_Returns_Correct_Prodotto()
    {
      var handler = GetHandler();
      var aggregate = handler.Get<TestAggregate>(goodId, repository.Object);
      Assert.AreEqual(goodId, aggregate.Id);
      repository.Verify(x => x.GetById<TestAggregate>(goodId), Times.Once());
    }

    [Test, ExpectedException(ExpectedMessage = "TestAggregate not found")]
    public void Get_Throw_Exception_If_Prodotto_Not_Found()
    {
      var handler = GetHandler();
      handler.Get<TestAggregate>(badId, repository.Object);
    }
  }
}

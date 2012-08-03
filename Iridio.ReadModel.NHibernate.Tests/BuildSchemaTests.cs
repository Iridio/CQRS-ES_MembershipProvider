using NUnit.Framework;

namespace Iridio.ReadModel.NHibernate.Tests
{
  [TestFixture]
  public class BuildSchemaTests
  {
    private static string sqlConnection = @"Data Source=.\sqlexpress;initial catalog=IridioMembershipProviderCQRS;Integrated Security=SSPI";
    private static INHSessionBuilder sessionBuilder;

    [SetUp]
    public void FixtureSetUp()
    {
      sessionBuilder = new NHSqlServer(sqlConnection);
    }

    [TearDown]
    public void FixtureTearDown()
    {
      sessionBuilder.GetSession().Close();
    }

    [Test]
    public void Can_generate_schema()
    {
      var session = sessionBuilder.GetSession();
      sessionBuilder.BuildSchema(session, false, "c:\\temp", "Iridio");
    }
  }
}

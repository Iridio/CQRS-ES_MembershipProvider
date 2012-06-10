using System.Collections.Generic;
using NHibernate;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Dtos;
using System.Linq;

namespace Iridio.ReadModel.NHibernate.Persistors
{
  public class NHUsersPersistor : NHPersistorBase, IUsersPersistor
  {
    public NHUsersPersistor(INHSessionBuilder sessionBuilder)
      : base(sessionBuilder)
    {
    }

    public User GetUserByName(string userName, string appName)
    {
      User user = null;
      ISession session = SessionBuilder.GetSession();
      using (ITransaction transaction = session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
      {
        try
        {
          user = session.QueryOver<User>().Where(x => x.UserName == userName && x.ApplicationName == appName).Future().FirstOrDefault();
          transaction.Commit();
        }
        finally
        {
          if (transaction.IsActive)
            transaction.Rollback();
        }
      }
      return user;
    }

    public string GetUserNameByEmail(string email, string appName)
    {
      string userName = "";
      ISession session = SessionBuilder.GetSession();
      using (ITransaction transaction = session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
      {
        try
        {
          var user = session.QueryOver<User>().Where(x => x.ApplicationName == appName && x.Email == email).Future().FirstOrDefault();
          transaction.Commit();

          if (user != null)
            userName = user.UserName;
        }
        finally
        {
          if (transaction.IsActive)
            transaction.Rollback();
        }
      }
      return userName;
    }

    public IList<User> GetUsers(int pageIndex, int pageSize, string appName, out int totalItems)
    {
      IList<User> users = new List<User>();
      ISession session = SessionBuilder.GetSession();
      totalItems = 0;
      using (ITransaction transaction = session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
      {
        try
        {
          var queryOver = session.QueryOver<User>().Where(x => x.ApplicationName == appName);
          var rowCountQuery = queryOver.ToRowCountQuery();
          if (pageSize > 0)
            users = queryOver.Take(pageSize).Skip(pageIndex * pageSize).Future().ToList();
          else
            users = queryOver.Future().ToList();
          totalItems = rowCountQuery.FutureValue<int>().Value;
          transaction.Commit();
        }
        finally
        {
          if (transaction.IsActive)
            transaction.Rollback();
        }
      }
      return users;
    }

    public IList<User> FindUsersByEmail(string email, int pageIndex, int pageSize, string appName)
    {
      IList<User> users = new List<User>();
      ISession session = SessionBuilder.GetSession();
      using (ITransaction transaction = session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
      {
        try
        {
          users = session.QueryOver<User>().Where(x => x.ApplicationName == appName && x.Email != null && x.Email.Contains(email)).Take(pageSize).Skip(pageIndex * pageSize).Future().ToList();
          transaction.Commit();
        }
        finally
        {
          if (transaction.IsActive)
            transaction.Rollback();
        }
      }
      return users;
    }

    public IList<User> FindUsersByName(string userName, int pageIndex, int pageSize, string appName)
    {
      IList<User> users = new List<User>();
      ISession session = SessionBuilder.GetSession();
      using (ITransaction transaction = session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
      {
        try
        {
          users = session.QueryOver<User>().Where(x => x.ApplicationName == appName && x.UserName != null && x.UserName.Contains(userName)).Take(pageSize).Skip(pageIndex * pageSize).Future().ToList();
          transaction.Commit();
        }
        finally
        {
          if (transaction.IsActive)
            transaction.Rollback();
        }
      }
      return users;
    }

    public int GetNumberOfUsersOnline(System.DateTime compareTime, string appName)
    {
      IList<User> users = new List<User>();
      ISession session = SessionBuilder.GetSession();
      int total = 0;
      using (ITransaction transaction = session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
      {
        try
        {
          total = session.QueryOver<User>().Where(x => x.ApplicationName == appName && x.LastActivityDate >= compareTime).RowCount();
          transaction.Commit();
        }
        finally
        {
          if (transaction.IsActive)
            transaction.Rollback();
        }
      }
      return total;
    }

    public IList<User> GetUsersById(IList<int> usersId)
    {
      IList<User> users = new List<User>();
      ISession session = SessionBuilder.GetSession();
      using (ITransaction transaction = session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
      {
        try
        {
          users = session.QueryOver<User>().AndRestrictionOn(x => x.Id).IsInG<int>(usersId).List();
          transaction.Commit();
        }
        finally
        {
          if (transaction.IsActive)
            transaction.Rollback();
        }
      }
      return users;
    }
  }
}
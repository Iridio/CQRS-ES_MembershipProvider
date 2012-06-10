using System;
using System.Collections.Generic;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Dtos;

namespace Iridio.ReadModel.Concrete
{
  public class UsersRepository : Repository<User>, IUsersRepository
  {
    private readonly Func<IUsersPersistor> userPersistor;

    public UsersRepository(Func<IUsersPersistor> persistor)
      : base(persistor)
    {
      this.userPersistor = persistor;
    }

    public User GetUserByName(string userName, string appName)
    {
      return userPersistor().GetUserByName(userName, appName);
    }

    public IList<User> GetUsers(int pageIndex, int pageSize, string appName, out int totalItems)
    {
      totalItems = 0;
      if (pageIndex > 0)
        pageIndex--;
      return userPersistor().GetUsers(pageIndex, pageSize, appName, out totalItems);
    }

    public int GetNumberOfUsersOnline(DateTime compareTime, string appName)
    {
      return userPersistor().GetNumberOfUsersOnline(compareTime, appName);
    }

    public string GetUserNameByEmail(string email, string appName)
    {
      var result = userPersistor().GetUserNameByEmail(email, appName);
      if (result == null)
        result = "";
      return result;
    }

    public IList<User> FindUsersByEmail(string email, int pageIndex, int pageSize, string appName)
    {
      pageIndex--;
      if (pageIndex < 0)
        pageIndex = 0;
      return userPersistor().FindUsersByEmail(email, pageIndex, pageSize, appName);
    }

    public IList<User> FindUsersByName(string userName, int pageIndex, int pageSize, string appName)
    {
      pageIndex--;
      if (pageIndex < 0)
        pageIndex = 0;
      return userPersistor().FindUsersByName(userName, pageIndex, pageSize, appName);
    }

    //public IList<User> GetUsersInRole(string roleName, string userNameToMatch, string appName)
    //{
    //  return userPersistor().GetUsersInRole(roleName, userNameToMatch, appName);
    //}

  }
}
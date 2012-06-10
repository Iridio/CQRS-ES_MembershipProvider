using System;
using System.Collections.Generic;
using Iridio.ReadModel.Dtos;

namespace Iridio.ReadModel.Abstracts
{
  public interface IUsersRepository : IRepository<User>
  {
    User GetUserByName(string userName, string appName);
    string GetUserNameByEmail(string email, string appName);
    IList<User> GetUsers(int pageIndex, int pageSize, string appName, out int totalItems);
    IList<User> FindUsersByEmail(string email, int pageIndex, int pageSize, string appName);
    IList<User> FindUsersByName(string UserName, int pageIndex, int pageSize, string appName);
    int GetNumberOfUsersOnline(DateTime compareTime, string appName);
    //IList<User> GetUsersInRole(string roleName, string userNameToMatch, string appName);
  }
}

using System.Collections.Generic;
using Iridio.ReadModel.Dtos;

namespace Iridio.ReadModel.Abstracts
{
  public interface IUsersService
  {
    bool SendRegisterCongratulationsToEmail(User user);
    bool SendResetPasswordToEmail(string email, string newPassword);
    User GetUserByName(string userName, string appName);
    User GetUserByProviderUserKey(object providerUserKey, string appName);
    IList<User> GetUsers(int pageIndex, int pageSize, string appName, out int totalItems);
    IList<User> GetUsers(string appName);
  }
}
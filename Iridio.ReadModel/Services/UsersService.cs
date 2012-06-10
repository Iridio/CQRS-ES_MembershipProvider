using System;
using System.Collections.Generic;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Dtos;

namespace Iridio.ReadModel.Services
{
  public class UsersService : ServiceBase, IUsersService
  {
    private readonly IEmailService emailService;
    private readonly IUsersRepository usersRepository;

    public UsersService(IEmailService emailService, IUsersRepository usersRepository)
    {
      this.emailService = emailService;
      this.usersRepository = usersRepository;
    }

    public User GetUserByName(string userName, string appName)
    {
      return usersRepository.GetUserByName(userName, appName);
    }

    public bool SendRegisterCongratulationsToEmail(User user)
    {
      return emailService.Send(user.Email, Resources.Email.RegisterCongratulationsSubject, Resources.Email.RegisterCongratulationsBody, true);
    }

    public User GetUserByProviderUserKey(object providerUserKey, string appName)
    {
      if (providerUserKey != null)
        return usersRepository.GetBy((Guid)providerUserKey);
      return null;
    }

    public bool SendResetPasswordToEmail(string email, string newPassword)
    {
      if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(newPassword))
        return false;
      return emailService.Send(email, Resources.Email.ResetPasswordConfirmationSubject, string.Format(Resources.Email.ResetPasswordConfirmationBody, newPassword), true);
    }

    public IList<User> GetUsers(int pageIndex, int pageSize, string appName, out int totalItems)
    {
      return usersRepository.GetUsers(pageIndex, pageSize, appName, out totalItems);
    }

    public IList<User> GetUsers(string appName)
    {
      int total = 0;
      return GetUsers(0, 0, appName, out total);
    }
  }
}

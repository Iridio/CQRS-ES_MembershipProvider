using System;
using Iridio.ReadModel.Dtos;

namespace Iridio.MembershipProvider
{
  public sealed class IridioMembershipUser : System.Web.Security.MembershipUser
  {
    public UserProfile Profile { get; set; }

    public IridioMembershipUser(string providerName, string name, object providerUserKey, string email, string passwordQuestion, string comment, bool isApproved, bool isLockedOut, DateTime creationDate,
      DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate, UserProfile profile)
      : base(providerName, name, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate)
    {
      Profile = profile;
    }
  }
}
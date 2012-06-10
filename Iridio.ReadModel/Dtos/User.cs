using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridio.ReadModel.Dtos
{
  public class User : Dto
  {
    public User()
    {
      Profile = new UserProfile();
    }

    public virtual string UserName { get; set; }
    public virtual string ApplicationName { get; set; }
    public virtual string Email { get; set; }
    public virtual string Comment { get; set; }
    public virtual string Password { get; set; }
    public virtual string PasswordQuestion { get; set; }
    public virtual string PasswordAnswer { get; set; }

    public virtual bool IsApproved { get; set; }
    public virtual bool IsOnline { get; set; }
    public virtual bool IsLockedOut { get; set; }

    public virtual DateTime? LastActivityDate { get; set; }
    public virtual DateTime? LastLoginDate { get; set; }
    public virtual DateTime? LastPasswordChangedDate { get; set; }
    public virtual DateTime? CreationDate { get; set; }
    public virtual DateTime? LastLockedOutDate { get; set; }
    public virtual DateTime? FailedPasswordAttemptWindowStart { get; set; }
    public virtual DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }
    public virtual DateTime? PrevLoginDate { get; set; }

    public virtual int FailedPasswordAttemptCount { get; set; }
    public virtual int FailedPasswordAnswerAttemptCount { get; set; }

    public virtual UserProfile Profile { get; set; }
  }
}

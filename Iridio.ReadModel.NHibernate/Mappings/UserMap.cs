using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iridio.ReadModel.Dtos;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Iridio.ReadModel.NHibernate.Mappings
{
  public class UserMap : BaseMap<User>
  {
    public UserMap()
      : base("Users")
    {
      Property(x => x.UserName, m => m.Length(255));
      Property(x => x.ApplicationName, m => m.Length(255));
      Property(x => x.Email, m => m.Length(128));
      Property(x => x.Comment, m => m.Length(255));
      Property(x => x.Password, m => m.Length(255));
      Property(x => x.PasswordQuestion, m => m.Length(255));
      Property(x => x.PasswordAnswer, m => m.Length(255));
      Property(x => x.IsApproved);
      Property(x => x.IsOnline);
      Property(x => x.IsLockedOut);
      Property(x => x.LastActivityDate);
      Property(x => x.LastLoginDate);
      Property(x => x.LastPasswordChangedDate);
      Property(x => x.CreationDate);
      Property(x => x.LastLockedOutDate);
      Property(x => x.FailedPasswordAttemptWindowStart);
      Property(x => x.FailedPasswordAnswerAttemptWindowStart);
      Property(x => x.PrevLoginDate);
      Property(x => x.FailedPasswordAttemptCount);
      Property(x => x.FailedPasswordAnswerAttemptCount);
      Component(x => x.Profile, m =>
      {
        m.ManyToOne(p => p.Language, cm =>
        {
          cm.Column("LanguageId");
          cm.Fetch(FetchKind.Join);
        });
      });
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Iridio.ReadModel.Dtos;

namespace Iridio.ReadModel.Abstracts
{
  public interface IRepository<T> where T : Dto
  {
    T GetBy(Guid id);
    IList<T> GetAll(int pageIndex, int pageSize, out int totalItems);
    IList<T> GetAll();
    IList<T> GetAllOrderedBy(Expression<Func<T, string>> orderBy);
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Iridio.ReadModel.Abstracts;
using Iridio.ReadModel.Dtos;

namespace Iridio.ReadModel.Concrete
{
  public class Repository<T> : IRepository<T> where T : Dto
  {
    protected readonly Func<IPersistor> persistor;

    public Repository(Func<IPersistor> persistor)
    {
      this.persistor = persistor;
    }

    protected IQueryable<T> Find()
    {
      return persistor().Find<T>();
    }

    protected IQueryable<T> Find(Expression<Func<T, bool>> predicate)
    {
      return persistor().Find(predicate);
    }

    public virtual T GetBy(Guid id)
    {
      return persistor().GetById<T>(id);
    }

    public virtual IList<T> GetAll(int pageIndex, int pageSize, out int totalItems)
    {
      totalItems = 0;
      if (pageIndex > 0)
        pageIndex--;
      totalItems = Find().Count();
      if (pageSize > 0)
        return Find().Skip(pageIndex * pageSize).Take(pageSize).ToList();
      else
        return Find().ToList();
    }

    public virtual IList<T> GetAll()
    {
      int totalItems = 0;
      return GetAll(0, 0, out totalItems);
    }

    public virtual IList<T> GetAllOrderedBy(Expression<Func<T, string>> orderBy)
    {
      return Find().OrderBy(orderBy).ToList();
    }
  }
}
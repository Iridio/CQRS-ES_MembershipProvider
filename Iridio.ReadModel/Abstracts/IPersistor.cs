using System;
using System.Linq;
using System.Linq.Expressions;
using Iridio.ReadModel.Dtos;

namespace Iridio.ReadModel.Abstracts
{
  public interface IPersistor
  {
    void Delete<T>(T entity) where T : Dto;
    void Create<T>(T entity) where T : Dto;
    void Update<T>(T entity) where T : Dto;
    T GetById<T>(Guid id) where T : Dto;
    IQueryable<T> Find<T>() where T : Dto;
    IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : Dto;
  }
}

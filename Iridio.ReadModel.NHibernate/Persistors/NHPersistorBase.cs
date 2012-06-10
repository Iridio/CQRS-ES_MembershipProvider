using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using Iridio.ReadModel.Dtos;
using Iridio.ReadModel.Abstracts;

namespace Iridio.ReadModel.NHibernate.Persistors
{
  public class NHPersistorBase : IPersistor
  {
    protected readonly INHSessionBuilder SessionBuilder;

    public NHPersistorBase(INHSessionBuilder sessionBuilder)
    {
      SessionBuilder = sessionBuilder;
    }

    public T GetById<T>(Guid id) where T : Dto
    {
      T result = null;
      ISession session = SessionBuilder.GetSession();
      using (ITransaction transaction = session.BeginTransaction()) //la transazione è impostata nel config
      {
        try
        {
          result = (T)session.Get(typeof(T), id);
          transaction.Commit();
        }
        catch (Exception ex)
        {
          throw ex;
        }
        finally
        {
          if (transaction.IsActive) //in caso di errore mi assicuro che si chiuda
            transaction.Rollback();
        }
      }
      return result;
    }

    public void Create<T>(T entity) where T : Dto
    {
      ISession session = SessionBuilder.GetSession();
      using (ITransaction transaction = session.BeginTransaction())
      {
        try
        {
          session.Save(entity);
          transaction.Commit();
        }
        catch (Exception ex)
        {
          throw ex;
        }
        finally
        {
          if (transaction.IsActive) //in caso di errore mi assicuro che si chiuda
            transaction.Rollback();
        }
      }
    }

    public void Update<T>(T entity) where T : Dto
    {
      ISession session = SessionBuilder.GetSession();
      using (ITransaction transaction = session.BeginTransaction())
      {
        try
        {
          session.Update(entity);
          transaction.Commit();
        }
        catch (Exception ex)
        {
          throw ex;
        }
        finally
        {
          if (transaction.IsActive) //in caso di errore mi assicuro che si chiuda
            transaction.Rollback();
        }
      }
    }

    public void Delete<T>(T entity) where T : Dto
    {
      ISession session = SessionBuilder.GetSession();
      using (ITransaction transaction = session.BeginTransaction())
      {
        try
        {
          session.Delete(entity);
          transaction.Commit();
        }
        catch (Exception ex)
        {
          throw ex;
        }
        finally
        {
          if (transaction.IsActive) //in caso di errore mi assicuro che si chiuda
            transaction.Rollback();
        }
      }
    }

    public IQueryable<T> Find<T>() where T : Dto
    {
      return SessionBuilder.GetSession().Query<T>();
    }

    public IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : Dto
    {
      return SessionBuilder.GetSession().Query<T>().Where(predicate);
    }
  }
}
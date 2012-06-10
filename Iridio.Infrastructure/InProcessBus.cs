using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autofac;
using EventStore.Dispatcher;
using Iridio.DomainModel.CommandHandlers;
using Iridio.Messages.Commands;
using Iridio.Messages.Events;

namespace Iridio.Infrastructure
{
  public class InProcessBus : IServiceBus, IDispatchCommits
  {
    private readonly IComponentContext container;
    private readonly Dictionary<Type, List<Action<Event>>> routes = new Dictionary<Type, List<Action<Event>>>();

    public InProcessBus(IComponentContext container)
    {
      this.container = container;
    }

    public void Send<T>(T command) where T : Command
    {
      if (command == null) throw new ArgumentNullException("command");
      container.Resolve<IHandleCommand<T>>().Handle(command);
    }

    public void RegisterHandler<T>(Action<T> handler) where T : Event
    {
      List<Action<Event>> handlers;
      if (!routes.TryGetValue(typeof(T), out handlers))
      {
        handlers = new List<Action<Event>>();
        routes.Add(typeof(T), handlers);
      }
      handlers.Add(DelegateAdjuster.CastArgument<Event, T>(x => handler(x)));
    }

    public void Dispatch(EventStore.Commit commit)
    {
      foreach (var @event in commit.Events)
      {
        List<Action<Event>> handlers;
        if (!routes.TryGetValue(@event.Body.GetType(), out handlers))
          return;
        foreach (var handler in handlers)
          handler((Event)@event.Body);
      }
    }

    #region Dispose
    private bool disposed;
    public void Dispose()
    {
      Dispose(true);
      // Use SupressFinalize in case a subclass
      // of this type implements a finalizer.
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        if (disposing)
        {
          //noop atm
        }
        disposed = true;
      }
    }
    #endregion
  }

  public static class DelegateAdjuster
  {
    public static Action<BaseT> CastArgument<BaseT, DerivedT>(Expression<Action<DerivedT>> source) where DerivedT : BaseT
    {
      if (typeof(DerivedT) == typeof(BaseT))
        return (Action<BaseT>)((Delegate)source.Compile());
      var sourceParameter = Expression.Parameter(typeof(BaseT), "source");
      var result = Expression.Lambda<Action<BaseT>>(Expression.Invoke(source, Expression.Convert(sourceParameter, typeof(DerivedT))), sourceParameter);
      return result.Compile();
    }
  }
}

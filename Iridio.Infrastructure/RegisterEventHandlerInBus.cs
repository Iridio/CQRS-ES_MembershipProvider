using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Iridio.ReadModel.EventsHandlers;
using System.Reflection;
using Iridio.Messages.Events;

namespace Iridio.Infrastructure
{
  public class RegisterEventHandlerInBus
  {
    private static MethodInfo _createPublishActionMethod;
    private static MethodInfo _registerMethod;

    public static void Boot(IContainer container)
    {
      new RegisterEventHandlerInBus().RegisterEventHandler(container);
    }

    private void RegisterEventHandler(IContainer container)
    {
      var bus = container.Resolve<IServiceBus>();
      _createPublishActionMethod = GetType().GetMethod("CreatePublishAction");
      _registerMethod = bus.GetType().GetMethod("RegisterHandler");
      var handlers = typeof(EventsHandler).Assembly.GetExportedTypes().Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IHandleEvent<>)));
      foreach (var handlerType in handlers)
      {
        var handleEventTypes = handlerType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandleEvent<>));
        foreach (var handleEventType in handleEventTypes)
        {
          var eventHandler = container.Resolve(handleEventType);
          var genericArgs = handleEventType.GetGenericArguments();
          foreach (var genericArg in genericArgs)
          {
            var action = CreateTheProperAction(genericArg, eventHandler);
            RegisterTheCreatedAction(bus, genericArg, action);
          }
        }
      }
    }

    public Action<TEvent> CreatePublishAction<TEvent, TEventHandler>(TEventHandler eventHandler)
      where TEvent : Event
      where TEventHandler : IHandleEvent<TEvent>
    {
      return eventHandler.Handle;
    }

    private void RegisterTheCreatedAction(IServiceBus bus, Type handleEventType, object action)
    {
      _registerMethod.MakeGenericMethod(handleEventType).Invoke(bus, new[] { action });
    }

    private object CreateTheProperAction(Type eventType, object eventHandler)
    {
      return _createPublishActionMethod.MakeGenericMethod(eventType, eventHandler.GetType()).Invoke(this, new[] { eventHandler });
    }
  }
}

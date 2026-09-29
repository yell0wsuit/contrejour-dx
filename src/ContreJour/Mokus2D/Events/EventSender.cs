using System;
using System.Collections.Generic;

using Mokus2D.Util.Extensions;

namespace Mokus2D.Events
{
    public class EventSender
    {
        private readonly List<Action> listeners = new(64);

        private readonly List<Action> listenersCopy = new(64);

        public bool Enabled { get; set; }

        public EventSender()
        {
            Enabled = true;
        }

        public virtual void SendEvent()
        {
            if (!Enabled)
            {
                return;
            }
            listenersCopy.Clear();
            listenersCopy.AddItemsNoGarbage(listeners);
            foreach (Action item in listenersCopy)
            {
                item();
            }
        }

        public static EventSender operator +(EventSender eventSender, Action action)
        {
            eventSender.AddListener(action);
            return eventSender;
        }

        public static EventSender operator -(EventSender eventSender, Action action)
        {
            eventSender.RemoveListener(action);
            return eventSender;
        }

        public void AddListener(Action selector)
        {
            listeners.Add(selector);
        }

        public virtual void RemoveListeners()
        {
            listeners.Clear();
        }

        public void RemoveListener(Action selector)
        {
            _ = listeners.Remove(selector);
        }
    }
    public class EventSender<T> : EventSender
    {
        private readonly List<Action<T>> parameterListeners = new(64);

        private readonly List<Action<T>> parameterListenersCopy = new(64);

        public void SendEvent(T eventObject)
        {
            SendObject(eventObject);
            base.SendEvent();
        }

        public static EventSender<T> operator +(EventSender<T> eventSender, Action<T> action)
        {
            eventSender.AddListener(action);
            return eventSender;
        }

        public static EventSender<T> operator -(EventSender<T> eventSender, Action<T> action)
        {
            eventSender.RemoveListener(action);
            return eventSender;
        }

        public override void SendEvent()
        {
            base.SendEvent();
            SendObject(default);
        }

        private void SendObject(T eventObject)
        {
            parameterListenersCopy.Clear();
            parameterListenersCopy.AddItemsNoGarbage(parameterListeners);
            foreach (Action<T> item in parameterListenersCopy)
            {
                item(eventObject);
            }
        }

        public void AddListener(Action<T> action)
        {
            parameterListeners.Add(action);
        }

        public void RemoveListener(Action<T> action)
        {
            _ = parameterListeners.Remove(action);
        }

        public override void RemoveListeners()
        {
            base.RemoveListeners();
            parameterListeners.Clear();
        }
    }
}

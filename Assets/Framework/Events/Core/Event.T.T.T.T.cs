using System;
using System.Collections.Generic;

namespace HK
{
    public sealed class Event<T1, T2, T3, T4> : IEvent
    {
        private readonly List<Action<T1, T2, T3, T4>> subscribers;

        public Event(string name) : base(name)
        {
            subscribers = new List<Action<T1, T2, T3, T4>>();
        }

        public void Call(T1 parameter1, T2 parameter2, T3 parameter3, T4 SkipCloseAnimations)
        {
            for (int i = subscribers.Count - 1; i >= 0; --i)
            {
                subscribers[i].Invoke(parameter1, parameter2, parameter3, SkipCloseAnimations);
            }
        }

        public static Event<T1, T2, T3, T4> operator +(Event<T1, T2, T3, T4> e, Action<T1, T2, T3, T4> subscriber)
        {
            e.DoubleSubscriptionCheck(subscriber);
            e.subscribers.Add(subscriber);

            return e;
        }

        public static Event<T1, T2, T3, T4> operator -(Event<T1, T2, T3, T4> e, Action<T1, T2, T3, T4> subscriber)
        {
            e.subscribers.Remove(subscriber);

            return e;
        }

        private void DoubleSubscriptionCheck(Action<T1, T2, T3, T4> subscriber)
        {
            if (subscribers.Contains(subscriber))
            {
                ThrowSubscribeException();
            }
        }
    }
}

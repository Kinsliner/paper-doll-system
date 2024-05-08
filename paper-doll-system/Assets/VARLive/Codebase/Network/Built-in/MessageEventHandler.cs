using System;
using System.Collections.Generic;

public class MessageEventHandler : IEventHandler
{
    private Dictionary<string, List<Delegate>> events = new Dictionary<string, List<Delegate>>();

    public void Bind(string eventName, Action action)
    {
        BindEvent(eventName, action);
    }

    public void Bind<T>(string eventName, Action<T> action)
    {
        BindEvent(eventName, action);
    }

    public void Bind<T1, T2>(string eventName, Action<T1, T2> action)
    {
        BindEvent(eventName, action);
    }

    public void Bind<T1, T2, T3>(string eventName, Action<T1, T2, T3> action)
    {
        BindEvent(eventName, action);
    }

    public void Bind<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> action)
    {
        BindEvent(eventName, action);
    }

    public void Bind<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> action)
    {
        BindEvent(eventName, action);
    }

    public void Bind<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> action)
    {
        BindEvent(eventName, action);
    }

    private void BindEvent(string eventName, Delegate action)
    {
        if (events.ContainsKey(eventName) == false)
        {
            events.Add(eventName, new List<Delegate>());
        }
        if (events[eventName].Contains(action) == false)
        {
            events[eventName].Add(action);
        }
    }

    public void Trigger(string eventName, params object[] datas)
    {
        if (events.ContainsKey(eventName))
        {
            if (datas == null)
            {
                foreach (var e in events[eventName])
                {
                    e.DynamicInvoke();
                }
            }
            else
            {
                if (datas.Length <= 0)
                {
                    foreach (var e in events[eventName])
                    {
                        e.DynamicInvoke();
                    }
                }
                if (datas.Length == 1)
                {
                    foreach (var e in events[eventName])
                    {
                        e.DynamicInvoke(datas[0]);
                    }
                }
                if (datas.Length == 2)
                {
                    foreach (var e in events[eventName])
                    {
                        e.DynamicInvoke(datas[0], datas[1]);
                    }
                }
                if (datas.Length == 3)
                {
                    foreach (var e in events[eventName])
                    {
                        e.DynamicInvoke(datas[0], datas[1], datas[2]);
                    }
                }
                if (datas.Length == 4)
                {
                    foreach (var e in events[eventName])
                    {
                        e.DynamicInvoke(datas[0], datas[1], datas[2], datas[3]);
                    }
                }
                if (datas.Length == 5)
                {
                    foreach (var e in events[eventName])
                    {
                        e.DynamicInvoke(datas[0], datas[1], datas[2], datas[3], datas[4]);
                    }
                }
                if (datas.Length == 6)
                {
                    foreach (var e in events[eventName])
                    {
                        e.DynamicInvoke(datas[0], datas[1], datas[2], datas[3], datas[4], datas[5]);
                    }
                }
            }
        }
    }

    public void Unbind(string eventName, Action action)
    {
        UnbindEvent(eventName, action);
    }

    public void Unbind<T>(string eventName, Action<T> action)
    {
        UnbindEvent(eventName, action);
    }

    public void Unbind<T1, T2>(string eventName, Action<T1, T2> action)
    {
        UnbindEvent(eventName, action);
    }

    public void Unbind<T1, T2, T3>(string eventName, Action<T1, T2, T3> action)
    {
        UnbindEvent(eventName, action);
    }

    public void Unbind<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> action)
    {
        UnbindEvent(eventName, action);
    }

    public void Unbind<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> action)
    {
        UnbindEvent(eventName, action);
    }

    public void Unbind<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> action)
    {
        UnbindEvent(eventName, action);
    }

    private void UnbindEvent(string eventName, Delegate action)
    {
        if (events.ContainsKey(eventName))
        {
            if (events[eventName].Contains(action))
            {
                events[eventName].Remove(action);
            }
        }
    }
}

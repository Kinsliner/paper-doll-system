using System;

public interface IEventHandler
{
    void Trigger(string eventName, params object[] datas);

    void Bind(string eventName, Action action);

    void Bind<T>(string eventName, Action<T> action);

    void Bind<T1, T2>(string eventName, Action<T1, T2> action);

    void Bind<T1, T2, T3>(string eventName, Action<T1, T2, T3> action);

    void Bind<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> action);

    void Bind<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> action);

    void Bind<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> action);

    void Unbind(string eventName, Action action);

    void Unbind<T>(string eventName, Action<T> action);

    void Unbind<T1, T2>(string eventName, Action<T1, T2> action);

    void Unbind<T1, T2, T3>(string eventName, Action<T1, T2, T3> action);

    void Unbind<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> action);

    void Unbind<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> action);

    void Unbind<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> action);
}

using LanMonoGameLibrary;
using System;

public class DelegateObserver : IObserver
{
    public readonly Action<object> Callback;

    /// <summary>
    /// Encapsulates a method that has a single parameter and does not return a value.
    /// </summary>
    /// <param name="callback"></param>
    public DelegateObserver(Action<object> callback)
    {
        Callback = callback;
    }

    // OnNotify execute the encapsulated method with 1 parameter
    // expression lambda with (object eventData) as the input-parameter
    // Callback(eventData) as the expression
    // This executes the expression
    public void OnNotify(object eventData) => Callback(eventData);
}

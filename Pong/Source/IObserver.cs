using System;

public interface IObserver
{
    void OnNotify(object eventData);
}

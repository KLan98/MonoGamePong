using System;

namespace LanMonoGameLibrary
{
    public interface IObserver
    {
        // This method takes 1 parameter and has no return value -> using delegate Action<T> is fitting 
        void OnNotify(object eventData);
    }
}

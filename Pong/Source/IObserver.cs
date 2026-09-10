using System;

namespace LanMonoGameLibrary
{
    public interface IObserver
    {
        void OnNotify(object eventData);
    }
}

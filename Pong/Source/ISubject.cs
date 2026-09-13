using System;
using System.Collections.Generic;
using LanMonoGameLibrary;

namespace Pong
{
    public interface ISubject
    {
        public Dictionary<EventType, List<IObserver>> ObserversDict { get; set; }
        public void Notify(EventType eventType, object eventData);
        public void AddObserver(EventType eventType, IObserver observer);
        public void RemoveObserver(EventType eventType, IObserver observer);
    }

    // naming convention: EMITTER_WHATHAPPENED
    public enum EventType
    {
        DEBUG_CONSOLE_NUMBER_OF_BALLS_CHOSEN,
        ASSET_MAMAGER_BALL_ASSETS_UPDATED,
        PHYSICS_MANAGER_BALL_PHYSICS_UPDATED,
        PHYSICS_MAMAGER_SCORED,
    }
}
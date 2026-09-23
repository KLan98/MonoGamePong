using System;
using System.Collections.Generic;
using System.Diagnostics;
using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using static Pong.GameConstants;

namespace Pong
{
    public class PongFSM : IObserver, ISubject
    {
        private Machine[] serveState;
        private Machine[] playingState;
        private Machine[] scoredState;
        private PhysicsManager physicsManager;
        private float stateEndCountdown = 0f;
        private string scoringMessage;

        public Dictionary<EventType, List<IObserver>> ObserversDict { get; set; }

        public PongFSM()
        {
            serveState = new Machine[1]
            {
                new Machine(0f, FSM_SERVE_BEGIN_COUNTDOWN, stateEndCountdown)
            };

            playingState = new Machine[0]
            {
                //new Machine(stateTime, FSM_PLAYING_BEGIN_COUNTDOWN, stateEndCountdown)
            };

            scoredState = new Machine[0]
            {
                //new Machine(stateTime, FSM_SCORED_BEGIN_COUNTDOWN, stateEndCountdown)
            };

            physicsManager = PhysicsManager.GetInstance();

            ObserversDict = new Dictionary<EventType, List<IObserver>>();
        }

        public void AddObserver(EventType eventType, IObserver observer)
        {
            if (ObserversDict.TryGetValue(eventType, out List<IObserver> observers))
            {
                observers.Add(observer);
            }

            else
            {
                ObserversDict.Add(eventType, new List<IObserver> { observer });
            }
        }

        public void Notify(EventType eventType, object eventData)
        {
            if (ObserversDict.TryGetValue(eventType, out List<IObserver> observers))
            {
                foreach (var observer in observers)
                {
                    observer.OnNotify(eventData);
                }
            }
        }

        public void OnNotify(object eventData)
        {
            playingState = new Machine[0];
            scoredState = new Machine[1]
            {
                new Machine(0f, FSM_SCORED_BEGIN_COUNTDOWN, stateEndCountdown)
            };

            scoringMessage = eventData as string;
        }

        public void RemoveObserver(EventType eventType, IObserver observer)
        {
            throw new NotImplementedException();
        }

        // Called in logic update
        public void UpdateMachines(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //-------------------------SERVE STATE---------------------------------
            for (int i = serveState.Length - 1; i >= 0; i--)
            {
                serveState[i].StateTime += deltaTime;

                if (serveState[i].StateTime >= FSM_SERVE_BEGIN_COUNTDOWN)
                {
                    // allow the ball to move in random direction
                    physicsManager.SetBallDirection();

                    // clear the serveState vector
                    serveState = new Machine[0];
                    playingState = new Machine[1]
                    {
                        new Machine(0f, FSM_PLAYING_BEGIN_COUNTDOWN, stateEndCountdown)
                    };
                }

                // ball stays stationary
                else
                {
                    // display countdown
                    Notify(EventType.FSM_SERVE_DISPLAY_COUNTDOWN_MESSAGE, serveState[i].StateTime);
                }
            }

            //------------------------PLAYER STATE--------------------------------
            for (int i = playingState.Length - 1; i >= 0; i--)
            {
                playingState[i].StateTime += deltaTime;
            }

            //--------------------------SCORED STATE-------------------------------
            for (int i = scoredState.Length - 1; i >= 0; i--)
            {
                scoredState[i].StateTime += deltaTime;

                if (scoredState[i].StateTime <= FSM_SCORED_BEGIN_COUNTDOWN)
                {
                    // small delay for this state
                }

                else if (scoredState[i].StateTime > FSM_SCORED_BEGIN_COUNTDOWN && scoredState[i].StateTime <= FSM_SCORED_INTERMEDIATE_TIME)
                {
                    // display text stating someone has scored
                    Notify(EventType.FSM_DISPLAY_SCORED_MESSAGE, scoringMessage);
                }

                else if (scoredState[i].StateTime > FSM_SCORED_INTERMEDIATE_TIME)
                {
                    // reset ball position
                    physicsManager.ResetBallPosition();

                    scoredState = new Machine[0];

                    serveState = new Machine[1]
                    {
                        new Machine(0f, FSM_SERVE_BEGIN_COUNTDOWN, stateEndCountdown)
                    };
                    Debug.WriteLine("Return to serve");
                }
            }
        }
    }
}

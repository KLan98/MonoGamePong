using System;
using System.Collections.Generic;
using System.Diagnostics;
using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using static Pong.GameConstants;
using static Pong.EventType;

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
        private int scoringEntityID; // the ID of scoring entity for updating scoring backend

        public Dictionary<EventType, List<IObserver>> ObserversDict { get; set; }

        public PongFSM()
        {
            // serve state is the initial state
            serveState = new Machine[1]
            {
                new Machine(0f, FSM_SERVE_BEGIN_COUNTDOWN, stateEndCountdown)
            };

            playingState = new Machine[0];

            scoredState = new Machine[0];

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
            // Ignore repeat scoring event while not in playing state
            if (playingState.Length > 0 && scoredState.Length == 0)
            {
                playingState = new Machine[0];
                scoredState = new Machine[1]
                {
                    new Machine(0f, FSM_SCORED_BEGIN_COUNTDOWN, stateEndCountdown)
                };

                if (!Core.TryGet(eventData, out ScoringData data))
                {
                    return;
                }

                scoringMessage = data.ScoringMessage;
                scoringEntityID = data.EntityID;

                Notify(FSM_STATE_CHANGED, GameState.Scored);
            }
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

                    Notify(FSM_STATE_CHANGED, GameState.Playing);
                }

                // ball stays stationary
                else
                {
                    // display countdown
                    Notify(FSM_SERVE_DISPLAY_COUNTDOWN_MESSAGE, serveState[i].StateTime);
                }
            }

            //------------------------PLAYER STATE--------------------------------
            for (int i = playingState.Length - 1; i >= 0; i--)
            {
                playingState[i].StateTime += deltaTime;

                // if play state > 10s x2 the ball's speed
                if (playingState[i].StateTime >= 10f)
                {
                    
                }
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
                    Notify(FSM_DISPLAY_SCORED_MESSAGE, scoringMessage);
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

                    // update backend score, no distinction between frontend and backend
                    // done on exit for avoiding multiple updates
                    Notify(FSM_SCORED_UPDATE_SCORE, scoringEntityID);
                    Notify(FSM_STATE_CHANGED, GameState.Serve);
                }
            }
        }
    }

    // payload of FSM_STATE_CHANGED
    public enum GameState
    {
        Serve,
        Playing,
        Scored
    }
}

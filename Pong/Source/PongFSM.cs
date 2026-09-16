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
        private bool scoreFlagRaised = false;

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
        }


        public void AddObserver(EventType eventType, IObserver observer)
        {
            throw new NotImplementedException();
        }

        public void Notify(EventType eventType, object eventData)
        {
            throw new NotImplementedException();
        }

        public void OnNotify(object eventData)
        {
            playingState = new Machine[0];
            scoredState = new Machine[1]
            {
                new Machine(0f, FSM_SCORED_BEGIN_COUNTDOWN, stateEndCountdown)
            };
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

                    // reset statetime
                    //serveState[i].StateTime = 0f;

                    // clear the serveState vector
                    serveState = new Machine[0];
                    playingState = new Machine[1]
                    {
                        new Machine(0f, FSM_PLAYING_BEGIN_COUNTDOWN, stateEndCountdown)
                    };

                    Debug.Write("Ball served");
                }

                else
                {
                    // display countdown

                    // ball stays stationary
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
                    Debug.WriteLine("Scored");
                }

                else if (scoredState[i].StateTime > FSM_SCORED_INTERMEDIATE_TIME + FSM_SCORED_BEGIN_COUNTDOWN)
                {
                    // reset ball position
                    physicsManager.ResetBallPosition();

                    // reset statetime
                    //scoredState[i].StateTime = 0f;

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

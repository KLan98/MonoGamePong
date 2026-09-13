using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pong;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Pong
{
    public class PhysicsManager : IObserver, ISubject
    {
        private EntityPhysics[] movingPhysics;
        private Vector2[] staticPositions;
        private static PhysicsManager instance;
        private Core core;
        private float[] easingTimeElapsed = new float[2]; // easing time elapsed for every moving entities 
        private float[] ballEasingTimeElapsed;
        private Sprite[] movingSprites = new Sprite[2];
        private Sprite[] sprites = new Sprite[3];
        private Sprite[] ballSprites;
        private int screenVWidth, screenVHeight;
        private Rectangle topCollider, botCollider, leftCollider, rightCollider;
        private Vector2 normal; // normal used for calculating reflection vector
        private EntityPhysics[] ballsOnScreen;
        private Vector2 screenVRes;
        private AssetsManager assetsManager;
        private Random random = new Random();

        public Dictionary<EventType, List<IObserver>> ObserversDict { get; set; }

        private PhysicsManager()
        {
            assetsManager = AssetsManager.GetInstance();
            movingSprites = assetsManager.GetMovingSprites();
            sprites = assetsManager.GetSprites();
            ballSprites = assetsManager.GetBallSprites();

            core = Core.GetInstance();
            screenVRes = core.GetVirtualResolution();

            // init physics objects 
            Vector2 playerInitPos = new Vector2 { X = 0, Y = screenVRes.Y / 2 - movingSprites[0].Size.Y / 2 };
            EntityPhysics playerPhysics = new EntityPhysics(playerInitPos, Vector2.Zero, GameConstants.PLAYER_MASS);

            Vector2 comIntPos = new Vector2 { X = screenVRes.X - movingSprites[1].Size.X, Y = playerInitPos.Y };
            EntityPhysics comPhysics = new EntityPhysics(comIntPos, Vector2.Zero, GameConstants.COM_MASS);

            Vector2 ballInitPos = new Vector2 { X = screenVRes.X / 2, Y = screenVRes.Y / 2 };
            EntityPhysics ballPhysics = new EntityPhysics(ballInitPos, Vector2.Zero, GameConstants.BALL_MASS);

            // Init positions of non-moving objects
            Vector2 boardInitPos = Vector2.Zero;
            Vector2 playerScoreBarInitPos = Vector2.Zero;
            Vector2 comScoreBarInitPos = new Vector2 { X = screenVRes.X - sprites[2].Size.X, Y = 0 };

            // should be synced with sprites array in asset manager
            staticPositions = new Vector2[3]
            {
            boardInitPos,
            playerScoreBarInitPos,
            comScoreBarInitPos
            };

            // should be synced with movingSprites array in asset manager
            movingPhysics = new EntityPhysics[2]
            {
            playerPhysics,
            comPhysics,
            };

            // the game always start with 1 ball
            ballsOnScreen = new EntityPhysics[1]
            {
            ballPhysics
            };
            ballsOnScreen[0].Direction = new Vector2(-1, 0); // reference the array index directly after its initialization
            ballEasingTimeElapsed = new float[1];

            screenVWidth = (int)core.GetVirtualResolution().X;
            screenVHeight = (int)core.GetVirtualResolution().Y;

            // Updated based on screen resolution
            topCollider = new Rectangle(0, 0, screenVWidth, 1);
            botCollider = new Rectangle(0, screenVHeight, screenVWidth, 1);
            leftCollider = new Rectangle(0, 0, 1, screenVHeight);
            rightCollider = new Rectangle(screenVWidth, 0, 1, screenVHeight);

            normal = Vector2.Zero;

            ObserversDict = new Dictionary<EventType, List<IObserver>>();
        }

        public static PhysicsManager Create()
        {
            if (instance != null)
            {
                // Throw exeption during compilation
                throw new InvalidOperationException("PhysicsManager instance already created.");
            }

            instance = new PhysicsManager();
            return instance;
        }

        public static PhysicsManager GetInstance()
        {
            return instance;
        }

        public EntityPhysics GetEntityPhysics(int index)
        {
            return movingPhysics[index];
        }

        public Vector2 GetDirection(int index)
        {
            return movingPhysics[index].Direction;
        }

        public void SetDirection(int index, Vector2 direction)
        {
            movingPhysics[index].Direction = direction;
        }

        public Vector2 GetPosition(int index)
        {
            return movingPhysics[index].Position;
        }

        public Vector2 GetBallPosition(int index)
        {
            return ballsOnScreen[index].Position;
        }

        // Add force to ball
        public void AddForce(int index, Vector2 force)
        {
            ballsOnScreen[index].Force += force;
        }

        // Add impulse to ball
        public void AddImpulse(int index, Vector2 impulse)
        {
            ballsOnScreen[index].Impulse += impulse;
        }

        // called in game-logic-update
        public void UpdatePhysics(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handling player and com physics
            for (int i = 0; i < movingPhysics.Length; i++)
            {
                Vector2 currentPosition = movingPhysics[i].Position; // store the current position
                Vector2 direction = movingPhysics[i].Direction;
                float currentSpeed = movingPhysics[i].Speed;

                // increase easing time only when moving
                if (movingPhysics[i].Direction != Vector2.Zero)
                {
                    easingTimeElapsed[i] += deltaTime;
                }

                // easing time so far / total ease duration, clamp it so that it won't exceed 1.0f
                float normalizedElapsed = MathF.Min(1.0f, easingTimeElapsed[i] / GameConstants.EASING_DURATION);

                // Easing function use normalized time as input 
                float easedTime = core.EaseInQuad(normalizedElapsed);

                // lerp between current speed and max speed, eased time as input, clamped between current speed and max speed
                float easedSpeed = MathHelper.Lerp(currentSpeed, GameConstants.MAX_SPEED, easedTime);

                float displacement = easedSpeed * deltaTime;

                Vector2 movedDistance = displacement * direction;
                Vector2 velocity = easedSpeed * direction; // speed * direction
                Vector2 newPosition = currentPosition + movedDistance;

                // construct a predicted rect based on the new position, this is the next position the entity will move to, but currently in currentPosition
                Rectangle predictedRect = new Rectangle((int)newPosition.X, (int)newPosition.Y, (int)movingSprites[i].Size.X, (int)movingSprites[i].Size.Y);

                // if player or com collides with either top or bottom bounds
                // if entity intersects top collider and is moving upward (negative movedDistance indicates moving upward)
                if (predictedRect.Intersects(topCollider) && movedDistance.Y < 0)
                {
                    newPosition.Y = currentPosition.Y; // block only upward movement
                    MovingStop(i);
                }

                // if entity intersects top collider and is moving downward (positive movedDistance indicates moving downward)
                else if (predictedRect.Intersects(botCollider) && movedDistance.Y > 0)
                {
                    newPosition.Y = currentPosition.Y; // block only downward movement
                    MovingStop(i);
                }

                // update all physics information lastly in order to predictedRect to have its affect
                movingPhysics[i].Position = newPosition;
                movingPhysics[i].Velocity = velocity;
                movingPhysics[i].Speed = easedSpeed;

                // normalized direction (skip when zero to avoid a 0/0 NaN)
                if (direction != Vector2.Zero)
                {
                    movingPhysics[i].Direction = Vector2.Normalize(direction);
                }
            }

            // Handling ball physics
            for (int i = 0; i < ballsOnScreen.Length; i++)
            {
                Vector2 currentPosition = ballsOnScreen[i].Position; // store the current position
                Vector2 direction = ballsOnScreen[i].Direction;
                float currentSpeed = ballsOnScreen[i].Speed;

                // increase easing time only when moving
                if (ballsOnScreen[i].Direction != Vector2.Zero)
                {
                    ballEasingTimeElapsed[i] += deltaTime;
                }

                // easing time so far / total ease duration, clamp it so that it won't exceed 1.0f
                float normalizedElapsed = MathF.Min(1.0f, ballEasingTimeElapsed[i] / GameConstants.EASING_DURATION);

                // Easing function use normalized time as input 
                float easedTime = core.EaseInQuad(normalizedElapsed);

                // lerp between current speed and max speed, eased time as input, clamped between current speed and max speed
                float easedSpeed = MathHelper.Lerp(currentSpeed, GameConstants.MAX_SPEED, easedTime);

                float displacement = easedSpeed * deltaTime;

                Vector2 movedDistance = displacement * direction;
                Vector2 velocity = easedSpeed * direction; // speed * direction
                Vector2 newPosition = currentPosition + movedDistance;

                Rectangle predictedRect = new Rectangle((int)newPosition.X, (int)newPosition.Y, (int)ballSprites[i].Size.X, (int)ballSprites[i].Size.Y);

                Rectangle playerCollider = new Rectangle((int)movingPhysics[0].Position.X, (int)movingPhysics[0].Position.Y, (int)movingSprites[0].Size.X, (int)movingSprites[0].Size.Y);

                Rectangle comCollider = new Rectangle((int)movingPhysics[1].Position.X, (int)movingPhysics[1].Position.Y, (int)movingSprites[1].Size.X, (int)movingSprites[1].Size.Y);

                if (predictedRect.Intersects(topCollider))
                {
                    normal = Vector2.UnitY;
                    direction = Vector2.Reflect(direction, normal);
                }

                else if (predictedRect.Intersects(botCollider))
                {
                    normal = -Vector2.UnitY;
                    direction = Vector2.Reflect(direction, normal);
                }

                else if (predictedRect.Intersects(leftCollider))
                {
                    // increment point for com
                    Notify(EventType.PHYSICS_MAMAGER_SCORED, 1);
                    ResetBallPosition();
                    newPosition = ballsOnScreen[0].Position;
                }

                else if (predictedRect.Intersects(rightCollider))
                {
                    // increment point for player
                    Notify(EventType.PHYSICS_MAMAGER_SCORED, 0);
                    ResetBallPosition();
                    newPosition = ballsOnScreen[0].Position;
                }

                else if (predictedRect.Intersects(playerCollider))
                {
                    normal = Vector2.UnitX;
                    direction = Vector2.Reflect(direction, normal);

                    // compute velocity and add the impulse based on that added impulse
                    Vector2 paddleVelocity = movingPhysics[(int)MovingEntities.Player].Velocity;
                    float massRatio = GameConstants.PLAYER_MASS / GameConstants.BALL_MASS;
                    AddImpulse(i, massRatio * paddleVelocity);
                }

                else if (predictedRect.Intersects(comCollider))
                {
                    normal = -Vector2.UnitX;
                    direction = Vector2.Reflect(direction, normal);

                    // compute velocity and add the impulse based on that added impulse
                    Vector2 paddleVelocity = movingPhysics[(int)MovingEntities.Com].Velocity;
                    float massRatio = GameConstants.PLAYER_MASS / GameConstants.BALL_MASS;
                    AddImpulse(i, massRatio * paddleVelocity);
                }

                // re-compute velocity from the (possibly reflected) direction, then fold in any pending impulse
                velocity = easedSpeed * direction;

                Vector2 impulse = ballsOnScreen[i].Impulse;

                // If AddImpulse called, and impulse is added then
                if (impulse != Vector2.Zero)
                {
                    // impulse (J) is a change in momentum: J = m * Δv, so Δv = J / m
                    // dividing the stored impulse by the ball's mass converts it into a velocity change,
                    // then adding it "kicks" the current velocity instantly (e.g. paddle hit)
                    velocity += impulse / GameConstants.BALL_MASS;

                    // velocity is a combined speed+direction vector; its length is the new scalar speed
                    easedSpeed = velocity.Length();

                    // re-derive a unit direction from the kicked velocity (velocity / its own length = unit vector)
                    // guarded so we don't divide by zero if the kick somehow cancels velocity out completely
                    if (easedSpeed > 0f)
                    {
                        direction = velocity / easedSpeed;
                    }
                    ballsOnScreen[i].Impulse = Vector2.Zero; // one-shot, consume immediately so it isn't re-applied next frame
                }

                ballsOnScreen[i].Position = newPosition;
                ballsOnScreen[i].Velocity = velocity;
                ballsOnScreen[i].Speed = easedSpeed;

                // normalized direction (skip when zero to avoid a 0/0 NaN)
                if (direction != Vector2.Zero)
                {
                    ballsOnScreen[i].Direction = Vector2.Normalize(direction);
                }
            }
        }

        public void MovingStop(int index)
        {
            movingPhysics[index].Speed = 0;
            movingPhysics[index].Direction = Vector2.Zero;
            easingTimeElapsed[index] = 0;
        }

        //public string TestElapsedGameTime(GameTime gameTime)
        //{
        //    return $"delta gameTime since the last frame {gameTime.ElapsedGameTime.Milliseconds / 1000f}";
        //}

        public Vector2 GetStaticPosition(int index)
        {
            return staticPositions[index];
        }

        public void SetStaticPosition(int index, Vector2 newPos)
        {
            // only handle the assigning of data, no rendering included 
            staticPositions[index] = newPos;
        }

        // set an entity to its init position
        public void ResetPosition(int index)
        {
            // get the current screen res
            Vector2 screenVRes = core.GetVirtualResolution();
            Debug.WriteLine($"{screenVRes}");
            MovingStop(index);

            // set init pos
            switch (index)
            {
                case (int)MovingEntities.Player:
                    movingPhysics[index].Position = new Vector2 { X = 0, Y = screenVRes.Y / 2 - movingSprites[0].Size.Y / 2 };
                    break;

                case (int)MovingEntities.Com:
                    movingPhysics[index].Position = new Vector2 { X = screenVRes.X - movingSprites[1].Size.X, Y = screenVRes.Y / 2 - movingSprites[1].Size.Y / 2 };
                    break;
            }
        }

        public void ResetBallPosition()
        {
            ballsOnScreen[0].Position = new Vector2 { X = screenVRes.X / 2, Y = screenVRes.Y / 2 };
        }

        public void OnNotify(object eventData)
        {
            Debug.WriteLine("event 2 fired");
            int numberOfBalls = (int)eventData;
            // update ball-related information, from assets manager
            ballSprites = assetsManager.GetBallSprites();
            ballEasingTimeElapsed = new float[numberOfBalls];

            // Re-init EntityPhysics array
            ballsOnScreen = new EntityPhysics[numberOfBalls];

            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector2 ballSize = ballSprites[i].Size;
                Vector2 ballInitPos = new Vector2
                {
                    X = (float)(random.NextDouble() * (screenVRes.X - ballSize.X)),
                    Y = (float)(random.NextDouble() * (screenVRes.Y - ballSize.Y))
                };
                float angle = (float)(random.NextDouble() * MathHelper.TwoPi);
                Vector2 ballDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                EntityPhysics ballPhysics = new EntityPhysics(ballInitPos, ballDirection, GameConstants.BALL_MASS);
                ballsOnScreen[i] = ballPhysics;
            }

            Notify(EventType.PHYSICS_MANAGER_BALL_PHYSICS_UPDATED, numberOfBalls);
        }

        public void Notify(EventType eventType, object eventData)
        {
            if (ObserversDict.TryGetValue(eventType, out List<IObserver> observers))
            {
                foreach (IObserver observer in observers)
                {
                    observer.OnNotify(eventData);
                }
            }
        }

        public void AddObserver(EventType eventType, IObserver observer)
        {
            // if the key already exists then add the observer to its associated list
            if (ObserversDict.TryGetValue(eventType, out List<IObserver> observers))
            {
                observers.Add(observer);
            }

            // if not then create a new key - value entry in the dictionary
            else
            {
                ObserversDict.Add(eventType, new List<IObserver> { observer });
            }
        }

        public void RemoveObserver(EventType eventType, IObserver observer)
        {
            if (ObserversDict.TryGetValue(eventType, out List<IObserver> observers))
            {
                observers.Remove(observer);
            }
        }

        public enum MovingEntities
        {
            Player,
            Com
        }

        public enum StaticEntities
        {
            Board,
            PlayerScoreBar,
            ComScoreBar
        }
    }
}
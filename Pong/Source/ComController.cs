using System;
using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using static Pong.GameConstants;

namespace Pong
{
    // Drives the COM paddle the same way InputManager drives the player paddle, through SetDirection/MovingStop
    public class ComController : IObserver
    {
        private static ComController instance;
        private const int COM = (int)PhysicsManager.MovingEntities.Com;
        private PhysicsManager physicsManager;
        private AssetsManager assetsManager;
        private Vector2 screenVRes;
        private Random random = new Random();
        private AIState state;
        private GameState gameState = GameState.Serve;

        public AIProfile Profile; // public field so the debug console can tune it live by ref
        public int Difficulty { get; private set; }
        public AIState State => state;

        private ComController()
        {
            physicsManager = PhysicsManager.GetInstance();
            assetsManager = AssetsManager.GetInstance();
            screenVRes = Core.GetInstance().GetVirtualResolution();

            SetDifficulty(AI_DEFAULT_DIFFICULTY);
            state.TrackedBall = -1;
            state.TargetY = screenVRes.Y / 2;
        }

        public static ComController Create()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("ComController instance already created.");
            }

            instance = new ComController();
            return instance;
        }

        public static ComController GetInstance()
        {
            return instance;
        }

        public void SetDifficulty(int difficulty)
        {
            Difficulty = difficulty;
            Profile = AIProfiles.Presets[difficulty];
        }

        // Listens to FSM state changes, AI only tracks the ball while playing
        public void OnNotify(object eventData)
        {
            if (!Core.TryGet(eventData, out GameState newState))
            {
                return;
            }

            gameState = newState;
            state.TrackedBall = -1;
        }

        // called in game-logic-update, before physics
        public void UpdateAI(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 paddleSize = assetsManager.GetMovingSprites()[COM].Size;
            float paddleCenterY = physicsManager.GetPosition(COM).Y + paddleSize.Y / 2;

            state.ThinkTimer -= deltaTime;
            state.ReactionTimer -= deltaTime;

            if (state.ThinkTimer <= 0f)
            {
                state.ThinkTimer = Profile.ThinkInterval;

                if (gameState == GameState.Playing)
                {
                    Vector2 ballSize = assetsManager.GetBallSprites()[0].Size;
                    float contactX = physicsManager.GetPosition(COM).X - ballSize.X / 2; // ball position is its center
                    Think(in Profile, ref state, physicsManager.GetBalls(), contactX, ballSize.Y / 2, screenVRes.Y, random);
                }

                // not playing, wait at center for the next serve
                else
                {
                    state.TargetY = screenVRes.Y / 2;
                }

                // keep the target reachable so the paddle doesn't push against the screen edges
                state.TargetY = MathHelper.Clamp(state.TargetY, paddleSize.Y / 2, screenVRes.Y - paddleSize.Y / 2);
            }

            // intent is recomputed every frame so the paddle stops on target, not one think interval late
            state.Intent = DecideIntent(in Profile, in state, paddleCenterY);
            ApplyIntent(state.Intent);
        }

        //----------------------------------PRIVATE METHODS----------------------------------------------

        // Picks the ball to track and updates TargetY, reads only the passed in data
        private static void Think(in AIProfile profile, ref AIState s, ReadOnlySpan<EntityPhysics> balls, float contactX, float ballRadius, float screenHeight, Random random)
        {
            // track the approaching ball that reaches the COM paddle first
            int trackedBall = -1;
            float earliestTime = float.MaxValue;

            for (int i = 0; i < balls.Length; i++)
            {
                float velocityX = balls[i].Velocity.X;

                // ignore balls moving away from COM or already past the paddle
                if (velocityX <= 0f || balls[i].Position.X > contactX)
                {
                    continue;
                }

                float timeToReach = (contactX - balls[i].Position.X) / velocityX;
                if (timeToReach < earliestTime)
                {
                    earliestTime = timeToReach;
                    trackedBall = i;
                }
            }

            // no ball approaching
            if (trackedBall == -1)
            {
                s.TrackedBall = -1;
                if (profile.ReturnToCenter)
                {
                    s.TargetY = screenHeight / 2;
                }
                return;
            }

            // a new approach started, hesitate and commit to a fresh aim error
            if (trackedBall != s.TrackedBall)
            {
                s.TrackedBall = trackedBall;
                s.ReactionTimer = profile.ReactionDelay;
                s.CurrentError = ((float)random.NextDouble() * 2f - 1f) * profile.AimErrorPx;
            }

            EntityPhysics ball = balls[trackedBall];
            float targetY = profile.PredictBounces
                ? PredictInterceptY(ball.Position.Y, ball.Velocity.Y, earliestTime, ballRadius, screenHeight - ballRadius)
                : ball.Position.Y;

            s.TargetY = targetY + s.CurrentError;
        }

        // Closed form ball Y after time t, folding it back into [minY, maxY] to account for top/bottom bounces
        private static float PredictInterceptY(float y, float velocityY, float t, float minY, float maxY)
        {
            float range = maxY - minY;
            if (range <= 0f)
            {
                return y;
            }

            float period = 2f * range;
            float unfolded = (y - minY) + velocityY * t;
            float m = unfolded % period;
            if (m < 0f)
            {
                m += period;
            }

            return minY + (m <= range ? m : period - m);
        }

        private static int DecideIntent(in AIProfile profile, in AIState s, float paddleCenterY)
        {
            // still reacting to a new approach
            if (s.ReactionTimer > 0f)
            {
                return 0;
            }

            float diff = s.TargetY - paddleCenterY;
            if (MathF.Abs(diff) <= profile.DeadZonePx)
            {
                return 0;
            }

            return diff > 0f ? 1 : -1;
        }

        // Only touch physics when the intent actually changes, MovingStop resets the speed easing
        private void ApplyIntent(int intent)
        {
            Vector2 currentDirection = physicsManager.GetDirection(COM);

            if (intent == 0)
            {
                if (currentDirection != Vector2.Zero)
                {
                    physicsManager.MovingStop(COM);
                }
                return;
            }

            Vector2 desiredDirection = new Vector2(0, intent);
            if (currentDirection != desiredDirection)
            {
                physicsManager.SetDirection(COM, desiredDirection);
            }
        }
    }
}

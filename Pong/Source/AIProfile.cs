namespace Pong
{
    // Tuning data for the COM paddle, one per difficulty, fields are public so the debug console can edit them by ref
    public struct AIProfile
    {
        public float ReactionDelay; // seconds the AI waits before reacting to a newly approaching ball
        public float ThinkInterval; // seconds between re-deciding the target, lower = more responsive
        public float AimErrorPx; // max random offset added to the predicted intercept, resampled once per approach
        public float DeadZonePx; // paddle stops when its center is within this distance of the target, prevents jitter
        public bool PredictBounces; // true = predict intercept including wall bounces, false = chase the ball's current Y
        public bool ReturnToCenter; // drift back to center when no ball is approaching

        public AIProfile(float reactionDelay, float thinkInterval, float aimErrorPx, float deadZonePx, bool predictBounces, bool returnToCenter)
        {
            ReactionDelay = reactionDelay;
            ThinkInterval = thinkInterval;
            AimErrorPx = aimErrorPx;
            DeadZonePx = deadZonePx;
            PredictBounces = predictBounces;
            ReturnToCenter = returnToCenter;
        }
    }

    // Runtime state of the COM paddle AI, owned and mutated by ComController only
    public struct AIState
    {
        public float ThinkTimer; // counts down to the next think
        public float ReactionTimer; // counts down after a new approach starts, paddle holds still until it hits 0
        public float TargetY; // desired paddle center Y in virtual resolution
        public float CurrentError; // aim error for the current approach
        public int TrackedBall; // index into the ball array, -1 when no ball is approaching
        public int Intent; // -1 up, 0 stop, +1 down
    }

    public static class AIProfiles
    {
        // should be synced with Names
        public static readonly AIProfile[] Presets = new AIProfile[3]
        {
            new AIProfile(0.35f, 0.25f, 60f, 20f, false, false), // Easy
            new AIProfile(0.2f, 0.15f, 35f, 12f, true, true), // Normal
            new AIProfile(0.08f, 0.05f, 10f, 6f, true, true), // Hard
        };

        public static readonly string[] Names = new string[3] { "Easy", "Normal", "Hard" };
    }
}

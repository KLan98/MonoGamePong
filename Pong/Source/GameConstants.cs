
namespace Pong
{
    public static class GameConstants
    {
        public const int DEFAULT_NUMBER_OF_BALLS = 1;
        public const int BALL_NUMBER_SLIDER_MIN = 1;
        public const int BALL_NUMBER_SLIDER_MAX = 50000;
        public const float BALL_MASS = 1f;
        public const float COM_MASS = 1.25f;
        public const float PLAYER_MASS = 1.25f;
        public const float MAX_SPEED = 300f;
        public const float EASING_DURATION = 2f; // time it takes to reach max speed
        public const float FSM_SERVE_BEGIN_COUNTDOWN = 3f;
        public const float FSM_PLAYING_BEGIN_COUNTDOWN = 0f;
        public const float FSM_SCORED_BEGIN_COUNTDOWN = 0.5f;
        public const int FSM_GAMEPLAY_STATES = 3;
        public const float FSM_SCORED_INTERMEDIATE_TIME = 3f; // the time after begin countdown and before state end
    }
}

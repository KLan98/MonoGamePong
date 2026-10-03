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
        public const float MAX_SPEED = 500f;
        public const float EASING_DURATION = 2f; // time it takes to reach max speed
        public const float FSM_SERVE_BEGIN_COUNTDOWN = 3f;
        public const float FSM_PLAYING_BEGIN_COUNTDOWN = 0f;
        public const float FSM_SCORED_BEGIN_COUNTDOWN = 0.5f;
        public const int FSM_GAMEPLAY_STATES = 3;
        public const float FSM_SCORED_INTERMEDIATE_TIME = 3f; // the time after begin countdown and before state end
        public const string PLAYER_SCORED_MESSAGE = "PLAYER SCORED";
        public const string COM_SCORED_MESSAGE = "COM SCORED";
        public const float VIRTUAL_HEIGHT = 720f; // this is the resolution for mouse coordinates, sprite positions, UI layout, physics,these stay fixed internally
        public const float VIRTUAL_WIDTH = 1280f;
        public const int AI_DEFAULT_DIFFICULTY = 1; // index into AIProfiles.Presets
        public const float BALL_SERVE_MAX_ANGLE = 45f; // degrees above/below horizontal
        public const float BALL_BOUNCE_MAX_ANGLE = 50f; // maximum angle (above/below) the ball is allowed to bounce back after a collision
    }
}

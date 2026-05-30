namespace WinFormsPong;

// =========================================================================
// CONSTANTS & CONFIGURATION
// =========================================================================
internal static class PongConfig
{
    public const bool USE_MOUSE_CONTROL = true; // Toggle: true = Mouse, false = Self-play
    public const int WINDOW_WIDTH = 800;
    public const int WINDOW_HEIGHT = 600;
    public const int PADDLE_WIDTH = 15;
    public const int PADDLE_HEIGHT = 100;
    public const int BALL_SIZE = 15;
    public const float PADDLE_SPEED = 8.0f;
    public const float BALL_SPEED_BASE = 5.0f;
    public const float BALL_SPEED_INCREMENT = 0.2f;
    public const int FPS_TARGET = 60;
}

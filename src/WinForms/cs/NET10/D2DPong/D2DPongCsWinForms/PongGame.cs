namespace WinFormsPong;

// =========================================================================
// GAME STATE & LOGIC
// =========================================================================
internal class PongGame
{
    public float LeftPaddleY { get; set; }
    public float RightPaddleY { get; set; }
    public float BallX { get; set; }
    public float BallY { get; set; }
    public float BallVelX { get; set; }
    public float BallVelY { get; set; }
    public int LeftScore { get; set; }
    public int RightScore { get; set; }
    public float CurrentBallSpeed { get; set; }

    public Point MousePos { get; set; }

    public void Initialize(int width, int height)
    {
        LeftPaddleY = (height - PongConfig.PADDLE_HEIGHT) / 2f;
        RightPaddleY = (height - PongConfig.PADDLE_HEIGHT) / 2f;
        BallX = width / 2f;
        BallY = height / 2f;
        ResetBall(width, height);
        CurrentBallSpeed = PongConfig.BALL_SPEED_BASE;
    }

    private void ResetBall(int width, int height)
    {
        BallX = width / 2f;
        BallY = height / 2f;
        float dirX = BallVelX > 0 ? 1f : -1f;
        BallVelX = dirX * PongConfig.BALL_SPEED_BASE;
        BallVelY = (float)(Random.Shared.NextDouble() * 2 - 1) * PongConfig.BALL_SPEED_BASE;
    }

    public void Update(int width, int height, MouseState mouseState)
    {
        // 1. Input / AI
        if (PongConfig.USE_MOUSE_CONTROL)
        {
            float targetY = mouseState.Y - PongConfig.PADDLE_HEIGHT / 2f;
            targetY = Math.Clamp(targetY, 0, height - PongConfig.PADDLE_HEIGHT);
            LeftPaddleY = targetY;
        }
        else
        {
            // Self-play AI (follow ball with delay)
            float targetY = BallY - PongConfig.PADDLE_HEIGHT / 2f;
            LeftPaddleY += (targetY - LeftPaddleY) * 0.08f;
            RightPaddleY += (targetY - RightPaddleY) * 0.06f;
        }

        // 2. Ball Movement
        BallX += BallVelX;
        BallY += BallVelY;

        // 3. Wall Collisions
        if (BallY <= 0 || BallY >= height - PongConfig.BALL_SIZE) BallVelY *= -1;

        // 4. Paddle Collisions
        // Left Paddle
        if (BallX <= PongConfig.PADDLE_WIDTH && BallY + PongConfig.BALL_SIZE >= LeftPaddleY && BallY <= LeftPaddleY + PongConfig.PADDLE_HEIGHT)
        {
            BallVelX = Math.Abs(BallVelX) * 1.05f; // Speed up slightly
            BallVelX = Math.Max(BallVelX, 10f);
            CurrentBallSpeed += PongConfig.BALL_SPEED_INCREMENT;
        }
        // Right Paddle
        if (BallX + PongConfig.BALL_SIZE >= width - PongConfig.PADDLE_WIDTH && BallY + PongConfig.BALL_SIZE >= RightPaddleY && BallY <= RightPaddleY + PongConfig.PADDLE_HEIGHT)
        {
            BallVelX = -Math.Abs(BallVelX) * 1.05f;
            BallVelX = Math.Min(BallVelX, -10f);
            CurrentBallSpeed += PongConfig.BALL_SPEED_INCREMENT;
        }

        // 5. Scoring
        if (BallX < -PongConfig.BALL_SIZE) { RightScore++; ResetBall(width, height); CurrentBallSpeed = PongConfig.BALL_SPEED_BASE; }
        if (BallX > width) { LeftScore++; ResetBall(width, height); CurrentBallSpeed = PongConfig.BALL_SPEED_BASE; }
    }
}

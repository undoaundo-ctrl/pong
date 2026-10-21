using System;
using System.Text;

internal static class Program
{
    private const int Width = 60;
    private const int Height = 20;
    private const int PaddleHeight = 4;

    private static int playerY;
    private static int cpuY;
    private static double ballX;
    private static double ballY;
    private static double ballVX = 1;
    private static double ballVY = 0.6;
    private static int playerScore;
    private static int cpuScore;
    private static int rally;
    private static int bestRally;
    private static bool over;

    private static void Main()
    {
        Term.Setup("Pong");
        bool again = true;
        while (again)
        {
            Reset();
            again = Play();
        }

        Term.Line();
        Term.Line(Term.Yellow + "  Final score  You " + playerScore + " : " + cpuScore + " CPU   Longest rally " + bestRally + Term.Reset);
        Term.AnyKey("Press any key to exit...");
    }

    private static void Reset()
    {
        playerY = Height / 2 - PaddleHeight / 2;
        cpuY = Height / 2 - PaddleHeight / 2;
        ballX = Width / 2.0;
        ballY = Height / 2.0;
        ballVX = 1;
        ballVY = 0.6;
        rally = 0;
        over = false;
    }

    private static bool Play()
    {
        long last = DateTime.UtcNow.Ticks;
        Term.ClearScreen();

        while (!over)
        {
            while (Term.KeyAvailable)
            {
                ConsoleKeyInfo? key = Term.Key();
                if (!key.HasValue) break;
                if (key.Value.Key == ConsoleKey.UpArrow) playerY = Math.Max(0, playerY - 1);
                else if (key.Value.Key == ConsoleKey.DownArrow) playerY = Math.Min(Height - PaddleHeight, playerY + 1);
            }

            long now = DateTime.UtcNow.Ticks;
            if (now - last >= TimeSpan.TicksPerMillisecond * 28)
            {
                last = now;
                Step();
            }

            Render();
            Term.Sleep(20);
        }

        Render();
        Term.Line();
        string winner = playerScore > cpuScore ? "You win!" : "The computer wins.";
        Term.Line(Term.BrightGreen + "  " + winner + "  (" + playerScore + " - " + cpuScore + ")" + Term.Reset);
        ConsoleKeyInfo? again = Term.WaitKey("  Press Enter to play again, Q to quit...");
        return again.HasValue && again.Value.Key != ConsoleKey.Q && again.Value.Key != ConsoleKey.Escape;
    }

    private static void Step()
    {
        int cpuTarget = (int)ballY - PaddleHeight / 2;
        int diff = cpuTarget - cpuY;
        int move = 0;
        if (diff > 0) move = Math.Min(diff, 1);
        else if (diff < 0) move = Math.Max(diff, -1);
        cpuY += move;
        if (cpuY < 0) cpuY = 0;
        if (cpuY > Height - PaddleHeight) cpuY = Height - PaddleHeight;

        ballX += ballVX;
        ballY += ballVY;

        if (ballY < 0.5) { ballY = 0.5; ballVY = Math.Abs(ballVY); }
        if (ballY > Height - 1.5) { ballY = Height - 1.5; ballVY = -Math.Abs(ballVY); }

        int leftPaddle = 1;
        int rightPaddle = Width - 2;

        if (ballVX < 0 && ballX <= leftPaddle + 1 && ballX >= leftPaddle - 1)
        {
            if ((int)ballY >= playerY && (int)ballY <= playerY + PaddleHeight)
            {
                ballVX = Math.Abs(ballVX) + 0.35;
                ballVY += ((ballY - (playerY + PaddleHeight / 2.0)) / (PaddleHeight / 2.0)) * 0.9;
                rally++;
            }
        }

        if (ballVX > 0 && ballX >= rightPaddle - 1 && ballX <= rightPaddle + 1)
        {
            if ((int)ballY >= cpuY && (int)ballY <= cpuY + PaddleHeight)
            {
                ballVX = -(Math.Abs(ballVX) + 0.35);
                ballVY += ((ballY - (cpuY + PaddleHeight / 2.0)) / (PaddleHeight / 2.0)) * 0.9;
                rally++;
            }
        }

        if (rally > bestRally) bestRally = rally;

        if (ballX < -1)
        {
            cpuScore++;
            rally = 0;
            Serve(1);
        }
        else if (ballX > Width + 1)
        {
            playerScore++;
            rally = 0;
            Serve(-1);
        }

        if (playerScore >= 11 || cpuScore >= 11) over = true;
    }

    private static void Serve(double direction)
    {
        ballX = Width / 2.0;
        ballY = Height / 2.0;
        ballVX = direction * 1.4;
        ballVY = (Rng.NextDouble() - 0.5) * 1.2;
    }

    private static readonly Random Rng = new Random();

    private static void Render()
    {
        char[,] board = new char[Height, Width];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++) board[y, x] = ' ';
        }

        for (int i = 0; i < PaddleHeight; i++)
        {
            if (playerY + i < Height) board[playerY + i, 1] = '|';
            if (cpuY + i < Height) board[cpuY + i, Width - 2] = '|';
        }

        int bx = (int)ballX;
        int by = (int)ballY;
        if (bx >= 0 && bx < Width && by >= 0 && by < Height) board[by, bx] = 'O';

        StringBuilder sb = new StringBuilder();
        sb.Append(Term.Cyan).Append('+').Append(new string('-', Width)).Append('+').Append(Term.Reset).Append('\n');
        for (int y = 0; y < Height; y++)
        {
            sb.Append(Term.Cyan).Append('|').Append(Term.Reset);
            for (int x = 0; x < Width; x++)
            {
                char c = board[y, x];
                if (c == 'O') sb.Append(Term.BrightYellow).Append('O').Append(Term.Reset);
                else if (c == '|') sb.Append(Term.BrightCyan).Append('|').Append(Term.Reset);
                else sb.Append(c);
            }
            sb.Append(Term.Cyan).Append('|').Append(Term.Reset).Append('\n');
        }
        sb.Append(Term.Cyan).Append('+').Append(new string('-', Width)).Append('+').Append(Term.Reset).Append('\n');
        sb.Append(Term.Yellow).Append("  You ").Append(playerScore)
          .Append("   CPU ").Append(cpuScore)
          .Append("   Rally ").Append(rally).Append(Term.Reset).Append('\n');

        Term.ClearScreen();
        Term.Write(sb.ToString());
    }
}

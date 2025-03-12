using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        static void Main()
        {
            Game game = new Game();
            game.Run();
        }
    }

    class Game
    {
        private const int SCREEN_WIDTH = 32;
        private const int SCREEN_HEIGHT = 16;
        private const int INITIAL_LENGTH = 5;
        private const int GAME_SPEED = 500;

        private readonly Random random = new Random();
        private readonly List<int> snakeBodyX = new List<int>();
        private readonly List<int> snakeBodyY = new List<int>();
        private int foodX;
        private int foodY;
        private int score;
        private bool isGameOver;
        private string direction;
        private Pixel snakeHead;

        public Game()
        {
            Console.WindowHeight = SCREEN_HEIGHT;
            Console.WindowWidth = SCREEN_WIDTH;
            score = INITIAL_LENGTH;
            direction = "RIGHT";
            snakeHead = new Pixel(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2, ConsoleColor.Red);
            SpawnFood();
        }

        public void Run()
        {
            while (!isGameOver)
            {
                Draw();
                HandleInput();
                Update();
                Thread.Sleep(GAME_SPEED);
            }
            DisplayGameOver();
        }

        private void Draw()
        {
            Console.Clear();
            DrawBorders();
            DrawSnake();
            DrawFood();
        }

        private void DrawBorders()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < SCREEN_WIDTH; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, SCREEN_HEIGHT - 1);
                Console.Write("■");
            }
            for (int i = 0; i < SCREEN_HEIGHT; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(SCREEN_WIDTH - 1, i);
                Console.Write("■");
            }
        }

        private void DrawSnake()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < snakeBodyX.Count; i++)
            {
                Console.SetCursorPosition(snakeBodyX[i], snakeBodyY[i]);
                Console.Write("■");
                if (snakeBodyX[i] == snakeHead.X && snakeBodyY[i] == snakeHead.Y)
                {
                    isGameOver = true;
                }
            }
            Console.SetCursorPosition(snakeHead.X, snakeHead.Y);
            Console.ForegroundColor = snakeHead.Color;
            Console.Write("■");
        }

        private void DrawFood()
        {
            Console.SetCursorPosition(foodX, foodY);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("■");
        }

        private void HandleInput()
        {
            if (!Console.KeyAvailable) return;

            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow when direction != "DOWN":
                    direction = "UP";
                    break;
                case ConsoleKey.DownArrow when direction != "UP":
                    direction = "DOWN";
                    break;
                case ConsoleKey.LeftArrow when direction != "RIGHT":
                    direction = "LEFT";
                    break;
                case ConsoleKey.RightArrow when direction != "LEFT":
                    direction = "RIGHT";
                    break;
            }
        }

        private void Update()
        {
            snakeBodyX.Add(snakeHead.X);
            snakeBodyY.Add(snakeHead.Y);
            MoveHead();

            if (snakeHead.X == foodX && snakeHead.Y == foodY)
            {
                score++;
                SpawnFood();
            }
            else if (snakeBodyX.Count > score)
            {
                snakeBodyX.RemoveAt(0);
                snakeBodyY.RemoveAt(0);
            }

            CheckCollision();
        }

        private void MoveHead()
        {
            switch (direction)
            {
                case "UP":
                    snakeHead.Y--;
                    break;
                case "DOWN":
                    snakeHead.Y++;
                    break;
                case "LEFT":
                    snakeHead.X--;
                    break;
                case "RIGHT":
                    snakeHead.X++;
                    break;
            }
        }

        private void SpawnFood()
        {
            foodX = random.Next(1, SCREEN_WIDTH - 2);
            foodY = random.Next(1, SCREEN_HEIGHT - 2);
        }

        private void CheckCollision()
        {
            if (snakeHead.X == 0 || snakeHead.X == SCREEN_WIDTH - 1 || snakeHead.Y == 0 || snakeHead.Y == SCREEN_HEIGHT - 1)
            {
                isGameOver = true;
            }
        }

        private void DisplayGameOver()
        {
            Console.SetCursorPosition(SCREEN_WIDTH / 5, SCREEN_HEIGHT / 2);
            Console.WriteLine($"Game Over, Score: {score}");
            Console.ReadKey();
        }
    }

    class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ConsoleColor Color { get; }

        public Pixel(int x, int y, ConsoleColor color)
        {
            X = x;
            Y = y;
            Color = color;
        }
    }
}

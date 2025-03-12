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

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    class Game
    {
        private const int SCREEN_WIDTH = 32;
        private const int SCREEN_HEIGHT = 16;
        private const int GAME_SPEED = 200;

        private readonly Snake snake;
        private readonly Food food;
        private readonly Renderer renderer;
        private bool isGameOver;

        public Game()
        {
            Console.WindowHeight = SCREEN_HEIGHT;
            Console.WindowWidth = SCREEN_WIDTH;
            Console.CursorVisible = false;
            snake = new Snake(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
            food = new Food(SCREEN_WIDTH, SCREEN_HEIGHT);
            renderer = new Renderer(SCREEN_WIDTH, SCREEN_HEIGHT);
        }

        public void Run()
        {
            while (!isGameOver)
            {
                renderer.Draw(snake, food);
                HandleInput();
                Update();
                Thread.Sleep(GAME_SPEED);
            }
            renderer.DisplayGameOver(snake.Length);
        }

        private void HandleInput()
        {
            if (!Console.KeyAvailable) return;

            ConsoleKey key = Console.ReadKey(true).Key;
            snake.ChangeDirection(key);
        }

        private void Update()
        {
            snake.Move();
            if (snake.HasCollided(SCREEN_WIDTH, SCREEN_HEIGHT)) isGameOver = true;
            if (snake.EatFood(food)) food.Respawn(SCREEN_WIDTH, SCREEN_HEIGHT);
        }
    }

    class Snake
    {
        private readonly List<Pixel> body;
        private Direction direction;

        public int Length { get; private set; }
        public Pixel Head => body[0];
        public List<Pixel> Body => new List<Pixel>(body);

        public Snake(int startX, int startY)
        {
            body = new List<Pixel>();
            for (int i = 0; i < 5; i++)
            {
                body.Add(new Pixel(startX - i, startY, ConsoleColor.Green));
            }
            body[0] = new Pixel(startX, startY, ConsoleColor.Red); // Nastavení hlavy
            direction = Direction.Right;
            Length = 5;
        }

        public void ChangeDirection(ConsoleKey key)
        {
            direction = key switch
            {
                ConsoleKey.UpArrow when direction != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when direction != Direction.Up => Direction.Down,
                ConsoleKey.LeftArrow when direction != Direction.Right => Direction.Left,
                ConsoleKey.RightArrow when direction != Direction.Left => Direction.Right,
                _ => direction
            };
        }

        public void Move()
        {
            Pixel head = Head;
            int newX = head.X;
            int newY = head.Y;

            switch (direction)
            {
                case Direction.Up: newY--; break;
                case Direction.Down: newY++; break;
                case Direction.Left: newX--; break;
                case Direction.Right: newX++; break;
            }

            body.Insert(0, new Pixel(newX, newY, ConsoleColor.Red));
            body[1] = new Pixel(body[1].X, body[1].Y, ConsoleColor.Green); // Zajištění, že jen hlava je červená
            while (body.Count > Length)
            {
                body.RemoveAt(body.Count - 1);
            }
        }

        public bool EatFood(Food food)
        {
            if (Head.X == food.Position.X && Head.Y == food.Position.Y)
            {
                Length++;
                return true;
            }
            return false;
        }

        public bool HasCollided(int width, int height)
        {
            return Head.X == 0 || Head.X == width - 1 || Head.Y == 0 || Head.Y == height - 1 || body.Exists(p => p.X == Head.X && p.Y == Head.Y && p != Head);
        }
    }

    class Food
    {
        private readonly Random random = new Random();
        public Pixel Position { get; private set; }

        public Food(int width, int height)
        {
            Respawn(width, height);
        }

        public void Respawn(int width, int height)
        {
            Position = new Pixel(random.Next(1, width - 2), random.Next(1, height - 2), ConsoleColor.Cyan);
        }
    }

    class Renderer
    {
        private readonly int width;
        private readonly int height;

        public Renderer(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void Draw(Snake snake, Food food)
        {
            Console.Clear();
            DrawBorders();
            DrawPixel(food.Position);
            foreach (Pixel p in snake.Body)
            {
                DrawPixel(p);
            }
        }

        private void DrawBorders()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < width; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, height - 1);
                Console.Write("■");
            }
            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(width - 1, i);
                Console.Write("■");
            }
        }

        private void DrawPixel(Pixel pixel)
        {
            Console.SetCursorPosition(pixel.X, pixel.Y);
            Console.ForegroundColor = pixel.Color;
            Console.Write("■");
        }

        public void DisplayGameOver(int score)
        {
            Console.SetCursorPosition(width / 5, height / 2);
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

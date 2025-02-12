﻿using System;
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
        private const int ScreenWidth = 32;
        private const int ScreenHeight = 16;
        private const int InitialLength = 5;
        private const int GameSpeed = 500;

        private readonly Random _random = new Random();
        private readonly List<int> _snakeBodyX = new List<int>();
        private readonly List<int> _snakeBodyY = new List<int>();
        private int _foodX;
        private int _foodY;
        private int _score;
        private bool _isGameOver;
        private string _direction;
        private Pixel _snakeHead;

        public Game()
        {
            Console.WindowHeight = ScreenHeight;
            Console.WindowWidth = ScreenWidth;
            _score = InitialLength;
            _direction = "RIGHT";
            _snakeHead = new Pixel(ScreenWidth / 2, ScreenHeight / 2, ConsoleColor.Red);
            SpawnFood();
        }

        public void Run()
        {
            while (!_isGameOver)
            {
                Draw();
                HandleInput();
                Update();
                Thread.Sleep(GameSpeed);
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
            for (int i = 0; i < ScreenWidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, ScreenHeight - 1);
                Console.Write("■");
            }
            for (int i = 0; i < ScreenHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(ScreenWidth - 1, i);
                Console.Write("■");
            }
        }

        private void DrawSnake()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < _snakeBodyX.Count; i++)
            {
                Console.SetCursorPosition(_snakeBodyX[i], _snakeBodyY[i]);
                Console.Write("■");
                if (_snakeBodyX[i] == _snakeHead.X && _snakeBodyY[i] == _snakeHead.Y)
                {
                    _isGameOver = true;
                }
            }
            Console.SetCursorPosition(_snakeHead.X, _snakeHead.Y);
            Console.ForegroundColor = _snakeHead.Color;
            Console.Write("■");
        }

        private void DrawFood()
        {
            Console.SetCursorPosition(_foodX, _foodY);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("■");
        }

        private void HandleInput()
        {
            if (!Console.KeyAvailable) return;

            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow when _direction != "DOWN":
                    _direction = "UP";
                    break;
                case ConsoleKey.DownArrow when _direction != "UP":
                    _direction = "DOWN";
                    break;
                case ConsoleKey.LeftArrow when _direction != "RIGHT":
                    _direction = "LEFT";
                    break;
                case ConsoleKey.RightArrow when _direction != "LEFT":
                    _direction = "RIGHT";
                    break;
            }
        }

        private void Update()
        {
            _snakeBodyX.Add(_snakeHead.X);
            _snakeBodyY.Add(_snakeHead.Y);
            MoveHead();

            if (_snakeHead.X == _foodX && _snakeHead.Y == _foodY)
            {
                _score++;
                SpawnFood();
            }
            else if (_snakeBodyX.Count > _score)
            {
                _snakeBodyX.RemoveAt(0);
                _snakeBodyY.RemoveAt(0);
            }

            CheckCollision();
        }

        private void MoveHead()
        {
            switch (_direction)
            {
                case "UP":
                    _snakeHead.Y--;
                    break;
                case "DOWN":
                    _snakeHead.Y++;
                    break;
                case "LEFT":
                    _snakeHead.X--;
                    break;
                case "RIGHT":
                    _snakeHead.X++;
                    break;
            }
        }

        private void SpawnFood()
        {
            _foodX = _random.Next(1, ScreenWidth - 2);
            _foodY = _random.Next(1, ScreenHeight - 2);
        }

        private void CheckCollision()
        {
            if (_snakeHead.X == 0 || _snakeHead.X == ScreenWidth - 1 || _snakeHead.Y == 0 || _snakeHead.Y == ScreenHeight - 1)
            {
                _isGameOver = true;
            }
        }

        private void DisplayGameOver()
        {
            Console.SetCursorPosition(ScreenWidth / 5, ScreenHeight / 2);
            Console.WriteLine($"Game Over, Score: {_score}");
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

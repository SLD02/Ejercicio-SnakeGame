using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        const int Width = 40;
        const int Height = 20;

        enum Direction { Up, Down, Left, Right }

        static void Main()
        {
            Console.CursorVisible = false;
            Console.Title = "Snake";

            int best = 0;
            bool playAgain = true;

            while (playAgain)
            {
                int score = PlayGame(best);
                if (score > best) best = score;

                Console.SetCursorPosition(0, Height + 3);
                Console.WriteLine($"Game Over! Puntos: {score} | Récord: {best}");
                Console.WriteLine("Presiona Enter para jugar de nuevo o cualquier otra tecla para salir.");
                playAgain = Console.ReadKey(true).Key == ConsoleKey.Enter;
                Console.Clear();
            }

            Console.CursorVisible = true;
        }

        static int PlayGame(int best)
        {
            Console.Clear();
            DrawBorder();

            var snake = new List<(int x, int y)>
            {
                (Width / 2, Height / 2),
                (Width / 2 - 1, Height / 2),
                (Width / 2 - 2, Height / 2)
            };

            var dir = Direction.Right;
            var rnd = new Random();
            var food = PlaceFood(snake, rnd);
            int score = 0;
            bool paused = false;

            DrawFood(food);
            DrawScore(score, best);

            while (true)
            {
                // Leer teclas pendientes (solo la última cuenta)
                var newDir = dir;
                while (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            if (dir != Direction.Down) newDir = Direction.Up;
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            if (dir != Direction.Up) newDir = Direction.Down;
                            break;
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            if (dir != Direction.Right) newDir = Direction.Left;
                            break;
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            if (dir != Direction.Left) newDir = Direction.Right;
                            break;
                        case ConsoleKey.Spacebar:
                            paused = !paused;
                            break;
                        case ConsoleKey.Escape:
                            return score;
                    }
                }
                dir = newDir;

                if (paused)
                {
                    Thread.Sleep(50);
                    continue;
                }

                // Calcular nueva cabeza
                var head = snake[0];
                var newHead = dir switch
                {
                    Direction.Up => (head.x, head.y - 1),
                    Direction.Down => (head.x, head.y + 1),
                    Direction.Left => (head.x - 1, head.y),
                    _ => (head.x + 1, head.y)
                };

                // Choque con paredes o consigo misma
                if (newHead.Item1 <= 0 || newHead.Item1 >= Width - 1 ||
                    newHead.Item2 <= 0 || newHead.Item2 >= Height - 1 ||
                    snake.Contains(newHead))
                {
                    return score;
                }

                // Mover serpiente
                Draw(snake[0], 'o', ConsoleColor.Green);
                snake.Insert(0, newHead);

                if (newHead == food)
                {
                    score++;
                    food = PlaceFood(snake, rnd);
                    DrawFood(food);
                    DrawScore(score, best);
                }
                else
                {
                    var tail = snake[snake.Count - 1];
                    Draw(tail, ' ', ConsoleColor.Black);
                    snake.RemoveAt(snake.Count - 1);
                }

                Draw(snake[0], '@', ConsoleColor.Yellow);

                // La velocidad aumenta un poco con los puntos
                int delay = Math.Max(40, 120 - score * 3);
                Thread.Sleep(delay);
            }
        }

        static (int x, int y) PlaceFood(List<(int x, int y)> snake, Random rnd)
        {
            (int x, int y) pos;
            do
            {
                pos = (rnd.Next(1, Width - 1), rnd.Next(1, Height - 1));
            } while (snake.Contains(pos));
            return pos;
        }

        static void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int x = 0; x < Width; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write('#');
                Console.SetCursorPosition(x, Height - 1);
                Console.Write('#');
            }
            for (int y = 0; y < Height; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write('#');
                Console.SetCursorPosition(Width - 1, y);
                Console.Write('#');
            }
            Console.ResetColor();

            Console.SetCursorPosition(0, Height + 1);
            Console.WriteLine("Flechas/WASD: mover | Espacio: pausa | Esc: salir");
        }

        static void DrawFood((int x, int y) food) => Draw(food, '*', ConsoleColor.Red);

        static void DrawScore(int score, int best)
        {
            Console.SetCursorPosition(0, Height);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Puntos: {score}   Récord: {best}   ");
            Console.ResetColor();
        }

        static void Draw((int x, int y) pos, char ch, ConsoleColor color)
        {
            Console.SetCursorPosition(pos.x, pos.y);
            Console.ForegroundColor = color;
            Console.Write(ch);
            Console.ResetColor();
        }
    }
}

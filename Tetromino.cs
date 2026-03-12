using System;
using System.Drawing;

namespace TetrisWinForms
{
    public readonly struct Tetromino
    {
        public bool[,] Shape { get; }
        public Color Color { get; }

        private static readonly Random Random = new();

        private Tetromino(bool[,] shape, Color color)
        {
            Shape = shape;
            Color = color;
        }

        public Tetromino Rotate()
        {
            var rotated = new bool[4, 4];

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    rotated[x, y] = Shape[3 - y, x];
                }
            }

            return new Tetromino(rotated, Color);
        }

        public static Tetromino CreateRandom()
        {
            int index = Random.Next(7);
            return index switch
            {
                0 => CreateI(),
                1 => CreateO(),
                2 => CreateT(),
                3 => CreateS(),
                4 => CreateZ(),
                5 => CreateJ(),
                6 => CreateL(),
                _ => CreateO()
            };
        }

        private static Tetromino CreateI()
        {
            var shape = new bool[4, 4];
            for (int x = 0; x < 4; x++)
                shape[x, 1] = true;
            return new Tetromino(shape, Color.Cyan);
        }

        private static Tetromino CreateO()
        {
            var shape = new bool[4, 4];
            shape[1, 1] = true;
            shape[2, 1] = true;
            shape[1, 2] = true;
            shape[2, 2] = true;
            return new Tetromino(shape, Color.Yellow);
        }

        private static Tetromino CreateT()
        {
            var shape = new bool[4, 4];
            shape[1, 1] = true;
            shape[2, 1] = true;
            shape[3, 1] = true;
            shape[2, 2] = true;
            return new Tetromino(shape, Color.Magenta);
        }

        private static Tetromino CreateS()
        {
            var shape = new bool[4, 4];
            shape[2, 1] = true;
            shape[3, 1] = true;
            shape[1, 2] = true;
            shape[2, 2] = true;
            return new Tetromino(shape, Color.Lime);
        }

        private static Tetromino CreateZ()
        {
            var shape = new bool[4, 4];
            shape[1, 1] = true;
            shape[2, 1] = true;
            shape[2, 2] = true;
            shape[3, 2] = true;
            return new Tetromino(shape, Color.Red);
        }

        private static Tetromino CreateJ()
        {
            var shape = new bool[4, 4];
            shape[1, 1] = true;
            shape[1, 2] = true;
            shape[2, 2] = true;
            shape[3, 2] = true;
            return new Tetromino(shape, Color.Blue);
        }

        private static Tetromino CreateL()
        {
            var shape = new bool[4, 4];
            shape[3, 1] = true;
            shape[1, 2] = true;
            shape[2, 2] = true;
            shape[3, 2] = true;
            return new Tetromino(shape, Color.Orange);
        }
    }
}


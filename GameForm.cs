using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisWinForms
{
    public partial class GameForm : Form
    {
        private const int Columns = 10;
        private const int Rows = 20;
        private const int CellSize = 24;

        private readonly Timer _timer;
        private readonly Color?[,] _board = new Color?[Columns, Rows];

        private Tetromino _current = Tetromino.CreateRandom();
        private Tetromino _next = Tetromino.CreateRandom();
        private int _currentX = Columns / 2 - 2;
        private int _currentY = 0;
        private bool _gameOver;
        private int _score;

        public GameForm()
        {
            InitializeComponent();

            _timer = new Timer
            {
                Interval = 500
            };
            _timer.Tick += TimerOnTick;
            _timer.Start();

            this.Paint += OnPaintGame;
            this.KeyDown += OnKeyDownGame;
        }

        private void TimerOnTick(object? sender, EventArgs e)
        {
            if (_gameOver)
                return;

            if (CanMove(_current, _currentX, _currentY + 1))
            {
                _currentY++;
            }
            else
            {
                LockPiece();
                ClearLines();
                SpawnNext();
            }

            Invalidate();
        }

        private void OnKeyDownGame(object? sender, KeyEventArgs e)
        {
            if (_gameOver)
            {
                if (e.KeyCode == Keys.Space)
                {
                    ResetGame();
                }
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (CanMove(_current, _currentX - 1, _currentY))
                        _currentX--;
                    break;
                case Keys.Right:
                    if (CanMove(_current, _currentX + 1, _currentY))
                        _currentX++;
                    break;
                case Keys.Down:
                    if (CanMove(_current, _currentX, _currentY + 1))
                        _currentY++;
                    break;
                case Keys.Up:
                    var rotated = _current.Rotate();
                    if (CanMove(rotated, _currentX, _currentY))
                        _current = rotated;
                    break;
                case Keys.Space:
                    // Hard drop
                    while (CanMove(_current, _currentX, _currentY + 1))
                        _currentY++;
                    TimerOnTick(this, EventArgs.Empty);
                    break;
            }

            Invalidate();
        }

        private void ResetGame()
        {
            Array.Clear(_board);
            _current = Tetromino.CreateRandom();
            _next = Tetromino.CreateRandom();
            _currentX = Columns / 2 - 2;
            _currentY = 0;
            _score = 0;
            _gameOver = false;
            _timer.Start();
        }

        private void SpawnNext()
        {
            _current = _next;
            _next = Tetromino.CreateRandom();
            _currentX = Columns / 2 - 2;
            _currentY = 0;

            if (!CanMove(_current, _currentX, _currentY))
            {
                _gameOver = true;
                _timer.Stop();
            }
        }

        private void LockPiece()
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (_current.Shape[x, y])
                    {
                        int boardX = _currentX + x;
                        int boardY = _currentY + y;
                        if (boardX >= 0 && boardX < Columns && boardY >= 0 && boardY < Rows)
                        {
                            _board[boardX, boardY] = _current.Color;
                        }
                    }
                }
            }
        }

        private void ClearLines()
        {
            int cleared = 0;

            for (int y = Rows - 1; y >= 0; y--)
            {
                bool full = true;
                for (int x = 0; x < Columns; x++)
                {
                    if (_board[x, y] == null)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    cleared++;
                    // Move all rows above down
                    for (int yy = y; yy > 0; yy--)
                    {
                        for (int x = 0; x < Columns; x++)
                        {
                            _board[x, yy] = _board[x, yy - 1];
                        }
                    }
                    for (int x = 0; x < Columns; x++)
                    {
                        _board[x, 0] = null;
                    }
                    y++; // Re-check same row index after shifting
                }
            }

            if (cleared > 0)
            {
                _score += cleared * 100;
                _timer.Interval = Math.Max(100, _timer.Interval - cleared * 10);
            }
        }

        private bool CanMove(Tetromino piece, int newX, int newY)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (!piece.Shape[x, y])
                        continue;

                    int boardX = newX + x;
                    int boardY = newY + y;

                    if (boardX < 0 || boardX >= Columns || boardY < 0 || boardY >= Rows)
                        return false;

                    if (_board[boardX, boardY] != null)
                        return false;
                }
            }

            return true;
        }

        private void OnPaintGame(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.Black);

            int boardWidthPx = Columns * CellSize;
            int boardHeightPx = Rows * CellSize;
            var boardRect = new Rectangle(10, 10, boardWidthPx, boardHeightPx);

            // Draw background
            using (var bgBrush = new SolidBrush(Color.FromArgb(16, 16, 16)))
            {
                g.FillRectangle(bgBrush, boardRect);
            }

            // Draw settled blocks
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    var color = _board[x, y];
                    if (color != null)
                    {
                        DrawCell(g, boardRect.Left + x * CellSize, boardRect.Top + y * CellSize, color.Value);
                    }
                }
            }

            // Draw current piece
            if (!_gameOver)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        if (!_current.Shape[x, y])
                            continue;

                        int drawX = boardRect.Left + (_currentX + x) * CellSize;
                        int drawY = boardRect.Top + (_currentY + y) * CellSize;
                        DrawCell(g, drawX, drawY, _current.Color);
                    }
                }
            }

            // Draw grid
            using (var gridPen = new Pen(Color.FromArgb(40, 40, 40)))
            {
                for (int x = 0; x <= Columns; x++)
                {
                    int px = boardRect.Left + x * CellSize;
                    g.DrawLine(gridPen, px, boardRect.Top, px, boardRect.Bottom);
                }

                for (int y = 0; y <= Rows; y++)
                {
                    int py = boardRect.Top + y * CellSize;
                    g.DrawLine(gridPen, boardRect.Left, py, boardRect.Right, py);
                }
            }

            // Draw side panel
            int panelLeft = boardRect.Right + 20;
            using var whiteBrush = new SolidBrush(Color.White);
            using var smallFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
            using var bigFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold);

            g.DrawString("Score", bigFont, whiteBrush, panelLeft, 20);
            g.DrawString(_score.ToString(), bigFont, whiteBrush, panelLeft, 50);

            g.DrawString("Next", bigFont, whiteBrush, panelLeft, 100);

            // Draw next piece preview
            int previewCell = 18;
            int previewOffsetX = panelLeft;
            int previewOffsetY = 130;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (_next.Shape[x, y])
                    {
                        int drawX = previewOffsetX + x * previewCell;
                        int drawY = previewOffsetY + y * previewCell;
                        DrawCell(g, drawX, drawY, _next.Color, previewCell);
                    }
                }
            }

            g.DrawString("Controls:", smallFont, whiteBrush, panelLeft, 230);
            g.DrawString("← → : move", smallFont, whiteBrush, panelLeft, 250);
            g.DrawString("↑ : rotate", smallFont, whiteBrush, panelLeft, 270);
            g.DrawString("↓ : soft drop", smallFont, whiteBrush, panelLeft, 290);
            g.DrawString("Space : hard drop", smallFont, whiteBrush, panelLeft, 310);

            if (_gameOver)
            {
                string text = "Game Over";
                string retry = "Press Space to restart";

                var textSize = g.MeasureString(text, bigFont);
                var retrySize = g.MeasureString(retry, smallFont);

                float cx = boardRect.Left + boardRect.Width / 2f;
                float cy = boardRect.Top + boardRect.Height / 2f;

                g.DrawString(text, bigFont, whiteBrush, cx - textSize.Width / 2, cy - textSize.Height);
                g.DrawString(retry, smallFont, whiteBrush, cx - retrySize.Width / 2, cy + 5);
            }
        }

        private static void DrawCell(Graphics g, int x, int y, Color color, int size = CellSize)
        {
            var rect = new Rectangle(x, y, size, size);
            using var brush = new SolidBrush(color);
            using var borderPen = new Pen(Color.FromArgb(20, 20, 20));

            g.FillRectangle(brush, rect);
            g.DrawRectangle(borderPen, rect);

            // simple shine effect
            using var lightBrush = new SolidBrush(Color.FromArgb(80, Color.White));
            var shine = new Rectangle(x + 2, y + 2, size - 4, size / 3);
            g.FillRectangle(lightBrush, shine);
        }
    }
}


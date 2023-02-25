using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Mines.Controllers
{
    public static class MapController
    {
        private const int MapSize = 8;
        private const int CellSize = 50;

        private static int _currentPictureSet;

        private static readonly int[,] _map = new int[MapSize, MapSize];

        private static readonly Button[,] _buttons = new Button[MapSize, MapSize];

        private static Image _spriteSet;

        private static bool _isFirstStep;

        private static Point FirstCoord { get; set; }

        private static Form _form;

        private static void ConfigureMapSize(Form current)
        {
            current.Width = MapSize * CellSize + 20;
            current.Height = (MapSize + 1) * CellSize;
        }

        public static void Init(Form current)
        {
            _form = current;
            _currentPictureSet = 0;
            _isFirstStep = true;
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent;
            if (directoryInfo?.Parent != null)
                _spriteSet = new Bitmap(Path.Combine(
                    directoryInfo.Parent.FullName, "Sprites\\tiles.png"));
            ConfigureMapSize(current);
            InitMap();
            InitButtons(current);
        }

        private static void InitMap()
        {
            for (var i = 0; i < MapSize; i++)
            for (var j = 0; j < MapSize; j++)
                _map[i, j] = 0;
        }

        private static void InitButtons(Form current)
        {
            for (var i = 0; i < MapSize; i++)
            for (var j = 0; j < MapSize; j++)
            {
                var button = new Button
                {
                    Location = new Point(j * CellSize, i * CellSize) , 
                    Size = new Size(CellSize, CellSize), 
                    Image = FindNeededImages(0, 0)
                };

                button.MouseUp += OnButtonPressedMouse;
                current.Controls.Add(button);
                _buttons[i, j] = button;
            }
        }

        private static void OnButtonPressedMouse(object sender, MouseEventArgs e)
        {
            var pressedButton = sender as Button;

            switch (e.Button.ToString())
            {
                case "Right":
                    OnRightButtonPressed(pressedButton);
                    break;
                case "Left":
                    OnLeftButtonPressed(pressedButton);
                    break;
            }
        }

        private static void OnRightButtonPressed(ButtonBase pressedButton)
        {
            _currentPictureSet++;
            _currentPictureSet %= 3;

            var posX = 0;
            var posY = 0;

            switch (_currentPictureSet)
            {
                case 0:
                    posX = 0;
                    posY = 0;
                    break;
                case 1:
                    posX = 0;
                    posY = 2;
                    break;
                case 2:
                    posX = 2;
                    posY = 2;
                    break;
            }

            pressedButton.Image = FindNeededImages(posX, posY);
        }

        private static void OnLeftButtonPressed(Button pressedButton)
        {
            if (pressedButton == null) throw new ArgumentNullException(nameof(pressedButton));
            pressedButton.Enabled = false;
            var iButton = pressedButton.Location.Y / CellSize;
            var jButton = pressedButton.Location.X / CellSize;

            if (_isFirstStep)
            {
                FirstCoord = new Point(jButton, iButton);
                SeedMap();
                CountCellBomb();
                _isFirstStep = false;
            }

            OpenCells(iButton, jButton);

            if (_map[iButton, jButton] != -1) return;

            ShowAllBombs(iButton, jButton);
            MessageBox.Show("Defeat");
            _form.Controls.Clear();
            Init(_form);
        }

        private static void ShowAllBombs(int iBomb, int jBomb)
        {
            for (var i = 0; i < MapSize; i++)
            for (var j = 0; j < MapSize; j++)
            {
                if (i == iBomb && j == jBomb) continue;
                if (_map[i, j] == -1) _buttons[i, j].Image = FindNeededImages(3, 2);
            }
        }

        private static Image FindNeededImages(int xPos, int yPos)
        {
            Image image = new Bitmap(CellSize, CellSize);
            var g = Graphics.FromImage(image);
            g.DrawImage(_spriteSet, new Rectangle(new Point(0, 0), new Size(CellSize, CellSize)), 0 + 32 * xPos,
                0 + 32 * yPos, 33, 33, GraphicsUnit.Pixel);

            return image;
        }

        private static void SeedMap()
        {
            var r = new Random();
            var number = r.Next(5, 10);

            for (var i = 0; i < number; i++)
            {
                var posI = r.Next(0, MapSize - 1);
                var posJ = r.Next(0, MapSize - 1);

                while (_map[posI, posJ] == -1 || !_buttons[posI, posJ].Enabled)
                {
                    posI = r.Next(0, MapSize - 1);
                    posJ = r.Next(0, MapSize - 1);
                }

                _map[posI, posJ] = -1;
            }
        }

        private static void CountCellBomb()
        {
            for (var i = 0; i < MapSize; i++)
            for (var j = 0; j < MapSize; j++)
                if (_map[i, j] == -1)
                    for (var k = i - 1; k < i + 2; k++)
                    for (var l = j - 1; l < j + 2; l++)
                    {
                        if (!IsInBorder(k, l) || _map[k, l] == -1)
                            continue;
                        _map[k, l] = _map[k, l] + 1;
                    }
        }

        private static void OpenCell(int i, int j)
        {
            _buttons[i, j].Enabled = false;

            switch (_map[i, j])
            {
                case 1:
                    _buttons[i, j].Image = FindNeededImages(1, 0);
                    break;
                case 2:
                    _buttons[i, j].Image = FindNeededImages(2, 0);
                    break;
                case 3:
                    _buttons[i, j].Image = FindNeededImages(3, 0);
                    break;
                case 4:
                    _buttons[i, j].Image = FindNeededImages(4, 0);
                    break;
                case 5:
                    _buttons[i, j].Image = FindNeededImages(0, 1);
                    break;
                case 6:
                    _buttons[i, j].Image = FindNeededImages(1, 1);
                    break;
                case 7:
                    _buttons[i, j].Image = FindNeededImages(2, 1);
                    break;
                case 8:
                    _buttons[i, j].Image = FindNeededImages(3, 1);
                    break;
                case -1:
                    _buttons[i, j].Image = FindNeededImages(1, 2);
                    break;
                case 0:
                    _buttons[i, j].Image = FindNeededImages(0, 0);
                    break;
            }
        }

        private static void OpenCells(int i, int j)
        {
            OpenCell(i, j);

            if (_map[i, j] > 0)
                return;

            for (var k = i - 1; k < i + 2; k++)
            for (var l = j - 1; l < j + 2; l++)
            {
                if (!IsInBorder(k, l))
                    continue;
                if (!_buttons[k, l].Enabled)
                    continue;
                if (_map[k, l] == 0)
                    OpenCells(k, l);
                else if (_map[k, l] > 0)
                    OpenCell(k, l);
            }
        }

        private static bool IsInBorder(int i, int j)
        {
            return i >= 0 && j >= 0 && j <= MapSize - 1 && i <= MapSize - 1;
        }
    }
}
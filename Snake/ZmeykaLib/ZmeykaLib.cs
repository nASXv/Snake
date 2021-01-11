using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;

namespace ZmeykaLib {

    public struct Position {
        public int x, y;
        public bool Ravno(Position pos) { return (pos.x == x && pos.y == y); }
        public Position(int x = 0, int y = 0) { this.x = x; this.y = y; }
        public Position(Position pos) { x = pos.x; y = pos.y; }
    }

    public class Cell {
        public enum Type {
            empty,
            snake,
            apple
        }
        public Type type = Type.empty;
    }

    public class ZmeykaClass {
        int[] length = new int[2];
        public Position position, oldPosition;
        public Position[] bodyPositions;
        Random random = new Random();

        public int apples = 0;
        public bool isAlive = true, increaseSize = false;
        public string direction = "up", nextDirection = "up";
        public int mapSize = 16;
        public Cell[,] cells;

        ControllerGameplay gameplay;
        Rectangle[] rects;
        Image[] images;
        Canvas canvas;

        Color Empty = Colors.Transparent;
        Color Snake = Colors.Green;
        Color Apple = Colors.Red;

        BitmapImage[] sprites; // 0 - bodyend, 1 - body, 2 - head, 3 - angle, 4 - apple 
        BitmapImage empty;
        void SetSprites() {
            sprites = new BitmapImage[5];
            for (int i = 0; i < sprites.Length; i++) {
                sprites[i] = new BitmapImage();
                sprites[i].BeginInit();
                sprites[i].UriSource = new Uri("pack://application:,,/Resources/sprite_" + i + ".png");
                sprites[i].EndInit();
            }

            empty = new BitmapImage();
            empty.BeginInit(); empty.UriSource = new Uri("pack://application:,,/Resources/sprite_empty.png"); empty.EndInit();
        }

        int cellSize = 25;

        public ControllerGraphics(ControllerGameplay gameplay, Canvas canvas) {
            this.gameplay = gameplay;
            this.canvas = canvas;

            Start();
        }

        void Start() {
            SetSprites();
            CreateMap();
        }

        public void Update() {
            UpdateMap();
        }

        void CreateMap() {

            int count = gameplay.mapSize * gameplay.mapSize, mapSize = gameplay.mapSize;
            //rects = new Rectangle[count];
            images = new Image[count];

            for (int x = 0, y = 0, i = 0; y < mapSize; y++) {
                x = 0;
                while (x < mapSize) {

                    //Rectangle rect = new Rectangle();
                    //rects[i] = rect;

                    Image img = new Image();
                    images[i] = img;

                    img.Width = cellSize;
                    img.Height = cellSize;

                    canvas.Children.Insert(i, img);
                    Canvas.SetTop(img, y * cellSize);
                    Canvas.SetLeft(img, x * cellSize);



                    i++;
                    x++;

                    //Thread.Sleep(100);
                }
            }
        }





        void Die() {
            isAlive = false;
        }

        void SpawnApple() {
            Position pos = new Position(random.Next(0, mapSize), random.Next(0, mapSize));

            while (cells[pos.x, pos.y].type != Cell.Type.empty) {
                pos = new Position(random.Next(0, mapSize), random.Next(0, mapSize));
            }

            cells[pos.x, pos.y].type = Cell.Type.apple;
        }

        void Collect() {
            apples++;
            cells[position.x, position.y].type = Cell.Type.empty;

            SpawnApple();
            IncreaseSize();
        }

        public void KeyPressed(object sender, KeyEventArgs e) {
            string oldDirection = direction;
            switch (e.Key) {
                case Key.Up: if (direction != "down") nextDirection = "up"; break;
                case Key.Down: if (direction != "up") nextDirection = "down"; break;
                case Key.Left: if (direction != "right") nextDirection = "left"; break;
                case Key.Right: if (direction != "left") nextDirection = "right"; break;
                case Key.R: //restart
                    break;
            }
        }

        void IncreaseSize() {
            Position[] oldBody = new Position[bodyPositions.Length];
            for (int i = 0; i < oldBody.Length; i++)
                oldBody[i] = new Position(bodyPositions[i]);

            bodyPositions = new Position[bodyPositions.Length + 1];

            for (int i = 0; i < oldBody.Length; i++)
                bodyPositions[i] = new Position(oldBody[i].x, oldBody[i].y);
            bodyPositions[bodyPositions.Length - 1] = new Position(bodyPositions[oldBody.Length - 1]);
        }

        void SpawnSnake() {
            isAlive = true;
            position = new Position(random.Next(0, mapSize - 1), random.Next(0, mapSize - 1));

            bodyPositions = new Position[2];
            bodyPositions[0] = position;
            bodyPositions[1] = new Position(bodyPositions[0].x, bodyPositions[0].y + 1);
            //bodyPositions[2] = new Position();
        }

        public void Update() {
            if (isAlive) {
                CheckBorder();
                Move();
                CheckObstacle();
            }

            FillCells();
        }

        void CheckBorder() {
            if (position.x == 0 && nextDirection == "left") position = new Position(mapSize, position.y);
            if (position.x == mapSize - 1 && nextDirection == "right") position = new Position(-1, position.y);
            if (position.y == 0 && nextDirection == "up") position = new Position(position.x, mapSize);
            if (position.y == mapSize - 1 && nextDirection == "down") position = new Position(position.x, -1);
        }

        void CheckObstacle() {
            Cell cell = cells[position.x, position.y];

            switch (cell.type) {
                case Cell.Type.empty: break;
                case Cell.Type.apple: Collect(); break;
                case Cell.Type.snake: Die(); break;
            }
        }

        void FillCells() {
            foreach (Cell x in cells) {
                if (x.type != Cell.Type.apple) x.type = Cell.Type.empty;
            }

            foreach (Position x in bodyPositions)
                cells[x.x, x.y].type = Cell.Type.snake;
        }

        void Move() {
            direction = nextDirection;

            Position movement = new Position();
            switch (direction) {
                case "up": movement.y = -1; break;
                case "down": movement.y = 1; break;
                case "left": movement.x = -1; break;
                case "right": movement.x = 1; break;
            }
            Position[] oldBody = new Position[bodyPositions.Length];
            for (int i = 0; i < oldBody.Length; i++) {
                oldBody[i] = new Position(bodyPositions[i].x, bodyPositions[i].y);
            }

            position.x += movement.x;
            position.y += movement.y;

            bodyPositions[0] = position;

            //move body
            for (int i = 1; i < bodyPositions.Length; i++) {
                bodyPositions[i] = oldBody[i - 1];
            }
        }

        public void Start() {
            cells = new Cell[mapSize, mapSize];
            for (int x = 0, y = 0; y < mapSize; y++) {
                x = 0;
                while (x < mapSize) {
                    cells[x, y] = new Cell();

                    x++;
                }
            }
            this.gameplay = gameplay;
            this.canvas = canvas;


            SpawnSnake();
            SpawnApple();

            Start();
        }

        public ControllerGameplay(bool start = false) {
            if (start) Start();
        }
    }
}
}

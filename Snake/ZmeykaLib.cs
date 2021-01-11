using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZmeykaLib
{

    public struct Position
    {
        public int x, y;
        public bool Ravno(Position pos) { return (pos.x == x && pos.y == y); }
        public Position(int x = 0, int y = 0) { this.x = x; this.y = y; }
        public Position(Position pos) { x = pos.x; y = pos.y; }
    }

    public class Cell
    {
        public enum Type
        {
            nothing,
            snake,
            point
        }

        public Type type = Type.nothing;
    }

    public class ZmeykaClass
    {
        int[] length = new int[2];
        public Position position, oldPosition;
        public Position[] bodyPositions;
        Random random = new Random();
        public bool isAlive = true, increaseSize = false;
        public string direction = "up", nextDirection = "up";
        public int mapSize = 16, apples = 0, record = 0;
        public Cell[,] cells;
        Color Empty = Colors.Transparent, Snake, Apple, DeadSnake;
        Rectangle[] rects;
        Canvas canvas;
        private System.Windows.Threading.DispatcherTimer gameTickTimer = new System.Windows.Threading.DispatcherTimer();
        double cellSize = 24, space = 6.7;
        TextBlock text_score, text_record;





        void TImers_Launch() 
        {
            gameTickTimer.Tick -= UpdateEvents;
            gameTickTimer.Tick += UpdateEvents;
            gameTickTimer.Interval = new TimeSpan(1000000);
            gameTickTimer.Start();
        }

        void UpdateEvents(object source, EventArgs e)
        {
            FrameUpdate();
            if(!isAlive) {
                gameTickTimer.Stop();
            }
        }

        public ZmeykaClass(Canvas canvas, TextBlock text_score, TextBlock text_record, bool start = true)
        {
            this.canvas = canvas;
            this.text_record = text_record;
            this.text_score = text_score;

            if(start) {
                Start();
            }
        }

        public void Start()
        {
            canvas.Children.Clear();

            apples = 0;

            Set_Score();
            Map_Create();
            Set_Colors();
            Spawn_Snake();
            Spawn_Point();
            CreateMap();
            TImers_Launch();
        }

        void Map_Create()
        {
            cells = new Cell[mapSize, mapSize];

            for(int x = 0, y = 0; y < mapSize; y++)
            {
                x = 0;
                while(x < mapSize)
                {
                    cells[x, y] = new Cell();

                    x++;
                }
            }
        }

        void Set_Colors()
        {
            Empty = (Color)ColorConverter.ConvertFromString("#e3e3e3");
            Snake = (Color)ColorConverter.ConvertFromString("#06d6a0 ");
            Apple = (Color)ColorConverter.ConvertFromString("#f9c74f");
            DeadSnake = (Color)ColorConverter.ConvertFromString("#ffadad");
        }

        void Set_RED()
        { 
               Color color = DeadSnake;
               Snake = DeadSnake;
        }

        void Update_Record()
        {
            if(apples > record) {
                record = apples;
                text_record.Text ="" + record;
            }
        }

        void Set_Score()
        {
            text_score.Text ="" + apples;
        }

        void Die() 
        {
            Update_Record();
            Set_RED();
            isAlive = false;
        }

        void Spawn_Point()
        {
            Position pos = new Position(random.Next(0, mapSize), random.Next(0, mapSize));

            while(cells[pos.x, pos.y].type != Cell.Type.nothing) 
            {
                pos = new Position(random.Next(0, mapSize), random.Next(0, mapSize));
            }



            cells[pos.x, pos.y].type = Cell.Type.point;
        }

        void Collect()
        {
            apples++;

            cells[position.x, position.y].type = Cell.Type.nothing;

            Set_Score();

            Spawn_Point();

            SizePlus();
        }

        public void Input(object sender, KeyEventArgs e)
        {
            switch(e.Key) {
                case Key.Up: if(direction != "down") nextDirection = "up"; break;
                case Key.Down: if(direction != "up") nextDirection = "down"; break;
                case Key.Left: if(direction != "right") nextDirection = "left"; break;
                case Key.Right: if(direction != "left") nextDirection = "right"; break;
                default: break;
            }
        }

        void SizePlus()
        {
            Position[] oldBody = new Position[bodyPositions.Length];

            for(int i = 0; i < oldBody.Length; i++)
            {
                oldBody[i] = new Position(bodyPositions[i]);
            }

            bodyPositions = new Position[bodyPositions.Length + 1];

            for(int i = 0; i < oldBody.Length; i++)
            {
                bodyPositions[i] = new Position(oldBody[i].x, oldBody[i].y);
            }

            bodyPositions[bodyPositions.Length - 1] = new Position(bodyPositions[oldBody.Length - 1]);
        }

        void Spawn_Snake()
        {
            bodyPositions = new Position[2];

            isAlive = true;
            position = new Position(random.Next(0, mapSize - 1), random.Next(0, mapSize - 1));

            bodyPositions[0] = position;
            bodyPositions[1] = new Position(bodyPositions[0].x, bodyPositions[0].y + 1);
        }

        public void FrameUpdate()
        {
            Check_Border();
            Move_Snake();
            Check_Obstacles();
            Cells_Fill();
            Map_Update();
        }

        void Check_Border()
        {
            if(position.x == 0 && nextDirection == "left") position = new Position(mapSize, position.y);
            if(position.x == mapSize - 1 && nextDirection == "right") position = new Position(-1, position.y);
            if(position.y == 0 && nextDirection == "up") position = new Position(position.x, mapSize);
            if(position.y == mapSize - 1 && nextDirection == "down") position = new Position(position.x, -1);
        }

        void Check_Obstacles()
        {
            Cell cell = cells[position.x, position.y];

            switch(cell.type) {
                case Cell.Type.nothing: break;
                case Cell.Type.point: Collect(); break;
                case Cell.Type.snake: Die(); break;
            }
        }

        void Cells_Fill()
        {
            foreach(Cell x in cells) {
                if(x.type != Cell.Type.point) x.type = Cell.Type.nothing;
            }

            foreach(Position x in bodyPositions)
                cells[x.x, x.y].type = Cell.Type.snake;
        }

        void Move_Snake() 
        {
            direction = nextDirection;

            Position movement = new Position();
            switch(direction) 
            {
                case "up": movement.y = -1; break;
                case "down": movement.y = 1; break;
                case "left": movement.x = -1; break;
                case "right": movement.x = 1; break;
            }

            position.x += movement.x;
            position.y += movement.y;

            Position[] oldBody = new Position[bodyPositions.Length];
            for(int i = 0; i < oldBody.Length; i++) 
            {
                oldBody[i] = new Position(bodyPositions[i].x, bodyPositions[i].y);
            }

            bodyPositions[0] = position;

            //move body
            for(int i = 1; i < bodyPositions.Length; i++) 
            {
                bodyPositions[i] = oldBody[i - 1];
            }

        }

        void CreateMap() 
        {
            rects = new Rectangle[mapSize * mapSize];

            for(int x = 0, y = 0, i = 0; y < mapSize; y++) 
            {
                x = 0;
                while(x < mapSize) 
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.Blue);
                    rect.Width = cellSize;
                    rect.Height = cellSize;
                    rects[i] = rect;
                    canvas.Children.Insert(i, rect);
                    Canvas.SetTop(rect, y * cellSize + (y + 1) * space);
                    Canvas.SetLeft(rect, x * cellSize + (x + 1) * space);
                    i++; x++;
                }
            }
        }

        void Map_Update() 
        {
            for(int x = 0, y = 0, i = 0; y < mapSize; y++)
            {
                x = 0;
                Color color = Colors.White;

                while(x < mapSize) 
                {

                    Rectangle rect = rects[i];

                    switch(cells[x, y].type) 
                    {
                        case Cell.Type.point: color = Apple; break;
                        case Cell.Type.nothing: color = Empty; break;
                        case Cell.Type.snake: color = Snake; break;
                        default: color = Colors.Black; break;
                    }

                    rect.Fill = new SolidColorBrush(color);

                    x++; i++;
                }
            }
        }

    }
}

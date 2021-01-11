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

            switch(cell.type)
            {
                case Cell.Type.nothing: break;
                case Cell.Type.point: Collect(); break;
                case Cell.Type.snake: Die(); break;
            }
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

    }
}

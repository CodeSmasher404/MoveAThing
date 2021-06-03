using System;
using System.Threading;

namespace MoveAThing
{
    abstract class Moveable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public virtual bool Equals(Moveable mov)
        {
            if (X == mov.X && Y == mov.Y) return true;
            return false;
        }
        public virtual void Rng()
        {
            Y = Random.Gen.Next(1, Env.Length + 1);
            X = Random.Gen.Next(1, Env.Width + 1);
        }
        public void Middle()
        {
            Y = (Env.Length + 1) / 2;
            X = (Env.Width + 1) / 2;
        }
        public void Tp(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int GetClosest(Moveable[] movArr)
        {
            int lowestDist = int.MaxValue;
            int closestIndex = 0;
            for (int i = 0; i < movArr.Length; i++)
            {
                int currDist = Math.Abs(movArr[i].X - X) + Math.Abs(movArr[i].Y - Y);
                if (currDist < lowestDist)
                {
                    lowestDist = currDist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }
    }
    class Play : Moveable
    {
        private Play() { }
        public static Play er = new Play();
        public int Lives { get; set; }
        public char Char { get; set; }
        public void Die()
        {
            er.Middle();
            er.Lives--;
            Stats.Deaths++;
            Console.Beep();
            Thread.Sleep(500);
        }
    }
    class Food : Moveable
    {
        public static char Char { get; set; }
        public static int Count { get; set; }
    }
    class Knight : Moveable
    {
        public static char ActiveChar { get; set; }
        public static char InactiveChar { get; set; }
        public char Char
        {
            get
            {
                if (CurrCooldown == 0) return ActiveChar;
                else return InactiveChar;
            }
        }

        public static int Count { get; set; }
        public static int JumpEvery { get; set; }
        public static int Cooldown { get; set; }

        public bool MoveXAxis { get; set; } = true;
        public int CurrCooldown;

        public void ChangeAxis(Moveable mov)
        {
            if (Y == mov.Y) MoveXAxis = true; //axis x
            else if (X == mov.X) MoveXAxis = false;//axis y
            else
            {
                if (MoveXAxis) MoveXAxis = false;
                else MoveXAxis = true;
            }
        }
        public void MoveTowards(Moveable mov)
        {
            if (CurrCooldown == 0)
            {
            MoveTowards:
                if (X != mov.X || Y != mov.Y)
                {
                    ChangeAxis(mov);
                    if (MoveXAxis) //axis x
                    {
                        if (X < mov.X) X++;
                        else if (X > mov.X) X--;
                        Stats.KnightMoves++;
                    }
                    else //axis y
                    {
                        if (Y < mov.Y) Y++;
                        else if (Y > mov.Y) Y--;
                        Stats.KnightMoves++;
                    }
                }
                if (Random.Gen.Next(0, JumpEvery + 1) == 1) goto MoveTowards;
            }
            else
            {
                CurrCooldown--;
            }
        }
        public void ResetCooldown()
        {
            CurrCooldown = Cooldown;
        }
    }
    class Enemy : Moveable
    {
        public static char Char { get; set; }
        public static int Count { get; set; }
        public static int SkipEvery { get; set; }

        public bool MoveXAxis { get; set; } = true;

        public void ChangeAxis()
        {
            if (Y == Play.er.Y) MoveXAxis = true; //axis x
            else if (X == Play.er.X) MoveXAxis = false;//axis y
            else
            {
                if (MoveXAxis) MoveXAxis = false;
                else MoveXAxis = true;
            }
        }
        public void Move()
        {
            if (Random.Gen.Next(0, SkipEvery + 1) != 1)
            {
                if (X != Play.er.X || Y != Play.er.Y)
                {
                    ChangeAxis();
                    if (MoveXAxis) //axis x
                    {
                        if (X < Play.er.X) X++;
                        else if (X > Play.er.X) X--;
                        Stats.EnemyMoves++;
                    }
                    else //axis y
                    {
                        if (Y < Play.er.Y) Y++;
                        else if (Y > Play.er.Y) Y--;
                        Stats.EnemyMoves++;
                    }
                }
            }
        }
    }
    class Obs : Moveable
    {
        public static char Char { get; set; }
        public static int Count { get; set; }
        public static int MaxSize { get; set; }

        public int Size { get; set; }
        public byte Axis { get; set; }
        // 1 = +x
        // 2 = +y
        // 3 = -x
        // 4 = -y

        public override void Rng()
        {
            Axis = (byte)Random.Gen.Next(1, 5);
            if (Axis == 1 || Axis == 3) //incase of x axis
            {
                Y = Random.Gen.Next(1, Env.Length + 1);
                X = Axis == 1 ? 1 : Env.Width;
                Size = Random.Gen.Next(2, MaxSize + 1);
            }
            else //incase of y axis
            {
                X = Random.Gen.Next(1, Env.Width + 1);
                Y = Axis == 2 ? 1 : Env.Length;
                Size = Random.Gen.Next(2, (int)(MaxSize / 2) + 1);
            }
        }
        public void Move()
        {
            switch (Axis)
            {
                case 1: X++; break;
                case 2: Y++; break;
                case 3: X--; break;
                case 4: Y--; break;
            }
            ifOutside();
        }
        void ifOutside()
        {
            switch (Axis)
            {
                case 1: if (X - Size > Env.Width) Rng(); break;
                case 2: if (Y - Size > Env.Length) Rng(); break;
                case 3: if (X + Size < 1) Rng(); break;
                case 4: if (Y + Size < 1) Rng(); break;
            }
        }
        public void SetEnv(ref char[][] envBodyArr)
        {
            int x = X - 1;
            int y = Y - 1;
            for (int i = 0; i < Size; i++)
            {
                switch (Axis)
                {
                    case 1: //+x
                        if (x - i >= 0 && x - i <= Env.Width - 1)
                        {
                            envBodyArr[y][x - i] = Obs.Char;
                        }
                        break;
                    case 2: //+y
                        if (y - i >= 0 && y - i <= Env.Length - 1)
                        {
                            envBodyArr[y - i][x] = Obs.Char;
                        }
                        break;
                    case 3: //-x
                        if (x + i >= 0 && x + i <= Env.Width - 1)
                        {
                            envBodyArr[y][x + i] = Obs.Char;
                        }
                        break;
                    case 4: //-y
                        if (y + i >= 0 && y + i <= Env.Length - 1)
                        {
                            envBodyArr[y + i][x] = Obs.Char;
                        }
                        break;
                }
            }
        }

        public override bool Equals(Moveable mov)
        {
            for (int i = 0; i < Size; i++)
            {
                switch (Axis)
                {
                    case 1:
                        if (X - i == mov.X && Y == mov.Y) return true;
                        break;
                    case 2:
                        if (Y - i == mov.Y && X == mov.X) return true;
                        break;
                    case 3:
                        if (X + i == mov.X && Y == mov.Y) return true;
                        break;
                    case 4:
                        if (Y + i == mov.Y && X == mov.X) return true;
                        break;
                }
            }
            return false;
        }
    }
}
using System;
using System.Windows.Input;
using System.Threading;

namespace MoveAThing
{
    partial class NoU
    {
        public static void Main()
        {
            NoU noU = new NoU();
            noU.Initialize();
            noU.Write();
            noU.MoveThings();
            Console.Clear();
            noU.WriteStats();

            Console.WriteLine("\n\n\nPress [ENTER] to quit...");
            Console.ReadLine();
            Console.ResetColor();
        }
        Food[] foodArr;
        Knight[] knightArr;
        Enemy[] enemyArr;
        Obs[] obsArr;
        Thread readMovement;
        public static int ToSleep;
        byte toMove;
        void Write()
        {
            Console.Clear();
            WriteOnce();
            WriteThings();
            Thread.Sleep(750);
        }
        void Initialize()
        {
            //input

            //title
            Console.Title = "Move A Thing And Eat And Avoid Obstacles And Stay Behind Knights And RUN From Enemies";

            //write mode choices
            Console.WriteLine(
                "Mode Select:\n" +
                "\n[C]\t-> Casual" +
                "\n[A]\t-> Average" +
                "\n[H]\t-> Hardcore" +
                $"\n[OTHER]\t-> use {Env.CustomSettingsFileName}"
            );

            //select mode through input
            switch ((char)Console.ReadKey().Key)
            {
                default: Env.Mode = 255; break;
                case 'C': Env.Mode = 0; break;
                case 'A': Env.Mode = 1; break;
                case 'H': Env.Mode = 2; break;
            }

            //load mode
            Env.LoadMode();

            //mode in title
            Console.Title += Env.ModeStr();

            /* set things */

            //set console window stuff
            Console.WindowHeight = Env.Length + 6;
            if (Env.Width + 4 < 68) Console.WindowWidth = 68;
            else Console.WindowWidth = Env.Width + 4;

            //set player location to middle
            Play.er.Middle();

            //set and fill food array and randomize
            foodArr = new Food[Food.Count];
            for (int i = 0; i < Food.Count; i++)
            {
                Food newFood = new Food();
                newFood.Rng();
                foodArr[i] = newFood;
            }

            //set and fill knight array and randomize
            knightArr = new Knight[Knight.Count];
            for (int i = 0; i < Knight.Count; i++)
            {
                Knight newKnight = new Knight();
                newKnight.Rng();
                knightArr[i] = newKnight;
            }

            //set and fill enemy array and randomize
            enemyArr = new Enemy[Enemy.Count];
            for (int i = 0; i < Enemy.Count; i++)
            {
                Enemy newEnemy = new Enemy();
                newEnemy.Rng();
                enemyArr[i] = newEnemy;
            }

            //set and fill obstacle array and randomize
            obsArr = new Obs[Obs.Count];
            for (int i = 0; i < Obs.Count; i++)
            {
                Obs newObs = new Obs();
                newObs.Rng();
                obsArr[i] = newObs;
            }

            //read movement
            readMovement = new Thread(ReadMovement);
            readMovement.SetApartmentState(ApartmentState.STA);
            readMovement.Start();

            //set lives
            Stats.Lives = Play.er.Lives;

            //build horizontal border
            Env.HorizBorder = "";
            for (int i = 0; i < Env.Width + 2; i++) Env.HorizBorder += "-";
        }
        void ReadMovement()
        {
            bool goBrr = true;
            while (goBrr)
            {
                if (Keyboard.IsKeyDown(Key.X) || Keyboard.IsKeyDown(Key.Delete)) toMove = 255;
                else if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up)) toMove = 1;
                else if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left)) toMove = 2;
                else if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down)) toMove = 3;
                else if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right)) toMove = 4;
                else toMove = 0;
            }
        }
        void WriteOnce()
        {
            //controls
            Console.WriteLine("[WASD/ARROWS]\t-> Move\n[X/DELETE]\t-> Quit");

            //top border
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" " + Env.HorizBorder);

            //bot border
            Console.SetCursorPosition(0, Env.Length + 3);
            Console.WriteLine(" " + Env.HorizBorder);
            Console.ResetColor();
        }
        void WriteThings()
        {
            //set environment body
            char[][] envBodyArr = new char[Env.Length][];
            //env
            for (int i = 0; i < Env.Length; i++)
            {
                envBodyArr[i] = new char[Env.Width];
                for (int k = 0; k < Env.Width; k++)
                {
                    envBodyArr[i][k] = Env.Char;
                }
            }
            //food
            foreach (Food currFood in foodArr)
            {
                envBodyArr[currFood.Y - 1][currFood.X - 1] = Food.Char;
            }
            //obstacle
            foreach (Obs currObs in obsArr)
            {
                currObs.SetEnv(ref envBodyArr);
            }
            //knight
            foreach (Knight currknight in knightArr)
            {
                envBodyArr[currknight.Y - 1][currknight.X - 1] = currknight.Char;
            }
            //enemy
            foreach (Enemy currEnemy in enemyArr)
            {
                envBodyArr[currEnemy.Y - 1][currEnemy.X - 1] = Enemy.Char;
            }
            //player
            envBodyArr[Play.er.Y - 1][Play.er.X - 1] = Play.er.Char;

            //string
            string envBody = "";
            for (int i = Env.Length - 1; i >= 0; i--)
            {
                envBody += " |" + String.Concat(envBodyArr[i]) + "|\n";
            }

            //write environment body
            Console.SetCursorPosition(0, 3);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(envBody);
            Console.ResetColor();

            //write thing positon
            Console.SetCursorPosition(0, Env.Length + 4);
            Console.WriteLine($"X,Y: {Play.er}\tFood: {Stats.Foods} \tLives: {Play.er.Lives}\tKills: {Stats.Kills}\tScore: {Stats.Score}    ");
        }
        void WriteStats()
        {
            //ending in title
            Console.Title += " " + Stats.Ending();

            //console window height
            Console.WindowHeight = 43;

            //write stats... duh
            Console.WriteLine($"\t{Stats.Ending()}\n");
            Console.WriteLine(
                $" <> <> <Properties> <> <>\n" +
                $"\n\t{Env.ModeStr()}\n" +
                $"\n\tSize: {Env.Width}x{Env.Length}" +
                $"\n\tTime Between Cycles: {ToSleep}ms" +
                $"\n\tLives: {Stats.Lives}" +
                $"\n\tFood Count: {Food.Count}" +
                $"\n\tEnemy Count: {Enemy.Count}" +
                $"\n\tEnemy Moves Skip Every (random): {Enemy.SkipEvery}" +
                $"\n\tObstacle Count: {Obs.Count}" +
                $"\n\tMax Obstacle Length: {Obs.MaxSize}" +
                $"\n\tKnight Count: {Knight.Count}" +
                $"\n\tKnight Moves Jump Every (random): {Knight.JumpEvery}" +
                $"\n\tKnight Cooldown (cycles): {Knight.Cooldown}"
            );
            Console.WriteLine(
                $"\n\tEnvironment Character: {Stats.EnvStr()}" +
                $"\n\tplayer Character:   {Play.er.Char}" +
                $"\n\tFood Character:     {Food.Char}" +
                $"\n\tEnemy Character:    {Enemy.Char}" +
                $"\n\tObstacle Character: {Obs.Char}" +
                $"\n\tKnight Active Character: {Knight.ActiveChar}" +
                $"\n\tKnight Inactive Character: {Knight.InactiveChar}"
            );
            Console.WriteLine($"\n {Env.HorizBorder}\n");
            Console.WriteLine(
                $" <> <> <Statistics> <> <>\n" +
                $"\n\tScore:\t{Stats.Score}" +
                $"\n\tMoves:\t{Stats.Moves}" +
                $"\n\tFood:\t{Stats.Foods}" +
                $"\n\tKills:\t{Stats.Kills}" +
                $"\n\tDeaths:\t{Stats.Deaths}" +
                $"\n\tEnemy Moves: {Stats.EnemyMoves}" +
                $"\n\tObstacle Moves: {Stats.ObsMoves}"
            );
        }
    }
}

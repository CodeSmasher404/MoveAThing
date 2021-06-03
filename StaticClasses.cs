using System;
using System.IO;
using System.Threading;

namespace MoveAThing
{
    static class Env
    {
        public static char Char { get; set; }
        public static int Length { get; set; }
        public static int Width { get; set; }
        public static byte Mode { get; set; }
        public static string HorizBorder { get; set; }

        /*
        Here's The Guide to Modes For Myself:
        envwidth, envlength, noutosleep,
        lives, foodcount, enemycount, enemyskipevery, obscount, obsmaxsize
        knightcount, knightjumpevery, knightcooldown
        */
        static int[][] modes = {
            new int[] { //casual
                145, 35, 125,
                5, 65, 3, 50, 9, 14,
                3, 4, 30
            },
            new int[] { //average
                145, 35, 110,
                3, 50, 3, 100, 12, 18,
                2, 8, 50
            },
            new int[] { //hardcore
                145, 35, 85,
                1, 45, 5, 0, 14, 24,
                2, 12, 65
            }
        };
        public static string CustomSettingsFileName = "CustomSettings.txt";

        static void LoadSettings(int[] modeValues)
        {
            Env.Width = modeValues[0];
            Env.Length = modeValues[1];
            NoU.ToSleep = modeValues[2];

            Play.er.Lives = modeValues[3];
            Food.Count = modeValues[4];
            Enemy.Count = modeValues[5];
            Enemy.SkipEvery = modeValues[6];
            Obs.Count = modeValues[7];
            Obs.MaxSize = modeValues[8];

            Knight.Count = modeValues[9];
            Knight.JumpEvery = modeValues[10];
            Knight.Cooldown = modeValues[11];
        }
        public static void LoadMode()
        {
            if (Mode != 255)
            {
                //load corresponding mode settings
                LoadSettings(modes[Mode]);
                //assign characters
                Env.Char = ' ';
                Play.er.Char = '@';
                Food.Char = '~';
                Enemy.Char = '#';
                Obs.Char = '%';
                Knight.ActiveChar = 'V';
                Knight.InactiveChar = 'v';
            }
            else
            {
                try
                {
                    //check if custom seetings file exist and create it if not
                    if (!File.Exists(CustomSettingsFileName))
                    {
                        string template =
                        "Environment Width: 1 #Enter an integer value after the colon." +
                        "\nEnvironment Length: 1" +
                        "\nTime Between Cycles (ms): 0" +
                        "\nLives: 0" +
                        "\nFood Count: 0" +
                        "\nEnemy Count: 0" +
                        "\nEnemy Moves Skip Every (random): 0" +
                        "\nObstacle Count: 0" +
                        "\nMax Obstacle Width (halved for length): 0" +
                        "\nKnight Count: 0" +
                        "\nKnight Moves Jump Every (random): 0" +
                        "\nKnight Cooldown (cycles): 0" +
                        "\nEnvironment Character (space for void): \" \" #Enter a single letter/symbol/space inside the quotes." +
                        "\nPlayer Character: \" \"" +
                        "\nFood Character: \" \"" +
                        "\nEnemy Character: \" \"" +
                        "\nObstacle Character: \" \"" +
                        "\nKnight Active Character: \" \"" +
                        "\nKnight Inactive Character: \" \"";
                        //create custom settings file
                        File.WriteAllText(CustomSettingsFileName, template);
                    }

                    //import custom settings file
                    string[] lines = File.ReadAllLines(CustomSettingsFileName);

                    //extract values and build custom mode
                    byte valueCount = 12;
                    int[] customMode = new int[valueCount];
                    for (byte i = 0; i < valueCount; i++)
                    {
                        int currValue = Int32.Parse(lines[i].Split(':', '#')[1]);
                        customMode[i] = currValue;
                    }

                    //load custom settings
                    LoadSettings(customMode);

                    //extract characters and create character array
                    byte charCount = 7;
                    char[] chars = new char[charCount];
                    for (byte i = valueCount; i < valueCount + charCount; i++)
                    {
                        char currChar = Char.Parse(lines[i].Split('"')[1]);
                        chars[i - valueCount] = currChar;
                    }

                    //assign characters
                    Env.Char = chars[0];
                    Play.er.Char = chars[1];
                    Food.Char = chars[2];
                    Enemy.Char = chars[3];
                    Obs.Char = chars[4];
                    Knight.ActiveChar = chars[5];
                    Knight.InactiveChar = chars[6];
                }
                catch
                {
                    Console.WriteLine($"Error Importing \"{CustomSettingsFileName}\".");
                    Thread.Sleep(1500);
                    NoU.Main();
                }
            }
        }
        public static string ModeStr()
        {
            switch (Mode)
            {
                default: return "<CUSTOM>";
                case 0: return "<CASUAL>";
                case 1: return "<AVERAGE>";
                case 2: return "<HARDCORE>";
            }
        }
    }
    static class Stats
    {
        public static int Moves { get; set; }
        public static int Foods { get; set; }
        public static int Lives { get; set; }
        public static int Deaths { get; set; }
        public static int EnemyMoves { get; set; }
        public static int KnightMoves { get; set; }
        public static int Kills { get; set; }
        public static int ObsMoves { get; set; }
        public static bool GameOver { get; set; } = false;
        public static int Score
        {
            get
            {
                return (int)((((Foods + Kills) * 5) + (Moves / 20)) * Math.Pow(0.90, Deaths));
            }
        }

        public static string Ending()
        {
            if (GameOver) return "(GAMEOVER)";
            else return "(QUITTED)";
        }
        public static string EnvStr()
        {
            if (Env.Char == ' ') return "<SPACE>";
            else return Char.ToString(Env.Char);
        }
    }
    static class Random
    {
        //for gen-ing random numbers
        public static readonly System.Random Gen = new System.Random();
    }
}
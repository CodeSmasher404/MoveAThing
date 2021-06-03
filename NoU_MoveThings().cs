using System;
using System.Threading;

namespace MoveAThing
{
    partial class NoU
    {
        void MoveThings()
        {
            bool goBrr = true;
            while (goBrr && Play.er.Lives > 0)
            {
                //move thing by changing x and y
                bool moved = false;
                switch (toMove)
                {
                    case 1:
                        if (Play.er.Y < Env.Length)
                        {
                            Play.er.Y++;
                            moved = true;
                        }
                        break;
                    case 2:
                        if (Play.er.X > 1)
                        {
                            Play.er.X--;
                            moved = true;
                        }
                        break;
                    case 3:
                        if (Play.er.Y > 1)
                        {
                            Play.er.Y--;
                            moved = true;
                        }
                        break;
                    case 4:
                        if (Play.er.X < Env.Width)
                        {
                            Play.er.X++;
                            moved = true;
                        }
                        break;
                    case 255:
                        goBrr = false;
                        readMovement.Abort();
                        continue;
                    default:
                        break;
                }

                //increase stats moves
                if (moved) Stats.Moves++;

                //enemy stuff
                foreach (Enemy currEnemy in enemyArr)
                {
                    currEnemy.Move();
                    //die
                    if (Play.er.Equals(currEnemy))
                    {
                        currEnemy.Rng();
                        Play.er.Die();

                        //gameover
                        if (Play.er.Lives == 0)
                        {
                            Stats.GameOver = true;
                            readMovement.Abort();
                            continue;
                        }
                    }
                }

                //knight stuff
                foreach (Knight currknight in knightArr)
                {
                    Enemy closestEnemy = enemyArr[Play.er.GetClosest(enemyArr)];
                    currknight.MoveTowards(closestEnemy);

                    if (currknight.Equals(closestEnemy) && currknight.CurrCooldown == 0)
                    {
                        closestEnemy.Rng();
                        Stats.Kills++;
                        currknight.ResetCooldown();
                    }
                }

                //obstacle stuff
                foreach (Obs currObs in obsArr)
                {
                    currObs.Move();
                    Stats.ObsMoves++;

                    if (currObs.Equals(Play.er))
                    {
                        currObs.Rng();
                        Play.er.Die();

                        //gameover
                        if (Play.er.Lives == 0)
                        {
                            Stats.GameOver = true;
                            readMovement.Abort();
                            continue;
                        }
                        //beep
                        Console.Beep();
                    }
                }

                //food stuff
                foreach (Food currFood in foodArr)
                {
                    if (Play.er.Equals(currFood))
                    {
                        currFood.Rng();
                        Stats.Foods++;
                    }
                }

                WriteThings();
                Thread.Sleep(ToSleep);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        const int MINIMAL_CMD_WIDTH     = 25;   //  Count of columns
      //const double TIME_COEFFICIENT   = 0.98; //  Time multiplier
        const int END_DELAY             = 1700; //  In milliseconds
        const char SNAKE                = '#';  //  Snake character
        const char POINT                = '♦';  //  Point character

        static int delay                = 150;  //  Step delay (initial value)

        static void Main(string[] args)
        {
            Console.Title = "Snake Game";
            Console.Write("Enter size of square field: ");

            //=============VARIABLES=============//
            int border; // Size of field
            read: while (!int.TryParse(Console.ReadLine(), out border)) Console.WriteLine("Wrong value!");
            if (border < 2)
            {
                Console.WriteLine("Wrong value!");
                goto read;
            }

            Random random = new Random();           //  Randomizer
            int x       = 1;                        //  X coordinate
            int y       = 2;                        //  Y coordinate
            int xPoint  = random.Next(1, border+1); //  X coordinate of point
            int yPoint  = random.Next(2, border+2); //  Y coordinate of point
            int score   = 1;                        //  Length of snake

            List<List<int>> coordXY = new List<List<int>>   //  Coordinates list (position of all parts of snake)
            {
                new List<int> { 0, 0 }
            };
            ConsoleKey lastKey = ConsoleKey.RightArrow; //  Last pressed key
            ConsoleKey currentKey;                      //  Current pressed key

            //=============CONSOLE SETUP=============//
            Console.Clear();
            try { Console.SetWindowSize((border + 2 > MINIMAL_CMD_WIDTH ? border + 2 : MINIMAL_CMD_WIDTH), border+3); }
            catch
            {
                Console.Write("Size of square field is too large");
                Console.ReadKey(true);
                return;
            }

            for (int i = 1; i <= border; ++i)   // Setting frame
            {
                Console.SetCursorPosition(0, i+1);          Console.Write("║");
                Console.SetCursorPosition(border+1, i+1);   Console.Write("║");
                Console.SetCursorPosition(i, 1);            Console.Write("═");
                Console.SetCursorPosition(i, border+2);     Console.Write("═");
            }
            Console.SetCursorPosition(0, 1);                Console.Write("╔");
            Console.SetCursorPosition(border+1, 1);         Console.Write("╗");
            Console.SetCursorPosition(0, border+2);         Console.Write("╚");
            Console.SetCursorPosition(border+1, border+2);  Console.Write("╝");

            Console.SetCursorPosition(0, 0);
            Console.WriteLine(" Score: " + score);
            Console.SetCursorPosition(xPoint, yPoint);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(POINT);

            //=============GAME CYCLE=============//
            while (x >= 1 && y >= 2 && x < border+1 && y < border+2 && !ContainsEqualCoordinates(coordXY, x, y))
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(SNAKE);
                coordXY.Add(new List<int> { x, y });

                if (xPoint == x && yPoint == y) // Getting point
                {
                    if (++score == border * border) break;
                  //delay = (int)(delay * TIME_COEFFICIENT);    //  Delay increasing
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(" Score: " + score);

                    int randomElement = random.Next(border * border - score);
                    int currentElement = -1;
                    for (int xtmp = 1; xtmp < border + 1; ++xtmp)
                        for (int ytmp = 2; ytmp < border + 2; ++ytmp)
                        {
                            if (!ContainsEqualCoordinates(coordXY, xtmp, ytmp)) currentElement++;
                            else continue;
                            if (currentElement == randomElement)
                            {
                                xPoint = xtmp;
                                yPoint = ytmp;
                                goto setpoint;
                            }
                        }

                    setpoint:
                    Console.SetCursorPosition(1, 0);
                    Console.SetCursorPosition(xPoint, yPoint);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(POINT);
                    //Console.Beep();   // Cause delay!!!
                }
                else
                {
                    Console.SetCursorPosition(coordXY[0][0], coordXY[0][1]);
                    Console.Write(" ");
                    coordXY.RemoveAt(0);
                }
                
                Thread.Sleep(delay);    //  Delay
                
                if (Console.KeyAvailable && !IsParallel(currentKey = Console.ReadKey(true).Key, lastKey))
                {
                    switch (currentKey)
                    {
                        case ConsoleKey.LeftArrow:
                            --x;
                            break;
                        case ConsoleKey.UpArrow:
                            --y;
                            break;
                        case ConsoleKey.RightArrow:
                            ++x;
                            break;
                        case ConsoleKey.DownArrow:
                            ++y;
                            break;
                        default: continue;
                    }
                    lastKey = currentKey;
                }
                else
                {
                    switch (lastKey)
                    {
                        case ConsoleKey.LeftArrow:
                            --x;
                            break;
                        case ConsoleKey.UpArrow:
                            --y;
                            break;
                        case ConsoleKey.RightArrow:
                            ++x;
                            break;
                        case ConsoleKey.DownArrow:
                            ++y;
                            break;
                    }
                }
                
            }
            //=============END OF GAME=============//
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game over   ");
            Thread.Sleep(END_DELAY);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Your score: " + score + "\nPress Enter to exit...");

            while (Console.ReadKey(true).Key != ConsoleKey.Enter);
        }

        static bool IsParallel(ConsoleKey key1, ConsoleKey key2)
        {
            if (key1 == key2) return true;

            if (key1 == ConsoleKey.LeftArrow || key1 == ConsoleKey.RightArrow)
                return key2 == ConsoleKey.LeftArrow || key2 == ConsoleKey.RightArrow;

            if (key1 == ConsoleKey.UpArrow || key1 == ConsoleKey.DownArrow)
                return key2 == ConsoleKey.UpArrow || key2 == ConsoleKey.DownArrow;

            return false;
        }

        static bool ContainsEqualCoordinates(List<List<int>> xyList, int x, int y)
        {
            int countOfElements = xyList.Count;
            for (int i = 0; i < countOfElements; ++i)
                if (xyList[i][0] == x && xyList[i][1] == y) return true;

            return false;
        }
    }
}
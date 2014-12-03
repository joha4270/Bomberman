using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;

namespace Bomberman
{
    class Program
    {
        public static Menu Menu;
        public static Game Game;
        public static Stopwatch MainStopwatch;
        public static long DeltaTime { get; private set; }

        private bool force = false;

        static void Main(string[] args)
        {
            //Create a new instance (not sure why acctualy)
            new Program().Start(args);
        }

        private void DebugingSetup()
        {

            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    Renderes.Menu[x, y] = (x + y) % 2 == 0 ?
                        new CharInfo('X', ConsoleColor.Red, ConsoleColor.White)
                        : new CharInfo((char)0);
                    //Renderes.GameField[x,y] = new CharInfo('X', ConsoleColor.Red, ConsoleColor.White);
                    //Renderes.EntityLayer[x,y] = new CharInfo('A', ConsoleColor.Red, ConsoleColor.White);

                }
            }
        }

        private static void Setup()
        {

            //Stopwatch for timekeeping
            MainStopwatch = new Stopwatch();
            MainStopwatch.Start();

            //Change the console window size
            //80,25 is horible for gaming
            Console.SetWindowSize(79, 37);
            Console.SetBufferSize(79, 37);
            //Need 1 extra line to prevent it from scolling after printing last
            //charecter. That is bad as it makes screen flicker up and down
            //It also looks horible

            //if just there were some kind of WinAPI that allowed me to write
            //to the console screen buffer without any kind of fuzz while being
            //2 orders of magnitude faster. That be great right?
            //Such as write to the $CONSOUT file?
            Console.CursorVisible = false;


            //Setup our various render layers
            //Renderes is just an evil global
            //*Swearword* conventions
            Renderes.Setup();


            //While this is not really the intended purpose of the Map class
            //it works without any problems as long as you don't try to use it
            //for a real game and there is no reason to reimplement the wheel 
            //to read "images"
            Map menuBackground = new Map(System.IO.File.OpenRead("Maps/_MenuPage.txt"));
            for (int x = 0; x < 79; x++)
            {
                for (int y = 0; y < 37; y++)
                {
                    Renderes.Background[x, y] = menuBackground.UnbrokenMap[x, y];
                }
            }

            //setup menu
            Menu = new Menu(Renderes.Menu);

            //code for adding menu options here

            //Let the menu decide how to place its stuff
            Menu.Setup();

            //Load background here
            //Animation maybe?

            //Render the menu for the first time
            //Background is the parrent to all other layes
            //So updating that updates all
            Renderes.Background.Update(true);

            Trace.WriteLine("Game initalized in "+ MainStopwatch.ElapsedMilliseconds + " ms");
            
        }

        private void Start(string[] args)
        {
            //Do all setup in a function instead of keeping it messy here
            Setup();
            
            
            //Main game loop
            //Executes 
            while(true)
            {
                //Save current timestamp
                long time = MainStopwatch.ElapsedMilliseconds;
                
                //Check for any keypresses
                //This had been the message pump for a Win32 C++ application
                //Also this have a big problem compared to a real message loop
                //I can only get a generic keyevent, not sperate keyup/keydown
                //Events
                //I might even have to resort to polling instead
                while(Console.KeyAvailable)
                {
                    //If there is any keys, do something with it
                    //If the key press combo results in the game exiting
                    //break the main loop
                    if (HandleKey(Console.ReadKey(true))) break;
                }

                //If the game is acctive and existing, update state
                if (Game != null && Game.Active)
                {
                    //it is inside here all the game logic is performed
                    Game.UpdateGameState(DeltaTime);
                }
                
                //Render our changes to the screen
                Renderes.Background.Update(force);
                force = false;

                //Max 60 hz
                while (MainStopwatch.ElapsedMilliseconds - time < (1000/60))
                {
                    DeltaTime = MainStopwatch.ElapsedMilliseconds - time;
                    
                }
                //Trace.WriteLine(String.Format("Finished tick in {0} ms", DeltaTime));
            }
        }



        private bool HandleKey(ConsoleKeyInfo info)
        {
            
            //if(info.KeyChar == 'd')
            //    DebugingSetup();

            //if (info.KeyChar == 'a')
            //    Renderes.Menu.Enable = !Renderes.Menu.Enable;

            //if (info.KeyChar == 'f')
            //    force = true;

            if (info.KeyChar == 'i' && Game == null)
            {
                Dictionary<ConsoleKey, PlayerDirection> keyBind = new Dictionary<ConsoleKey,PlayerDirection>
                {
                    {ConsoleKey.W, new PlayerDirection(0, Direction.Up)},
                    {ConsoleKey.S, new PlayerDirection(0, Direction.Down)},
                    {ConsoleKey.A, new PlayerDirection(0, Direction.Left)},
                    {ConsoleKey.D, new PlayerDirection(0, Direction.Right)},
                    {ConsoleKey.Spacebar, new PlayerDirection(0, Direction.Place)},
                    {ConsoleKey.UpArrow, new PlayerDirection(1, Direction.Up)},
                    {ConsoleKey.DownArrow, new PlayerDirection(1, Direction.Down)},
                    {ConsoleKey.LeftArrow, new PlayerDirection(1, Direction.Left)},
                    {ConsoleKey.RightArrow, new PlayerDirection(1, Direction.Right)},
                    {ConsoleKey.Enter, new PlayerDirection(1, Direction.Place)}
                };
                Game = new Game(new Map(File.OpenRead("Maps/75_29_field.txt")), 2, keyBind) {Active = true};
                Renderes.GameField.Enable = true;
                //force = true;
                return false;
            }

            if(Game != null && Game.Active)
            {
                Game.HandleKey(info);
                return false;
            }
            else
            {
                return Menu.HandleKey(info);
            }
        }
    }
}

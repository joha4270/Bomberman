using System;
using System.Collections.Generic;
using System.IO;

namespace Bomberman
{
    class Map
    {
        public Point Size { get; private set; }

        public readonly int[,] CollisionMap;
        public readonly CharInfo[,] UnbrokenMap;
        public readonly CharInfo[,] BrokenMap;
        public float DropChance = 0.5f;
        public float BombChance = 0.4f;
        public float PowerUpChance = 0.3f;
        public float BootsChance = 0.2f;
        public float HpUpCHance = 0.1f;


        public int PowerUp = 1;
        public int BombUp = 1;
        public float spdUp = 0.5f;
        public int HpUp = 1;

        public List<SpawnPoint> SpawnPoints;

        public Map(Stream fileStream)
        {
            int line = 0;
            try
            {

                SpawnPoints = new List<SpawnPoint>();
                StreamReader sr = new StreamReader(fileStream);
                String[] sizeString = sr.ReadLine().Split(' ');
                int sizeX = int.Parse(sizeString[0]);
                int sizeY = int.Parse(sizeString[1]);
                Size = new Point(sizeX, sizeY);

                while(!sr.EndOfStream)
                {
                    String type = sr.ReadLine();
                    if(type == "collision:")
                    {
                        CollisionMap = ReadColMap(sr, sizeX, sizeY, ref line);
                    }
                    else if(type == "broken:")
                    {
                        BrokenMap = ReadDisplayMap(sr, sizeX, sizeY, ref line);
                    }
                    else if (type == "start:")
                    {
                        UnbrokenMap = ReadDisplayMap(sr, sizeX, sizeY, ref line);
                    }
                    else
                    {
                        //yes yes, no exception handeling for controll flow, 
                        //i know
                        throw new Exception();
                    }
                }

            }
            catch(Exception ex)
            {
                MapFalureException nex = new MapFalureException(line, ex);
                
                throw nex; //CHANGE TO NEX
            }
        }

        private static CharInfo[,] ReadDisplayMap(StreamReader sr, int sizeX, int sizeY, ref int line)
        {
            CharInfo[,] arr = new CharInfo[sizeX, sizeY];
            for (int y = 0; y < sizeY; y++)
            {

                //Readblock is prob better but here i don't have to worry about
                //diffrences in newline size
                //Only unicode ):
                char[] inarr = sr.ReadLine().ToCharArray();
                line++;
                for (int x = 0; x < sizeX; x++)
                {
                    short attrib = short.Parse(inarr[x * 3 + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    attrib |= (short)(short.Parse(inarr[x*3 + 2].ToString(), System.Globalization.NumberStyles.HexNumber) << 4);
                    //ConsoleColor bg = (ConsoleColor)int.Parse(inarr[x * 3 + 2].ToString(), System.Globalization.NumberStyles.HexNumber);
                    //ConsoleColor fg = (ConsoleColor)int.Parse(inarr[x * 3 + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    arr[x, y] = new CharInfo(inarr[x * 3], attrib);
                }
            }

            return arr;
        }

        private int[,] ReadColMap(StreamReader sr, int sizeX, int sizeY, ref int line)
        {
            int[,] arr = new int[sizeX, sizeY];
            Dictionary<char, SpawnPoint> spawns = new Dictionary<char, SpawnPoint>();

            for (int i = 0; i < sizeY; i++)
            {

                //Readblock is prob better but here i don't have to worry about
                //diffrences in newline size
                //Only unicode ):
                char[] inarr = sr.ReadLine().ToCharArray();
                line++;
                for (int x = 0; x < sizeX; x++)
                {
                    if ('9' >= inarr[x] && inarr[x] >= '0')
                    {  //number
                        arr[x, i] = inarr[x] - '0';
                    }
                    else
                    { //Else it is a spawn point or an error
                        if (spawns.ContainsKey(inarr[x]))
                        {
                            spawns[inarr[x]].ClearArear.Add(new Point(x,i));
                        }
                        else if('z' >= inarr[x] && inarr[x] >= 'a')
                        {
                            spawns.Add(inarr[x], new SpawnPoint());
                            spawns[inarr[x]].ClearArear.Add((new Point(x, i)));
                        }
                        else if('Z' >= inarr[x] && inarr[x] >= 'A')
                        {
                            if (spawns.ContainsKey((char)(inarr[x] - ('A' - 'a'))))
                            {
                                spawns[(char)(inarr[x] - ('A' - 'a'))].point = new Point(x,i);
                            }
                            else
                            {
                                spawns.Add((char)(inarr[x] - ('A' - 'a')), new SpawnPoint());
                                spawns[(char)(inarr[x] - ('A' - 'a'))].point = new Point(x, i);
                                spawns[inarr[x]].ClearArear.Add(new Point(x, i));

                            }
                        }
                        arr[x, i] = 0;
                    }
                }
            }

            SpawnPoints.AddRange(spawns.Values);
            return arr;
        }
    }
}

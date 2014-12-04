using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bomberman
{
    class Game
    {
        private Map _map;
        private readonly Player[] _players;
        private readonly Dictionary<ConsoleKey, PlayerDirection> _keyBinding;
        private readonly byte[,] _colMap; //declared as byte instead of bool since it
        //won't change structure size in memory and extra data can be kept
        //Broken, unbroken, unbreakable etc
        private readonly Random _random;

        public bool Active { get; set; }

        public Game(Map m, int players, Dictionary<ConsoleKey, PlayerDirection> keyBinding)
        {
            _keyBinding = keyBinding;
            _map = m;
            _keyBinding = keyBinding;
            _players = new Player[players];
            _colMap = new byte[m.Size.x, m.Size.y];
            _random = new Random();
            for (int x = 0; x < m.Size.x; x++)
            {
                for (int y = 0; y < m.Size.y; y++)
                {
                    _colMap[x, y] = (byte)(m.CollisionMap[x, y] > 0 ? 128 : 129);

                }
            }

            for (int i = 0; i < players; i++)
            {
                _players[i] = new Player(m.SpawnPoints[i].point, this) { Display = new CharInfo(1, 0xF)};
                Renderes.EntityLayer.Entities.Add(_players[i]);

                foreach (Point p in m.SpawnPoints[i].ClearArear)
                {
                    _colMap[p.x, p.y] = 0;
                }

                _colMap[m.SpawnPoints[i].point.x, m.SpawnPoints[i].point.y] = 0;
            }

            for (int x = 0; x < m.Size.x; x++)
            {
                for (int y = 0; y < m.Size.y; y++)
                {
                    if (IsOcupied(x, y))
                    {
                        Renderes.GameField[x, y] = m.UnbrokenMap[x, y];
                    }
                    else
                    {
                        Renderes.GameField[x, y] = m.BrokenMap[x, y];
                    }
                }
            }
            
        }

        public bool IsOcupied(int x, int y)
        {
            if(x < 0 || y < 0 || x >= _map.Size.x || y >= _map.Size.y)
                return true;
            //Check for most signifigant bit, if set, the block is colliding
            //anything belov can be used as housekeeping data later
            //Bit meanings from msb to lsb
            //7 Colliding
            //6 Unused
            //5 Unused
            //4 Unused
            //3 Unused
            //2 Unused
            //1 Unused
            //0 Permanent
            return (_colMap[x, y] >= 128);
            
        }

        internal void UpdateGameState(long delta)
        {
            if(Active == false) return;
            
            foreach (Player player in _players)
            {
                if (player.UpdatePos(delta))
                {
                    //Resharper was anoyed if i used player directley
                    Player player1 = player;
                    var pickups =
                        from entity in Renderes.EntityLayer.Entities
                        where entity != player1 &&
                              entity.Location == player1.Location &&
                              entity.GetType() == typeof (Powerup)
                        select entity;

                    IEnumerable<Entity> entities = pickups as Entity[] ?? pickups.ToArray();
                    foreach (Entity pickup in entities)
                    {
                        Powerup p = (Powerup) pickup;
                        player.MaxBombs += p.BombUp;
                        player.Hp += p.HpUp;
                        player.Speed += p.SpeedUp;
                        player.BombSize += p.PowerUp;

                    }

                    var list = entities.ToList();

                    Renderes.EntityLayer.Entities.RemoveAll(list.Contains);
                }
            }

            var exploding = Renderes.EntityLayer.Entities.FindAll(
                en => en.GetType() == typeof (Bomb) && (((Bomb) en).Fuse -= delta) <= 0);

            Renderes.EntityLayer.Entities.RemoveAll(exploding.Contains);

            foreach (Entity entity in exploding)
            {
                Bomb b = (Bomb) entity;

                if (b.Power > 0)
                {
                    ExplodeBomb(b, new Point(1, 0));
                    ExplodeBomb(b, new Point(-1, 0));
                    ExplodeBomb(b, new Point(0, 1));
                    ExplodeBomb(b, new Point(0, -1));
                }
            }
        }

        private void ExplodeBomb(Bomb b, Point dir)
        {
            for (int i = 0; i <= b.Power; i++)
            {
                
                Point ofset = dir*i;
                Bomb nBomb = new Bomb()
                {
                    Display = Options.BombGfx,
                    Location = b.Location + ofset,
                    Power = 0,
                    Fuse = Options.BombGfxTime
                };

                if 
                (
                    b.Location.x + ofset.x < 0 || 
                    b.Location.y + ofset.y < 0 ||
                    b.Location.x + ofset.x >= _map.Size.x ||
                    b.Location.y + ofset.y >= _map.Size.y
                    
                ) break;

                List<Entity> hit =
                    (from entity in Renderes.EntityLayer.Entities
                    where entity.Location == (b.Location + ofset)
                    && entity.OnHit() 
                    select entity).ToList();

                if (hit.ToList().Count != 0)
                {
                    Renderes.EntityLayer.Entities.RemoveAll(hit.Contains);
                }
                
                if (_colMap[b.Location.x + ofset.x, b.Location.y + ofset.y] >= 128)
                {
                    if ((_colMap[b.Location.x + ofset.x, b.Location.y + ofset.y] & 1) != 0)
                    {
                        _colMap[b.Location.x + ofset.x, b.Location.y + ofset.y] = 0;
                        Renderes.EntityLayer.Entities.Add(nBomb);
                        Renderes.GameField[b.Location.x + ofset.x, b.Location.y + ofset.y] =
                            _map.BrokenMap[b.Location.x + ofset.x, b.Location.y + ofset.y];

                        DropHandle(b.Location + ofset);
                    }
                    break;
                }
                else
                {
                    Renderes.EntityLayer.Entities.Add(nBomb);
                }
                
            }
        }

        private void DropHandle(Point location)
        {
            if (_random.NextDouble() < _map.DropChance)
            {
                float sum = _map.BootsChance + _map.BombChance + _map.HpUpCHance + _map.PowerUpChance;
                sum -= ((float)_random.NextDouble()*sum);
                
                if ((sum -= _map.BombChance) < 0)
                {
                    Powerup p = new Powerup()
                    {
                        BombUp = _map.BombUp,
                        Display = Options.BombAmountUpDisplay,
                        Location = location
                    };
                    Renderes.EntityLayer.Entities.Add(p);
                }
                else if((sum -= _map.PowerUpChance) < 0)
                {
                    Powerup p = new Powerup()
                    {
                        Display = Options.BombPowerUpDisplay,
                        Location = location,
                        PowerUp = _map.PowerUp
                    };
                    Renderes.EntityLayer.Entities.Add(p);
                }
                else if((sum -= _map.BootsChance) < 0)
                {
                    Powerup p = new Powerup()
                    {
                        Display = Options.BootsDisplay,
                        Location = location,
                        SpeedUp = _map.spdUp
                    };
                    Renderes.EntityLayer.Entities.Add(p);
                }
                else  //hpup
                {
                    Powerup p = new Powerup()
                    {
                        Display = Options.HpupDisplay,
                        Location = location,
                        HpUp = _map.HpUp
                    };
                    Renderes.EntityLayer.Entities.Add(p);
                }

            }
        }

        internal void HandleKey(ConsoleKeyInfo consoleKeyInfo)
        {
            if (_keyBinding.ContainsKey(consoleKeyInfo.Key))
            {
                _players[_keyBinding[consoleKeyInfo.Key].PlayerId]
                    .SetDirection(_keyBinding[consoleKeyInfo.Key].Direction);
            }
        }


        internal void CreateBomb(Player player)
        {
            int ExistingBombs = Renderes.EntityLayer.Entities.Count(
                entity => entity.GetType() == typeof (Bomb) && ((Bomb) entity).Owner == player);

            if (ExistingBombs >= player.MaxBombs) return;
            Bomb b = new Bomb()
            {
                Owner = player,
                Display = Options.BombDisplay,
                Fuse = Options.BombFuse,
                Power = player.BombSize,
                Location = player.Location
            };
            Renderes.EntityLayer.Entities.Add(b);
        }
    }
}

using System;
using System.Diagnostics;

namespace Bomberman
{
    class Player : Entity
    {
        public override Point Location
        {
            get { return new Point(_xPos, _yPos); }
        }

        public float Speed = 2;
        public int Hp = 3;
        public int MaxBombs = 1;
        public int BombSize = 2;

        public bool UpdatePos(long time)
        {
            float moveDist = (time*Speed)/1000;
            //Trace.WriteLine(String.Format("Player moved {0}", moveDist));
            _yMove += _directionY*moveDist;
            _xMove += _directionX*moveDist;

            if (Math.Abs(_xMove) >= 1)
            {
                int move = _xMove > 0 ? 1 : -1;
                _xMove -= Math.Sign(_xMove);
                if (_hostGame.IsOcupied(_xPos + move, _yPos))
                {
                    Trace.WriteLine(String.Format("Player hit something in ({0},{1})", _xPos + move, _yPos));
                    SetDirection(Direction.Still);
                    return false;
                }
                _xPos += move;

                
                Trace.WriteLine(String.Format("Player pos updated to ({0},{1})", _xPos, _yPos));
                return true;
            }

            if(Math.Abs(_yMove) >= 1)
            {
                int move = _yMove > 0 ? 1 : -1;
                _yMove -= Math.Sign(_yMove);
                if (_hostGame.IsOcupied(_xPos, _yPos + move))
                {
                    Trace.WriteLine(String.Format("Player hit something in ({0},{1})", _xPos, _yPos + move));
                    SetDirection(Direction.Still);
                    return false;
                }
                _yPos += move;

                
                Trace.WriteLine(String.Format("Player pos updated to ({0},{1})", _xPos, _yPos));
                return true;
            }

            return false;
        }

        public bool SetDirection(Direction d)
        {
            if (_nDir == d)
                return false;

            if(d <= Direction.Still)
            {
                if (d == Direction.Down)
                {
                    _directionX = 0;
                    _directionY = 1;
                }
                else if (d == Direction.Up)
                {
                    _directionX = 0;
                    _directionY = -1;
                }
                else if(d == Direction.Left)
                {
                    _directionX = -1;
                    _directionY = 0;
                }
                else if (d == Direction.Right)
                {
                    _directionX = 1;
                    _directionY = 0;
                }
                else if(d == Direction.Still)
                {
                    _directionX = 0;
                    _directionY = 0;

                }
                _nDir = d;
                return true;
            }
            else if (d == Direction.Place)
            {
                Trace.WriteLine("Placing bomb");
                _hostGame.CreateBomb(this);
            }

            return false;
        }

        private int _xPos;
        private int _yPos;
        private float _xMove;
        private float _yMove;
        private float _directionX;
        private float _directionY;
        private Direction _nDir;
        private readonly Game _hostGame;

        public Player(Point point, Game game)
        {
            _nDir = Direction.Still;
            _directionY = 0;
            _directionX = 0;
            _xPos = point.x;
            _yPos = point.y;
            _hostGame = game;
        }

        
        
    }
}

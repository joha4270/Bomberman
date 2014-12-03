using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman
{
    internal struct PlayerDirection
    {
        public int PlayerId;
        public Direction Direction;

        public PlayerDirection(int playerId, Direction direction)
        {
            PlayerId = playerId;
            Direction = direction;
        }
    }
}

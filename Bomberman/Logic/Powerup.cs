using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman
{
    class Powerup : Entity
    {
        public float SpeedUp = 0;
        public int BombUp = 0;
        public int HpUp = 0;
        public int PowerUp = 0;

        public override bool OnHit()
        {
            return true;
        }
    }
}

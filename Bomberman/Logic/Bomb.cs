namespace Bomberman
{
    class Bomb : Entity
    {
        public int Power;
        public long Fuse;
        public Player Owner;

        public override bool OnHit()
        {
            if (Power != 0)
                Fuse = 0;


            return false;
        }
    }
}

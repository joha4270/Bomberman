namespace Bomberman
{
    class Entity
    {
        public virtual CharInfo Display { get; set; }
        public virtual Point Location { get; set; }
        public virtual bool OnHit() { return false; }
    }
}

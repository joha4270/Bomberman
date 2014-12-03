using System.Collections.Generic;
using System.Diagnostics;

namespace Bomberman
{
    class EntityRender : Renderable
    {
        readonly CharInfo _alphaCharInfo = new CharInfo() {Attributes = 0, Char = new CharUnion() {AsciiChar = 0}};
        public readonly List<Entity> Entities;

        public EntityRender(int xSize, int ySize) : base(xSize, ySize)
        {
            Entities = new List<Entity>();
        }

        public override CharInfo this[int x, int y]
        {
            get 
            {
                Point p = new Point(x,y);
                int entityIndex = Entities.FindIndex((t) => t.Location == p);

                if (entityIndex >= 0)
                {
                    
                    return Entities[entityIndex].Display;

                }
                else
                {
                    return _alphaCharInfo;
                }

            }
            set { ; }
        }
    }
}

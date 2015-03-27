using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Components
{
    public class Portal : GDComp
    {
        public Portal(Game1 game, Vector2 position)
            : base(game, Shape.None, position, new Vector2())
        {

        }
    }
}

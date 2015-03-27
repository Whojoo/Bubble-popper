using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GameDesign_2.FPS
{
    public interface IDraw<T>
        where T : class
    {
        void LoadContent(ContentManager contentManager);
        void Draw(DrawingContext context, T objectToDraw);
    }
}

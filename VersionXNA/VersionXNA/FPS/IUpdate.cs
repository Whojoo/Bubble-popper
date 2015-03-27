using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.FPS
{
    public interface IUpdate<T>
        where T : class
    {
        void Update(GameTime gameTime, T objectToUpdate);
    }
}

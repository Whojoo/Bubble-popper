using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.States
{
    public interface IState
    {
        void Activate();
        void Update(GameTime gameTime);
    }
}

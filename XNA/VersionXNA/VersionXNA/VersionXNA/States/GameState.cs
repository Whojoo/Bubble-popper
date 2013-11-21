using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Screens;

namespace GameDesign_2.States
{
    public class GameState : IState
    {
        protected GameplayScreen Parent { get; private set; }

        public GameState(GameplayScreen parent)
        {
            Parent = parent;
        }

        public virtual void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
        }
    }
}

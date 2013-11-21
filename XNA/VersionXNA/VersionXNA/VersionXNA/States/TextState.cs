using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.States
{
    public class TextState : IState
    {
        protected virtual TextObject Parent { get; set; }

        public TextState(TextObject parent)
        {
            Parent = parent;
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}

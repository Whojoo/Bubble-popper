using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.States;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Text
{
    public class MenuEntry : TextObject
    {
        protected IState State { get; set; }

        public MenuEntry(string text)
            : base(text)
        {
        }

        public virtual void SetActive(bool active)
        {
        }
    }
}

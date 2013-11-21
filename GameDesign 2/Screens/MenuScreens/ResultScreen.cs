using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDesign_2.Screens.MenuScreens
{
    public class ResultScreen : MenuScreen
    {
        public ResultScreen(Game1 game, string title)
            : base(game, title)
        {
        }

        protected override void EntryClicked(int index)
        {
            Manager.Pop();
            Manager.Push(new MainMenuScreen(GDGame));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Screens.MenuScreens
{
    public class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen(Game1 game)
            : base(game, "Bubblepopper")
        {
            Entries.Add(new MenuEntry("hello"));
            Entries.Add(new MenuEntry("this is"));
            Entries.Add(new MenuEntry("my awesome"));
            Entries.Add(new MenuEntry("game :D"));
        }

        protected override void EntryClicked(int index)
        {
            Manager.Pop();
            Manager.Push(new GameplayScreen(GDGame, new Vector2(1280, 720), 100000));
        }
    }
}

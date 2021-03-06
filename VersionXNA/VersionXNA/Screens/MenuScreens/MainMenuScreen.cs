﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Screens.GameplayScreens;
using GameDesign_2.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Screens.MenuScreens
{
    public class MainMenuScreen : MenuScreen
    {
        private enum EntryOption : int
        {
            Dynamics,
            Narrative
        }

        public MainMenuScreen(Game1 game)
            : base(game, "Bubblepopper")
        {
            Entries.Add(new MenuEntry("Dynamics"));
            Entries.Add(new MenuEntry("Narrative"));
        }

        protected override void EntryClicked(int index)
        {
            Manager.Pop();

            switch (index)
            {
                case (int)EntryOption.Dynamics:
                    Manager.Push(new DynamicsScreen(GDGame));
                    break;
                case (int)EntryOption.Narrative:
                    Manager.Push(new Narrative(GDGame));
                    break;
            }
        }
    }
}

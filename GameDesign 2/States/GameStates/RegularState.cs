using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Screens;

namespace GameDesign_2.States.GameStates
{
    public class RegularState : GameState
    {
        private int agroBorder;
        private int[] borders;
        private bool recheckBorder = false;

        public RegularState(StateMachine parent, int[] agroBorders)
            : base(parent)
        {
            borders = agroBorders;
            agroBorder = GetNextBorder(parent.Screen, agroBorders);
        }

        private int GetNextBorder(GameplayScreen screen, int[] agroBorders)
        {
            int score = (int)screen.Player.ScoreBar.GetScorePercentage();

            for (int i = 0; i < agroBorders.Length; i++)
            {
                if (score < agroBorders[i])
                {
                    return agroBorders[i];
                }
            }

            //No more borders so return max score.
            return 100;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (recheckBorder)
            {
                agroBorder = GetNextBorder(Parent.Screen, borders);
                recheckBorder = false;
            }

            if (Parent.Screen.Player.ScoreBar.GetScorePercentage() > agroBorder)
            {
                Parent.PushState(new AgroState(Parent, agroBorder));
                recheckBorder = true;
            }

            base.Update(gameTime);
        }
    }
}

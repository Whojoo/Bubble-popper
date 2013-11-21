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

        public RegularState(GameplayScreen parent, int[] agroBorders)
            : base(parent)
        {
            borders = agroBorders;
            agroBorder = GetNextBorder(parent, agroBorders);
        }

        private int GetNextBorder(GameplayScreen parent, int[] agroBorders)
        {
            int score = (int)parent.Player.ScoreBar.GetScorePercentage();

            for (int i = 0; i < agroBorders.Length; i++)
            {
                if (score < agroBorders[i])
                {
                    return agroBorders[i];
                }
            }

            return 100;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (recheckBorder)
            {
                agroBorder = GetNextBorder(Parent, borders);
                recheckBorder = false;
            }

            if (Parent.Player.ScoreBar.GetScorePercentage() > agroBorder)
            {
                Parent.CurrentState = new AgroState(Parent, agroBorder);
                recheckBorder = true;
            }

            base.Update(gameTime);
        }
    }
}

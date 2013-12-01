using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.Components.Player;
using GameDesign_2.Screens;
using Microsoft.Xna.Framework;

namespace GameDesign_2.States.GameStates
{
    public class RegularState : GameState
    {
        public int AgroBorder { get; private set; }

        private int[] borders;
        private bool recheckBorder = false;
        private bool isTough;

        public RegularState(StateMachine parent, int[] agroBorders, bool isTough)
            : base(parent)
        {
            borders = agroBorders;
            AgroBorder = GetNextBorder(parent.Screen, agroBorders);
            this.isTough = isTough;
        }

        public override void Activate()
        {
            Parent.Screen.GDGame.Background = isTough ? 
                Color.CadetBlue : Color.CornflowerBlue;
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
                AgroBorder = GetNextBorder(Parent.Screen, borders);
                recheckBorder = false;
            }

            if (Parent.Screen.Player.ScoreBar.GetScorePercentage() > AgroBorder)
            {
                Parent.Proceed(this);
                recheckBorder = true;
                return;
            }

            //Is this the tougher RegularState?
            if (isTough)
            {
                const int agroDist = 100;
                GameComponentCollection components = Parent.Screen.Components;
                PlayerBall player = Parent.Screen.Player;

                //Add a small agro radius around each enemy ScoreBall.
                foreach (GDComp comp in components.OfType<GDComp>().Where<GDComp>(x => x is ScoreBall))
                {
                    ScoreBall ball = (ScoreBall)comp;

                    if (ball.ScoreState == ScoreBall.State.Enemy)
                    {
                        float dist = (ball.Position - player.Position).Length();
                        dist -= ball.HalfSize.X + player.HalfSize.X;

                        if (dist < agroDist)
                        {
                            ball.Target = player;
                        }
                        else
                        {
                            ball.Target = null;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }
    }
}

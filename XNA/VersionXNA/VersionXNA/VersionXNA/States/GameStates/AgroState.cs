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
    public class AgroState : GameState
    {
        //The marge the player has to get out of.
        private const int percentalMarge = 10;
        private const int agroDist = 150;

        private int agroBorder;

        public AgroState(GameplayScreen parent, int agroBorder)
            : base(parent)
        {
            this.agroBorder = agroBorder;
            parent.GDGame.Background = Color.Black;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            PlayerBall player = Parent.Player;
            ScoreBall ball;
            foreach (GDComp comp in Parent.Components)
            {
                if ((ball = comp as ScoreBall) != null &&
                    ball.ScoreState == ScoreBall.State.Enemy)
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

            if (player.ScoreBar.GetScorePercentage() > agroBorder + percentalMarge ||
                player.ScoreBar.GetScorePercentage() < agroBorder - percentalMarge)
            {
                RemoveState();
            }

            base.Update(gameTime);
        }

        private void RemoveState()
        {
            ScoreBall ball;
            foreach (GDComp comp in Parent.Components)
            {
                if ((ball = comp as ScoreBall) != null)
                {
                    ball.Target = null;
                }
            }

            Parent.GDGame.Background = Color.CornflowerBlue;
            Parent.RemoveTopState();
        }
    }
}

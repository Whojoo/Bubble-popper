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
        private const int agroDist = 200;

        private int agroBorder;
        private int oldBalance;

        public AgroState(StateMachine parent, int agroBorder)
            : base(parent)
        {
            this.agroBorder = agroBorder;
            oldBalance = Spawner.GetInstance().FriendliesPerEnemies;
            Spawner.GetInstance().FriendliesPerEnemies = 2;
        }

        public override void Activate()
        {
            Parent.Screen.GDGame.Background = Color.Black;
            base.Activate();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            PlayerBall player = Parent.Screen.Player;
            ScoreBall ball;
            foreach (GDComp comp in Parent.Screen.Components)
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

            if (player.ScoreBar.GetScorePercentage() > agroBorder + percentalMarge)
            {
                RemoveState(true);
            }
            else if (player.ScoreBar.GetScorePercentage() < agroBorder - percentalMarge)
            {
                RemoveState(false);
            }

            base.Update(gameTime);
        }

        private void RemoveState(bool proceed)
        {
            ScoreBall ball;
            foreach (GDComp comp in Parent.Screen.Components)
            {
                if ((ball = comp as ScoreBall) != null)
                {
                    ball.Target = null;
                }
            }

            Spawner.GetInstance().FriendliesPerEnemies = oldBalance;

            Parent.Screen.GDGame.Background = Color.CornflowerBlue;
            Parent.PopState();
            if (proceed)
            {
                Parent.Proceed(this);
            }
        }
    }
}

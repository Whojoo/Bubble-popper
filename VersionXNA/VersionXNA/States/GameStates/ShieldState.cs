using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.Components.Player;
using Microsoft.Xna.Framework;

namespace GameDesign_2.States.GameStates
{
    public class ShieldState : GameState
    {
        //The marge the player has to get out of.
        private const int PercentalMarge = 10;
        private const int AgroDist = 350;
        private const float StateDuration = 30;

        private float timer;
        private float startScore;
        private int oldBalance;
        private ShieldBall shield;

        public ShieldState(StateMachine parent)
            : base(parent)
        {
            //Create the shield.
            Vector2 worldSize = parent.Screen.WorldSize;
            Rectangle world = new Rectangle(0, 0,
                (int)worldSize.X, (int)worldSize.Y);
            shield = new ShieldBall(parent.Screen.GDGame, world, this);
            shield.Initialize();
        }

        public override void Activate()
        {
            oldBalance = Spawner.GetInstance().FriendliesPerEnemies;
            Spawner.GetInstance().FriendliesPerEnemies = 0;
            Parent.Screen.GDGame.Background = Color.DarkBlue;
            Parent.Screen.Components.Add(shield);
            timer = 0;

            //Get the current score.
            startScore = Parent.Screen.Player.ScoreBar.GetScorePercentage();

            ScoreBar bar = Parent.Screen.Player.ScoreBar;
            bar.ForceMultiplier(0.001f, StateDuration);

            base.Activate();
        }

        public override void Deactivate()
        {
            Spawner.GetInstance().FriendliesPerEnemies = oldBalance;
            Parent.Screen.Components.Remove(shield);
            base.Deactivate();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            PlayerBall player = Parent.Screen.Player;
            ScoreBall ball;

            //Did we pass the agro border?
            if (player.ScoreBar.GetScorePercentage() > startScore + PercentalMarge)
            {
                RemoveState(true);
            }

            //Timer.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += dt;
            if (timer >= StateDuration)
            {
                Parent.Proceed(this);
                return;
            }

            //Agro logic.
            foreach (GDComp comp in Parent.Screen.Components)
            {
                if ((ball = comp as ScoreBall) != null &&
                    ball.ScoreState == ScoreBall.State.Enemy)
                {
                    float dist = (ball.Position - player.Position).Length();
                    dist -= ball.HalfSize.X + player.HalfSize.X;

                    if (dist < AgroDist)
                    {
                        ball.Target = player;
                    }
                    else
                    {
                        ball.Target = null;
                    }
                }
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

            Parent.Screen.Player.ScoreBar.ResetMultiplier();

            Parent.Screen.Components.Remove(shield);

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

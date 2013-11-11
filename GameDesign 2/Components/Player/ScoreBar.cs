using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Components.Player
{
    public class ScoreBar : GDComp
    {
        //Drop the score by 5% after 10 seconds.
        private const int SecondsPerPointDrop = 10;
        private const int PercentDropByTimeBorder = 5;

        private readonly int PercentPerSecond;

        public int Score { get; private set; }

        private int toAdd;
        private int toSubtract;
        private int goalScore;
        private float timeCounter;

        public ScoreBar(Game1 game, Vector2 position, Vector2 halfSize, int goalScore)
            : base(game, Shape.None, position, halfSize)
        {
            toAdd = 0;
            toSubtract = 0;
            this.goalScore = goalScore;
            PercentPerSecond = (int)(goalScore * 0.01f);
            Score = 0;
        }

        public void AddScore(int amount)
        {
            if (amount < 0)
            {
                return;
            }

            toAdd += amount;
        }

        public void SubtractScore(int amount)
        {
            if (amount < 0)
            {
                amount = Math.Abs(amount);
            }

            toSubtract += amount;
        }

        public override void Update(GameTime gameTime)
        {
            //Delta Time.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //First calculate how many points max can be add or subtracted this frame.
            int pointsThisFrame = (int)(PercentPerSecond * dt);

            //Are there points to subtract?
            if (toSubtract > 0)
            {
                int loss = pointsThisFrame < toSubtract ? pointsThisFrame : toSubtract;

                Score -= loss;
                toSubtract -= loss;
            }
            //Are there points to add?
            else if (toAdd > 0)
            {
                int gain = pointsThisFrame < toAdd ? pointsThisFrame : toAdd;

                Score += gain;
                toAdd -= gain;
            }

            //Lastly the timer.
            timeCounter += dt;
            if (timeCounter >= SecondsPerPointDrop)
            {
                timeCounter -= SecondsPerPointDrop;
                SubtractScore((int)(PercentDropByTimeBorder * 0.01f * goalScore));
            }

            base.Update(gameTime);
        }
    }
}

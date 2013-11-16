using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameDesign_2.Components.Player;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Components
{
    public class ScoreBall : Ball
    {
        public enum State
        {
            Enemy,
            Friendly
        }

        private const float Speed = 100;
        private int sinusoidIndex;
        private Vector2 reverseIndex;
        public State ScoreState { get; private set; }

        public ScoreBall(Game1 game, Vector2 position, int sinusoidIndex)
            : base(game, position, 10)
        {
            this.sinusoidIndex = sinusoidIndex;
            reverseIndex = new Vector2(1, 1);
            ScoreState = State.Enemy;
        }

        public ScoreBall(Game1 game, Vector2 position, int sinusoidIndex, State scoreState)
            : base(game, position, 10)
        {
            this.sinusoidIndex = sinusoidIndex;
            reverseIndex = new Vector2(1, 1);
            ScoreState = scoreState;
        }

        private void AdjustVelocity()
        {
            //Add a steering to the velocity. ReverseIndex decides if this goes left or right.
            Vector2 steeringVel = new Vector2(Speed * reverseIndex.X,
                Sinusoid.GetInstance().GetSinusoid(sinusoidIndex) * Speed * reverseIndex.Y);

            Velocity += steeringVel;
        }

        /// <summary>
        /// Change this ball's state.
        /// </summary>
        /// <param name="state">The new state.</param>
        public void ChangeState(State state)
        {
            ScoreState = state;

            switch (state)
            {
                case State.Enemy:
                    Color = Color.Red;
                    break;
                case State.Friendly:
                    Color = GetFriendlyColor();
                    break;
            }
        }

        public override bool CheckCollisionWith(GameTime gameTime, GDComp other)
        {
            //Early out if we have another ScoreBall.
            if (other is ScoreBall)
            {
                return false;
            }
            else if (other is PlayerBall)
            {
                if (CircleCircleCollision(other))
                {
                    PlayerHit(other as PlayerBall);
                    return false;
                }

                return false;
            }
            else
            {
                return other.CheckCollisionWith(gameTime, this);
            }
        }

        private void CorrectVelocity()
        {
            float lengthSQ = Velocity.LengthSquared();

            if (lengthSQ > Speed * Speed)
            {
                float comparison = Speed / (float)Math.Sqrt(lengthSQ);

                Velocity *= comparison;
            }
        }

        private Color GetFriendlyColor()
        {
            Random randy = new Random();

            //Green and blue can be between 0 and 255.
            int green = randy.Next(0, 255);
            int blue = randy.Next(0, 255);

            //Enemies are red so make sure the red value is the lowest.
            int red = randy.Next(0, green < blue ? green : blue);

            //Randomized alpha. Keep it above 0 but below 1 by: (X + 0.5) * 0.6.
            float alpha = (float)(randy.NextDouble() + 0.5f) * 0.6f;

            return new Color(red, green, blue, alpha);
        }

        private void PlayerHit(PlayerBall playerBall)
        {
            if (Remove)
            {
                return;
            }

            const int AddScore = 50;

            switch (ScoreState)
            {
                case State.Enemy:
                    playerBall.SubtractScore();
                    break;
                case State.Friendly:
                    playerBall.AddScore(AddScore);
                    break;
            }

            //Let the spawner remove this Scoreball.
            Spawner.GetInstance().RemoveBall(this);
        }

        public void ReverseXMovement()
        {
            reverseIndex.X *= -1;
        }

        public void ReverseYMovement()
        {
            reverseIndex.Y *= -1;
        }

        public override void Update(GameTime gameTime)
        {
            AdjustVelocity();
            CorrectVelocity();
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }
    }
}

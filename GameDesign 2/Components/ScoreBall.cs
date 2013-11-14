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
        public State ScoreState { get; set; }

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

        private void CorrectVelocity()
        {
            float lengthSQ = Velocity.LengthSquared();

            if (lengthSQ > Speed * Speed)
            {
                float comparison = Speed / (float)Math.Sqrt(lengthSQ);

                Velocity *= comparison;
            }
        }

        public override bool CheckCollisionWith(GameTime gameTime, GDComp other)
        {
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

        private void PlayerHit(PlayerBall playerBall)
        {
            const int AddScore = 50;

            switch (ScoreState)
            {
                case State.Enemy:
                    playerBall.SubtractScore();
                    break;
                case State.Friendly:
                    playerBall.AddScore(AddScore);
                    break;
                default:
                    Debug.WriteLine("Uh-oh");
                    break;
            }

            Remove = true;
        }

        public void ReverseXMovement()
        {
            reverseIndex.X *= -1;
        }

        public void ReverseYMovement()
        {

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Components
{
    public class ScoreBall : Ball
    {
        private const float Speed = 100;
        private int sinusoidIndex;
        private int reverseIndex;

        public ScoreBall(Game1 game, Vector2 position, int sinusoidIndex)
            : base(game, position, 10)
        {
            this.sinusoidIndex = sinusoidIndex;
            reverseIndex = 1;
        }

        private void AdjustVelocity()
        {
            //Add a steering to the velocity. ReverseIndex decides if this goes left or right.
            Vector2 steeringVel = new Vector2(Speed * reverseIndex,
                Sinusoid.GetInstance().GetSinusoid(sinusoidIndex) * Speed);

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
            else
            {
                return other.CheckCollisionWith(gameTime, this);
            }
        }

        public void ReverseXMovement()
        {
            reverseIndex *= -1;
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

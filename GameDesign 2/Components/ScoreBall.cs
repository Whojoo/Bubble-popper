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
        private const float TimeAlive = 15;

        private int sinusoidIndex;
        private float timeLeft;
        private Vector2 reverseIndex;
        private bool initialised;

        public State ScoreState { get; private set; }
        public PlayerBall Target { get; set; }

        public ScoreBall(Game1 game, Vector2 position, int sinusoidIndex)
            : base(game, position, 10)
        {
            this.sinusoidIndex = sinusoidIndex;
            reverseIndex = new Vector2(1, 1);
            ScoreState = State.Enemy;
            timeLeft = TimeAlive;
            initialised = false;
        }

        public override void Initialize()
        {
            timeLeft = TimeAlive;

            if (!initialised)
            {
                initialised = true;

                base.Initialize();
            }
        }

        private void AdjustVelocity()
        {
            //Add a steering to the velocity. ReverseIndex decides if this goes left or right.
            Vector2 steeringVel;

            if (Target != null)
            {
                Vector2 dir = Target.Position - Position;
                dir.Normalize();
                steeringVel = dir * Speed;
            }
            else
            {
                steeringVel = new Vector2(Speed * reverseIndex.X,
                Sinusoid.GetInstance().GetSinusoid(sinusoidIndex) * Speed * reverseIndex.Y);
            }

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

            return new Color((new Color(new Vector3(0,
                randy.Next(150, 250),
                randy.Next(150, 250)))), 0.05f);
        }

        private void PlayerHit(PlayerBall playerBall)
        {
            if (Remove)
            {
                return;
            }

            switch (ScoreState)
            {
                case State.Enemy:
                    playerBall.SubtractScore();
                    break;
                case State.Friendly:
                    playerBall.AddScore();
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
            timeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeLeft <= 0)
            {
                Spawner.GetInstance().RemoveBall(this);
            }

            AdjustVelocity();
            CorrectVelocity();
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }
    }
}

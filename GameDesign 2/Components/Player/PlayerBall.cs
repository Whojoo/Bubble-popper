using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDesign_2.Components.Player
{
    public class PlayerBall : Ball
    {
        private const float PlayerRadius = 30;

        public ScoreBar ScoreBar { get; private set; }

        private int goalScore;

        public PlayerBall(Game1 game, Vector2 position, int goalScore)
            : base(game, position, PlayerRadius)
        {
            ScoreBar = new ScoreBar(game, goalScore);
            this.goalScore = goalScore;
            Color = Color.Blue;
            Scale = 1;
        }

        public override void Initialize()
        {
            //Get the screen's center.
            Viewport vp = GDGame.GraphicsDevice.Viewport;
            Vector2 center = new Vector2(vp.Width * 0.5f, vp.Height * 0.5f);

            //Put the mouse back in the screen's center.
            Mouse.SetPosition((int)center.X, (int)center.Y);

            //Add the scoreBar.
            GDGame.GetActiveScreen().HuDComponents.Add(ScoreBar);

            base.Initialize();
        }

        public void AddScore(int amount)
        {
            ScoreBar.AddScore((int)(goalScore * 0.025f));
        }

        public override bool CheckCollisionWith(GameTime gameTime, GDComp other)
        {
            if (other is Wall)
            {
                return other.CheckCollisionWith(gameTime, this);
            }
            else if (other is ScoreBall)
            {
                return other.CheckCollisionWith(gameTime, this);
            }
            return false;
        }

        public void Move(GameTime gameTime)
        {
            //Get the screen's center.
            Viewport vp = GDGame.GraphicsDevice.Viewport;
            Vector2 center = new Vector2(vp.Width * 0.5f, vp.Height * 0.5f);

            //Get the current mouse position.
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);

            //Put the mouse back in the screen's center.
            Mouse.SetPosition((int)center.X, (int)center.Y);

            //Calculate the velocity.
            Velocity = mousePos - center;

            //Change the position.
            Position += Velocity;

            //Collision handling uses a frame's velocity, which is multiplied by the elapsed time.
            //But the player's velocity is equal to real time mouse movement.
            //So to prevent errors, we have to divide the velocity by the elapsed time.
            Velocity /= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void SubtractScore()
        {
            ScoreBar.SubtractScore((int)(goalScore * 0.025f));
        }

        public override void Update(GameTime gameTime)
        {
            //Only update the player (and the mouse placement) if this game is the active window.
            if (GDGame.IsActive)
            {
                Move(gameTime);
            }

            //Start growing when the score reachest 25%. Stop at 80% with a radius of 100.
            float scorePercentage = ScoreBar.GetScorePercentage();
            if (scorePercentage > 25 && scorePercentage <= 80)
            {
                float growFactor = (120f / 60f) - 1;
                Scale = 1 + growFactor * ((scorePercentage - 25) / 55f);
            }
            else if (scorePercentage > 80)
            {
                Scale = 120f / 60f;
            }
            else
            {
                Scale = 1;
            }

            base.Update(gameTime);
        }

        public override Vector2 HalfSize
        {
            get
            {
                return base.HalfSize * Scale;
            }
            set
            {
                base.HalfSize = value;
            }
        }
    }
}

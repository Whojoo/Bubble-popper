﻿using System;
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
        public const float ScoreDropDefault = 5f;
        public const float ScoreAddDefault = 2.5f;

        private const float MinRadius = 30;
        private const float MaxRadius = 40;

        public ScoreBar ScoreBar { get; private set; }
        public PowerBar PowerBar { get; private set; }

        //Score adds or drops on their default values.
        private float scoreDroppedByEnemy;
        private float scoreAddedByFriendly;

        public PlayerBall(Game1 game, Vector2 position)
            : base(game, position, MinRadius)
        {
            scoreDroppedByEnemy = ScoreDropDefault;
            scoreAddedByFriendly = ScoreAddDefault;

            //Create the bars.
            ScoreBar = new ScoreBar(game);
            PowerBar = new PowerBar(game);

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

            //Add the bars to the HuD.
            GDGame.GetActiveScreen().HuDComponents.Add(ScoreBar);
            GDGame.GetActiveScreen().HuDComponents.Add(PowerBar);

            base.Initialize();
        }

        /// <summary>
        /// This function is called when the player hits a friendly circle.
        /// </summary>
        public void AddScore()
        {
            ScoreBar.AddScore(scoreAddedByFriendly);
            PowerBar.LoadPower(ScoreBar.Multiplier);
        }

        public override bool CheckCollisionWith(GameTime gameTime, GDComp other)
        {
            if (other is PowerBall)
            {
                return false;
            }

            return other.CheckCollisionWith(gameTime, this);
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

            //Last frame's timestep.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Collision handling uses a frame's velocity, which is multiplied by the elapsed time.
            //But the player's velocity is equal to real time mouse movement.
            //So to prevent errors, we have to divide the velocity by the elapsed time.
            Velocity /= dt;
        }

        /// <summary>
        /// This function is called when a enemy circle hits the player.
        /// </summary>
        public void SubtractScore()
        {
            ScoreBar.SubtractScore(scoreDroppedByEnemy, true);
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
                float growFactor = (MaxRadius / MinRadius) - 1;
                Scale = 1 + growFactor * ((scorePercentage - 25) / 55f);
            }
            else if (scorePercentage > 80)
            {
                Scale = MaxRadius / MinRadius;
            }
            else
            {
                Scale = 1;
            }

            //Power usage logic.
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                PowerBar.UsePower(Position);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Get or Set the score added by hitting a friendly.
        /// Max score is 100.
        /// Default = 2.5f.
        /// </summary>
        public float ScoreAddedByFriendly
        {
            get { return scoreAddedByFriendly; }
            set { scoreAddedByFriendly = value > 0 ? value : scoreAddedByFriendly; }
        }

        /// <summary>
        /// Get or Set the score dropped by hitting an enemy.
        /// Max score is 100.
        /// Default = 5f.
        /// </summary>
        public float ScoreDroppedByEnemy
        {
            get { return scoreDroppedByEnemy; }
            set { scoreDroppedByEnemy = value > 0 ? value : scoreDroppedByEnemy; }
        }
    }
}

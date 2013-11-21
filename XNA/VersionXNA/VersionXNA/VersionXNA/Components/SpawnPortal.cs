using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.Components
{
    public class SpawnPortal : GDComp
    {
        private const float PNGRadius = 100;

        private Random randy;
        private Vector2 scaleCorrection;
        private Texture2D texture;

        public SpawnPortal(Game1 game, Vector2 position, Vector2 halfSize)
            : base(game, Shape.None, position, halfSize)
        {
            //Randomly pressed numbers for seed.
            randy = new Random(5936);
            scaleCorrection = new Vector2(
                halfSize.X * 2 / PNGRadius,
                HalfSize.Y * 2 / PNGRadius);
        }

        protected override void LoadContent()
        {
            texture = GDGame.GetActiveScreen().Content.
                Load<Texture2D>("circle");
            Origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);

            base.LoadContent();
        }

        /// <summary>
        /// Get a random spawn position in this portal.
        /// </summary>
        /// <returns>The spawnposition.</returns>
        public Vector2 GetSpawnPosition()
        {
            //Use this X and Y position.
            float xPos = Position.X;
            float yPos = Position.Y;

            //Add or subtract a randomized HalfSize's Y value.
            yPos += ((float)randy.NextDouble() - 0.5f) * 2 * HalfSize.Y;

            return new Vector2(xPos, yPos);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            batch.Draw(texture, Position, null, Color.Purple, 0, Origin, scaleCorrection, SpriteEffects.None, 0);

            base.Draw(gameTime, batch);
        }
    }
}

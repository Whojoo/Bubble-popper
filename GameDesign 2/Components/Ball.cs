using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.Components
{
    public class Ball : GDComp
    {
        private const float PNGRadius = 100;

        private Texture2D circle;
        private float scaleCorrection;

        public Color Color { get; set; }
        protected float Scale { get; set; }
        protected SpriteEffects Effect { get; set; }

        public Ball(Game1 game, Vector2 position, float radius)
            : base(game, Shape.Circle, position, new Vector2(radius))
        {
            Color = Color.Red;
            Scale = 1;
            Effect = SpriteEffects.None;

            scaleCorrection = radius * 2 / PNGRadius;
        }

        protected override void LoadContent()
        {
            circle = GDGame.GetActiveScreen().Content.Load<Texture2D>("circle");

            Origin = new Vector2(circle.Width, circle.Height) * 0.5f;

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            batch.Draw(circle, Position, null, Color, 0, Origin, Scale * scaleCorrection, Effect, 0);
            //batch.Draw(circle, Position, null, Color, 0, new Vector2(), Scale * scaleCorrection, Effect, 0);

            base.Draw(gameTime, batch);
        }
    }
}

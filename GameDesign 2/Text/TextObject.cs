using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.Text
{
    public class TextObject
    {
        private string text;

        public float Scale { get; set; }
        public Color Color { get; set; }
        public SpriteEffects Effect { get; set; }
        public Vector2 Position { get; set; }

        public TextObject(string text)
        {
            this.text = text;

            Scale = 1;
            Color = Color.White;
            Effect = SpriteEffects.None;
            Position = new Vector2();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch batch, SpriteFont font)
        {
            Vector2 origin = font.MeasureString(text) * 0.5f;
            batch.DrawString(font, text, Position, Color, 0, origin, Scale, Effect, 0);
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }
}

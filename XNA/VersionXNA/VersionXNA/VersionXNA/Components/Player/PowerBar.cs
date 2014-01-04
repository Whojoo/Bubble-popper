using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.Components.Player
{
    public class PowerBar : GDComp
    {
        private float toAdd;
        private float power;
        private Texture2D tex;

        public PowerBar(Game1 game)
            : base(game, Shape.None, SetPosition(game), SetHalfSize(game))
        {
        }

        public override void Initialize()
        {
            toAdd = 0;
            power = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentManager content = GDGame.GetActiveScreen().Content;
            tex = content.Load<Texture2D>("square");

            base.LoadContent();
        }

        private static Vector2 SetHalfSize(Game1 game)
        {
            Viewport vp = game.GraphicsDevice.Viewport;
            float x = vp.Height * 9 / 10;
            float y = vp.Width * 0.5f;

            return new Vector2(x, y);
        }

        private static Vector2 SetPosition(Game1 game)
        {
            Viewport vp = game.GraphicsDevice.Viewport;
            float halfHeight = vp.Width * 0.5f * 0.8f;
            float halfWidth = vp.Height * 1 / 20;

            return new Vector2(halfWidth, halfHeight);
        }
    }

}

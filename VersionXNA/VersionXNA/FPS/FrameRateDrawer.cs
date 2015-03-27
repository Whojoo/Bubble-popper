using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.FPS
{
    public class FrameRateDrawer : IDraw<FrameRate>
    {
        SpriteFont spriteFont;
        private const string FontName = "MyFont";
        private readonly Vector2 _fpsPositionBlack = new Vector2(20, 20);
        private readonly Vector2 _fpsPositionWhite = new Vector2(20, 20);
        private readonly Vector2 _minPositionBlack = new Vector2(20, 50);
        private readonly Vector2 _minPositionWhite = new Vector2(20, 50);

        public void LoadContent(ContentManager contentManager)
        {
            spriteFont = contentManager.Load<SpriteFont>(FontName);
        }

        public void Draw(DrawingContext context, FrameRate objectToDraw)
        {
            context.SpriteBatch.DrawString(spriteFont,
                objectToDraw.Rate.ToString(), _fpsPositionBlack, Color.Black);
            context.SpriteBatch.DrawString(spriteFont,
                objectToDraw.Rate.ToString(), _fpsPositionWhite, Color.White);

            context.SpriteBatch.DrawString(spriteFont,
                objectToDraw.MinRate.ToString(), _minPositionBlack, Color.Black);
            context.SpriteBatch.DrawString(spriteFont,
                objectToDraw.MinRate.ToString(), _minPositionWhite, Color.Orange);

            objectToDraw.Counter++;
        }
    }
 
}

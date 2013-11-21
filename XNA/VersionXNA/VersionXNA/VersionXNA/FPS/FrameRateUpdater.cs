using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.FPS
{
    public class FrameRateUpdater : IUpdate<FrameRate>
    {
        public void Update(GameTime gameTime, FrameRate frameRate)
        {
            frameRate.ElapsedTime
                += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (frameRate.ElapsedTime >= 1000f)
            {
                frameRate.ElapsedTime -= 1000f;
                frameRate.Rate = frameRate.Counter;
                if (frameRate.SecondsElapsed > 0
                    && frameRate.MinRate > frameRate.Rate)
                {
                    frameRate.MinRate = frameRate.Rate;
                }
                frameRate.Counter = 0;
                frameRate.SecondsElapsed++;
            }
        }
    }
}

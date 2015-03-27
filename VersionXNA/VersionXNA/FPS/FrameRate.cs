using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDesign_2.FPS
{
    public class FrameRate
    {
        /// <summary>
        /// Current FPS
        /// </summary>
        public int Rate;

        /// <summary>
        /// Frame counter
        /// </summary>
        public int Counter;

        /// <summary>
        /// Time elapsed from the last update
        /// </summary>
        public float ElapsedTime;

        /// <summary>
        /// Min FPS
        /// </summary>
        public int MinRate = 999;

        /// <summary>
        /// Seconds elapsed
        /// </summary>
        public int SecondsElapsed;
    }
}

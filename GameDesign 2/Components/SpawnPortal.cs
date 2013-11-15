using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Components
{
    public class SpawnPortal : GDComp
    {
        private Random randy;

        public SpawnPortal(Game1 game, Vector2 position, Vector2 halfSize)
            : base(game, Shape.None, position, halfSize)
        {
            //Randomly pressed numbers for seed.
            randy = new Random(5936);
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
    }
}

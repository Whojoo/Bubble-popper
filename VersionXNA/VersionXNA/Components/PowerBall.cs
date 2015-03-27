using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Components
{
    public class PowerBall : Ball
    {
        private const float MaxRadius = 600f;

        public PowerBall(Game1 game, Vector2 position)
            : base(game, position, 0.5f)
        {
            Color = new Color(100, 0, 100, 0.2f);
        }

        public override bool CheckCollisionWith(GameTime gameTime, GDComp other)
        {
            other.CheckCollisionWith(gameTime, this);
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            //Calculate the bruto grow.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Calculate the grow and multiply by an half to make it last 2 seconds.
            float grow = dt * MaxRadius * 0.5f;

            //Adjust the grow to the 300px radius limit.
            grow = MaxRadius - HalfSize.X * 2 < grow ? 
                MaxRadius - HalfSize.X * 2 : grow;

            Scale += grow;

            if (Scale >= MaxRadius)
            {
                Remove = true;
            }

            base.Update(gameTime);
        }
    }
}

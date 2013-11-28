using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components.Sensors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.Components
{
    public abstract class GDComp : DrawableGameComponent
    {
        public Game1 GDGame { get; private set; }
        public Shape Shape { get; private set; }
        public bool Remove { get; set; }

        protected Vector2 Origin { get; set; }

        private Vector2 halfSize;
        private Vector2 position;
        private Vector2 velocity;

        public GDComp(Game1 game, Shape shape, Vector2 position, Vector2 halfSize)
            : base(game)
        {
            GDGame = game;
            Shape = shape;
            HalfSize = halfSize;
            Position = position;
            velocity = new Vector2();
            Remove = false;
        }

        public override void Initialize()
        {
            //LoadContent();

            base.Initialize();
        }

        public virtual bool CheckCollisionWith(GameTime gameTime, GDComp other)
        {
            //Is either shape collidable?
            if (Shape == Shape.None || other.Shape == Shape.None)
            {
                return false;
            }

            //If this is a sensor then this point would never be reached, so...
            //Is the other a sensor?
            if (other.Shape == Shape.Sensor)
            {
                //Make sure the sensor becomes a 'this' and then end this check.
                other.CheckCollisionWith(gameTime, this);
                return false;
            }

            //Is this a circle?
            if (Shape == Shape.Circle)
            {
                //Is the other a circle?
                if (other.Shape == Shape.Circle)
                {
                    return CircleCircleCollision(other);
                }
                //Is the other a box?
                else
                {
                    return CircleBoxCollision(other);
                }
            }
            //Is this a box?
            else
            {
                //Is the other a circle?
                if (other.Shape == Shape.Circle)
                {
                    return other.CircleBoxCollision(this);
                }
                //Is the other a box?
                else
                {
                    return BoxBoxCollision(other);
                }
            }
        }

        /// <summary>
        /// Check if 2 box shaped objects collide.
        /// </summary>
        /// <param name="other">The other box.</param>
        public bool BoxBoxCollision(GDComp other)
        {
            bool left, right, up, down;

            //Simple collision check using the position and halfsizes.
            left = position.X - halfSize.X < other.Position.X + other.HalfSize.X;
            right = position.X + halfSize.X > other.Position.X - other.HalfSize.X;
            up = position.Y - halfSize.Y < other.Position.Y + other.HalfSize.Y;
            down = position.Y + halfSize.Y > other.Position.Y - other.HalfSize.Y;

            return left && right && up && down;
        }

        /// <summary>
        /// Checks if a circle and box collide. 
        /// This function assumes that this is the circle and the other a the box.
        /// </summary>
        /// <param name="other">The box.</param>
        public bool CircleBoxCollision(GDComp other)
        {
            //Calculate the distance between origins.
            Vector2 circleDist = new Vector2();
            circleDist.X = Math.Abs(position.X - other.Position.X);
            circleDist.Y = Math.Abs(position.Y - other.Position.Y);

            //Are the 2 objects close to eachother?
            if (circleDist.X > (other.HalfSize.X + HalfSize.X) ||
                circleDist.Y > (other.HalfSize.Y + HalfSize.Y))
            {
                return false;
            }

            //Is the circle in the box?
            if (circleDist.X <= other.HalfSize.X ||
                circleDist.Y <= other.HalfSize.Y)
            {
                return true;
            }

            //Now use Pythagoras to check if the distance from the box is smaller
            //than the circle's radius.
            float cornerDistSQ = (circleDist - other.HalfSize).LengthSquared();

            return cornerDistSQ <= halfSize.X * halfSize.X;
        }

        /// <summary>
        /// Checks if 2 circles collide or not.
        /// </summary>
        /// <param name="other">The other circle.</param>
        public bool CircleCircleCollision(GDComp other)
        {
            //Simple collision check using both the circles' radius.
            float distSQ = (position - other.Position).LengthSquared();
            float radiiSQ = (halfSize.X + other.HalfSize.X) * (halfSize.X + other.HalfSize.X);

            return distSQ < radiiSQ;
        }

        public Rectangle GetRect()
        {
            return new Rectangle(
                (int)(Position.X - HalfSize.X),
                (int)(Position.Y - HalfSize.Y),
                (int)(HalfSize.X * 2),
                (int)(HalfSize.Y * 2));
        }

        public override void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch batch)
        {
        }

        public virtual void Unload()
        {
        }

        public virtual Vector2 HalfSize
        {
            get { return halfSize; }
            set { halfSize = value; }
        }

        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public virtual Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
    }

    public enum Shape
    {
        Circle,
        Box,
        Sensor,
        None
    }
}

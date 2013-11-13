using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.Components
{
    public class Wall : GDComp
    {
        private const float PNGSize = 100;

        private Texture2D wall;
        private Vector2 scaleCorrection;

        public Wall(Game1 game, Vector2 topLeft, Vector2 bottomRight)
            : base(game, Shape.Box, 
            (topLeft + bottomRight) * 0.5f, 
            (bottomRight - topLeft) * 0.5f)
        {
            scaleCorrection = new Vector2();
            scaleCorrection.X += HalfSize.X * 2 / PNGSize;
            scaleCorrection.Y += HalfSize.Y * 2 / PNGSize;
            wall = new Texture2D(game.GraphicsDevice, 100, 100);
        }

        protected override void LoadContent()
        {
            wall = GDGame.GetActiveScreen().Content.Load<Texture2D>("square");

            Origin = new Vector2(wall.Width, wall.Height) * 0.5f;

            base.LoadContent();
        }

        public override bool CheckCollisionWith(GameTime gameTime, GDComp other)
        {
            //Ignore all walls.
            if (other is Wall)
            {
                return false;
            }

            //Is there actually a collision?
            if (!base.CheckCollisionWith(gameTime, other))
            {
                return false;
            }

            //From here on it can only be a ball. But just to be sure, check it.
            if (other.Shape == Components.Shape.Circle)
            {
                CircleBoxCollisionHandling(gameTime, other as Ball);
            }

            //This only triggers handling so just return false.
            return false;
        }

        private void CircleBoxCollisionHandling(GameTime gameTime, Ball other)
        {
            //Safety check.
            if (other == null)
            {
                return;
            }

            //Put the circle back according the velocity.
            other.Position -= other.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Calculate the connectionline.
            LineSegment connectionLine = new LineSegment(Position, other.Position);

            //Get each point.
            Vector2 p1 = Position - HalfSize;
            Vector2 p2 = new Vector2(Position.X + HalfSize.X, Position.Y - HalfSize.Y);
            Vector2 p3 = Position + HalfSize;
            Vector2 p4 = new Vector2(Position.X - HalfSize.X, Position.Y + HalfSize.Y);

            //Fill an array with all lines.
            LineSegment[] lines = new LineSegment[4];
            lines[0] = new LineSegment(p1, p2);
            lines[1] = new LineSegment(p2, p3);
            lines[2] = new LineSegment(p3, p4);
            lines[3] = new LineSegment(p4, p1);

            //Get the vector in which we reflect the velocity;
            Vector2 reflectionVec = Vector2.Zero;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IntersectsWith(connectionLine) &&
                    connectionLine.IntersectsWith(lines[i]))
                {
                    reflectionVec = lines[i].Direction();

                    //Is the other a ScoreBall?
                    if (other is ScoreBall)
                    {
                        //Is this a vertical collision?
                        reflectionVec.Normalize();
                        float dot = Math.Abs(Vector2.Dot(reflectionVec, Vector2.UnitY));

                        const float dotBorder = 0.5f;

                        if (dot > dotBorder)
                        {
                            //Reverse the x movement.
                            ScoreBall temp = other as ScoreBall;
                            temp.ReverseXMovement();
                        }
                        else
                        {
                            //Reverse the y movement.
                            ScoreBall temp = other as ScoreBall;
                            temp.ReverseYMovement();
                        }
                    }

                }
            }

            //Reflect the velocity.
            other.Velocity = Vector2.Reflect(other.Velocity, reflectionVec);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            float rotation = 0;
            float depth = 0;
            Color colo = Color.White;
            batch.Draw(wall, Position, null, colo, rotation, Origin, scaleCorrection, 
                SpriteEffects.None, depth);

            base.Draw(gameTime, batch);
        }
    }
}

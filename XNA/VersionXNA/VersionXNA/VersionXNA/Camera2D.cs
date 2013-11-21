using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2
{
    public class Camera2D : GDComp
    {
        //Lower and Upper zoom limits.
        private const float LowerZoomLimit = 0.2f;
        private const float UpperZoomLimit = 1.8f;

        //Variable Decleration.
        private Matrix view;
        private float rotation;
        private float scale;

        //Public properties.
        /// <summary>
        /// Get or Set the worldsize.
        /// Default = game's viewport width and height.
        /// </summary>
        public Vector2 WorldSize { get; set; }

        /// <summary>
        /// Creates a default 2D camera. All variables will contain default values.
        /// </summary>
        /// <param name="game">The current game</param>
        public Camera2D(Game1 game)
            : base(game, Shape.None, new Vector2(), new Vector2())
        {
            //Simple default values.
            rotation = 0;
            scale = 0.5f * (LowerZoomLimit + UpperZoomLimit);
            view = new Matrix();

            //Default values determined from the current game.
            Viewport viewport = game.GraphicsDevice.Viewport;

            WorldSize = new Vector2(viewport.Width, viewport.Height);
            base.HalfSize = Vector2.Multiply(WorldSize, 0.5f);
            base.Position = new Vector2(base.HalfSize.X, base.HalfSize.Y);
        }

        public Matrix GetView()
        {
            return view;
        }

        /// <summary>
        /// Sets the view matrix with the current rotation, scale and position.
        /// </summary>
        private void SetView()
        {
            //First put the camera back at (0, 0). Then in the screen's center.
            Vector3 invPos = new Vector3(Vector2.Multiply(Position, -1), 0);
            view = Matrix.CreateTranslation(invPos) *
                Matrix.CreateScale(scale) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(HalfSize, 0));
        }

        public override void Update(GameTime gameTime)
        {
            SetView();
        }

        /// <summary>
        /// Get the camera's current halfsize. The camera's halfsize depends on
        /// the zoomlevel.
        /// Default = screensize / (scale * 0.5).
        /// </summary>
        public override Vector2 HalfSize
        {
            get
            {
                return base.HalfSize / scale;
            }
        }

        /// <summary>
        /// Get or Set the camera's position. The camera can't be positioned out of the world.
        /// Default = screensize / 2.
        /// </summary>
        public override Vector2 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                float tempX = value.X;
                float tempY = value.Y;

                //Divide the halfsize by the scale in case we zoomed in or out.
                Vector2 tempHS = HalfSize;

                //Keep the X value in the world.
                if (tempX - tempHS.X < 0)
                {
                    tempX = tempHS.X;
                }
                else if (tempX + tempHS.X > WorldSize.X)
                {
                    tempX = WorldSize.X - tempHS.X;
                }

                //Keep the Y value in the world.
                if (tempY - tempHS.Y < 0)
                {
                    tempY = tempHS.Y;
                }
                else if (tempY + tempHS.Y > WorldSize.Y)
                {
                    tempY = WorldSize.Y - tempHS.Y;
                }

                base.Position = new Vector2(tempX, tempY);
            }
        }

        /// <summary>
        /// Get the current rotation. The rotation is in radians.
        /// Default = 0.
        /// </summary>
        public float Rotation
        {
            get
            {
                return rotation;
            }
            private set
            {
                rotation = value;
            }
        }

        /// <summary>
        /// Get or Set the current screensize.
        /// Default = game's viewport width and height.
        /// </summary>
        public Vector2 ScreenSize
        {
            get
            {
                return base.HalfSize * 2;
            }
            set
            {
                base.HalfSize = value / 2;
            }
        }

        /// <summary>
        /// Get or Set the zoom level. 
        /// Default = (LowerZoomLimit + UpperZoomLimit) / 2.
        /// </summary>
        public float Zoom
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                if (scale < LowerZoomLimit) 
                {
                    scale = LowerZoomLimit;
                }
                else if (scale > UpperZoomLimit)
                {
                    scale = UpperZoomLimit;
                }
            }
        }
    }
}

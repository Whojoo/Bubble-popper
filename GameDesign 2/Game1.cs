#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using GameDesign_2.Screens;
using GameDesign_2.Screens.MenuScreens;
using GameDesign_2.FPS;
#endregion

namespace GameDesign_2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public Camera2D Camera { get; private set; }

        private ScreenManager manager;

        private FrameRate frameRate = new FrameRate();
        private FrameRateDrawer frameRateDrawer = new FrameRateDrawer();
        private FrameRateUpdater frameRateUpdater = new FrameRateUpdater();
        private DrawingContext drawingContext = new DrawingContext();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsFixedTimeStep = false;
            IsMouseVisible = false;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
            Viewport vp = new Viewport(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y,
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            GraphicsDevice.Viewport = vp;

            Spawner.GetInstance().Game = this;

            Components.Add(Camera = new Camera2D(this));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            frameRateDrawer.LoadContent(Content);
            drawingContext.SpriteBatch = spriteBatch;
            drawingContext.GraphicsDevice = spriteBatch.GraphicsDevice;

            manager = new ScreenManager(this, new MainMenuScreen(this));
            Components.Add(manager);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            frameRateUpdater.Update(gameTime, frameRate);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            spriteBatch.Begin();
            frameRateDrawer.Draw(drawingContext, frameRate);
            spriteBatch.End();
        }

        public Screen GetActiveScreen()
        {
            return manager.ActiveScreen;
        }
    }
}

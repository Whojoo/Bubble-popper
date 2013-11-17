using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.Components.Player;
using GameDesign_2.Screens.MenuScreens;
using GameDesign_2.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDesign_2.Screens
{
    public class GameplayScreen : Screen
    {
        public PlayerBall Player { get; protected set; }
        private bool debugMode = false;
        private Texture2D quadTreeTex;

        private QuadTree quadTree;
        private Vector2 worldSize;
        private IState currentState;

        public GameplayScreen(Game1 game, Vector2 worldSize, int goalScore)
            : base(game)
        {
            this.worldSize = worldSize;
            //Make the tree a bit wider for outer walls.
            quadTree = new QuadTree(new Rectangle(
                -10, -10, (int)worldSize.X + 20, (int)worldSize.Y + 20));

            //Add the 4 walls on the outside of the world.
            Components.Add(new Wall(GDGame, new Vector2(-10, -10), new Vector2(worldSize.X + 10, 0)));
            Components.Add(new Wall(GDGame, new Vector2(worldSize.X, -10), new Vector2(worldSize.X + 10, worldSize.Y)));
            Components.Add(new Wall(GDGame, new Vector2(0, worldSize.Y), new Vector2(worldSize.X + 10, worldSize.Y + 10)));
            Components.Add(new Wall(GDGame, new Vector2(-10, 0), new Vector2(0, worldSize.Y + 10)));

            //Add the player to world.
            Components.Add(Player = new PlayerBall(GDGame, new Vector2(300, 300), goalScore));

            //Give the camera the new world size.
            GDGame.Camera.WorldSize = worldSize + new Vector2(0, 100);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            quadTreeTex = Content.Load<Texture2D>("square");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (currentState != null)
            {
                currentState.Update(gameTime);
            }

            //Update the sinusoid graphs.
            Sinusoid.GetInstance().Update(gameTime);

            //Update the Spawner.
            Spawner.GetInstance().Update();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Manager.Pop();
                Manager.Push(new MainMenuScreen(GDGame));
                return;
            }

            CheckCameraChanges(gameTime);

            base.Update(gameTime);

            //Update collision after the initial updates.
            //Clear and refill the quadtree.
            quadTree.Clear();
            quadTree.Insert(Components);

            ////Now loop through all objects.
            List<GDComp> possibleColliders = new List<GDComp>();
            for (int i = 0; i < Components.Count; i++)
            {
                //Cast the current component;
                GDComp comp = Components[i] as GDComp;

                //Clear the old list and refill it.
                possibleColliders.Clear();
                quadTree.GetPossibleColliders(possibleColliders,
                    comp.GetRect());

                //Now loop through the possibleColliders for collisions.
                for (int j = 0; j < possibleColliders.Count; j++)
                {
                    if (!comp.Equals(possibleColliders[j]))
                    {
                        comp.CheckCollisionWith(gameTime, possibleColliders[j]);
                    }
                }
            }
            
            for (int i = Components.Count - 1; i >= 0; i--)
            {
                if ((Components[i] as GDComp).Remove)
                {
                    Components.RemoveAt(i);
                }
            }
        }

        private void CheckCameraChanges(GameTime gameTime)
        {
            //First camera position check.
            KeyboardState currentKey = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 move = new Vector2();
            const float speed = 500;

            //Left or Right.
            if (currentKey.IsKeyDown(Keys.Left))
            {
                move.X += -speed * dt;
            }
            else if (currentKey.IsKeyDown(Keys.Right))
            {
                move.X += speed * dt;
            }

            //Up or Down.
            if (currentKey.IsKeyDown(Keys.Up))
            {
                move.Y += -speed * dt;
            }
            else if (currentKey.IsKeyDown(Keys.Down))
            {
                move.Y += speed * dt;
            }

            GDGame.Camera.Position += move;
        }

        public override void Draw(GameTime gameTime)
        {
            if (debugMode)
            {
                Matrix transform = GDGame.Camera.GetView();
                Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, transform);
                quadTree.Draw(Batch, Color.Red, quadTreeTex);
                Batch.End(); 
            }

            base.Draw(gameTime);
        }

        public override void Unload()
        {
            //Reset the sinusoid instance for other screens.
            Sinusoid.GetInstance().Reset();

            //Reset the Spawner.
            Spawner.GetInstance().Reset();

            base.Unload();
        }

        public void GameOver()
        {
        }

        public void Won()
        {
        }
    }
}

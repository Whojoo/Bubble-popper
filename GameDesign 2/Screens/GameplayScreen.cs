using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.Components.Player;
using GameDesign_2.Screens.MenuScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDesign_2.Screens
{
    public class GameplayScreen : Screen
    {
        public PlayerBall Player { get; protected set; }
        private bool debugMode = true;
        private Texture2D quadTreeTex;

        private QuadTree quadTree;
        private Vector2 worldSize;
        private MouseState lastMouse;

        public GameplayScreen(Game1 game, Vector2 worldSize, int goalScore)
            : base(game)
        {
            this.worldSize = worldSize;
            quadTree = new QuadTree(new Rectangle(
                0, 0, (int)worldSize.X, (int)worldSize.Y));

            Components.Add(Player = new PlayerBall(GDGame, new Vector2(300, 300), goalScore));
        }

        public override void Initialize()
        {
            Components.Add(new Wall(GDGame, new Vector2(0, 0), new Vector2(1280, 50)));
            Components.Add(new Wall(GDGame, new Vector2(1230, 50), new Vector2(1280, 720)));
            Components.Add(new Wall(GDGame, new Vector2(0, 670), new Vector2(1230, 720)));
            Components.Add(new Wall(GDGame, new Vector2(0, 50), new Vector2(50, 670)));

            //Add sinusoid graphs.
            Sinusoid sin = Sinusoid.GetInstance();

            Random randy = new Random(200);
            const int sinusoids = 20;
            for (int i = 0; i < sinusoids; i++)
            {
                sin.AddGraph((float)randy.NextDouble() + 2.0f, i, (float)randy.NextDouble() + 0.5f);
            }

            lastMouse = Mouse.GetState();

            for (int i = 0; i < 50; i++)
            {
                Vector2 position = new Vector2(randy.Next(100, 900), randy.Next(100, 600));
                ScoreBall ball;
                Components.Add(ball = new ScoreBall(GDGame, position, randy.Next(0, sinusoids)));

                if (randy.Next(2, 5) > 3)
                {
                    ball.ReverseXMovement();
                    ball.ReverseYMovement();
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            quadTreeTex = Content.Load<Texture2D>("square");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Update the sinusoid graphs.
            Sinusoid.GetInstance().Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Manager.Pop();
                Manager.Push(new MainMenuScreen(GDGame));
            }

            //if (Mouse.GetState().RightButton == ButtonState.Pressed &&
            //    lastMouse.RightButton == ButtonState.Released)
            //{
            //    ScoreBall temp;
            //    Components.Add(temp = new ScoreBall(GDGame, Player.Position, 0));
            //    temp.Initialize();
            //    quadTree.Insert(temp);
            //}
            //lastMouse = Mouse.GetState();


            //Update collision after the initial updates.
            //Clear and refill the quadtree.
            quadTree.Clear();
            for (int i = 0; i < Components.Count; i++)
            {
                quadTree.Insert((GDComp)Components[i]);
            }

            //Now loop through all objects.
            List<GDComp> possibleColliders = new List<GDComp>();
            for (int i = 0; i < Components.Count; i++)
            {
                //Cast the current component;
                GDComp comp = Components[i] as GDComp;

                //Clear the old list and refill it.
                possibleColliders.Clear();
                quadTree.GetPossibleColliders(possibleColliders,
                    comp.GetRect());
                int duplicate = possibleColliders.IndexOf(comp);

                //Now loop through the possibleColliders for collisions.
                for (int j = 0; j < possibleColliders.Count; j++)
                {
                    if (j == duplicate)
                    {
                        continue;
                    }
                    if (comp is PlayerBall ||
                        possibleColliders[j] is PlayerBall)
                    {
                        Console.WriteLine(possibleColliders.Count);
                    }
                    
                    comp.CheckCollisionWith(gameTime, possibleColliders[j]);
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

            base.Unload();
        }

        internal void GameOver()
        {
        }

        internal void Won()
        {
        }
    }
}

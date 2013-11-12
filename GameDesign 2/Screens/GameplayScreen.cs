using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.Components.Player;
using GameDesign_2.Screens.MenuScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameDesign_2.Screens
{
    public class GameplayScreen : Screen
    {
        public PlayerBall Player { get; protected set; }

        private QuadTree quadTree;
        private Vector2 worldSize;

        public GameplayScreen(Game1 game, Vector2 worldSize)
            : base(game)
        {
            this.worldSize = worldSize;
            quadTree = new QuadTree(new Rectangle(
                0, 0, (int)worldSize.X, (int)worldSize.Y));
        }

        public override void Initialize()
        {
            Components.Add(Player = new PlayerBall(GDGame, new Vector2(300, 300)));
            Components.Add(new Wall(GDGame, new Vector2(0, 0), new Vector2(1280, 50)));
            Components.Add(new Wall(GDGame, new Vector2(1230, 50), new Vector2(1280, 720)));
            Components.Add(new Wall(GDGame, new Vector2(0, 670), new Vector2(1230, 720)));
            Components.Add(new Wall(GDGame, new Vector2(0, 50), new Vector2(50, 670)));

            //Add sinusoid graphs.
            Sinusoid sin = Sinusoid.GetInstance();
            const float amplitude = 1;
            const float period = 2.5f;
            int xOffset = 0;

            sin.AddGraph(period, xOffset++, amplitude);
            //xOffset *= 2;
            sin.AddGraph(period, xOffset++, amplitude);
            //xOffset *= 2;
            sin.AddGraph(period, xOffset++, amplitude);
            //xOffset *= 2;
            sin.AddGraph(period, xOffset++, amplitude);
            //xOffset *= 2;
            sin.AddGraph(period, xOffset++, amplitude);
            sin.AddGraph(period, xOffset++, amplitude);
            sin.AddGraph(period, xOffset++, amplitude);
            sin.AddGraph(period, xOffset++, amplitude);
            sin.AddGraph(period, xOffset++, amplitude);
            sin.AddGraph(period, xOffset++, amplitude);
            sin.AddGraph(period, xOffset++, amplitude);

            Random randy = new Random(200);

            for (int i = 0; i < 2000; i++)
            {
                Vector2 position = new Vector2(randy.Next(100, 900), randy.Next(100, 600));
                ScoreBall ball;
                Components.Add(ball = new ScoreBall(GDGame, position, randy.Next(0, 10)));

                if (randy.Next(2, 5) > 3)
                {
                    ball.ReverseXMovement();
                    ball.ReverseYMovement();
                }
            }

            ScoreBar bar;
            HuDComponents.Add(bar = new ScoreBar(GDGame, 5000000));
            bar.AddScore(5000000);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Update the sinusoid graphs.
            Sinusoid.GetInstance().Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Manager.Pop();
                Manager.Push(new MainMenuScreen(GDGame));
            }

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

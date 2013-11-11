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

        public GameplayScreen(Game1 game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            Components.Add(Player = new PlayerBall(GDGame, new Vector2(300, 300)));
            Components.Add(new Wall(GDGame, new Vector2(500, 25), new Vector2(100, 50)));

            //Add sinusoid graphs.
            Sinusoid sin = Sinusoid.GetInstance();
            const float amplitude = 1;
            const float period = 2.5f;
            int xOffset = 0;

            sin.AddGraph(period, xOffset++, amplitude);
            xOffset *= 2;
            sin.AddGraph(period, xOffset++, amplitude);
            xOffset *= 2;
            sin.AddGraph(period, xOffset++, amplitude);
            xOffset *= 2;
            sin.AddGraph(period, xOffset++, amplitude);
            xOffset *= 2;
            sin.AddGraph(period, xOffset++, amplitude);

            for (int i = 0; i < 500; i++)
            {
                Components.Add(new ScoreBall(GDGame, new Vector2(0, 100), sin.GetRandomIndex()));
            }
            

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
        }

        public override void Unload()
        {
            //Reset the sinusoid instance for other screens.
            Sinusoid.GetInstance().Reset();

            base.Unload();
        }

        internal void GameOver()
        {
            throw new NotImplementedException();
        }

        internal void Won()
        {
            throw new NotImplementedException();
        }
    }
}

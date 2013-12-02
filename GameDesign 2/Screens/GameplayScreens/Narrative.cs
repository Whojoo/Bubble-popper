using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.States.GameStates;
using GameDesign_2.States.StateMachines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameDesign_2.Screens.GameplayScreens
{
    public class Narrative : GameplayScreen
    {
        private KeyboardState last;

        public Narrative(Game1 game)
            : base(game, new Vector2(1500, 1500))
        {
        }

        public override void Initialize()
        {
            const float BlockWidthOrHeight = 50;

            //Very very simple level design. Mostly for testing a lot of space.
            //Horzontal bars.
            float midY = WorldSize.Y * 0.5f - BlockWidthOrHeight * 0.5f;
            float width = WorldSize.X * 0.15f;
            Vector2 topLeft = new Vector2(0, midY);
            Components.Add(new Wall(GDGame, topLeft, topLeft + new Vector2(width, BlockWidthOrHeight)));
            topLeft = new Vector2(WorldSize.X * 0.25f, midY);
            Components.Add(new Wall(GDGame, topLeft, topLeft + new Vector2(width, BlockWidthOrHeight)));
            topLeft = new Vector2(WorldSize.X * 0.6f, midY);
            Components.Add(new Wall(GDGame, topLeft, topLeft + new Vector2(width, BlockWidthOrHeight)));
            topLeft = new Vector2(WorldSize.X * 0.85f, midY);
            Components.Add(new Wall(GDGame, topLeft, topLeft + new Vector2(width, BlockWidthOrHeight)));

            //Vertical bars.
            float midX = WorldSize.X * 0.5f - BlockWidthOrHeight * 0.5f;
            float height = width;
            topLeft = new Vector2(midX, 0);
            Components.Add(new Wall(GDGame, topLeft, topLeft + new Vector2(BlockWidthOrHeight, height)));
            topLeft = new Vector2(midX, WorldSize.Y * 0.25f);
            Components.Add(new Wall(GDGame, topLeft, topLeft + new Vector2(BlockWidthOrHeight, height)));
            topLeft = new Vector2(midX, WorldSize.Y * 0.6f);
            Components.Add(new Wall(GDGame, topLeft, topLeft + new Vector2(BlockWidthOrHeight, height)));
            topLeft = new Vector2(midX, WorldSize.Y * 0.85f);
            Components.Add(new Wall(GDGame, topLeft, topLeft + new Vector2(BlockWidthOrHeight, height)));

            //Portals.
            Vector2 hs = new Vector2(30, 50);
            SpawnPortal portal;
            Spawner spawner = Spawner.GetInstance();

            //Top left.
            Components.Add(portal = new SpawnPortal(GDGame,
                new Vector2(WorldSize.X * 0.075f, WorldSize.Y * 0.075f), hs));
            spawner.AddPortal(portal);
            Components.Add(portal = new SpawnPortal(GDGame,
                new Vector2(WorldSize.X * 0.075f, WorldSize.Y * 0.325f), hs));
            spawner.AddPortal(portal);

            //Bottom left.
            Components.Add(portal = new SpawnPortal(GDGame,
                new Vector2(WorldSize.X * 0.075f, WorldSize.Y * 0.675f), hs));
            spawner.AddPortal(portal);
            Components.Add(portal = new SpawnPortal(GDGame,
                new Vector2(WorldSize.X * 0.075f, WorldSize.Y * 0.925f), hs));
            spawner.AddPortal(portal);

            //Top right.
            Components.Add(portal = new SpawnPortal(GDGame,
                new Vector2(WorldSize.X * 0.925f, WorldSize.Y * 0.075f), hs));
            spawner.AddPortal(portal);
            Components.Add(portal = new SpawnPortal(GDGame,
                new Vector2(WorldSize.X * 0.925f, WorldSize.Y * 0.325f), hs));
            spawner.AddPortal(portal);

            //Bottom right.
            Components.Add(portal = new SpawnPortal(GDGame,
                new Vector2(WorldSize.X * 0.925f, WorldSize.Y * 0.675f), hs));
            spawner.AddPortal(portal);
            Components.Add(portal = new SpawnPortal(GDGame,
                new Vector2(WorldSize.X * 0.925f, WorldSize.Y * 0.925f), hs));
            spawner.AddPortal(portal);

            //Position the player in the center.
            Player.Position = WorldSize * 0.5f;
            GDGame.Camera.Position = Player.Position;

            //Now add some numbers to the sinusoid instance.
            Sinusoid sin = Sinusoid.GetInstance();
            Random randy = new Random(200);
            const int sinusoids = 20;
            for (int i = 0; i < sinusoids; i++)
            {
                sin.AddGraph((float)randy.NextDouble() + 2.0f, i, (float)randy.NextDouble() + 0.5f);
            }

            StateMachine = new RegularStateMachine(this);
            StateMachine.PushState(new RegularState(StateMachine, false, 25, 50, 75));

            spawner.MaximumAlive = 200;

            last = Keyboard.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState current = Keyboard.GetState();

            if (current.IsKeyDown(Keys.Space) && last.IsKeyUp(Keys.Space))
            {
                Won();
            }

            last = current;

            base.Update(gameTime);
        }

        public override void Won()
        {
            if (StateMachine is RegularStateMachine)
            {
                StateMachine = new BossStateMachine(this);
            }
            else
            {
                base.Won();
            }
        }
    }
}

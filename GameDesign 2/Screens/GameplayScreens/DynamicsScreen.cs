using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.Components.Player;
using GameDesign_2.States.GameStates;
using GameDesign_2.States.StateMachines;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Screens.GameplayScreens
{
    public class DynamicsScreen : GameplayScreen
    {
        public DynamicsScreen(Game1 game)
            : base(game, new Vector2(1840, 1500))
        {
        }

        public override void Initialize()
        {
            /**
             * Walls are placed arcording to a drawing. Player can grow and shrink.
             * Wall placements are made according this growing and shrinking.
             * The player's size can vary from 60 to 100. Smaller areas width is 90. 
             * Larger areas have 150 width or more.
             * Block's width or height has to be 50.
             * Placement is from top to bottom, left to right.
             */
            const int BlockWidthOrHeight = 50;

            //Row 1.
            Vector2 topLeft = new Vector2(780, 90);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(530, BlockWidthOrHeight))));

            //Row 2.
            topLeft = new Vector2(230, 300);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(540, BlockWidthOrHeight))));

            topLeft = new Vector2(1140, 230);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(400, BlockWidthOrHeight))));
            //Bar attached to the last one.
            topLeft = topLeft + new Vector2(350, BlockWidthOrHeight);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(BlockWidthOrHeight, 150))));

            //Row 3.
            topLeft = new Vector2(600, 430);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(540, BlockWidthOrHeight))));

            //Row 4.
            topLeft = new Vector2(0, 630);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(200, BlockWidthOrHeight))));

            topLeft = new Vector2(350, 630);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(340, BlockWidthOrHeight))));

            //Player spawn cage.
            //Cage will be 270 x 270. Starts 90 lower than last row.
            //Top 2 horizontal bars. 90 x 50
            topLeft = new Vector2(780, 570);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(90, BlockWidthOrHeight))));
            topLeft = new Vector2(960, 570);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(90, BlockWidthOrHeight))));

            //Vertical bars. 50 x 170.
            topLeft = new Vector2(780, 620);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(BlockWidthOrHeight, 100))));
            topLeft = new Vector2(1000, 620);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(BlockWidthOrHeight, 100))));

            //Lower 2 horizontal bars. 90 x 50
            topLeft = new Vector2(780, 720);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(90, BlockWidthOrHeight))));
            topLeft = new Vector2(960, 720);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(90, BlockWidthOrHeight))));

            //Spawn the player in the center.
            Player.Position = new Vector2(915, 670);
            GDGame.Camera.Position = Player.Position;

            //Row 4 continue.
            topLeft = new Vector2(1140, 630);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(200, BlockWidthOrHeight))));

            topLeft = new Vector2(1490, 630);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(350, BlockWidthOrHeight))));

            //Row 5.
            topLeft = new Vector2(90, 905);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(BlockWidthOrHeight, 175))));
            //The bar attached to last one.
            topLeft = new Vector2(90, 1080);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(500, BlockWidthOrHeight))));

            topLeft = new Vector2(490, 880);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(550, BlockWidthOrHeight))));

            //Row 6.
            topLeft = new Vector2(870, 1040);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(BlockWidthOrHeight, 240))));
            //The bar attache to last one.
            topLeft = new Vector2(490, 1280);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(430, BlockWidthOrHeight))));

            //Row 7.
            topLeft = new Vector2(1010, 1280);
            Components.Add(new Wall(GDGame, topLeft, topLeft + (new Vector2(550, BlockWidthOrHeight))));

            //Now add the portals.
            Spawner spawner = Spawner.GetInstance();
            SpawnPortal portal;
            Vector2 hs = new Vector2(30, 50);

            //Portal row 1.
            Components.Add(portal = new SpawnPortal(GDGame, new Vector2(915, 350), hs));
            spawner.AddPortal(portal);
            Components.Add(portal = new SpawnPortal(GDGame, new Vector2(1515, 160), hs));
            spawner.AddPortal(portal);

            //Portal row 2.
            Components.Add(portal = new SpawnPortal(GDGame, new Vector2(80, 550), hs));
            spawner.AddPortal(portal);
            Components.Add(portal = new SpawnPortal(GDGame, new Vector2(1750, 550), hs));
            spawner.AddPortal(portal);

            //Portal row 3.
            Components.Add(portal = new SpawnPortal(GDGame, new Vector2(80, 800), hs));
            spawner.AddPortal(portal);
            Components.Add(portal = new SpawnPortal(GDGame, new Vector2(1600, 800), hs));
            spawner.AddPortal(portal);

            //Portal row 4.
            Components.Add(portal = new SpawnPortal(GDGame, new Vector2(80, 1250), hs));
            spawner.AddPortal(portal);
            Components.Add(portal = new SpawnPortal(GDGame, new Vector2(1400, 1150), hs));
            spawner.AddPortal(portal);

            //Portal row 5.
            Components.Add(portal = new SpawnPortal(GDGame, new Vector2(1050, 1420), hs));
            spawner.AddPortal(portal);

            //Change spawner default values.
            spawner.MaximumAlive = 200;
            spawner.FriendliesPerEnemies = 5;

            //Now add some numbers to the sinusoid instance.
            Sinusoid sin = Sinusoid.GetInstance();
            Random randy = new Random(200);
            const int sinusoids = 20;
            for (int i = 0; i < sinusoids; i++)
            {
                sin.AddGraph((float)randy.NextDouble() + 2.0f, i, (float)randy.NextDouble() + 0.5f);
            }

            int[] borders = {25, 75};

            StateMachine = new RegularStateMachine(this);
            StateMachine.PushState(new RegularState(StateMachine, borders, true));

            base.Initialize();
        }
    }
}

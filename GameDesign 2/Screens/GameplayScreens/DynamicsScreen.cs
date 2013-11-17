using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.Components.Player;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Screens.GameplayScreens
{
    public class DynamicsScreen : GameplayScreen
    {
        public DynamicsScreen(Game1 game)
            : base(game, new Vector2(1840, 1500), 100000)
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

            base.Initialize();
        }
    }
}

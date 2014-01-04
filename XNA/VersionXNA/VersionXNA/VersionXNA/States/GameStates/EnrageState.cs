using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.Components.Player;
using GameDesign_2.Screens;
using Microsoft.Xna.Framework;

namespace GameDesign_2.States.GameStates
{
    /// <summary>
    /// An EnrageState which causes all ScoreBalls to be enemy and to chase the player.
    /// </summary>
    public class EnrageState : GameState
    {
        private const float EnrageTime = 10;

        private float enrageTimer;
        private int oldBalance;

        public EnrageState(StateMachine parent)
            : base(parent)
        {
            enrageTimer = 0;
        }

        public override void Activate()
        {
            Parent.Screen.GDGame.Background = Color.DarkRed;
            oldBalance = Spawner.GetInstance().FriendliesPerEnemies;
            Spawner.GetInstance().FriendliesPerEnemies = 0;

            //Force the multiplier to 2.
            Parent.Screen.Player.ScoreBar.ForceMultiplier(2, EnrageTime);

            base.Activate();
        }

        public override void Update(GameTime gameTime)
        {
            GameplayScreen screen = (GameplayScreen)Parent.Screen;
            PlayerBall player = screen.Player;

            foreach (ScoreBall comp in screen.Components.OfType<ScoreBall>())
            {
                comp.Target = player;
            }

            enrageTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (enrageTimer >= EnrageTime)
            {
                Remove();
            }

            base.Update(gameTime);
        }

        private void Remove()
        {
            GameplayScreen screen = (GameplayScreen)Parent.Screen;

            foreach (ScoreBall comp in screen.Components.OfType<ScoreBall>())
            {
                comp.Target = null;
            }

            Parent.Proceed(Parent.PopState());
        }
    }
}

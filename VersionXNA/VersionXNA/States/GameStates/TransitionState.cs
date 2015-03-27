using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.States.GameStates
{
    public class TransitionState : GameState
    {
        private const float TransitionTime = 2f;

        private float timer;
        private float scorePerSecond;

        public TransitionState(StateMachine parent)
            : base(parent)
        {
            timer = 0;
            scorePerSecond = (parent.Screen.Player.ScoreBar.Score - 5) / TransitionTime;
        }

        public override void Activate()
        {
            Parent.Screen.GDGame.Background = Color.Gray;
        }

        public override void Update(GameTime gameTime)
        {
            //Make sure the field is empty.
            Spawner.GetInstance().Clear();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            timer += dt;
            if (timer >= TransitionTime)
            {
                Parent.PopState();
                Parent.Proceed(this);
            }
            else
            {
                Parent.Screen.Player.ScoreBar.DropScore(scorePerSecond * dt);
            }

            base.Update(gameTime);
        }
    }
}

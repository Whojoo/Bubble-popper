using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Screens;
using Microsoft.Xna.Framework;

namespace GameDesign_2.States.StateMachines
{
    /// <summary>
    /// Simple statemachine. Only addition is that Proceed() now increases the difficulty a bit.
    /// </summary>
    public class RegularStateMachine : StateMachine
    {
        public RegularStateMachine(GameplayScreen screen)
            : base(screen)
        {
        }

        public override void Proceed(GameState caller)
        {
            Spawner spawner = Spawner.GetInstance();

            if (spawner.FriendliesPerEnemies > 2)
            {
                spawner.FriendliesPerEnemies -= 2;
                spawner.MaximumAlive += 50;
            }
        }
    }
}

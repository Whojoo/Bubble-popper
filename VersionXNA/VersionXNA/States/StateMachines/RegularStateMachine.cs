using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Screens;
using GameDesign_2.States.GameStates;
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
            //Clear the score stacks.
            Screen.Player.ScoreBar.ClearStacks();

            if (caller is AgroState)
            {
                //AgroState does all the logic work. No need to add much logic here.
                Spawner spawner = Spawner.GetInstance();

                if (spawner.FriendliesPerEnemies > 2)
                {
                    spawner.FriendliesPerEnemies -= 2;
                    spawner.MaximumAlive += 50;
                }
            }
            else if (caller is RegularState)
            {
                RegularState temp = (RegularState)caller;
                PushState(new AgroState(this, temp.AgroBorder));
            }
        }
    }
}

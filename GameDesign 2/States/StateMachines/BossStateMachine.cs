using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components.Player;
using GameDesign_2.Screens;
using GameDesign_2.States.GameStates;
using Microsoft.Xna.Framework;

namespace GameDesign_2.States.StateMachines
{
    /// <summary>
    /// A simple BossStateMachine made to give the user a tougher. Switches to the next
    /// state only happen if the player finished the current state.
    /// Scores by friendlies are set to 0.5f.
    /// 
    /// - It starts with a TransitionState, setting score back to 5%.
    /// - RegularState.         End: 15%            Balance: 3 : 1
    /// - Tough RegularState.   End: 30%            Balance: 5 : 1
    /// - AgroState.            End: 40%            Balance: 1 : 1
    /// - RegularState.         End: 55%            Balance: 4 : 1
    /// - EnrageState.          End: 10 seconds     Balance: 0 : 1
    /// - Tough RegularState.   End: 65%            Balance: 3 : 1
    /// - AgroState.            End: 75%            Balance: 1 : 1
    /// - EnrageState.          End: 10 seconds     Balance: 0 : 1
    /// - Tough RegularState.   End: 90%            Balance: 2 : 1
    /// - AgroState.            End: 100%           Balance: 1 : 1
    /// </summary>
    public class BossStateMachine : StateMachine
    {
        //This number represents a state above.
        private int stateNumber;

        public BossStateMachine(GameplayScreen screen)
            : base(screen)
        {
            stateNumber = 0;
            PushState(new TransitionState(this));
        }

        public override void Proceed(GameState caller)
        {
            //Clear the score stacks.
            Screen.Player.ScoreBar.ClearStacks();

            //Change the score values.
            PlayerBall player = Screen.Player;
            const float diffValue = 0.1f;
            player.ScoreAddedByFriendly -= diffValue;
            player.ScoreDroppedByEnemy += diffValue;

            switch (stateNumber)
            {
                case 0: //To RegularState till 15%.
                    stateNumber++;
                    PushState(new RegularState(this, false, 15, 55));
                    Spawner.GetInstance().FriendliesPerEnemies = 3;
                    break;
                case 1: //To tough RegularState till 30%.
                    stateNumber++;
                    PushState(new RegularState(this, true, 30));
                    Spawner.GetInstance().FriendliesPerEnemies = 5;
                    break;
                case 2: //To AgroState till 40%. Repeat if below 20%.
                    stateNumber++;
                    PushState(new AgroState(this, 30));
                    break;
                case 3: //RegularState again till 55%.
                    //Release the AgroState and the tough RegularState.
                    PopState();
                    stateNumber++;
                    Spawner.GetInstance().FriendliesPerEnemies = 4;
                    break;
                case 4: //EnrageState.
                    stateNumber++;
                    PopState();
                    PushState(new EnrageState(this));
                    break;
                case 5: //Tough RegularState till 65%.
                    stateNumber++;
                    PushState(new RegularState(this, true, 65, 90));
                    Spawner.GetInstance().FriendliesPerEnemies = 3;
                    break;
                case 6: //AgroState till 75%. Repeat if below 55%.
                    stateNumber++;
                    PushState(new AgroState(this, 65));
                    break;
                case 7: //EnrageState.
                    stateNumber++;
                    PushState(new EnrageState(this));
                    break;
                case 8: //Tough RegularState till 90%.
                    stateNumber++;
                    Spawner.GetInstance().FriendliesPerEnemies = 2;
                    break;
                case 9: //AgroState till 100%. Repeat if below 80%.
                    stateNumber++;
                    PushState(new AgroState(this, 90));
                    break;
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}

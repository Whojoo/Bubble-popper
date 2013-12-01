using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Screens;

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
    /// - Tough RegularState.   End: 60%            Balance: 3 : 1
    /// - AgroState.            End: 75%            Balance: 1 : 1
    /// - EnrageState.          End: 10 seconds     Balance: 0 : 1
    /// - Tough RegularState.   End: 90%            Balance: 2 : 1
    /// - AgroState.            End: 100%           Balance: 1 : 1
    /// </summary>
    public class BossStateMachine : StateMachine
    {
        public BossStateMachine(GameplayScreen screen)
            : base(screen)
        {

        }

        public override void Proceed(GameState caller)
        {
            
        }
    }
}

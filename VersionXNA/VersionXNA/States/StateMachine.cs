using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Screens;
using Microsoft.Xna.Framework;

namespace GameDesign_2.States
{
    /// <summary>
    /// Abstract class for dealing with states. This StateMachine works like a stack.
    /// </summary>
    public abstract class StateMachine
    {
        public readonly GameplayScreen Screen;

        private List<GameState> states;
        private int top;

        private int currentState;
        private int totalStates;

        public StateMachine(GameplayScreen screen, int totalStates)
        {
            Screen = screen;
            states = new List<GameState>(5);

            this.totalStates = totalStates;
            currentState = 1;

            //Start at -1 because the first state will be index 0;
            top = -1;
        }

        /// <summary>
        /// Get the top state on the stack.
        /// </summary>
        public GameState Peek()
        {
            return states[top];
        }

        public bool IsEmpty()
        {
            return states.Count == 0;
        }

        /// <summary>
        /// Pushes a new state on the stack.
        /// </summary>
        /// <param name="state">The new state.</param>
        public void PushState(GameState state)
        {
            if (!IsEmpty())
            {
                Peek().Deactivate();
            }
            states.Add(state);
            top++;
            state.Activate();
        }

        /// <summary>
        /// Pops the top state from the stack and returns it.
        /// </summary>
        /// <returns>The popped state.</returns>
        public GameState PopState()
        {
            //Do we have anything on the stack?
            if (top < 0)
            {
                return null;
            }

            GameState toReturn = states[top];
            states.RemoveAt(top--);
            if (top >= 0)
            {
                states[top].Activate();
            }

            return toReturn;
        }

        /// <summary>
        /// Use this function to announce a succesfull pass in the last state.
        /// It can help in making the game more difficult over time.
        /// </summary>
        /// <param name="caller">The caller. Can be used as identifier for different
        /// state difficulties.</param>
        public virtual void Proceed(GameState caller)
        {
            currentState++;
        }

        public int GetCurrentState()
        {
            return currentState;
        }

        public int GetTotalStates()
        {
            return totalStates;
        }

        public virtual void Update(GameTime gameTime)
        {
            states[top].Update(gameTime);
        }
    }
}

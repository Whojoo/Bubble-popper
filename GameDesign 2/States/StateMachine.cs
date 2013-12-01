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

        public StateMachine(GameplayScreen screen)
        {
            Screen = screen;
            states = new List<GameState>(5);

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

        /// <summary>
        /// Pushes a new state on the stack.
        /// </summary>
        /// <param name="state">The new state.</param>
        public void PushState(GameState state)
        {
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

            states[top].Activate();

            return toReturn;
        }

        /// <summary>
        /// Use this function to announce a succesfull pass in the last state.
        /// It can help in making the game more difficult over time.
        /// </summary>
        /// <param name="caller">The caller. Can be used as identifier for different
        /// state difficulties.</param>
        public abstract void Proceed(GameState caller);

        public virtual void Update(GameTime gameTime)
        {
            states[top].Update(gameTime);
        }
    }
}

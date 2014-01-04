using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameDesign_2.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDesign_2.Screens
{
    public class MenuScreen : Screen
    {
        protected SpriteFont Font;
        protected List<MenuEntry> Entries { get; private set; }
        protected KeyboardState LastState { get; set; }
        protected int CurrentIndex { get; set; }

        private TextObject titleText;
        private float counter;

        public MenuScreen(Game1 game, string title)
            : base(game)
        {
            Font = Content.Load<SpriteFont>("MyFont");

            Vector2 origin = Font.MeasureString(title) * 0.5f;
            Vector2 position = game.Camera.ScreenSize * 0.5f;
            position.Y = origin.Y + position.Y * 0.15f;

            Entries = new List<MenuEntry>();
            
            //Add the title text.
            titleText = new TextObject(title)
            {
                Color = Color.White,
                Effect = SpriteEffects.None,
                Position = position,
                Scale = 1.5f
            };

            LastState = Keyboard.GetState();
            CurrentIndex = 0;

            game.Background = Color.Black;
        }

        protected virtual void HandleInput(GameTime gameTime)
        {
            KeyboardState current = Keyboard.GetState();

            //Check for up and down presses.
            if (current.IsKeyDown(Keys.Down) && LastState.IsKeyUp(Keys.Down))
            {
                //Tell the current entry that it isn't the active entry anymore.
                Entries[CurrentIndex].SetActive(false);

                CurrentIndex++;
                if (CurrentIndex >= Entries.Count)
                {
                    CurrentIndex = 0;
                }

                //Tell the new entry that it is the active entry now.
                Entries[CurrentIndex].SetActive(true);
            }
            else if (current.IsKeyDown(Keys.Up) && LastState.IsKeyUp(Keys.Up))
            {
                //Tell the current entry that it isn't the active entry anymore.
                Entries[CurrentIndex].SetActive(false);

                CurrentIndex--;
                if (CurrentIndex < 0)
                {
                    CurrentIndex = Entries.Count - 1;
                }

                //Tell the new entry that it is the active entry now.
                Entries[CurrentIndex].SetActive(true);
            }

            //Check for spacebar and return presses.
            if ((current.IsKeyDown(Keys.Enter) && LastState.IsKeyUp(Keys.Enter)) ||
                (current.IsKeyDown(Keys.Space) && LastState.IsKeyUp(Keys.Space)))
            {
                EntryClicked(CurrentIndex);
            }
        }

        protected virtual void EntryClicked(int index)
        {
        }

        public override void Update(GameTime gameTime)
        {
            const float period = 3;
            counter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (counter >= period)
            {
                counter -= period;
            }

            //Handle the input and then update the last keyboardstate.
            HandleInput(gameTime);
            LastState = Keyboard.GetState();

            titleText.Update(gameTime);

            UpdateEntries(gameTime);

            base.Update(gameTime);
        }

        protected virtual void UpdateEntries(GameTime gameTime)
        {
            //Set the x position for each entry in the middle.
            float xPos = GDGame.GraphicsDevice.Viewport.Width * 0.5f;

            //Save the amount of entries in a variable.
            byte entryCount = (byte)Entries.Count;

            //Set the yPos on the starting value.
            float yPos = GDGame.GraphicsDevice.Viewport.Height * 0.2f;

            //Set the y 'jump' value.
            float yJump = yPos / 3;
            if (yJump * (entryCount + 2) >
                GDGame.GraphicsDevice.Viewport.Height * 0.8f)
            { //Make sure the entries fit on the screen.
                yJump = GDGame.GraphicsDevice.Viewport.Height / (entryCount + 2);
            }

            for (int i = entryCount - 1; i >= 0; i--)
            {
                //Update the position.
                Entries[i].Position = new Vector2(xPos, yPos + yJump * i);

                //For now, color the entry depending on selection.
                Entries[i].Color = i == CurrentIndex ? Color.Red : Color.White;

                Entries[i].Update(gameTime);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            Batch.Begin();
            titleText.Draw(Batch, Font);

            foreach (MenuEntry me in Entries)
            {
                me.Draw(Batch, Font);
            }
            Batch.End();
        }

        public override void Unload()
        {
            
            base.Unload();
        }
    }
}

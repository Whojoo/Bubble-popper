using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.Components.Player
{
    /// <summary>
    /// A PowerBar which loads when the player hits friendly circles.
    /// It increases with 1% * ScoreMultiplier per friendly circle hit.
    /// The power drops once you use it. You can't gain power while the power is droppping.
    /// After the drop you can gain power again. This serves as a kind of cooldown for balance.
    /// </summary>
    public class PowerBar : GDComp
    {
        //Pixel constant.
        private const float TextureSize = 100;

        //Bar constants.
        private const float MaxPower = 100;
        private const float PercentPerSecond = 50f;
        private readonly Vector2 PowerBarScale = new Vector2(0.75f, 0.98f);

        //Bar colors.
        private readonly Color Background = Color.Black;
        private readonly Color PowerReady = new Color(0, 150, 0);
        private readonly Color PowerNeutral = new Color(0, 0, 150);
        private readonly Color PowerUsed = new Color(150, 0, 0);

        public float Power { get; private set; }

        private float toAdd;
        private Texture2D tex;
        private Vector2 scale;
        private ScoreState state;

        /// <summary>
        /// Creates a PowerBar instance.
        /// </summary>
        /// <param name="game">The running game.</param>
        public PowerBar(Game1 game)
            : base(game, Shape.None, SetPosition(game), SetHalfSize(game))
        {
        }

        public override void Initialize()
        {
            toAdd = 0;
            Power = 100;

            state = ScoreState.Balance;

            float xScale = HalfSize.X * 2 / TextureSize;
            float yScale = HalfSize.Y * 2 / TextureSize;
            scale = new Vector2(xScale, yScale);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentManager content = GDGame.GetActiveScreen().Content;
            tex = content.Load<Texture2D>("square");

            Origin = new Vector2(tex.Width, tex.Height) * 0.5f;

            base.LoadContent();
        }

        /// <summary>
        /// Sets the halfSize depending on the screen.
        /// </summary>
        /// <param name="game">The running game</param>
        /// <returns></returns>
        private static Vector2 SetHalfSize(Game1 game)
        {
            Viewport vp = game.GraphicsDevice.Viewport;
            float halfHeight = vp.Width * 0.5f * 0.8f;
            float halfWidth = vp.Height * 1 / 20;

            return new Vector2(halfWidth, halfHeight) * 0.5f;
        }

        /// <summary>
        /// Sets the position depending on the screen.
        /// </summary>
        /// <param name="game">The running game</param>
        /// <returns></returns>
        private static Vector2 SetPosition(Game1 game)
        {
            Viewport vp = game.GraphicsDevice.Viewport;
            float x = vp.Width * 1 / 20;
            float y = vp.Height * 0.5f;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Increases the power on the PowerBar.
        /// </summary>
        /// <param name="scoreMultiplier">Current score multiplier.</param>
        public void LoadPower(float scoreMultiplier)
        {
            //Only add if we are below 100 and aren't droppping.
            if (state == ScoreState.Balance && Power + toAdd < 100)
            {
                const float percentageAddedPerGain = 2.5f;

                //Calculate the bruto addition.
                float brutoAddition = percentageAddedPerGain * scoreMultiplier;

                //Calculate what the power will be with the current toAdd.
                float futurePower = Power + toAdd;

                //How much power can we still get?
                float powerGainLeft = 100 - futurePower;

                //Now calculate toAdd.
                toAdd += powerGainLeft < brutoAddition ? powerGainLeft : brutoAddition;
            }
        }

        /// <summary>
        /// Uses the power and starts dropping it.
        /// The power gain cooldown automaticly starts.
        /// </summary>
        /// <param name="playerPos">The player's position for the power.</param>
        public void UsePower(Vector2 playerPos)
        {
            if (state != ScoreState.Gain)
            {
                //Don't activate a power if you can't use it.
                return;
            }

            //Drop the power over time.
            toAdd = -100;
            state = ScoreState.Loss;

            //Create the PowerBall.
            PowerBall temp = new PowerBall(GDGame, playerPos);
            GDGame.GetActiveScreen().Components.Add(temp);
            temp.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Power addition logic.
            if (toAdd != 0)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float powerThisFrame = dt * PercentPerSecond;
                float diffValue;

                //How much do we add?
                if (toAdd > 0)
                {
                    diffValue = toAdd < powerThisFrame ? toAdd : powerThisFrame;
                    toAdd -= diffValue;
                }
                else
                {
                    diffValue = toAdd > -powerThisFrame ? toAdd : -powerThisFrame;
                    toAdd -= diffValue;
                }

                //And now add the Power.
                Power += diffValue;
            }

            //Power state logic.
            switch (state)
            {
                case ScoreState.Loss:
                    if (Power == 0)
                    {
                        state = ScoreState.Balance;
                    }
                    break;
                case ScoreState.Balance:
                    if (Power == 100)
                    {
                        state = ScoreState.Gain;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            const float rotation = 0;
            const float depth = 0;
            const SpriteEffects effect = SpriteEffects.None;

            //First the background.
            batch.Draw(tex, Position, null, Background, rotation, Origin, scale, effect, depth);

            //Position the bar.
            Vector2 barPos = new Vector2(
                Position.X,
                Position.Y + HalfSize.Y * PowerBarScale.Y);

            //Calculate it's scale.
            Vector2 barScale = new Vector2(
                scale.X * PowerBarScale.X,
                scale.Y * PowerBarScale.Y * Power * 0.01f);

            //What's the color?
            Color color;
            switch (state)
            {
                case ScoreState.Loss:
                    color = PowerUsed;
                    break;
                case ScoreState.Gain:
                    color = PowerReady;
                    break;
                case ScoreState.Balance:
                    color = PowerNeutral;
                    break;
                default:
                    color = Color.White;
                    break;
            }

            //Now draw the bar.
            batch.Draw(tex, barPos, null, color, MathHelper.Pi, new Vector2(tex.Width * 0.5f, 0),
                barScale, effect, depth);

            base.Draw(gameTime);
        }

        public override void Unload()
        {
            tex.Dispose();

            base.Unload();
        }
    }

}

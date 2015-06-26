using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameDesign_2.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.Components.Player
{
    /// <summary>
    /// The ScoreBar used during gameplay. This bar keeps track of the current score and time.
    /// The gained or lossed score adds at X% per second. 
    /// The players looses score after a certain amount of time.
    /// Score subtraction has priority over score addition.
    /// The player can build up a multiplier but hitting an enemy will also drop with
    /// that same multiplier before the multiplier is reset.
    /// </summary>
    public class ScoreBar : GDComp
    {
        //Drop the score by 1% after 10 seconds.
        private const int SecondsPerPointDrop = 5;
        private const int Goal = 100;
        private const float PercentDropByTimeBorder = 1f;
        private const float PercentPerSecond = 10f;
        //Stop dropping scores after 3.5 seconds.
        private const float ScoreDropTimeLimit = 3.5f;
        //Time border for the multipier.
        private const float MultiplierTimeBorder = 2.5f;

        //Bar colors.
        private readonly Color Background = Color.Black;
        private readonly Color PointLoss = new Color(new Vector3(150, 0, 0));      //Red.
        private readonly Color PointGain = new Color(new Vector3(0, 150, 0));      //Green.
        private readonly Color PointBalance = new Color(new Vector3(0, 0, 150));   //Blue.

        //Bar scale constant.
        private readonly Vector2 ColorBarScale = new Vector2(0.98f, 0.75f);

        //Pixel constant.
        private const int TextureSize = 100;

        public float Score { get; private set; }
        private ScoreState State { get; set; }

        private float toAdd;
        private float timeCounter;
        private float timeLeft;
        private float scoreDropTimer;
        private Texture2D texture;
        private bool isTimeLimited = false;
        private Vector2 scale;
        private SpriteFont font;

        //Multiplier variables.
        private float multiplier;
        private float multiplierTimer;
        private float multiplierCounter;
        private float forcedTimer;
        private bool resetMultiplier;
        
        /// <summary>
        /// Basic ScoreBar with no time limit.
        /// </summary>
        /// <param name="game">The current running game.</param>
        public ScoreBar(Game1 game)
            : base(game, Shape.None, SetPosition(game), SetHalfSize(game))
        {
            timeCounter = 0;
        }

        /// <summary>
        /// A ScoreBar with a possible timeLimit. If timeLimit is 0, then there's no timelimit.
        /// </summary>
        /// <param name="game">The current running game.</param>
        /// <param name="goalScore">The level's goal score.</param>
        /// <param name="timeLimit">The level's timelimit (0 means no limit).</param>
        public ScoreBar(Game1 game, float timeLimit)
            : this(game)
        {
            isTimeLimited = timeLimit > 0;

            if (isTimeLimited)
            {
                timeLeft = timeLimit;
            }
        }

        public override void Initialize()
        {
            Score = 5f;
            toAdd = 0;

            scoreDropTimer = 0;

            ResetMultiplier();

            State = ScoreState.Balance;

            float xScale = HalfSize.X * 2 / TextureSize;
            float yScale = HalfSize.Y * 2 / TextureSize;
            scale = new Vector2(xScale, yScale);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentManager content = GDGame.GetActiveScreen().Content;
            texture = content.Load<Texture2D>("square");
            font = content.Load<SpriteFont>("GDFont");

            Origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);

            base.LoadContent();
        }

        /// <summary>
        /// Adds score over time. The amount is added to a pile which slowly becomes less.
        /// Score subtraction has priority over addition.
        /// </summary>
        /// <param name="amount">Amount of score the player has earned.</param>
        public void AddScore(float amount)
        {
            if (amount < 0)
            {
                return;
            }

            toAdd += amount;

            multiplierCounter++;
            multiplierTimer = 0;
            if (multiplierCounter >= 5)
            {
                multiplier += 0.25f;
                multiplierCounter = 0;
            }
        }

        public void ClearStacks()
        {
            toAdd = 0;
            scoreDropTimer = 0;
        }

        public void DropScore(float amount)
        {
            toAdd = 0;
            scoreDropTimer = 0;

            Score -= amount;
        }

        /// <summary>
        /// Force the multiplier to a certain amount for a certain amount of time.
        /// </summary>
        /// <param name="multiplier">The new multiplier.</param>
        /// <param name="timer">For how long.</param>
        public void ForceMultiplier(float multiplier, float timer)
        {
            forcedTimer = timer;
            this.multiplier = multiplier;
        }

        public float GetScorePercentage()
        {
            return Score;
        }

        public void ResetMultiplier()
        {
            multiplierTimer = 0;
            multiplier = 1;
            multiplierCounter = 0;
            forcedTimer = 0;
            resetMultiplier = false;

        }

        /// <summary>
        /// Subtracts score over time. The amount is added to a pile which slowly becomes less.
        /// Score subtraction has priority over addition.
        /// </summary>
        /// <param name="amount">Amount of score the player has lossed.</param>
        /// <param name="hitByBall">Did the player hit a ball?</param>
        public void SubtractScore(float amount, bool hitByBall)
        {
            if (amount < 0)
            {
                amount = Math.Abs(amount);
            }

            if (hitByBall)
            {
                resetMultiplier = true;
            }

            toAdd -= amount;
        }

        /// <summary>
        /// Sets this bar's halfsize.
        /// </summary>
        /// <param name="game">The running game.</param>
        /// <returns></returns>
        private static Vector2 SetHalfSize(Game1 game)
        {
            Viewport vp = game.GraphicsDevice.Viewport;
            float halfWidth = vp.Width * 0.5f * 0.8f;
            float halfHeight = vp.Height * 1 / 20;

            return new Vector2(halfWidth, halfHeight) * 0.5f;
        }

        /// <summary>
        /// Sets this bar's position.
        /// </summary>
        /// <param name="game">The running game.</param>
        /// <returns></returns>
        private static Vector2 SetPosition(Game1 game)
        {
            Viewport vp = game.GraphicsDevice.Viewport;
            float y = vp.Height * 9 / 10;
            float x = vp.Width * 0.5f;

            return new Vector2(x, y);
        }

        public override void Update(GameTime gameTime)
        {
            //Delta Time.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (toAdd != 0)
            {
                //First calculate how many points max can be add or subtracted this frame.
                float pointsThisFrame = (PercentPerSecond * dt);

                //Are we gaining or loosing points?
                bool isDone;
                if (toAdd < 0)
                {
                    //Reset the multiplier.
                    ResetMultiplier();

                    //Check if these are the last points to subtract.
                    pointsThisFrame *= -1;
                    isDone = toAdd > pointsThisFrame;

                    //Change to the appropriate state.
                    State = ScoreState.Loss;

                    //Don't drop score for to long.
                    scoreDropTimer += dt;
                    if (scoreDropTimer >= ScoreDropTimeLimit)
                    {
                        ClearStacks();
                    }
                }
                else
                {
                    //Add the multiplier.
                    pointsThisFrame *= multiplier;

                    //Check if thes are the last points to add.
                    isDone = toAdd < pointsThisFrame;

                    //Change to the appropiate state.
                    State = ScoreState.Gain;

                    //Reset the scoreDropTimer.
                    scoreDropTimer = 0;
                }

                //Check the value which we add to or subtract from the score.
                float diffValue = isDone ? toAdd : pointsThisFrame;

                //Change toAdd and the score with score to be added or subtracted this frame.
                toAdd -= diffValue;
                Score += diffValue;
            }
            else
            {
                //Reset the scoreDropTimer.
                scoreDropTimer = 0;

                //Change to the appropiate state.
                State = ScoreState.Balance;
            }

            //Check if reached any score border.
            if (Score <= 0)
            {
                (GDGame.GetActiveScreen() as GameplayScreen).GameOver();
            }
            else if (Score >= Goal)
            {
                (GDGame.GetActiveScreen() as GameplayScreen).Won();
            }

            //Multiplier logic.
            if (forcedTimer == 0)
            {
                multiplierTimer += dt;
                if (multiplierTimer >= MultiplierTimeBorder)
                {
                    ResetMultiplier();
                }

                //Lastly the time.
                timeCounter += dt;
                if (timeCounter >= SecondsPerPointDrop)
                {
                    timeCounter -= SecondsPerPointDrop;
                    SubtractScore(PercentDropByTimeBorder, false);
                }
            }
            else
            {
                forcedTimer -= dt;
                if (forcedTimer <= 0)
                {
                    ResetMultiplier();
                }
            }

            if (isTimeLimited)
            {
                timeLeft -= dt;
                if (timeLeft <= 0)
                {
                    (GDGame.GetActiveScreen() as GameplayScreen).GameOver();
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            const float rotation = 0;
            const float depth = 0;
            const SpriteEffects effect = SpriteEffects.None;

            //First the background.
            batch.Draw(texture, Position, null, Background, rotation, Origin, scale, effect, depth);

            //Now the bar's color.
            Color color;
            switch (State)
            {
                case ScoreState.Gain:
                    color = PointGain;
                    break;
                case ScoreState.Loss:
                    color = PointLoss;
                    break;
                default:
                    color = PointBalance;
                    break;
            }

            //Calculate the position for the colored bar.
            Vector2 cBarPos = new Vector2(
                Position.X - HalfSize.X * ColorBarScale.X,
                Position.Y);

            //Calculate the right scale.
            float xScale = GetScorePercentage() * 0.01f;
            Vector2 cBarScale = new Vector2(
                scale.X * xScale * ColorBarScale.X,
                scale.Y * ColorBarScale.Y);
            
            //Now draw the bar.
            batch.Draw(texture, cBarPos, null, color, rotation, new Vector2(0, texture.Height * 0.5f), 
                cBarScale, effect, depth);
            
            //Now the multiply text.
            string text = "Multiplier: x" + multiplier.ToString();
            Vector2 textSize = font.MeasureString(text);
            Vector2 textPos = new Vector2(Position.X,
                Position.Y + textSize.Y * 1.5f);

            batch.DrawString(font, text, textPos, Color.Black, 0, textSize * 0.5f, 1, SpriteEffects.None, 0);

            base.Draw(gameTime, batch);
        }

        public override void Unload()
        {
            texture.Dispose();

            base.Unload();
        }

        /// <summary>
        /// Get the current multiplier.
        /// </summary>
        public float Multiplier
        {
            get
            {
                return multiplier;
            }
        }
    }

    enum ScoreState
    {
        Loss,
        Gain,
        Balance
    }
}

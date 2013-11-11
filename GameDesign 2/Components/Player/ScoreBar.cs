using System;
using System.Collections.Generic;
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
    /// </summary>
    public class ScoreBar : GDComp
    {
        //Drop the score by 5% after 10 seconds.
        private const int SecondsPerPointDrop = 10;
        private const int PercentDropByTimeBorder = 5;

        //Bar colors.
        private const Color Background = Color.Black;
        private const Color PointLoss = new Color(new Vector3(150, 0, 0));      //Red.
        private const Color PointGain = new Color(new Vector3(0, 150, 0));      //Green.
        private const Color PointBalance = new Color(new Vector3(0, 0, 150));   //Blue.

        private readonly int PercentPerSecond;

        public int Score { get; private set; }

        private int toAdd;
        private int toSubtract;
        private int goalScore;
        private float timeCounter;
        private float timeLeft;
        private Texture2D texture;
        private bool isTimeLimited = false;
        
        /// <summary>
        /// Basic ScoreBar with no time limit.
        /// </summary>
        /// <param name="game">The current running game.</param>
        /// <param name="goalScore">The level's goal score.</param>
        public ScoreBar(Game1 game, int goalScore)
            : base(game, Shape.None, SetPosition(game), SetHalfSize(game))
        {
            toAdd = 0;
            toSubtract = 0;
            this.goalScore = goalScore;
            PercentPerSecond = (int)(goalScore * 0.01f);
            Score = 0;
        }

        /// <summary>
        /// A ScoreBar with a possible timeLimit. If timeLimit is 0, then there's no timelimit.
        /// </summary>
        /// <param name="game">The current running game.</param>
        /// <param name="goalScore">The level's goal score.</param>
        /// <param name="timeLimit">The level's timelimit (0 means no limit).</param>
        public ScoreBar(Game1 game, int goalScore, float timeLimit)
            : this(game, goalScore)
        {
            isTimeLimited = timeLimit > 0;

            if (isTimeLimited)
            {
                timeLeft = timeLimit;
            }
        }

        protected override void LoadContent()
        {
            ContentManager content = GDGame.GetActiveScreen().Content;
            texture = content.Load<Texture2D>("square");

            Origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);

            base.LoadContent();
        }

        /// <summary>
        /// Adds score over time. The amount is added to a pile which slowly becomes less.
        /// Score subtraction has priority over addition.
        /// </summary>
        /// <param name="amount">Amount of score the player has earned.</param>
        public void AddScore(int amount)
        {
            if (amount < 0)
            {
                return;
            }

            toAdd += amount;
        }

        /// <summary>
        /// Subtracts score over time. The amount is added to a pile which slowly becomes less.
        /// Score subtraction has priority over addition.
        /// </summary>
        /// <param name="amount">Amount of score the player has lossed.</param>
        public void SubtractScore(int amount)
        {
            if (amount < 0)
            {
                amount = Math.Abs(amount);
            }

            toSubtract += amount;
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
            float halfHeight = vp.Height * 19 / 20;

            return new Vector2(halfWidth, halfHeight);
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

            //First calculate how many points max can be add or subtracted this frame.
            int pointsThisFrame = (int)(PercentPerSecond * dt);

            //Are there points to subtract?
            if (toSubtract > 0)
            {
                int loss = pointsThisFrame < toSubtract ? pointsThisFrame : toSubtract;

                Score -= loss;
                toSubtract -= loss;
            }
            //Are there points to add?
            else if (toAdd > 0)
            {
                int gain = pointsThisFrame < toAdd ? pointsThisFrame : toAdd;

                Score += gain;
                toAdd -= gain;
            }

            //Check if reahed any score border.
            if (Score <= 0)
            {
                (GDGame.GetActiveScreen() as GameplayScreen).GameOver();
            }
            else if (Score >= goalScore)
            {
                (GDGame.GetActiveScreen() as GameplayScreen).Won();
            }

            //Lastly the time.
            timeCounter += dt;
            if (timeCounter >= SecondsPerPointDrop)
            {
                timeCounter -= SecondsPerPointDrop;
                SubtractScore((int)(PercentDropByTimeBorder * 0.01f * goalScore));
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

        public override void Unload()
        {
            texture.Dispose();

            base.Unload();
        }
    }
}

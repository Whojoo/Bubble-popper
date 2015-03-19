using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components.Player;
using GameDesign_2.Screens;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Components
{
    public class ShieldBall : Ball
    {
        private const float MinSize = 30;
        private const float MaxSize = 200;
        private const float TimeAlive = 5f;

        private bool justSpawned;
        private Rectangle world;
        private Random randy;
        private float aliveCounter;

        public ShieldBall(Game1 game, Rectangle world)
            : base(game, new Vector2(), 0.5f)
        {
            //Modify the color.
            Color color = Color.Yellow;
            color.A = (byte)(256 * 0.5f);
            Color = color;

            randy = new Random(5789);

            //Calculate the correction and copy the world.
            int correction = (int)MaxSize;

            this.world = world;

            //Correct the world so we can't spawn on the outside.
            world.X += correction;
            world.Y += correction;
            world.Height -= correction;
            world.Width -= correction;

            Respawn();
        }

        public void Respawn()
        {
            //We have just spawned.
            justSpawned = true;

            //Reset the size and counter.
            Scale = MinSize;
            aliveCounter = 0;

            //Get a random position in the world.
            Vector2 temp = new Vector2(
                randy.Next(world.X, world.Width),   //X value.
                randy.Next(world.Y, world.Height)); //Y value.
            Position = temp;
        }

        public override bool CheckCollisionWith(GameTime gameTime, GDComp other)
        {
            //Respawn if we spawned inside a wall.
            if (justSpawned && other is Wall && CircleBoxCollision(other))
            {
                Respawn();
            }

            //Kill all the enemies (if the player is inside).
            if (!(GDGame.GetActiveScreen() is GameplayScreen))
            {
                return false;
            }
            PlayerBall player = ((GameplayScreen)GDGame.GetActiveScreen()).Player;
            if (other is ScoreBall && CircleCircleCollision(other) &&
                CircleCircleCollision(player))
            {
                Spawner.GetInstance().RemoveBall(other as ScoreBall);
                //player.AddScore();
            }

            //Is the player in the shield?
            if (other is PlayerBall && CircleCircleCollision(other))
            {
                const float growthPerSecond = (MaxSize - MinSize) * 0.5f;
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Is the player moving?
                if (other.Velocity.LengthSquared() > 0)
                {
                    //Time to grow (if possible).
                    if (Scale < MaxSize)
                    {
                        //How much do we grow?
                        float growth = growthPerSecond * dt;

                        //Now grow.
                        Scale = Scale + growth < MaxSize ?
                            Scale + growth : MaxSize;
                    }

                    //Now time to exhaust the shield.
                    aliveCounter += dt;
                    if (aliveCounter >= TimeAlive)
                    {
                        Respawn();
                    }
                }
                else
                {
                    //Time to shrink (if possible).
                    if (Scale > MinSize)
                    {
                        //How much do we shrink?
                        float shrink = growthPerSecond * dt * 0.25f;

                        //Now shrink.
                        Scale = Scale - shrink > MinSize ?
                            Scale - shrink : MinSize;
                    }
                }
            }

            return false;
        }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            Console.WriteLine("yay");
            base.Draw(gameTime, batch);
        }
    }
}

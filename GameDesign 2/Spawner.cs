using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using GameDesign_2.Components.Player;
using GameDesign_2.Screens;

namespace GameDesign_2
{
    public class Spawner
    {
        private const int DefaultMaximum = 1000;
        private const int DefaultFriendliesPerEnemies = 20;

        //Use a spawns per frame since the Spawner will compensate when we get below minimum.
        private const int SpawnsPerFrame = 1;

        /// <summary>
        /// The maximum amount of ScoreBalls that can be active.
        /// Default = 1000.
        /// </summary>
        public int MaximumAlive { get; set; }

        /// <summary>
        /// The running game. You have to add this yourself.
        /// </summary>
        public Game1 Game { get; set; }

        private static Spawner instance;

        private List<ScoreBall> graveyard;
        private List<ScoreBall> active;
        private List<SpawnPortal> portals;

        //Used to circle through spawnlocations.
        private int friendlyPortalIndex;
        private int enemyPortalIndex;

        //Used to keep track of friendlies and enemies.
        private int enemies;
        private int friendlies;
        private int friendliesPerEnemies;

        private Spawner()
        {
            graveyard = new List<ScoreBall>();
            active = new List<ScoreBall>();
            portals = new List<SpawnPortal>();

            MaximumAlive = DefaultMaximum;

            friendlyPortalIndex = 0;
            enemyPortalIndex = 0;

            enemies = 0;
            friendlies = 0;
            friendliesPerEnemies = DefaultFriendliesPerEnemies;
        }

        /// <summary>
        /// Get the singleton Spawner.
        /// </summary>
        public static Spawner GetInstance()
        {
            if (instance == null)
            {
                instance = new Spawner();
            }

            return instance;
        }

        /// <summary>
        /// Adds a ScoreBall to the game.
        /// </summary>
        /// <param name="portal"></param>
        private void AddBall()
        {
            ScoreBall ball;
            int lastInGraveyard = graveyard.Count - 1;

            //Do we need an enemy?
            bool isEnemy = enemies == 0 || (int)(friendlies / enemies) >= friendliesPerEnemies;
            
            //Get the right portal.
            SpawnPortal portal = GetNextPortal(isEnemy);

            //Get a ScoreBall from the graveyard or create a new one.
            if (lastInGraveyard >= 0)
            {
                ball = graveyard[lastInGraveyard];
                graveyard.RemoveAt(lastInGraveyard);
                ball.Remove = false;
                ball.Position = portal.GetSpawnPosition();
            }
            else
            {
                ball = new ScoreBall(Game, portal.GetSpawnPosition(),
                    Sinusoid.GetInstance().GetRandomIndex());
            }

            Game.GetActiveScreen().Components.Add(ball);
            active.Add(ball);
            ball.Initialize();

            //Do we need a friendly?
            if (!isEnemy)
            {
                ball.ChangeState(ScoreBall.State.Friendly);
                friendlies++;
            }
            //We need an enemy.
            else
            {
                ball.ChangeState(ScoreBall.State.Enemy);
                enemies++;
            }
        }

        /// <summary>
        /// Adept the current amount of ScoreBalls to the current friendlies per enemies balance.
        /// </summary>
        private void AdeptScoreBallStates()
        {
            if (enemies == 0)
            {
                return;
            }

            int currentBalance = (int)(friendlies / enemies);

            int difference = friendliesPerEnemies - currentBalance;

            //Do we need to do anything at all?
            if (difference == 0)
            {
                return;
            }
            //Do we require more enemies?
            else if (difference < 0)
            {
                difference = Math.Abs(difference);
                PlayerBall player = (Game.GetActiveScreen() as GameplayScreen).Player;

                //Make sure the ScoreBalls aren't about to hit the player.
                const float SafeRangeMultiplier = 1.5f;
                float requiredDistSQ = player.HalfSize.X * SafeRangeMultiplier;
                requiredDistSQ *= requiredDistSQ;

                int i = 0;
                while (i < difference)
                {
                    if (i == active.Count)
                    {
                        //Don't go beyond the active counter.
                        return;
                    }
                    else if ((active[i++].Position - player.Position).LengthSquared() < requiredDistSQ)
                    {
                        continue;
                    }

                    active[i].ChangeState(ScoreBall.State.Enemy);

                    //Adept the enemies and friendlies counter.
                    enemies++;
                    friendlies--;

                    i++;
                }
            }
            //We require more friendlies.
            else
            {
                //It doesn't matter if the ScoreBalls are close to the player.
                for (int i = 0; i < difference; i++)
                {
                    active[i].ChangeState(ScoreBall.State.Friendly);

                    //Adept the enemies and friendlies counter.
                    enemies--;
                    friendlies++;
                }
            }
        }

        /// <summary>
        /// Add a portal to the spawner list. 
        /// </summary>
        /// <param name="portal">The new portal.</param>
        public void AddPortal(SpawnPortal portal)
        {
            portals.Add(portal);
        }

        /// <summary>
        /// Clears the playingfield.
        /// </summary>
        public void Clear()
        {
            for (int i = active.Count - 1; i >= 0; i--)
            {
                RemoveBall(active[i]);
            }
        }

        /// <summary>
        /// Get a portal from the portal list. This function makes sure you
        /// circle through all the portals.
        /// </summary>
        /// <param name="isEnemy">Are we spawning an enemy?</param>
        /// <returns>A SpawnPortal for a location to spawn.</returns>
        private SpawnPortal GetNextPortal(bool isEnemy)
        {
            //Return the right portal depending on enemy or friendly.
            if (isEnemy)
            {
                return portals[enemyPortalIndex >= portals.Count ?
                    (enemyPortalIndex = 0) : enemyPortalIndex++];
            }
            else
            {
                return portals[friendlyPortalIndex >= portals.Count ?
                    (friendlyPortalIndex = 0) : friendlyPortalIndex++];
            }
        }

        /// <summary>
        /// Removes a ScoreBall from the active list.
        /// </summary>
        /// <param name="ball"></param>
        public void RemoveBall(ScoreBall ball)
        {
            //Set the ball for removing. The game-loop will remove it when it's safe.
            ball.Remove = true;
            active.Remove(ball);

            if (ball.ScoreState == ScoreBall.State.Enemy)
            {
                enemies--;
            }
            else
            {
                friendlies--;
            }

            graveyard.Add(ball);
        }

        public void Reset()
        {
            portals.Clear();

            //Unload and clear the graveyard.
            foreach (GDComp comp in graveyard)
            {
                comp.Unload();
                comp.Dispose();
            }
            graveyard.Clear();

            //Clear the active list. They are unloaded by GameplayScreen.
            active.Clear();

            //Put the variables back to default.
            MaximumAlive = DefaultMaximum;
            friendlyPortalIndex = 0;

            //Reset the counters.
            friendlies = 0;
            enemies = 0;
            friendliesPerEnemies = DefaultFriendliesPerEnemies;
        }

        public void Update()
        {
            //Check if there is need for more or less ScoreBalls.
            int totalActive = active.Count;
            int toAdd = totalActive >= MaximumAlive ? 0 : SpawnsPerFrame;

            //Can we spawn anything?
            if (portals.Count == 0)
            {
                return;
            }

            //Add the ScoreBalls.
            for (int i = 0; i < toAdd; i++)
            {
                AddBall();
            }
        }

        /// <summary>
        /// The amount of friendlies per enemies.
        /// Default = 20.
        /// </summary>
        public int FriendliesPerEnemies 
        {
            get
            {
                return friendliesPerEnemies;
            }
            set
            {
                if (value < 0)
                {
                    return;
                }

                friendliesPerEnemies = value;

                //Adept some scoreballs to the new value.
                AdeptScoreBallStates();
            }
        }
    }
}

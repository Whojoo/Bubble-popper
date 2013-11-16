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
        private const int DefaultMinimum = 100;
        private const int DefaultMaximum = 1000;
        private const int DefaultFriendliesPerEnemies = 20;

        //Use a spawns per frame since the Spawner will compensate when we get below minimum.
        private const int SpawnsPerFrame = 1;

        /// <summary>
        /// The minimum amount of ScoreBall balls that have to be active.
        /// Default = 100.
        /// </summary>
        public int MinimumAlive { get; set; }

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
        private int portalIndex;

        //Used to keep track of friendlies and enemies.
        private int enemies;
        private int friendlies;
        private int friendliesPerEnemies;

        private Spawner()
        {
            graveyard = new List<ScoreBall>();
            active = new List<ScoreBall>();
            portals = new List<SpawnPortal>();

            MinimumAlive = DefaultMinimum;
            MaximumAlive = DefaultMaximum;

            portalIndex = 0;

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
        private void AddBall(SpawnPortal portal)
        {
            ScoreBall ball;
            int lastInGraveyard = graveyard.Count - 1;

            //Get a ScoreBall from the graveyard or create a new one.
            if (lastInGraveyard >= 0)
            {
                ball = graveyard[lastInGraveyard];
                graveyard.RemoveAt(lastInGraveyard);
            }
            else
            {
                ball = new ScoreBall(Game, portal.GetSpawnPosition(),
                    Sinusoid.GetInstance().GetRandomIndex());
            }

            Game.GetActiveScreen().Components.Add(ball);
            active.Add(ball);

            //Do we need a friendly?
            if (((int)friendlies / enemies) < friendliesPerEnemies)
            {
                ball.ChangeState(ScoreBall.State.Friendly);
            }
            //We need an enemy.
            else
            {
                ball.ChangeState(ScoreBall.State.Enemy);
            }
        }

        /// <summary>
        /// Adept the current amount of ScoreBalls to the current friendlies per enemies balance.
        /// </summary>
        private void AdeptScoreBallStates()
        {
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
                const float SafeRangeMultiplier = 4;
                float requiredDistSQ = player.HalfSize.X * SafeRangeMultiplier;
                requiredDistSQ *= requiredDistSQ;

                int i = 0;
                while (i < difference)
                {
                    if ((active[i].Position - player.Position).LengthSquared() < requiredDistSQ)
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
        /// Get a portal from the portal list. This function makes sure you
        /// circle through all the portals.
        /// </summary>
        /// <returns>A SpawnPortal for a location to spawn.</returns>
        private SpawnPortal GetNextPortal()
        {
            SpawnPortal toReturn = portals[portalIndex++];
            if (portalIndex >= portals.Count)
            {
                portalIndex = 0;
            }

            return toReturn;
        }

        /// <summary>
        /// Removes a ScoreBall from the active list.
        /// </summary>
        /// <param name="ball"></param>
        public void RemoveBall(ScoreBall ball)
        {
            //Set the ball for removing. The game-loop will remove it when it's safe.
            ball.Remove = true;

            int index = active.IndexOf(ball);
            if (index != -1)
            {
                active.RemoveAt(index);
            }

            graveyard.Add(ball);
        }

        public void Reset()
        {
            portals.Clear();

            //Add all active balls to the graveyard.
            foreach (ScoreBall ball in active)
            {
                graveyard.Add(ball);
            }
            active.Clear();

            //Put the variables back to default.
            MinimumAlive = DefaultMinimum;
            MaximumAlive = DefaultMaximum;
            portalIndex = 0;
        }

        public void Update()
        {
            int toAdd = SpawnsPerFrame;
            int totalActive = active.Count;

            //Check if there is need for more or less ScoreBalls.
            if (totalActive < MinimumAlive)
            {
                //Don't catch up to quickly. if we lack 10 then 1 extra this frame will be fine.
                toAdd += (int)((MinimumAlive - totalActive) * 0.1f);
            }
            else if (totalActive >= MaximumAlive)
            {
                toAdd = 0;
            }

            //Add the ScoreBalls.
            for (int i = 0; i < toAdd; i++)
            {
                AddBall(GetNextPortal());
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
                if (value <= 0)
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

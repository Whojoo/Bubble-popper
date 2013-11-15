using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;

namespace GameDesign_2
{
    public class Spawner
    {
        private const int DefaultMinimum = 100;
        private const int DefaultMaximum = 1000;

        //Use a spawns per frame since the Spawner will compensate when we get below minimum.
        private const int SpawnsPerFrame = 1;

        /// <summary>
        /// The minimum amount of ScoreBall balls that have to be active.
        /// Default = 100
        /// </summary>
        public int MinimumAlive { get; set; }

        /// <summary>
        /// The maximum amount of ScoreBalls that can be active.
        /// Default = 1000
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

        private Spawner()
        {
            graveyard = new List<ScoreBall>();
            active = new List<ScoreBall>();
            portals = new List<SpawnPortal>();

            MinimumAlive = DefaultMinimum;
            MaximumAlive = DefaultMaximum;

            portalIndex = 0;
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
    }
}

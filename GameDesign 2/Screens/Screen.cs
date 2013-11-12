using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using GameDesign_2.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2.Screens
{
    /// <summary>
    /// This is a screen that can be added to the ScreenManager. Extend it and add components 
    /// to it in the Initialize() method. You can also override the Update() and Draw() method.
    /// </summary>
    public class Screen : GDComp
    {
        /// <summary>
        /// This member tells if this screen is initialized.
        /// </summary>
        public bool Initialized { get; private set; }

        /// <summary>
        /// Set this member to true if this screen doesn't cover the entire screen.
        /// </summary>
        public bool Translucent { get; set; }

        /// <summary>
        /// A reference to the ScreenManager.
        /// </summary>
        public ScreenManager Manager { get; internal set; }

        /// <summary>
        /// The GameComponentCollection, add components for this screen.
        /// </summary>
        public GameComponentCollection Components { get; private set; }

        /// <summary>
        /// The HuDComponents. These are drawn last to make sure they're on top.
        /// </summary>
        public GameComponentCollection HuDComponents { get; private set; }

        public SpriteBatch Batch { get; private set; }
        public ContentManager Content { get; private set; }

        private Thread collisionThread;
        private ThreadWork worker;
        private bool skipCollision = false;
        private float collisionElapsedTime = 0;

        public Screen(Game1 game)
            : base(game, Shape.None, new Vector2(), new Vector2())
        {
            this.Components = new GameComponentCollection();
            HuDComponents = new GameComponentCollection();

            Translucent = false;

            Content = new ContentManager(game.Services);
            Content.RootDirectory = "Content";
            Batch = new SpriteBatch(game.GraphicsDevice);
        }

        /// <summary>
        /// This method is called when this screen is back at the top of the stack.
        /// </summary>
        public virtual void Activated()
        {
        }

        /// <summary>
        /// This method is called when a screen is deactivated by an other screen.
        /// </summary>
        public virtual void Deactivated()
        {
        }

        /// <summary>
        /// Initialize every Component of this screen.
        /// </summary>
        public override void Initialize()
        {
            Initialized = true;
            foreach (GDComp comp in Components)
            {
                comp.Initialize();
            }

            foreach (GDComp comp in HuDComponents)
            {
                comp.Initialize();
            }
            base.Initialize();
        }

        /// <summary>
        /// Update every Enabled Component of this screen.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Major credits to Nils Dijk:
            foreach (GDComp gc in this.Components.OfType<GDComp>().Where<GDComp>(x => x.Enabled).OrderBy<IUpdateable, int>(x => x.UpdateOrder))
            {
                gc.Update(gameTime);
            }

            //UpdateCollider(gameTime);

            foreach (GDComp gc in this.HuDComponents.OfType<GDComp>().Where<GDComp>(x => x.Enabled).OrderBy<IUpdateable, int>(x => x.UpdateOrder))
            {
                gc.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public void UpdateCollider(GameTime gameTime)
        {
            for (int i = Components.Count - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    GDComp c1, c2;
                    if ((c1 = Components[i] as GDComp) != null &&
                        (c2 = Components[j] as GDComp) != null)
                    {
                        c1.CheckCollisionWith(gameTime, c2);
                    }
                }
            }
        }

        /// <summary>
        /// Draw every Visible Component of this screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Major credits to Nils Dijk:
            Matrix transform = GDGame.Camera.GetView();

            //Regular components. Need to be transformed.
            Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, transform);
            foreach (GDComp gc in this.Components.OfType<GDComp>().Where<GDComp>(x => x.Visible).OrderBy<GDComp, int>(x => x.DrawOrder))
                gc.Draw(gameTime, Batch);
            Batch.End();

            //HuDComponents which dont need to be transformed.
            Batch.Begin();
            foreach (GDComp gc in this.HuDComponents.OfType<GDComp>().Where<GDComp>(x => x.Visible).OrderBy<GDComp, int>(x => x.DrawOrder))
                gc.Draw(gameTime, Batch);
            Batch.End();

            base.Draw(gameTime);
        }

        public override void Unload()
        {
            Content.Unload();
            Content.Dispose();

            foreach (GDComp comp in Components)
            {
                comp.Unload();
                comp.Dispose();
            }

            foreach (GDComp comp in HuDComponents)
            {
                comp.Unload();
                comp.Dispose();
            }
        }
    }

    internal class ThreadWork
    {
        private const int ComponentPerThread = 300;
        private int index;
        private object locker = new object();
        private List<GDComp> components;

        public void Go(GameTime gameTime, List<GDComp> comps)
        {
            components = comps;
            index = comps.Count;
            Thread[] workers = new Thread[(int)(index / ComponentPerThread)];

            Update(gameTime, workers);
            Wait(workers);

            index = comps.Count;
        }

        private void Wait(Thread[] workers)
        {
            while (true)
            {
                bool temp = false;
                foreach (Thread t in workers)
                {
                    if (t.IsAlive)
                    {
                        temp = true;
                        break;
                    }
                }

                if (temp)
                    continue;

                break;
            }
        }

        private void CollisionCheck(GameTime gameTime, Thread[] workers)
        {
            
        }

        private void Update(GameTime gameTime, Thread[] workers)
        {
            for (int i = 0; i < workers.Length; i++)
            {
                workers[i] = new Thread(UpdateThreader);
                workers[i].Start(gameTime);
            }

            UpdateThreader(gameTime);
        }

        private void UpdateThreader(object gameTime)
        {
            GameTime elapsed = gameTime as GameTime;
            GDComp comp;
            while ((comp = GetNext()) != null)
            {
                comp.Update(elapsed);
            }
        }

        private GDComp GetNext()
        {
            lock (locker)
            {
                index--;

                if (index < 0)
                {
                    return null;
                }
                else
                {
                    return components[index];
                }
            }
        }

        public GameComponentCollection Components { get; set; }
    }
}
//Hello =D 
//Lol I can type anything without influencing your program, right?
//Yay xD

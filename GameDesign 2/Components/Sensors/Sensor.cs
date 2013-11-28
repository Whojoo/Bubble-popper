using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2.Components.Sensors
{
    /// <summary>
    /// Simple basic sensor. This class assumes the sensor is a rectangle shape.
    /// </summary>
    public class Sensor : GDComp
    {
        /// <summary>
        /// The target the sensor might be following.
        /// </summary>
        public GDComp Target { get; set; }

        private List<GDComp> contactList;

        /// <summary>
        /// Basic sensor with a target it's following.
        /// </summary>
        public Sensor(Game1 game, GDComp target, Vector2 halfSize)
            : base(game, Shape.Sensor, target.Position, halfSize)
        {
            Target = target;
            contactList = new List<GDComp>();
        }

        /// <summary>
        /// Basic sensor with position (0, 0) and no target to follow.
        /// </summary>
        public Sensor(Game1 game, Vector2 halfSize)
            : base(game, Shape.Sensor, new Vector2(), halfSize)
        {
            contactList = new List<GDComp>();
        }

        public override bool CheckCollisionWith(GameTime gameTime, GDComp other)
        {
            //Clear the contactlist.
            contactList.Clear();

            //Check for overlap.
            if (other.Shape == Components.Shape.Box &&
                BoxBoxCollision(other))
            {
                contactList.Add(other);
            }
            else if (other.Shape == Components.Shape.Circle &&
                other.CircleBoxCollision(this))
            {
                contactList.Add(other);
            }

            //It either doesn't overlapp or the other is a sensor.
            return false;
        }

        /// <summary>
        /// Get the contactlist.
        /// </summary>
        /// <returns></returns>
        public List<GDComp> GetContactList()
        {
            return contactList;
        }

        public override void Update(GameTime gameTime)
        {
            if (Target != null)
            {
                Position = Target.Position;
            }

            base.Update(gameTime);
        }
    }
}

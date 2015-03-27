using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDesign_2
{
    public struct LineSegment
    {
        private Vector2 start;
        private Vector2 end;

        public LineSegment(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }

        public Vector2 Direction()
        {
            return end - start;
        }

        public Vector2 Normal()
        {
            Vector2 dir = Direction();
            return new Vector2(dir.Y, -dir.X);
        }

        public bool IntersectsWith(LineSegment line)
        {
            Vector2 normal = Normal();
            Vector2 direction = line.Direction();
            float constant = Vector2.Dot(normal, start);

            float lamba = (constant - Vector2.Dot(line.Start, normal)) /
                Vector2.Dot(direction, normal);

            return 0 <= lamba && lamba <= 1;
        }

        public Vector2 Start
        {
            get
            {
                return start;
            }
        }

        public Vector2 End
        {
            get
            {
                return end;
            }
        }
    }
}

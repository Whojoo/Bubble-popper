using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using Microsoft.Xna.Framework;

namespace GameDesign_2
{
    /// <summary>
    /// A simple Quad Tree used for both updating and collision detection.
    /// Huge thanks to Steven Lambert.
    /// Link: http://gamedev.tutsplus.com/tutorials/implementation/quick-tip-use-quadtrees-to-detect-likely-collisions-in-2d-space/
    /// </summary>
    public class QuadTree
    {
        //Node constants.
        private const int MaxLevel = 10;
        private const int MaxLeaves = 5;
        private const int Nodes = 4;

        //Node sorting constants.
        private const int TopLeft = 0;
        private const int BottomLeft = 1;
        private const int BottomRight = 2;
        private const int TopRight = 3;

        public readonly Rectangle Bounds;

        private int level;
        private QuadTree[] nodes;
        private List<GDComp> leaves;

        public QuadTree(int level, Rectangle bounds)
        {
            this.level = level;
            Bounds = bounds;
            nodes = new QuadTree[Nodes];
            leaves = new List<GDComp>();
        }

        public int GetIndex(Rectangle rect)
        {
            int index = -1;

            //Get this node's X and Y center point.
            float xMid = Bounds.X + Bounds.Width * 0.5f;
            float yMid = Bounds.Y + Bounds.Height * 0.5f;

            //Does the object fit completely on the top OR bottom side?
            bool isInTop = rect.Y + rect.Height < yMid;
            bool isInBottom = rect.Y > yMid;
            if (isInTop ^ isInBottom)
            {
                //If not, early out.
                return index;
            }

            //Is the object completely on the left side?
            if (rect.X + rect.Width < yMid)
            {
                //Only need to check for top now, or else we would've had an early out.
                if (isInTop)
                {
                    return TopLeft;
                }
                else
                {
                    return BottomLeft;
                }
            }
            //Is the object completely on the right side?
            else if (rect.X > yMid)
            {
                //Only need to check for top now, or else we would've had an early out.
                if (isInTop)
                {
                    return TopRight;
                }
                else
                {
                    return BottomRight;
                }
            }

            //If all fails, then it fits multiple rects, return -1.
            return index;
        }
    }
}

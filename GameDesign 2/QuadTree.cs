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
        private const int MaxLevel = 10;
        private const int MaxLeaves = 5;
        private const int Nodes = 4;

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
    }
}

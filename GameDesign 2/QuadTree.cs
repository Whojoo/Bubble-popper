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
        /// <summary>
        /// Enum used for a quadrant's index.
        /// </summary>
        public enum Index : int
        {
            TopLeft,
            BottomLeft,
            BottomRight,
            TopRight
        }

        //Node constants.
        private const int MaxLevel = 10;
        private const int MaxLeaves = 5;
        private const int Nodes = 4;

        //Readonly bounds.
        public readonly Rectangle Bounds;

        //Other variables.
        private int level;
        private QuadTree[] nodes;
        private List<GDComp> leaves;

        /// <summary>
        /// Creates a (possible) sub-node.
        /// </summary>
        /// <param name="level">The node's depth. Top node is 0.</param>
        /// <param name="bounds">The node's bounds. Top node's bounds should be the world size.</param>
        public QuadTree(int level, Rectangle bounds)
        {
            this.level = level;
            Bounds = bounds;
            nodes = new QuadTree[Nodes];
            leaves = new List<GDComp>();
        }

        /// <summary>
        /// Creates the top node.
        /// </summary>
        /// <param name="bounds">The world's size.</param>
        public QuadTree(Rectangle bounds)
            : this(0, bounds)
        {
        }

        /// <summary>
        /// Clears this node and all it's child nodes.
        /// If this is the Top node, then the entire tree gets cleared.
        /// The Top node is still available for use.
        /// </summary>
        public void Clear()
        {
            //Do we have any sub-notes?
            if (nodes != null)
            {
                //Clear all sub-notes.
                for (int i = 0; i < Nodes; i++)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }

            //Now clear this node's stuff.
            leaves.Clear();
        }

        /// <summary>
        /// Get all leaf objects from this node and the child nodes.
        /// </summary>
        public List<GDComp> GetAll(List<GDComp> returnList)
        {
            //Add this node's leaves.
            returnList.AddRange(leaves);

            //Add child node's leaves.
            if (nodes[0] != null)
            {
                for (int i = 0; i < Nodes; i++)
                {
                    nodes[i].GetAll(returnList);
                }
            }

            return returnList;
        }

        public int GetIndex(Rectangle rect)
        {
            //The constant if the object doesn't completely fit in a child node.
            const int NotInJustOneChildNode = -1;

            //Get this node's X and Y center point.
            float xMid = Bounds.X + Bounds.Width * 0.5f;
            float yMid = Bounds.Y + Bounds.Height * 0.5f;

            //Does the object fit completely on the top OR bottom side?
            bool isInTop = rect.Y + rect.Height < yMid;
            bool isInBottom = rect.Y > yMid;
            if (isInTop ^ isInBottom)
            {
                //If not, early out.
                return NotInJustOneChildNode;
            }

            //Is the object completely on the left side?
            if (rect.X + rect.Width < yMid)
            {
                //Only need to check for top now, or else we would've had an early out.
                if (isInTop)
                {
                    return (int)Index.TopLeft;
                }
                else
                {
                    return (int)Index.BottomLeft;
                }
            }
            //Is the object completely on the right side?
            else if (rect.X > yMid)
            {
                //Only need to check for top now, or else we would've had an early out.
                if (isInTop)
                {
                    return (int)Index.TopRight;
                }
                else
                {
                    return (int)Index.BottomRight;
                }
            }

            //If all fails, then it fits multiple rects, return -1.
            return NotInJustOneChildNode;
        }

        /// <summary>
        /// Get all the leaf objects from a node's quadrant.
        /// The TopLeft quadrant also gives this node's leaf objects.
        /// </summary>
        /// <param name="index">The Quadrant's index.</param>
        public List<GDComp> GetQuadrant(Index index, List<GDComp> returnList)
        {
            if (index == Index.TopLeft)
            { //Add this node's leaves if the given index is TopLeft.
                returnList.AddRange(leaves);
            }

            if (nodes[0] != null)
            { //Get all other objects in the asked quadrant.
                nodes[(int)index].GetAll(returnList);
            }

            //Return the list.
            return returnList;
        }

        public void Insert(GDComp comp)
        {
            //Get the rectangle to work with.
            Rectangle rect = comp.GetRect();

            //Do we have any child node?
            if (nodes[0] != null)
            {
                int index = GetIndex(rect);

                //Does it fit completely in a child node?
                if (index != -1)
                {
                    //Insert it in a child node and return afterwards.
                    nodes[index].Insert(comp);
                    return;
                }
            }
        }

        /// <summary>
        /// Creates the 4 child nodes.
        /// </summary>
        private void Split()
        {
            //Calculate the half width and half height.
            int hw = (int)(Bounds.Width * 0.5f);
            int hh = (int)(Bounds.Height * 0.5f);

            int x = Bounds.X;
            int y = Bounds.Y;
            int newLevel = level + 1;

            //Create the 4 new nodes.
            nodes[(int)Index.TopLeft] = new QuadTree(newLevel, new Rectangle(
                x, y, hw, hh));
            nodes[(int)Index.BottomLeft] = new QuadTree(newLevel, new Rectangle(
                x, y + hh, hw, hh));
            nodes[(int)Index.BottomRight] = new QuadTree(newLevel, new Rectangle(
                x + hw, y + hh, hw, hh));
            nodes[(int)Index.TopRight] = new QuadTree(newLevel, new Rectangle(
                x + hw, y, hw, hh));
        }

        /// <summary>
        /// Update's a single quadrant.
        /// This method is Thread safe. The top node's leaves can only be reached with the TopLeft index.
        /// To use this method in a thread:
        /// Thread t = new Thread(o => UpdateQuadrant(gameTime, index);
        /// t.Start();
        /// </summary>
        /// <param name="gameTime">The game's timing mechanism.</param>
        /// <param name="index">The quadrant's index. TopLeft also takes the top node's leaves.</param>
        public void UpdateQuadrant(GameTime gameTime, Index index)
        {
            //Create the list and get all the leaves.
            List<GDComp> toUpdate = new List<GDComp>();
            GetQuadrant(index, toUpdate);

            //Update the leaves.
            for (int i = toUpdate.Count - 1; i >= 0; i--)
            {
                toUpdate[i].Update(gameTime);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameDesign_2.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_2
{
    /// <summary>
    /// A simple Quad Tree used for collision detection.
    /// Use Clear just before the collision checks followed by Insert on all objects.
    /// This way you keep the tree updated.
    /// 
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
        private const int MaxLevel = 5;
        private const int MaxLeaves = 1;
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
            if (nodes[0] != null)
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
            if (!(isInTop ^ isInBottom))
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
        /// Insert a component in the tree.
        /// </summary>
        /// <param name="comp"></param>
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

            //Object doesnt fit in a child, add it to this leaf.
            leaves.Add(comp);

            //Do we need to split?
            if (nodes[0] == null && level < MaxLevel && leaves.Count > MaxLeaves)
            {
                Split();

                int i = 0;

                while (i < leaves.Count)
                {
                    Rectangle tempRect = leaves[i].GetRect();
                    int index = GetIndex(tempRect);

                    //Does the object fit in a child node?
                    if (index != -1)
                    {
                        //Replace the object to the child node.
                        nodes[index].Insert(leaves[i]);
                        leaves.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a list of components into the tree.
        /// </summary>
        /// <param name="components">A list of components which implements IList</param>
        public void Insert(IList<IGameComponent> components)
        {
            foreach (GDComp comp in components)
            {
                Insert(comp);
            }
        }

        /// <summary>
        /// Get all possible colliders for a certain object.
        /// </summary>
        /// <param name="returnList">The list that will contain all the possible colliders.</param>
        /// <param name="rect">The source object's rectangle.</param>
        public List<GDComp> GetPossibleColliders(List<GDComp> returnList, Rectangle rect)
        {
            //First check child nodes.
            int index = GetIndex(rect);
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].GetPossibleColliders(returnList, rect);
            }

            //Now add this node's leaves.
            returnList.AddRange(leaves);

            return returnList;
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

        public void Draw(SpriteBatch batch, Color color, Texture2D tex)
        {
            if (nodes[0] != null)
            {
                for (int i = 0; i < Nodes; i++)
                {
                    if (i % 2 == 0)
                    {
                        nodes[i].Draw(batch, Color.Red, tex);
                    }
                    else
                    {
                        nodes[i].Draw(batch, Color.Black, tex);
                    }
                }
            }
            else
            {
                batch.Draw(tex, Bounds, new Color(color, 0.1f));
            }
        }

        public void Debug()
        {
            string toWrite = "";
            for (int i = 0; i < level; i++)
            {
                toWrite += "- ";
            }

            for (int i = 0; i < leaves.Count; i++)
            {
                if (leaves[i] is Components.Player.PlayerBall)
                {
                    toWrite += "Player, ";
                }
                else if (leaves[i] is Components.ScoreBall)
                {
                    toWrite += (leaves[i] as Components.ScoreBall).ColorT + ", ";
                }
            }

            Console.WriteLine(toWrite);

            if (nodes[0] != null)
            {
                for (int i = 0; i < Nodes; i++)
                {
                    nodes[i].Debug();
                }
            }
        }
    }
}

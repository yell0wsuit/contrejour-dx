using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.Seidel
{
    internal abstract class Node
    {
        protected Node LeftChild;

        public List<Node> ParentList;

        protected Node RightChild;

        protected Node(Node left, Node right)
        {
            ParentList = [];
            LeftChild = left;
            RightChild = right;
            left?.ParentList.Add(this);
            right?.ParentList.Add(this);
        }

        public abstract Sink Locate(Edge s);

        public void Replace(Node node)
        {
            foreach (Node parent in node.ParentList)
            {
                if (parent.LeftChild == node)
                {
                    parent.LeftChild = this;
                }
                else
                {
                    parent.RightChild = this;
                }
            }
            ParentList.AddRange(node.ParentList);
        }
    }
}

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Collections.QuadTree
{
    public class QuadTree<T>(Vector2 minLeafSize, int maxObjectsPerLeaf) where T : class, IQuadTreeObject<T>
    {
        private readonly Vector2 minLeafSize = minLeafSize;

        private readonly int maxObjectsPerLeaf = maxObjectsPerLeaf;

        public QuadTreeNode<T> Root { get; private set; }

        public void Add(T quadObject)
        {
            RectangleFloat bounds = quadObject.Bounds;
            if (Root == null)
            {
                Vector2 vector = new(MathF.Ceiling(bounds.Width / minLeafSize.X), MathF.Ceiling(bounds.Height / minLeafSize.Y));
                float num = Math.Max(vector.X, vector.Y);
                Vector2 vector2 = minLeafSize * num;
                Vector2 center = bounds.Center;
                Vector2 position = center - XnaMath.Divide(vector2, 2f);
                Root = new QuadTreeNode<T>(new RectangleFloat(position, vector2));
            }
            while (!Root.Bounds.Contains(bounds))
            {
                ExpandRoot(bounds);
            }
            InsertNodeObject(Root, quadObject);
        }

        public void Query(Vector2 point, Action<T> callback)
        {
            Query(new RectangleFloat(point, new Vector2(0f)), callback);
        }

        public void Query(RectangleFloat bounds, Action<T> callback)
        {
            if (Root != null)
            {
                Query(bounds, Root, callback);
            }
        }

        private static void Query(RectangleFloat bounds, QuadTreeNode<T> node, Action<T> callback)
        {
            if (node == null || !bounds.Intersects(node.Bounds))
            {
                return;
            }
            foreach (T @object in node.Objects)
            {
                T current = @object;
                if (bounds.Intersects(current.Bounds))
                {
                    callback(current);
                }
            }
            QuadTreeNode<T>[] nodes = node.Nodes;
            foreach (QuadTreeNode<T> node2 in nodes)
            {
                Query(bounds, node2, callback);
            }
        }

        private void ExpandRoot(RectangleFloat newChildBounds)
        {
            bool flag = Root.Bounds.Y < newChildBounds.Y;
            bool flag2 = Root.Bounds.X < newChildBounds.X;
            QuadDirection quadDirection = (!flag) ? (flag2 ? QuadDirection.SW : QuadDirection.SE) : ((!flag2) ? QuadDirection.NE : QuadDirection.NW);
            float x = (quadDirection is QuadDirection.NW or QuadDirection.SW) ? Root.Bounds.X : (Root.Bounds.X - Root.Bounds.Width);
            float y = (quadDirection is QuadDirection.NW or QuadDirection.NE) ? Root.Bounds.Y : (Root.Bounds.Y - Root.Bounds.Height);
            RectangleFloat bounds = new(x, y, Root.Bounds.Width * 2f, Root.Bounds.Height * 2f);
            QuadTreeNode<T> quadTreeNode = new(bounds);
            SetupChildNodes(quadTreeNode);
            quadTreeNode[quadDirection] = Root;
            Root = quadTreeNode;
        }

        private void InsertNodeObject(QuadTreeNode<T> node, T quadObject)
        {
            if (!node.Bounds.Contains(quadObject.Bounds))
            {
                throw new InvalidOperationException("This should not happen, child does not fit within node bounds");
            }
            if (!node.HasChildNodes() && node.Objects.Count + 1 > maxObjectsPerLeaf)
            {
                SetupChildNodes(node);
                List<T> list = [.. node.Objects];
                List<T> list2 = [];
                foreach (T item in list)
                {
                    T current = item;
                    QuadTreeNode<T>[] nodes = node.Nodes;
                    foreach (QuadTreeNode<T> quadTreeNode in nodes)
                    {
                        if (quadTreeNode != null && quadTreeNode.Bounds.Contains(current.Bounds))
                        {
                            list2.Add(current);
                        }
                    }
                }
                foreach (T item2 in list2)
                {
                    RemoveQuadObjectFromNode(item2);
                    InsertNodeObject(node, item2);
                }
            }
            QuadTreeNode<T>[] nodes2 = node.Nodes;
            foreach (QuadTreeNode<T> quadTreeNode2 in nodes2)
            {
                if (quadTreeNode2 != null && quadTreeNode2.Bounds.Contains(quadObject.Bounds))
                {
                    InsertNodeObject(quadTreeNode2, quadObject);
                    return;
                }
            }
            AddQuadObjectToNode(node, quadObject);
        }

        private void RemoveQuadObjectFromNode(T quadObject)
        {
            QuadTreeNode<T> node = quadObject.Node;
            _ = node.Objects.Remove(quadObject);
            quadObject.Node = null;
            quadObject.BoundsChanged -= OnQuadObjectBoundsChanged;
        }

        private void AddQuadObjectToNode(QuadTreeNode<T> node, T quadObject)
        {
            node.Objects.Add(quadObject);
            quadObject.Node = node;
            quadObject.BoundsChanged += OnQuadObjectBoundsChanged;
        }

        private void OnQuadObjectBoundsChanged(T sender)
        {
            T val = sender;
            if (val == null)
            {
                return;
            }
            QuadTreeNode<T> node = val.Node;
            if (!node.Bounds.Contains(val.Bounds) || node.HasChildNodes())
            {
                RemoveQuadObjectFromNode(val);
                Add(val);
                if (node.Parent != null)
                {
                    CheckChildNodes(node.Parent);
                }
            }
        }

        private void SetupChildNodes(QuadTreeNode<T> node)
        {
            if (minLeafSize.X <= node.Bounds.Width / 2f && minLeafSize.Y <= node.Bounds.Height / 2f)
            {
                node[QuadDirection.NW] = new QuadTreeNode<T>(node.Bounds.X, node.Bounds.Y, node.Bounds.Width / 2f, node.Bounds.Height / 2f);
                node[QuadDirection.NE] = new QuadTreeNode<T>(node.Bounds.X + (node.Bounds.Width / 2f), node.Bounds.Y, node.Bounds.Width / 2f, node.Bounds.Height / 2f);
                node[QuadDirection.SW] = new QuadTreeNode<T>(node.Bounds.X, node.Bounds.Y + (node.Bounds.Height / 2f), node.Bounds.Width / 2f, node.Bounds.Height / 2f);
                node[QuadDirection.SE] = new QuadTreeNode<T>(node.Bounds.X + (node.Bounds.Width / 2f), node.Bounds.Y + (node.Bounds.Height / 2f), node.Bounds.Width / 2f, node.Bounds.Height / 2f);
            }
        }

        public void Remove(T quadObject)
        {
            QuadTreeNode<T> node = quadObject.Node;
            RemoveQuadObjectFromNode(quadObject);
            if (node.Parent != null)
            {
                CheckChildNodes(node.Parent);
            }
        }

        private void CheckChildNodes(QuadTreeNode<T> node)
        {
            if (GetQuadObjectCount(node) > maxObjectsPerLeaf)
            {
                return;
            }
            List<T> childObjects = GetChildObjects(node);
            foreach (T item in childObjects)
            {
                if (!node.Objects.Contains(item))
                {
                    RemoveQuadObjectFromNode(item);
                    AddQuadObjectToNode(node, item);
                }
            }

            node[QuadDirection.NW]?.Parent = null;
            node[QuadDirection.NW] = null;
            node[QuadDirection.NE]?.Parent = null;
            node[QuadDirection.NE] = null;
            node[QuadDirection.SW]?.Parent = null;
            node[QuadDirection.SW] = null;
            node[QuadDirection.SE]?.Parent = null;
            node[QuadDirection.SE] = null;
            if (node.Parent != null)
            {
                CheckChildNodes(node.Parent);
                return;
            }
            int num = 0;
            QuadTreeNode<T> quadTreeNode = null;
            QuadTreeNode<T>[] nodes = node.Nodes;
            foreach (QuadTreeNode<T> quadTreeNode2 in nodes)
            {
                if (quadTreeNode2 != null && GetQuadObjectCount(quadTreeNode2) > 0)
                {
                    num++;
                    quadTreeNode = quadTreeNode2;
                    if (num > 1)
                    {
                        break;
                    }
                }
            }
            if (num != 1)
            {
                return;
            }
            QuadTreeNode<T>[] nodes2 = node.Nodes;
            foreach (QuadTreeNode<T> quadTreeNode3 in nodes2)
            {
                if (quadTreeNode3 != quadTreeNode)
                {
                    quadTreeNode3.Parent = null;
                }
            }
            Root = quadTreeNode;
        }

        private static List<T> GetChildObjects(QuadTreeNode<T> node)
        {
            List<T> list = [.. node.Objects];
            QuadTreeNode<T>[] nodes = node.Nodes;
            foreach (QuadTreeNode<T> quadTreeNode in nodes)
            {
                if (quadTreeNode != null)
                {
                    list.AddRange(GetChildObjects(quadTreeNode));
                }
            }
            return list;
        }

        public int GetQuadObjectCount()
        {
            return Root == null ? 0 : GetQuadObjectCount(Root);
        }

        private static int GetQuadObjectCount(QuadTreeNode<T> node)
        {
            int num = node.Objects.Count;
            QuadTreeNode<T>[] nodes = node.Nodes;
            foreach (QuadTreeNode<T> quadTreeNode in nodes)
            {
                if (quadTreeNode != null)
                {
                    num += GetQuadObjectCount(quadTreeNode);
                }
            }
            return num;
        }

        public int GetQuadNodeCount()
        {
            return Root == null ? 0 : GetQuadNodeCount(Root, 1);
        }

        private static int GetQuadNodeCount(QuadTreeNode<T> node, int count)
        {
            if (node == null)
            {
                return count;
            }
            QuadTreeNode<T>[] nodes = node.Nodes;
            foreach (QuadTreeNode<T> quadTreeNode in nodes)
            {
                if (quadTreeNode != null)
                {
                    count++;
                }
            }
            return count;
        }

        public List<QuadTreeNode<T>> GetAllNodes()
        {
            List<QuadTreeNode<T>> list = [];
            if (Root != null)
            {
                list.Add(Root);
                GetChildNodes(Root, list);
            }
            return list;
        }

        private static void GetChildNodes(QuadTreeNode<T> node, ICollection<QuadTreeNode<T>> results)
        {
            QuadTreeNode<T>[] nodes = node.Nodes;
            foreach (QuadTreeNode<T> quadTreeNode in nodes)
            {
                if (quadTreeNode != null)
                {
                    results.Add(quadTreeNode);
                    GetChildNodes(quadTreeNode, results);
                }
            }
        }
    }
}

using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.Seidel
{
    internal sealed class QueryGraph(Node head)
    {
        private Node _head = head;

        private Trapezoid Locate(Edge edge)
        {
            return _head.Locate(edge).Trapezoid;
        }

        public List<Trapezoid> FollowEdge(Edge edge)
        {
            List<Trapezoid> list = [Locate(edge)];
            for (int i = 0; edge.Q.X > list[i].RightPoint.X; i++)
            {
                if (edge.IsAbove(list[i].RightPoint))
                {
                    list.Add(list[i].UpperRight);
                }
                else
                {
                    list.Add(list[i].LowerRight);
                }
            }
            return list;
        }

        private void Replace(Sink sink, Node node)
        {
            if (sink.ParentList.Count == 0)
            {
                _head = node;
            }
            else
            {
                node.Replace(sink);
            }
        }

        public void Case1(Sink sink, Edge edge, Trapezoid[] tList)
        {
            YNode lChild = new(edge, Sink.Isink(tList[1]), Sink.Isink(tList[2]));
            XNode rChild = new(edge.Q, lChild, Sink.Isink(tList[3]));
            XNode node = new(edge.P, Sink.Isink(tList[0]), rChild);
            Replace(sink, node);
        }

        public void Case2(Sink sink, Edge edge, Trapezoid[] tList)
        {
            YNode rChild = new(edge, Sink.Isink(tList[1]), Sink.Isink(tList[2]));
            XNode node = new(edge.P, Sink.Isink(tList[0]), rChild);
            Replace(sink, node);
        }

        public void Case3(Sink sink, Edge edge, Trapezoid[] tList)
        {
            YNode node = new(edge, Sink.Isink(tList[0]), Sink.Isink(tList[1]));
            Replace(sink, node);
        }

        public void Case4(Sink sink, Edge edge, Trapezoid[] tList)
        {
            YNode lChild = new(edge, Sink.Isink(tList[0]), Sink.Isink(tList[1]));
            XNode node = new(edge.Q, lChild, Sink.Isink(tList[2]));
            Replace(sink, node);
        }
    }
}

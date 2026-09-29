using System;
using System.Collections.Generic;

using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision
{
    public class DynamicTree<T>
    {
        internal const int NullNode = -1;

        private readonly Stack<int> _raycastStack = new(256);

        private readonly Stack<int> _queryStack = new(256);

        private int _freeList;

        private int _nodeCapacity;

        private int _nodeCount;

        private TreeNode<T>[] _nodes;

        private int _root;

        public int Height => _root == -1 ? 0 : _nodes[_root].Height;

        public float AreaRatio
        {
            get
            {
                if (_root == -1)
                {
                    return 0f;
                }
                TreeNode<T> treeNode = _nodes[_root];
                float perimeter = treeNode.AABB.Perimeter;
                float num = 0f;
                for (int i = 0; i < _nodeCapacity; i++)
                {
                    TreeNode<T> treeNode2 = _nodes[i];
                    if (treeNode2.Height >= 0)
                    {
                        num += treeNode2.AABB.Perimeter;
                    }
                }
                return num / perimeter;
            }
        }

        public int MaxBalance
        {
            get
            {
                int num = 0;
                for (int i = 0; i < _nodeCapacity; i++)
                {
                    TreeNode<T> treeNode = _nodes[i];
                    if (treeNode.Height > 1)
                    {
                        int child = treeNode.Child1;
                        int child2 = treeNode.Child2;
                        int val = Math.Abs(_nodes[child2].Height - _nodes[child].Height);
                        num = Math.Max(num, val);
                    }
                }
                return num;
            }
        }

        public DynamicTree()
        {
            _root = -1;
            _nodeCapacity = 16;
            _nodeCount = 0;
            _nodes = new TreeNode<T>[_nodeCapacity];
            for (int i = 0; i < _nodeCapacity - 1; i++)
            {
                _nodes[i] = new TreeNode<T>
                {
                    ParentOrNext = i + 1,
                    Height = 1
                };
            }
            _nodes[_nodeCapacity - 1] = new TreeNode<T>
            {
                ParentOrNext = -1,
                Height = 1
            };
            _freeList = 0;
        }

        public int AddProxy(ref AABB aabb, T userData)
        {
            int num = AllocateNode();
            Vector2 vector = new(0.1f, 0.1f);
            _nodes[num].AABB.LowerBound = aabb.LowerBound - vector;
            _nodes[num].AABB.UpperBound = aabb.UpperBound + vector;
            _nodes[num].UserData = userData;
            _nodes[num].Height = 0;
            InsertLeaf(num);
            return num;
        }

        public void RemoveProxy(int proxyId)
        {
            RemoveLeaf(proxyId);
            FreeNode(proxyId);
        }

        public bool MoveProxy(int proxyId, ref AABB aabb, Vector2 displacement)
        {
            if (_nodes[proxyId].AABB.Contains(ref aabb))
            {
                return false;
            }
            RemoveLeaf(proxyId);
            AABB aABB = aabb;
            Vector2 vector = new(0.1f, 0.1f);
            aABB.LowerBound -= vector;
            aABB.UpperBound += vector;
            Vector2 vector2 = 2f * displacement;
            if (vector2.X < 0f)
            {
                aABB.LowerBound.X += vector2.X;
            }
            else
            {
                aABB.UpperBound.X += vector2.X;
            }
            if (vector2.Y < 0f)
            {
                aABB.LowerBound.Y += vector2.Y;
            }
            else
            {
                aABB.UpperBound.Y += vector2.Y;
            }
            _nodes[proxyId].AABB = aABB;
            InsertLeaf(proxyId);
            return true;
        }

        public T GetUserData(int proxyId)
        {
            return _nodes[proxyId].UserData;
        }

        public void GetFatAABB(int proxyId, out AABB fatAABB)
        {
            fatAABB = _nodes[proxyId].AABB;
        }

        public void Query(Func<int, bool> callback, ref AABB aabb)
        {
            _queryStack.Clear();
            _queryStack.Push(_root);
            while (_queryStack.Count > 0)
            {
                int num = _queryStack.Pop();
                if (num == -1)
                {
                    continue;
                }
                TreeNode<T> treeNode = _nodes[num];
                if (!AABB.TestOverlap(ref treeNode.AABB, ref aabb))
                {
                    continue;
                }
                if (treeNode.IsLeaf())
                {
                    if (!callback(num))
                    {
                        break;
                    }
                }
                else
                {
                    _queryStack.Push(treeNode.Child1);
                    _queryStack.Push(treeNode.Child2);
                }
            }
        }

        public void RayCast(Func<RayCastInput, int, float> callback, ref RayCastInput input)
        {
            Vector2 value = input.Point1;
            Vector2 point = input.Point2;
            Vector2 vector = point - value;
            vector.Normalize();
            Vector2 value2 = MathUtils.Abs(new Vector2(0f - vector.Y, vector.X));
            float num = input.MaxFraction;
            AABB b = default;
            Vector2 value3 = value + (num * (point - value));
            Vector2.Min(ref value, ref value3, out b.LowerBound);
            Vector2.Max(ref value, ref value3, out b.UpperBound);
            _raycastStack.Clear();
            _raycastStack.Push(_root);
            RayCastInput arg = default;
            while (_raycastStack.Count > 0)
            {
                int num2 = _raycastStack.Pop();
                if (num2 == -1)
                {
                    continue;
                }
                TreeNode<T> treeNode = _nodes[num2];
                if (!AABB.TestOverlap(ref treeNode.AABB, ref b))
                {
                    continue;
                }
                Vector2 center = treeNode.AABB.Center;
                Vector2 extents = treeNode.AABB.Extents;
                float num3 = Math.Abs(Vector2.Dot(new Vector2(0f - vector.Y, vector.X), value - center)) - Vector2.Dot(value2, extents);
                if (num3 > 0f)
                {
                    continue;
                }
                if (treeNode.IsLeaf())
                {
                    arg.Point1 = input.Point1;
                    arg.Point2 = input.Point2;
                    arg.MaxFraction = num;
                    float num4 = callback(arg, num2);
                    if (num4 == 0f)
                    {
                        break;
                    }
                    if (num4 > 0f)
                    {
                        num = num4;
                        Vector2 value4 = value + (num * (point - value));
                        b.LowerBound = Vector2.Min(value, value4);
                        b.UpperBound = Vector2.Max(value, value4);
                    }
                }
                else
                {
                    _raycastStack.Push(treeNode.Child1);
                    _raycastStack.Push(treeNode.Child2);
                }
            }
        }

        private int AllocateNode()
        {
            if (_freeList == -1)
            {
                TreeNode<T>[] nodes = _nodes;
                _nodeCapacity *= 2;
                _nodes = new TreeNode<T>[_nodeCapacity];
                Array.Copy(nodes, _nodes, _nodeCount);
                for (int i = _nodeCount; i < _nodeCapacity - 1; i++)
                {
                    _nodes[i] = new TreeNode<T>
                    {
                        ParentOrNext = i + 1,
                        Height = -1
                    };
                }
                _nodes[_nodeCapacity - 1] = new TreeNode<T>
                {
                    ParentOrNext = -1,
                    Height = -1
                };
                _freeList = _nodeCount;
            }
            int freeList = _freeList;
            _freeList = _nodes[freeList].ParentOrNext;
            _nodes[freeList].ParentOrNext = -1;
            _nodes[freeList].Child1 = -1;
            _nodes[freeList].Child2 = -1;
            _nodes[freeList].Height = 0;
            _nodes[freeList].UserData = default;
            _nodeCount++;
            return freeList;
        }

        private void FreeNode(int nodeId)
        {
            _nodes[nodeId].ParentOrNext = _freeList;
            _nodes[nodeId].Height = -1;
            _freeList = nodeId;
            _nodeCount--;
        }

        private void InsertLeaf(int leaf)
        {
            if (_root == -1)
            {
                _root = leaf;
                _nodes[_root].ParentOrNext = -1;
                return;
            }
            AABB aabb = _nodes[leaf].AABB;
            int num = _root;
            while (!_nodes[num].IsLeaf())
            {
                int child = _nodes[num].Child1;
                int child2 = _nodes[num].Child2;
                float perimeter = _nodes[num].AABB.Perimeter;
                AABB aABB = default;
                aABB.Combine(ref _nodes[num].AABB, ref aabb);
                float perimeter2 = aABB.Perimeter;
                float num2 = 2f * perimeter2;
                float num3 = 2f * (perimeter2 - perimeter);
                float num4;
                if (_nodes[child].IsLeaf())
                {
                    AABB aABB2 = default;
                    aABB2.Combine(ref aabb, ref _nodes[child].AABB);
                    num4 = aABB2.Perimeter + num3;
                }
                else
                {
                    AABB aABB3 = default;
                    aABB3.Combine(ref aabb, ref _nodes[child].AABB);
                    float perimeter3 = _nodes[child].AABB.Perimeter;
                    float perimeter4 = aABB3.Perimeter;
                    num4 = perimeter4 - perimeter3 + num3;
                }
                float num5;
                if (_nodes[child2].IsLeaf())
                {
                    AABB aABB4 = default;
                    aABB4.Combine(ref aabb, ref _nodes[child2].AABB);
                    num5 = aABB4.Perimeter + num3;
                }
                else
                {
                    AABB aABB5 = default;
                    aABB5.Combine(ref aabb, ref _nodes[child2].AABB);
                    float perimeter5 = _nodes[child2].AABB.Perimeter;
                    float perimeter6 = aABB5.Perimeter;
                    num5 = perimeter6 - perimeter5 + num3;
                }
                if (num2 < num4 && num4 < num5)
                {
                    break;
                }
                num = (!(num4 < num5)) ? child2 : child;
            }
            int num6 = num;
            int parentOrNext = _nodes[num6].ParentOrNext;
            int num7 = AllocateNode();
            _nodes[num7].ParentOrNext = parentOrNext;
            _nodes[num7].UserData = default;
            _nodes[num7].AABB.Combine(ref aabb, ref _nodes[num6].AABB);
            _nodes[num7].Height = _nodes[num6].Height + 1;
            if (parentOrNext != -1)
            {
                if (_nodes[parentOrNext].Child1 == num6)
                {
                    _nodes[parentOrNext].Child1 = num7;
                }
                else
                {
                    _nodes[parentOrNext].Child2 = num7;
                }
                _nodes[num7].Child1 = num6;
                _nodes[num7].Child2 = leaf;
                _nodes[num6].ParentOrNext = num7;
                _nodes[leaf].ParentOrNext = num7;
            }
            else
            {
                _nodes[num7].Child1 = num6;
                _nodes[num7].Child2 = leaf;
                _nodes[num6].ParentOrNext = num7;
                _nodes[leaf].ParentOrNext = num7;
                _root = num7;
            }
            for (num = _nodes[leaf].ParentOrNext; num != -1; num = _nodes[num].ParentOrNext)
            {
                num = Balance(num);
                int child3 = _nodes[num].Child1;
                int child4 = _nodes[num].Child2;
                _nodes[num].Height = 1 + Math.Max(_nodes[child3].Height, _nodes[child4].Height);
                _nodes[num].AABB.Combine(ref _nodes[child3].AABB, ref _nodes[child4].AABB);
            }
        }

        private void RemoveLeaf(int leaf)
        {
            if (leaf == _root)
            {
                _root = -1;
                return;
            }
            int parentOrNext = _nodes[leaf].ParentOrNext;
            int parentOrNext2 = _nodes[parentOrNext].ParentOrNext;
            int num = (_nodes[parentOrNext].Child1 != leaf) ? _nodes[parentOrNext].Child1 : _nodes[parentOrNext].Child2;
            if (parentOrNext2 != -1)
            {
                if (_nodes[parentOrNext2].Child1 == parentOrNext)
                {
                    _nodes[parentOrNext2].Child1 = num;
                }
                else
                {
                    _nodes[parentOrNext2].Child2 = num;
                }
                _nodes[num].ParentOrNext = parentOrNext2;
                FreeNode(parentOrNext);
                int num2;
                for (num2 = parentOrNext2; num2 != -1; num2 = _nodes[num2].ParentOrNext)
                {
                    num2 = Balance(num2);
                    int child = _nodes[num2].Child1;
                    int child2 = _nodes[num2].Child2;
                    _nodes[num2].AABB.Combine(ref _nodes[child].AABB, ref _nodes[child2].AABB);
                    _nodes[num2].Height = 1 + Math.Max(_nodes[child].Height, _nodes[child2].Height);
                }
            }
            else
            {
                _root = num;
                _nodes[num].ParentOrNext = -1;
                FreeNode(parentOrNext);
            }
        }

        private int Balance(int iA)
        {
            TreeNode<T> treeNode = _nodes[iA];
            if (treeNode.IsLeaf() || treeNode.Height < 2)
            {
                return iA;
            }
            int child = treeNode.Child1;
            int child2 = treeNode.Child2;
            TreeNode<T> treeNode2 = _nodes[child];
            TreeNode<T> treeNode3 = _nodes[child2];
            int num = treeNode3.Height - treeNode2.Height;
            if (num > 1)
            {
                int child3 = treeNode3.Child1;
                int child4 = treeNode3.Child2;
                TreeNode<T> treeNode4 = _nodes[child3];
                TreeNode<T> treeNode5 = _nodes[child4];
                treeNode3.Child1 = iA;
                treeNode3.ParentOrNext = treeNode.ParentOrNext;
                treeNode.ParentOrNext = child2;
                if (treeNode3.ParentOrNext != -1)
                {
                    if (_nodes[treeNode3.ParentOrNext].Child1 == iA)
                    {
                        _nodes[treeNode3.ParentOrNext].Child1 = child2;
                    }
                    else
                    {
                        _nodes[treeNode3.ParentOrNext].Child2 = child2;
                    }
                }
                else
                {
                    _root = child2;
                }
                if (treeNode4.Height > treeNode5.Height)
                {
                    treeNode3.Child2 = child3;
                    treeNode.Child2 = child4;
                    treeNode5.ParentOrNext = iA;
                    treeNode.AABB.Combine(ref treeNode2.AABB, ref treeNode5.AABB);
                    treeNode3.AABB.Combine(ref treeNode.AABB, ref treeNode4.AABB);
                    treeNode.Height = 1 + Math.Max(treeNode2.Height, treeNode5.Height);
                    treeNode3.Height = 1 + Math.Max(treeNode.Height, treeNode4.Height);
                }
                else
                {
                    treeNode3.Child2 = child4;
                    treeNode.Child2 = child3;
                    treeNode4.ParentOrNext = iA;
                    treeNode.AABB.Combine(ref treeNode2.AABB, ref treeNode4.AABB);
                    treeNode3.AABB.Combine(ref treeNode.AABB, ref treeNode5.AABB);
                    treeNode.Height = 1 + Math.Max(treeNode2.Height, treeNode4.Height);
                    treeNode3.Height = 1 + Math.Max(treeNode.Height, treeNode5.Height);
                }
                return child2;
            }
            if (num < -1)
            {
                int child5 = treeNode2.Child1;
                int child6 = treeNode2.Child2;
                TreeNode<T> treeNode6 = _nodes[child5];
                TreeNode<T> treeNode7 = _nodes[child6];
                treeNode2.Child1 = iA;
                treeNode2.ParentOrNext = treeNode.ParentOrNext;
                treeNode.ParentOrNext = child;
                if (treeNode2.ParentOrNext != -1)
                {
                    if (_nodes[treeNode2.ParentOrNext].Child1 == iA)
                    {
                        _nodes[treeNode2.ParentOrNext].Child1 = child;
                    }
                    else
                    {
                        _nodes[treeNode2.ParentOrNext].Child2 = child;
                    }
                }
                else
                {
                    _root = child;
                }
                if (treeNode6.Height > treeNode7.Height)
                {
                    treeNode2.Child2 = child5;
                    treeNode.Child1 = child6;
                    treeNode7.ParentOrNext = iA;
                    treeNode.AABB.Combine(ref treeNode3.AABB, ref treeNode7.AABB);
                    treeNode2.AABB.Combine(ref treeNode.AABB, ref treeNode6.AABB);
                    treeNode.Height = 1 + Math.Max(treeNode3.Height, treeNode7.Height);
                    treeNode2.Height = 1 + Math.Max(treeNode.Height, treeNode6.Height);
                }
                else
                {
                    treeNode2.Child2 = child6;
                    treeNode.Child1 = child5;
                    treeNode6.ParentOrNext = iA;
                    treeNode.AABB.Combine(ref treeNode3.AABB, ref treeNode6.AABB);
                    treeNode2.AABB.Combine(ref treeNode.AABB, ref treeNode7.AABB);
                    treeNode.Height = 1 + Math.Max(treeNode3.Height, treeNode6.Height);
                    treeNode2.Height = 1 + Math.Max(treeNode.Height, treeNode7.Height);
                }
                return child;
            }
            return iA;
        }

        public int ComputeHeight(int nodeId)
        {
            TreeNode<T> treeNode = _nodes[nodeId];
            if (treeNode.IsLeaf())
            {
                return 0;
            }
            int val = ComputeHeight(treeNode.Child1);
            int val2 = ComputeHeight(treeNode.Child2);
            return 1 + Math.Max(val, val2);
        }

        public int ComputeHeight()
        {
            return ComputeHeight(_root);
        }

        public void ValidateStructure(int index)
        {
            if (index != -1)
            {
                _ = _root;
                TreeNode<T> treeNode = _nodes[index];
                int child = treeNode.Child1;
                int child2 = treeNode.Child2;
                if (!treeNode.IsLeaf())
                {
                    ValidateStructure(child);
                    ValidateStructure(child2);
                }
            }
        }

        public void ValidateMetrics(int index)
        {
            if (index != -1)
            {
                TreeNode<T> treeNode = _nodes[index];
                int child = treeNode.Child1;
                int child2 = treeNode.Child2;
                if (!treeNode.IsLeaf())
                {
                    int height = _nodes[child].Height;
                    int height2 = _nodes[child2].Height;
                    _ = Math.Max(height, height2);
                    default(AABB).Combine(ref _nodes[child].AABB, ref _nodes[child2].AABB);
                    ValidateMetrics(child);
                    ValidateMetrics(child2);
                }
            }
        }

        public void Validate()
        {
            ValidateStructure(_root);
            ValidateMetrics(_root);
            int num = 0;
            int num2 = _freeList;
            while (num2 != -1)
            {
                num2 = _nodes[num2].ParentOrNext;
                num++;
            }
        }

        public void RebuildBottomUp()
        {
            int[] array = new int[_nodeCount];
            int num = 0;
            for (int i = 0; i < _nodeCapacity; i++)
            {
                if (_nodes[i].Height >= 0)
                {
                    if (_nodes[i].IsLeaf())
                    {
                        _nodes[i].ParentOrNext = -1;
                        array[num] = i;
                        num++;
                    }
                    else
                    {
                        FreeNode(i);
                    }
                }
            }
            while (num > 1)
            {
                float num2 = float.MaxValue;
                int num3 = -1;
                int num4 = -1;
                for (int j = 0; j < num; j++)
                {
                    AABB aabb = _nodes[array[j]].AABB;
                    for (int k = j + 1; k < num; k++)
                    {
                        AABB aabb2 = _nodes[array[k]].AABB;
                        AABB aABB = default;
                        aABB.Combine(ref aabb, ref aabb2);
                        float perimeter = aABB.Perimeter;
                        if (perimeter < num2)
                        {
                            num3 = j;
                            num4 = k;
                            num2 = perimeter;
                        }
                    }
                }
                int num5 = array[num3];
                int num6 = array[num4];
                TreeNode<T> treeNode = _nodes[num5];
                TreeNode<T> treeNode2 = _nodes[num6];
                int num7 = AllocateNode();
                TreeNode<T> treeNode3 = _nodes[num7];
                treeNode3.Child1 = num5;
                treeNode3.Child2 = num6;
                treeNode3.Height = 1 + Math.Max(treeNode.Height, treeNode2.Height);
                treeNode3.AABB.Combine(ref treeNode.AABB, ref treeNode2.AABB);
                treeNode3.ParentOrNext = -1;
                treeNode.ParentOrNext = num7;
                treeNode2.ParentOrNext = num7;
                array[num4] = array[num - 1];
                array[num3] = num7;
                num--;
            }
            _root = array[0];
            Validate();
        }

        public void ShiftOrigin(Vector2 newOrigin)
        {
            for (int i = 0; i < _nodeCapacity; i++)
            {
                _nodes[i].AABB.LowerBound -= newOrigin;
                _nodes[i].AABB.UpperBound -= newOrigin;
            }
        }
    }
}

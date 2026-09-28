using System;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public class DynamicTreeBroadPhase : IBroadPhase
{
    private int[] _moveBuffer;

    private int _moveCapacity;

    private int _moveCount;

    private Pair[] _pairBuffer;

    private int _pairCapacity;

    private int _pairCount;

    private int _proxyCount;

    private readonly Func<int, bool> _queryCallback;

    private int _queryProxyId;

    private readonly DynamicTree<FixtureProxy> _tree = new();

    public int ProxyCount => _proxyCount;

    public float TreeQuality => _tree.AreaRatio;

    public int TreeBalance => _tree.MaxBalance;

    public int TreeHeight => _tree.Height;

    public DynamicTreeBroadPhase()
    {
        _queryCallback = QueryCallback;
        _proxyCount = 0;
        _pairCapacity = 16;
        _pairCount = 0;
        _pairBuffer = new Pair[_pairCapacity];
        _moveCapacity = 16;
        _moveCount = 0;
        _moveBuffer = new int[_moveCapacity];
    }

    public int AddProxy(ref FixtureProxy proxy)
    {
        int num = _tree.AddProxy(ref proxy.AABB, proxy);
        _proxyCount++;
        BufferMove(num);
        return num;
    }

    public void RemoveProxy(int proxyId)
    {
        UnBufferMove(proxyId);
        _proxyCount--;
        _tree.RemoveProxy(proxyId);
    }

    public void MoveProxy(int proxyId, ref AABB aabb, Vector2 displacement)
    {
        if (_tree.MoveProxy(proxyId, ref aabb, displacement))
        {
            BufferMove(proxyId);
        }
    }

    public void TouchProxy(int proxyId)
    {
        BufferMove(proxyId);
    }

    private void BufferMove(int proxyId)
    {
        if (_moveCount == _moveCapacity)
        {
            int[] moveBuffer = _moveBuffer;
            _moveCapacity *= 2;
            _moveBuffer = new int[_moveCapacity];
            Array.Copy(moveBuffer, _moveBuffer, _moveCount);
        }
        _moveBuffer[_moveCount] = proxyId;
        _moveCount++;
    }

    private void UnBufferMove(int proxyId)
    {
        for (int i = 0; i < _moveCount; i++)
        {
            if (_moveBuffer[i] == proxyId)
            {
                _moveBuffer[i] = -1;
            }
        }
    }

    private bool QueryCallback(int proxyId)
    {
        if (proxyId == _queryProxyId)
        {
            return true;
        }
        if (_pairCount == _pairCapacity)
        {
            Pair[] pairBuffer = _pairBuffer;
            _pairCapacity *= 2;
            _pairBuffer = new Pair[_pairCapacity];
            Array.Copy(pairBuffer, _pairBuffer, _pairCount);
        }
        _pairBuffer[_pairCount].ProxyIdA = Math.Min(proxyId, _queryProxyId);
        _pairBuffer[_pairCount].ProxyIdB = Math.Max(proxyId, _queryProxyId);
        _pairCount++;
        return true;
    }

    public void GetFatAABB(int proxyId, out AABB aabb)
    {
        _tree.GetFatAABB(proxyId, out aabb);
    }

    public FixtureProxy GetProxy(int proxyId)
    {
        return _tree.GetUserData(proxyId);
    }

    public bool TestOverlap(int proxyIdA, int proxyIdB)
    {
        _tree.GetFatAABB(proxyIdA, out AABB fatAABB);
        _tree.GetFatAABB(proxyIdB, out AABB fatAABB2);
        return AABB.TestOverlap(ref fatAABB, ref fatAABB2);
    }

    public void UpdatePairs(BroadphaseHandler callback)
    {
        _pairCount = 0;
        for (int i = 0; i < _moveCount; i++)
        {
            _queryProxyId = _moveBuffer[i];
            if (_queryProxyId != -1)
            {
                _tree.GetFatAABB(_queryProxyId, out AABB fatAABB);
                _tree.Query(_queryCallback, ref fatAABB);
            }
        }
        _moveCount = 0;
        Array.Sort(_pairBuffer, 0, _pairCount);
        int j = 0;
        while (j < _pairCount)
        {
            Pair pair = _pairBuffer[j];
            FixtureProxy proxyA = _tree.GetUserData(pair.ProxyIdA);
            FixtureProxy proxyB = _tree.GetUserData(pair.ProxyIdB);
            callback(ref proxyA, ref proxyB);
            for (j++; j < _pairCount; j++)
            {
                Pair pair2 = _pairBuffer[j];
                if (pair2.ProxyIdA != pair.ProxyIdA || pair2.ProxyIdB != pair.ProxyIdB)
                {
                    break;
                }
            }
        }
    }

    public void Query(Func<int, bool> callback, ref AABB aabb)
    {
        _tree.Query(callback, ref aabb);
    }

    public void RayCast(Func<RayCastInput, int, float> callback, ref RayCastInput input)
    {
        _tree.RayCast(callback, ref input);
    }

    public void ShiftOrigin(Vector2 newOrigin)
    {
        _tree.ShiftOrigin(newOrigin);
    }
}

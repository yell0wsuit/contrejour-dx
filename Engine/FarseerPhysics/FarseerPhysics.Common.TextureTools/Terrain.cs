using System.Collections.Generic;

using FarseerPhysics.Collision;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.TextureTools;

public class Terrain
{
    public World World;

    public Vector2 Center;

    public float Width;

    public float Height;

    public int PointsPerUnit;

    public int CellSize;

    public int SubCellSize;

    public int Iterations = 2;

    public TriangulationAlgorithm Decomposer;

    private sbyte[,] _terrainMap;

    private List<Body>[,] _bodyMap;

    private float _localWidth;

    private float _localHeight;

    private int _xnum;

    private int _ynum;

    private AABB _dirtyArea;

    private Vector2 _topLeft;

    public Terrain(World world, AABB area)
    {
        World = world;
        Width = area.Width;
        Height = area.Height;
        Center = area.Center;
    }

    public Terrain(World world, Vector2 position, float width, float height)
    {
        World = world;
        Width = width;
        Height = height;
        Center = position;
    }

    public void Initialize()
    {
        _topLeft = new Vector2(Center.X - Width * 0.5f, Center.Y - (0f - Height) * 0.5f);
        _localWidth = Width * (float)PointsPerUnit;
        _localHeight = Height * (float)PointsPerUnit;
        _terrainMap = new sbyte[(int)_localWidth + 1, (int)_localHeight + 1];
        for (int i = 0; (float)i < _localWidth; i++)
        {
            for (int j = 0; (float)j < _localHeight; j++)
            {
                _terrainMap[i, j] = 1;
            }
        }
        _xnum = (int)(_localWidth / (float)CellSize);
        _ynum = (int)(_localHeight / (float)CellSize);
        _bodyMap = new List<Body>[_xnum, _ynum];
        _dirtyArea = new AABB(new Vector2(float.MaxValue, float.MaxValue), new Vector2(float.MinValue, float.MinValue));
    }

    public void ApplyData(sbyte[,] data, Vector2 offset = default(Vector2))
    {
        for (int i = 0; i < data.GetUpperBound(0); i++)
        {
            for (int j = 0; j < data.GetUpperBound(1); j++)
            {
                if ((float)i + offset.X >= 0f && (float)i + offset.X < _localWidth && (float)j + offset.Y >= 0f && (float)j + offset.Y < _localHeight)
                {
                    _terrainMap[(int)((float)i + offset.X), (int)((float)j + offset.Y)] = data[i, j];
                }
            }
        }
        RemoveOldData(0, _xnum, 0, _ynum);
    }

    public void ModifyTerrain(Vector2 location, sbyte value)
    {
        Vector2 vector = location - _topLeft;
        vector.X = vector.X * _localWidth / Width;
        vector.Y = vector.Y * (0f - _localHeight) / Height;
        if (vector.X >= 0f && vector.X < _localWidth && vector.Y >= 0f && vector.Y < _localHeight)
        {
            _terrainMap[(int)vector.X, (int)vector.Y] = value;
            if (vector.X < _dirtyArea.LowerBound.X)
            {
                _dirtyArea.LowerBound.X = vector.X;
            }
            if (vector.X > _dirtyArea.UpperBound.X)
            {
                _dirtyArea.UpperBound.X = vector.X;
            }
            if (vector.Y < _dirtyArea.LowerBound.Y)
            {
                _dirtyArea.LowerBound.Y = vector.Y;
            }
            if (vector.Y > _dirtyArea.UpperBound.Y)
            {
                _dirtyArea.UpperBound.Y = vector.Y;
            }
        }
    }

    public void RegenerateTerrain()
    {
        int num = (int)(_dirtyArea.LowerBound.X / (float)CellSize);
        if (num < 0)
        {
            num = 0;
        }
        int num2 = (int)(_dirtyArea.UpperBound.X / (float)CellSize) + 1;
        if (num2 > _xnum)
        {
            num2 = _xnum;
        }
        int num3 = (int)(_dirtyArea.LowerBound.Y / (float)CellSize);
        if (num3 < 0)
        {
            num3 = 0;
        }
        int num4 = (int)(_dirtyArea.UpperBound.Y / (float)CellSize) + 1;
        if (num4 > _ynum)
        {
            num4 = _ynum;
        }
        RemoveOldData(num, num2, num3, num4);
        _dirtyArea = new AABB(new Vector2(float.MaxValue, float.MaxValue), new Vector2(float.MinValue, float.MinValue));
    }

    private void RemoveOldData(int xStart, int xEnd, int yStart, int yEnd)
    {
        for (int i = xStart; i < xEnd; i++)
        {
            for (int j = yStart; j < yEnd; j++)
            {
                if (_bodyMap[i, j] != null)
                {
                    for (int k = 0; k < _bodyMap[i, j].Count; k++)
                    {
                        World.RemoveBody(_bodyMap[i, j][k]);
                    }
                }
                _bodyMap[i, j] = null;
                GenerateTerrain(i, j);
            }
        }
    }

    private void GenerateTerrain(int gx, int gy)
    {
        float num = gx * CellSize;
        float num2 = gy * CellSize;
        List<Vertices> list = MarchingSquares.DetectSquares(new AABB(new Vector2(num, num2), new Vector2(num + (float)CellSize, num2 + (float)CellSize)), SubCellSize, SubCellSize, _terrainMap, Iterations, combine: true);
        if (list.Count == 0)
        {
            return;
        }
        _bodyMap[gx, gy] = new List<Body>();
        Vector2 value = new Vector2(1f / (float)PointsPerUnit, 1f / (float)(-PointsPerUnit));
        foreach (Vertices item in list)
        {
            item.Scale(ref value);
            item.Translate(ref _topLeft);
            Vertices vertices = SimplifyTools.CollinearSimplify(item);
            List<Vertices> list2 = Triangulate.ConvexPartition(vertices, Decomposer);
            foreach (Vertices item2 in list2)
            {
                if (item2.Count > 2)
                {
                    _bodyMap[gx, gy].Add(BodyFactory.CreatePolygon(World, item2, 1f));
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.TextureTools;

public sealed class TextureConverter
{
    private const int ClosepixelsLength = 8;

    private static int[,] _closePixels = new int[8, 2]
    {
        { -1, -1 },
        { 0, -1 },
        { 1, -1 },
        { 1, 0 },
        { 1, 1 },
        { 0, 1 },
        { -1, 1 },
        { -1, 0 }
    };

    private uint[] _data;

    private int _dataLength;

    private int _width;

    private int _height;

    private VerticesDetectionType _polygonDetectionType;

    private uint _alphaTolerance;

    private float _hullTolerance;

    private bool _holeDetection;

    private bool _multipartDetection;

    private bool _pixelOffsetOptimization;

    private Matrix _transform = Matrix.Identity;

    private int _tempIsSolidX;

    private int _tempIsSolidY;

    public VerticesDetectionType PolygonDetectionType
    {
        get => _polygonDetectionType;
        set => _polygonDetectionType = value;
    }

    public bool HoleDetection
    {
        get => _holeDetection;
        set => _holeDetection = value;
    }

    public bool MultipartDetection
    {
        get => _multipartDetection;
        set => _multipartDetection = value;
    }

    public bool PixelOffsetOptimization
    {
        get => _pixelOffsetOptimization;
        set => _pixelOffsetOptimization = value;
    }

    public Matrix Transform
    {
        get => _transform;
        set => _transform = value;
    }

    public byte AlphaTolerance
    {
        get => (byte)(_alphaTolerance >> 24);
        set => _alphaTolerance = (uint)(value << 24);
    }

    public float HullTolerance
    {
        get => _hullTolerance;
        set
        {
            _hullTolerance = value > 4f ? 4f : value < 0.9f ? 0.9f : value;
        }
    }

    public TextureConverter()
    {
        Initialize(null, null, null, null, null, null, null, null);
    }

    public TextureConverter(byte? alphaTolerance, float? hullTolerance, bool? holeDetection, bool? multipartDetection, bool? pixelOffsetOptimization, Matrix? transform)
    {
        Initialize(null, null, alphaTolerance, hullTolerance, holeDetection, multipartDetection, pixelOffsetOptimization, transform);
    }

    public TextureConverter(uint[] data, int width)
    {
        Initialize(data, width, null, null, null, null, null, null);
    }

    public TextureConverter(uint[] data, int width, byte? alphaTolerance, float? hullTolerance, bool? holeDetection, bool? multipartDetection, bool? pixelOffsetOptimization, Matrix? transform)
    {
        Initialize(data, width, alphaTolerance, hullTolerance, holeDetection, multipartDetection, pixelOffsetOptimization, transform);
    }

    private void Initialize(uint[] data, int? width, byte? alphaTolerance, float? hullTolerance, bool? holeDetection, bool? multipartDetection, bool? pixelOffsetOptimization, Matrix? transform)
    {
        if (data != null && !width.HasValue)
        {
            throw new ArgumentNullException("width", "'width' can't be null if 'data' is set.");
        }
        if (data == null && width.HasValue)
        {
            throw new ArgumentNullException("data", "'data' can't be null if 'width' is set.");
        }
        if (data != null && width.HasValue)
        {
            SetTextureData(data, width.Value);
        }
        AlphaTolerance = alphaTolerance ?? (byte)20;
        HullTolerance = hullTolerance ?? 1.5f;
        HoleDetection = holeDetection.HasValue && holeDetection.Value;
        MultipartDetection = multipartDetection.HasValue && multipartDetection.Value;
        PixelOffsetOptimization = pixelOffsetOptimization.HasValue && pixelOffsetOptimization.Value;
        Transform = transform ?? Matrix.Identity;
    }

    private void SetTextureData(uint[] data, int width)
    {
        if (data == null)
        {
            throw new ArgumentNullException("data", "'data' can't be null.");
        }
        if (data.Length < 4)
        {
            throw new ArgumentOutOfRangeException("data", "'data' length can't be less then 4. Your texture must be at least 2 x 2 pixels in size.");
        }
        if (width < 2)
        {
            throw new ArgumentOutOfRangeException("width", "'width' can't be less then 2. Your texture must be at least 2 x 2 pixels in size.");
        }
        if (data.Length % width != 0)
        {
            throw new ArgumentException("'width' has an invalid value.");
        }
        _data = data;
        _dataLength = _data.Length;
        _width = width;
        _height = _dataLength / width;
    }

    public static Vertices DetectVertices(uint[] data, int width)
    {
        TextureConverter textureConverter = new(data, width);
        List<Vertices> list = textureConverter.DetectVertices();
        return list[0];
    }

    public static Vertices DetectVertices(uint[] data, int width, bool holeDetection)
    {
        TextureConverter textureConverter = new(data, width)
        {
            HoleDetection = holeDetection
        };
        TextureConverter textureConverter2 = textureConverter;
        List<Vertices> list = textureConverter2.DetectVertices();
        return list[0];
    }

    public static List<Vertices> DetectVertices(uint[] data, int width, float hullTolerance, byte alphaTolerance, bool multiPartDetection, bool holeDetection)
    {
        TextureConverter textureConverter = new(data, width)
        {
            HullTolerance = hullTolerance,
            AlphaTolerance = alphaTolerance,
            MultipartDetection = multiPartDetection,
            HoleDetection = holeDetection
        };
        TextureConverter textureConverter2 = textureConverter;
        List<Vertices> list = textureConverter2.DetectVertices();
        List<Vertices> list2 = [];
        for (int i = 0; i < list.Count; i++)
        {
            list2.Add(list[i]);
        }
        return list2;
    }

    public List<Vertices> DetectVertices()
    {
        if (_data == null)
        {
            throw new Exception("'_data' can't be null. You have to use SetTextureData(uint[] data, int width) before calling this method.");
        }
        if (_data.Length < 4)
        {
            throw new Exception("'_data' length can't be less then 4. Your texture must be at least 2 x 2 pixels in size. You have to use SetTextureData(uint[] data, int width) before calling this method.");
        }
        if (_width < 2)
        {
            throw new Exception("'_width' can't be less then 2. Your texture must be at least 2 x 2 pixels in size. You have to use SetTextureData(uint[] data, int width) before calling this method.");
        }
        if (_data.Length % _width != 0)
        {
            throw new Exception("'_width' has an invalid value. You have to use SetTextureData(uint[] data, int width) before calling this method.");
        }
        List<Vertices> detectedPolygons = [];
        Vector2? lastHoleEntrance = null;
        Vector2? entrance = null;
        List<Vector2> list = [];
        bool flag;
        do
        {
            Vertices vertices;
            if (detectedPolygons.Count == 0)
            {
                vertices = [.. CreateSimplePolygon(Vector2.Zero, Vector2.Zero)];
                if (vertices.Count > 2)
                {
                    entrance = GetTopMostVertex(vertices);
                }
            }
            else
            {
                if (!entrance.HasValue)
                {
                    break;
                }
                vertices = [.. CreateSimplePolygon(entrance.Value, new Vector2(entrance.Value.X - 1f, entrance.Value.Y))];
            }
            flag = false;
            if (vertices.Count > 2)
            {
                if (_holeDetection)
                {
                    while (true)
                    {
                        lastHoleEntrance = SearchHoleEntrance(vertices, lastHoleEntrance);
                        if (!lastHoleEntrance.HasValue || list.Contains(lastHoleEntrance.Value))
                        {
                            break;
                        }
                        list.Add(lastHoleEntrance.Value);
                        Vertices vertices2 = CreateSimplePolygon(lastHoleEntrance.Value, new Vector2(lastHoleEntrance.Value.X + 1f, lastHoleEntrance.Value.Y));
                        if (vertices2 == null || vertices2.Count <= 2)
                        {
                            continue;
                        }
                        switch (_polygonDetectionType)
                        {
                            case VerticesDetectionType.Integrated:
                                {
                                    vertices2.Add(vertices2[0]);
                                    if (SplitPolygonEdge(vertices, lastHoleEntrance.Value, out int _, out int vertex2Index))
                                    {
                                        vertices.InsertRange(vertex2Index, vertices2);
                                    }
                                    break;
                                }
                            case VerticesDetectionType.Separated:
                                vertices.Holes ??= [];
                                vertices.Holes.Add(vertices2);
                                break;
                        }
                    }
                }
                detectedPolygons.Add(vertices);
            }
            if ((_multipartDetection || vertices.Count <= 2) && SearchNextHullEntrance(detectedPolygons, entrance.Value, out entrance))
            {
                flag = true;
            }
        }
        while (flag);
        if (detectedPolygons == null || (detectedPolygons != null && detectedPolygons.Count == 0))
        {
            throw new Exception("Couldn't detect any vertices.");
        }
        if (PolygonDetectionType == VerticesDetectionType.Separated)
        {
            ApplyTriangulationCompatibleWinding(ref detectedPolygons);
        }
        if (_transform != Matrix.Identity)
        {
            ApplyTransform(ref detectedPolygons);
        }
        return detectedPolygons;
    }

    private void ApplyTriangulationCompatibleWinding(ref List<Vertices> detectedPolygons)
    {
        for (int i = 0; i < detectedPolygons.Count; i++)
        {
            detectedPolygons[i].Reverse();
            if (detectedPolygons[i].Holes != null && detectedPolygons[i].Holes.Count > 0)
            {
                for (int j = 0; j < detectedPolygons[i].Holes.Count; j++)
                {
                    detectedPolygons[i].Holes[j].Reverse();
                }
            }
        }
    }

    private void ApplyTransform(ref List<Vertices> detectedPolygons)
    {
        for (int i = 0; i < detectedPolygons.Count; i++)
        {
            detectedPolygons[i].Transform(ref _transform);
        }
    }

    public bool IsSolid(ref Vector2 v)
    {
        _tempIsSolidX = (int)v.X;
        _tempIsSolidY = (int)v.Y;
        return _tempIsSolidX >= 0 && _tempIsSolidX < _width && _tempIsSolidY >= 0 && _tempIsSolidY < _height && _data[_tempIsSolidX + (_tempIsSolidY * _width)] >= _alphaTolerance;
    }

    public bool IsSolid(ref int x, ref int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height && _data[x + (y * _width)] >= _alphaTolerance;
    }

    public bool IsSolid(ref int index)
    {
        return index >= 0 && index < _dataLength && _data[index] >= _alphaTolerance;
    }

    public bool InBounds(ref Vector2 coord)
    {
        return coord.X >= 0f && coord.X < _width && coord.Y >= 0f && coord.Y < _height;
    }

    private Vector2? SearchHoleEntrance(Vertices polygon, Vector2? lastHoleEntrance)
    {
        if (polygon == null)
        {
            throw new ArgumentNullException("'polygon' can't be null.");
        }
        if (polygon.Count < 3)
        {
            throw new ArgumentException("'polygon.MainPolygon.Count' can't be less then 3.");
        }
        int num = 0;
        int num2 = (!lastHoleEntrance.HasValue) ? ((int)GetTopMostCoord(polygon)) : ((int)lastHoleEntrance.Value.Y);
        int num3 = (int)GetBottomMostCoord(polygon);
        if (num2 > 0 && num2 < _height && num3 > 0 && num3 < _height)
        {
            for (int i = num2; i <= num3; i++)
            {
                List<float> list = SearchCrossingEdges(polygon, i);
                if (list.Count > 1 && list.Count % 2 == 0)
                {
                    for (int j = 0; j < list.Count; j += 2)
                    {
                        bool flag = false;
                        bool flag2 = false;
                        for (int k = (int)list[j]; k <= (int)list[j + 1]; k++)
                        {
                            if (IsSolid(ref k, ref i))
                            {
                                if (!flag2)
                                {
                                    flag = true;
                                    num = k;
                                }
                                if (flag && flag2)
                                {
                                    Vector2? result = new Vector2(num, i);
                                    if (DistanceToHullAcceptable(polygon, result.Value, higherDetail: true))
                                    {
                                        return result;
                                    }

                                    break;
                                }
                            }
                            else if (flag)
                            {
                                flag2 = true;
                            }
                        }
                    }
                }
                else
                {
                    _ = list.Count % 2;
                }
            }
        }
        return null;
    }

    private bool DistanceToHullAcceptableHoles(Vertices polygon, Vector2 point, bool higherDetail)
    {
        if (polygon == null)
        {
            throw new ArgumentNullException("polygon", "'polygon' can't be null.");
        }
        if (polygon.Count < 3)
        {
            throw new ArgumentException("'polygon.MainPolygon.Count' can't be less then 3.");
        }
        if (DistanceToHullAcceptable(polygon, point, higherDetail))
        {
            if (polygon.Holes != null)
            {
                for (int i = 0; i < polygon.Holes.Count; i++)
                {
                    if (!DistanceToHullAcceptable(polygon.Holes[i], point, higherDetail))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }

    private bool DistanceToHullAcceptable(Vertices polygon, Vector2 point, bool higherDetail)
    {
        if (polygon == null)
        {
            throw new ArgumentNullException("polygon", "'polygon' can't be null.");
        }
        if (polygon.Count < 3)
        {
            throw new ArgumentException("'polygon.Count' can't be less then 3.");
        }
        Vector2 end = polygon[^1];
        if (higherDetail)
        {
            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 start = polygon[i];
                if (LineTools.DistanceBetweenPointAndLineSegment(ref point, ref start, ref end) <= _hullTolerance || Vector2.Distance(point, start) <= _hullTolerance)
                {
                    return false;
                }
                end = polygon[i];
            }
            return true;
        }
        for (int j = 0; j < polygon.Count; j++)
        {
            Vector2 start = polygon[j];
            if (LineTools.DistanceBetweenPointAndLineSegment(ref point, ref start, ref end) <= _hullTolerance)
            {
                return false;
            }
            end = polygon[j];
        }
        return true;
    }

    private bool InPolygon(Vertices polygon, Vector2 point)
    {
        if (DistanceToHullAcceptableHoles(polygon, point, higherDetail: true))
        {
            List<float> list = SearchCrossingEdgesHoles(polygon, (int)point.Y);
            if (list.Count > 0 && list.Count % 2 == 0)
            {
                for (int i = 0; i < list.Count; i += 2)
                {
                    if (list[i] <= point.X && list[i + 1] >= point.X)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        return true;
    }

    private Vector2? GetTopMostVertex(Vertices vertices)
    {
        float num = float.MaxValue;
        Vector2? result = null;
        for (int i = 0; i < vertices.Count; i++)
        {
            if (num > vertices[i].Y)
            {
                num = vertices[i].Y;
                result = vertices[i];
            }
        }
        return result;
    }

    private float GetTopMostCoord(Vertices vertices)
    {
        float num = float.MaxValue;
        for (int i = 0; i < vertices.Count; i++)
        {
            if (num > vertices[i].Y)
            {
                num = vertices[i].Y;
            }
        }
        return num;
    }

    private float GetBottomMostCoord(Vertices vertices)
    {
        float num = float.MinValue;
        for (int i = 0; i < vertices.Count; i++)
        {
            if (num < vertices[i].Y)
            {
                num = vertices[i].Y;
            }
        }
        return num;
    }

    private List<float> SearchCrossingEdgesHoles(Vertices polygon, int y)
    {
        if (polygon == null)
        {
            throw new ArgumentNullException("polygon", "'polygon' can't be null.");
        }
        if (polygon.Count < 3)
        {
            throw new ArgumentException("'polygon.MainPolygon.Count' can't be less then 3.");
        }
        List<float> list = SearchCrossingEdges(polygon, y);
        if (polygon.Holes != null)
        {
            for (int i = 0; i < polygon.Holes.Count; i++)
            {
                list.AddRange(SearchCrossingEdges(polygon.Holes[i], y));
            }
        }
        list.Sort();
        return list;
    }

    private List<float> SearchCrossingEdges(Vertices polygon, int y)
    {
        List<float> list = [];
        if (polygon.Count > 2)
        {
            Vector2 vector = polygon[^1];
            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 vector2 = polygon[i];
                if (((vector2.Y >= y && vector.Y <= y) || (vector2.Y <= y && vector.Y >= y)) && vector2.Y != vector.Y)
                {
                    bool flag = true;
                    Vector2 vector3 = vector - vector2;
                    if (vector2.Y == y)
                    {
                        Vector2 vector4 = polygon[(i + 1) % polygon.Count];
                        Vector2 vector5 = vector2 - vector4;
                        flag = (!(vector3.Y > 0f)) ? (vector5.Y >= 0f) : (vector5.Y <= 0f);
                    }
                    if (flag)
                    {
                        list.Add(((y - vector2.Y) / vector3.Y * vector3.X) + vector2.X);
                    }
                }
                vector = vector2;
            }
        }
        list.Sort();
        return list;
    }

    private bool SplitPolygonEdge(Vertices polygon, Vector2 coordInsideThePolygon, out int vertex1Index, out int vertex2Index)
    {
        int num = 0;
        int index = 0;
        bool flag = false;
        float num2 = float.MaxValue;
        bool flag2 = false;
        Vector2 point = Vector2.Zero;
        List<float> list = SearchCrossingEdges(polygon, (int)coordInsideThePolygon.Y);
        vertex1Index = 0;
        vertex2Index = 0;
        point.Y = coordInsideThePolygon.Y;
        if (list != null && list.Count > 1 && list.Count % 2 == 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < coordInsideThePolygon.X)
                {
                    float num3 = coordInsideThePolygon.X - list[i];
                    if (num3 < num2)
                    {
                        num2 = num3;
                        point.X = list[i];
                        flag2 = true;
                    }
                }
            }
            if (flag2)
            {
                num2 = float.MaxValue;
                int num4 = polygon.Count - 1;
                for (int j = 0; j < polygon.Count; j++)
                {
                    Vector2 start = polygon[j];
                    Vector2 end = polygon[num4];
                    float num3 = LineTools.DistanceBetweenPointAndLineSegment(ref point, ref start, ref end);
                    if (num3 < num2)
                    {
                        num2 = num3;
                        num = j;
                        index = num4;
                        flag = true;
                    }
                    num4 = j;
                }
                if (flag)
                {
                    Vector2 vector = polygon[index] - polygon[num];
                    vector.Normalize();
                    Vector2 value = polygon[num];
                    float num3 = Vector2.Distance(value, point);
                    vertex1Index = num;
                    vertex2Index = num + 1;
                    polygon.Insert(num, (num3 * vector) + polygon[vertex1Index]);
                    polygon.Insert(num, (num3 * vector) + polygon[vertex2Index]);
                    return true;
                }
            }
        }
        return false;
    }

    private Vertices CreateSimplePolygon(Vector2 entrance, Vector2 last)
    {
        bool flag = false;
        bool flag2 = false;
        Vertices vertices = new(32);
        Vertices vertices2 = new(32);
        Vertices vertices3 = new(32);
        Vector2 current = Vector2.Zero;
        if (entrance == Vector2.Zero || !InBounds(ref entrance))
        {
            flag = SearchHullEntrance(out entrance);
            if (flag)
            {
                current = new Vector2(entrance.X - 1f, entrance.Y);
            }
        }
        else if (IsSolid(ref entrance))
        {
            if (IsNearPixel(ref entrance, ref last))
            {
                current = last;
                flag = true;
            }
            else if (SearchNearPixels(searchingForSolidPixel: false, ref entrance, out Vector2 foundPixel))
            {
                current = foundPixel;
                flag = true;
            }
            else
            {
                flag = false;
            }
        }
        if (flag)
        {
            vertices.Add(entrance);
            vertices2.Add(entrance);
            Vector2 next = entrance;
            while (true)
            {
                if (SearchForOutstandingVertex(vertices2, out Vector2 outstanding))
                {
                    if (flag2)
                    {
                        if (vertices3.Contains(outstanding))
                        {
                            vertices.Add(outstanding);
                        }
                        break;
                    }
                    vertices.Add(outstanding);
                    vertices2.RemoveRange(0, vertices2.IndexOf(outstanding));
                }
                last = current;
                current = next;
                if (!GetNextHullPoint(ref last, ref current, out next))
                {
                    break;
                }
                vertices2.Add(next);
                if (next == entrance && !flag2)
                {
                    flag2 = true;
                    vertices3.AddRange(vertices2);
                    if (vertices3.Contains(entrance))
                    {
                        _ = vertices3.Remove(entrance);
                    }
                }
            }
        }
        return vertices;
    }

    private bool SearchNearPixels(bool searchingForSolidPixel, ref Vector2 current, out Vector2 foundPixel)
    {
        for (int i = 0; i < 8; i++)
        {
            int x = (int)current.X + _closePixels[i, 0];
            int y = (int)current.Y + _closePixels[i, 1];
            if (!searchingForSolidPixel ^ IsSolid(ref x, ref y))
            {
                foundPixel = new Vector2(x, y);
                return true;
            }
        }
        foundPixel = Vector2.Zero;
        return false;
    }

    private bool IsNearPixel(ref Vector2 current, ref Vector2 near)
    {
        for (int i = 0; i < 8; i++)
        {
            int num = (int)current.X + _closePixels[i, 0];
            int num2 = (int)current.Y + _closePixels[i, 1];
            if (num >= 0 && num <= _width && num2 >= 0 && num2 <= _height && num == (int)near.X && num2 == (int)near.Y)
            {
                return true;
            }
        }
        return false;
    }

    private bool SearchHullEntrance(out Vector2 entrance)
    {
        for (int i = 0; i <= _height; i++)
        {
            for (int j = 0; j <= _width; j++)
            {
                if (IsSolid(ref j, ref i))
                {
                    entrance = new Vector2(j, i);
                    return true;
                }
            }
        }
        entrance = Vector2.Zero;
        return false;
    }

    private bool SearchNextHullEntrance(List<Vertices> detectedPolygons, Vector2 start, out Vector2? entrance)
    {
        bool flag = false;
        for (int i = (int)start.X + ((int)start.Y * _width); i <= _dataLength; i++)
        {
            if (IsSolid(ref i))
            {
                if (!flag)
                {
                    continue;
                }
                int num = i % _width;
                entrance = new Vector2(num, (i - num) / (float)_width);
                bool flag2 = false;
                for (int j = 0; j < detectedPolygons.Count; j++)
                {
                    if (InPolygon(detectedPolygons[j], entrance.Value))
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    return true;
                }
                flag = false;
            }
            else
            {
                flag = true;
            }
        }
        entrance = null;
        return false;
    }

    private bool GetNextHullPoint(ref Vector2 last, ref Vector2 current, out Vector2 next)
    {
        int indexOfFirstPixelToCheck = GetIndexOfFirstPixelToCheck(ref last, ref current);
        for (int i = 0; i < 8; i++)
        {
            int num = (indexOfFirstPixelToCheck + i) % 8;
            int x = (int)current.X + _closePixels[num, 0];
            int y = (int)current.Y + _closePixels[num, 1];
            if (x >= 0 && x < _width && y >= 0 && y <= _height && IsSolid(ref x, ref y))
            {
                next = new Vector2(x, y);
                return true;
            }
        }
        next = Vector2.Zero;
        return false;
    }

    private bool SearchForOutstandingVertex(Vertices hullArea, out Vector2 outstanding)
    {
        Vector2 vector = Vector2.Zero;
        bool result = false;
        if (hullArea.Count > 2)
        {
            int num = hullArea.Count - 1;
            Vector2 start = hullArea[0];
            Vector2 end = hullArea[num];
            for (int i = 1; i < num; i++)
            {
                Vector2 point = hullArea[i];
                if (LineTools.DistanceBetweenPointAndLineSegment(ref point, ref start, ref end) >= _hullTolerance)
                {
                    vector = hullArea[i];
                    result = true;
                    break;
                }
            }
        }
        outstanding = vector;
        return result;
    }

    private int GetIndexOfFirstPixelToCheck(ref Vector2 last, ref Vector2 current)
    {
        switch ((int)(current.X - last.X))
        {
            case 1:
                switch ((int)(current.Y - last.Y))
                {
                    case 1:
                        return 1;
                    case 0:
                        return 0;
                    case -1:
                        return 7;
                }
                break;
            case 0:
                switch ((int)(current.Y - last.Y))
                {
                    case 1:
                        return 2;
                    case -1:
                        return 6;
                }
                break;
            case -1:
                switch ((int)(current.Y - last.Y))
                {
                    case 1:
                        return 3;
                    case 0:
                        return 4;
                    case -1:
                        return 5;
                }
                break;
        }
        return 0;
    }
}

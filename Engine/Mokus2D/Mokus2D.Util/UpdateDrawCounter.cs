using System;
using System.Collections.Generic;
using System.Globalization;

using Mokus2D.Visual.Text;

namespace Mokus2D.Util;

public class UpdateDrawCounter : FpsCounter
{
    private DateTime _startTime;

    private float _drawTime;

    private float _updateTime;

    private float _staticUpdatesTime;

    private float _staticDrawsTime;

    private int _updatesCount;

    private int _drawsCount;

    public Label OutputLabel;

    private readonly Dictionary<string, object> _testValues = [];

    private int _drawCalls;

    private int _drawnTriangles;

    private int _updatedNodes;

    public string AdditionalText;

    public float DrawTime { get; private set; }

    public float UpdateTime { get; private set; }

    public float StaticUpdatesTime { get; private set; }

    public float StaticDrawsTime { get; private set; }

    public UpdateDrawCounter(int framesToCalculate)
        : base(framesToCalculate)
    {
    }

    public void SetTestValue(string name, object value)
    {
        _testValues[name] = value;
    }

    public void StartDraw()
    {
        _startTime = DateTime.UtcNow;
    }

    public void EndDraw()
    {
        float num = (float)(DateTime.UtcNow - _startTime).TotalSeconds;
        _drawTime += num;
        _staticDrawsTime += num;
        _drawsCount++;
        if (_drawsCount == 100)
        {
            _drawsCount = 0;
            StaticDrawsTime = _staticDrawsTime;
            _staticDrawsTime = 0f;
        }
    }

    public void StartUpdate()
    {
        _startTime = DateTime.UtcNow;
    }

    public void EndUpdate()
    {
        float num = (float)(DateTime.UtcNow - _startTime).TotalSeconds;
        _updateTime += num;
        _updatesCount++;
        _staticUpdatesTime += num;
        if (_updatesCount >= 100)
        {
            StaticUpdatesTime = _staticUpdatesTime;
            _updatesCount = 0;
            _staticUpdatesTime = 0f;
        }
    }

    public void IncreaseDrawCalls(string textureName, int triangles)
    {
        _drawCalls++;
        _drawnTriangles += triangles;
    }

    protected override void CalculateFps()
    {
        base.CalculateFps();
        DrawTime = _drawTime;
        UpdateTime = _updateTime;
        if (OutputLabel != null)
        {
            _ = OutputLabel.Clear();
            _ = OutputLabel.Append("FPS: ");
            _ = float.IsInfinity(Fps) ? OutputLabel.Append("Infinity") : OutputLabel.Append(Fps);
            _ = OutputLabel.AppendLine();
            AppendLine("U: ", UpdateTime);
            AppendLine("D: ", DrawTime);
            AppendLine("SU: ", StaticUpdatesTime);
            AppendLine("SD: ", StaticDrawsTime);
            AppendLine("Draw Calls: ", _drawCalls / (float)FramesToCalculate);
            AppendLine("Triangles: ", _drawnTriangles / (float)FramesToCalculate);
            AppendLine("Updated Nodes: ", _updatedNodes / (float)FramesToCalculate);
            foreach (KeyValuePair<string, object> testValue in _testValues)
            {
                _ = OutputLabel.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", testValue.Key, testValue.Value));
            }
            if (AdditionalText != null)
            {
                _ = OutputLabel.AppendLine(AdditionalText);
            }
        }
        _drawTime = 0f;
        _updateTime = 0f;
        _drawCalls = 0;
        _updatedNodes = 0;
        _drawnTriangles = 0;
    }

    private void AppendLine(string name, float value)
    {
        _ = OutputLabel.Append(name);
        _ = OutputLabel.Append(value);
        _ = OutputLabel.AppendLine();
    }

    public void IncreaseUpdates()
    {
        _updatedNodes++;
    }
}

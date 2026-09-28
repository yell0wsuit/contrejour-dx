using System;
using System.Collections.Generic;
using Mokus2D.Visual.Text;

namespace Mokus2D.Util;

public class UpdateDrawCounter : FpsCounter
{
	private const int StaticUpdatesCount = 100;

	private const string TestValueFormat = "{0}: {1}";

	private const string InfinityString = "Infinity";

	private DateTime _startTime;

	private float _drawTime;

	private float _updateTime;

	private float _staticUpdatesTime;

	private float _staticDrawsTime;

	private int _updatesCount;

	private int _drawsCount;

	public Label OutputLabel;

	private readonly Dictionary<string, object> _testValues = new Dictionary<string, object>();

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
			OutputLabel.Clear();
			OutputLabel.Append("FPS: ");
			if (float.IsInfinity(base.Fps))
			{
				OutputLabel.Append("Infinity");
			}
			else
			{
				OutputLabel.Append(base.Fps);
			}
			OutputLabel.AppendLine();
			AppendLine("U: ", UpdateTime);
			AppendLine("D: ", DrawTime);
			AppendLine("SU: ", StaticUpdatesTime);
			AppendLine("SD: ", StaticDrawsTime);
			AppendLine("Draw Calls: ", (float)_drawCalls / (float)FramesToCalculate);
			AppendLine("Triangles: ", (float)_drawnTriangles / (float)FramesToCalculate);
			AppendLine("Updated Nodes: ", (float)_updatedNodes / (float)FramesToCalculate);
			foreach (KeyValuePair<string, object> testValue in _testValues)
			{
				OutputLabel.AppendLine(string.Format("{0}: {1}", new object[2] { testValue.Key, testValue.Value }));
			}
			if (AdditionalText != null)
			{
				OutputLabel.AppendLine(AdditionalText);
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
		OutputLabel.Append(name);
		OutputLabel.Append(value);
		OutputLabel.AppendLine();
	}

	public void IncreaseUpdates()
	{
		_updatedNodes++;
	}
}

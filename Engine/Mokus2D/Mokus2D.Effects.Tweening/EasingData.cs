using System;
using Mokus2D.Data;

namespace Mokus2D.Effects.Tweening;

public class EasingData : IEasingData, ICleanable
{
	private static readonly Pool<EasingData> Pool = new Pool<EasingData>(() => new EasingData());

	private Func<float, float, float, float> _functionC;

	private Func<float, float, float> _functionB;

	private Func<float, float> _functionA;

	private float _dataA;

	private float _dataB;

	public static EasingData NewOrNull(Func<float, float, float, float> function, float dataA, float dataB)
	{
		if (function != null)
		{
			return Pool.New().Initialize(function, dataA, dataB);
		}
		return null;
	}

	public static EasingData NewOrNull(Func<float, float, float> function, float dataA)
	{
		if (function != null)
		{
			return Pool.New().Initialize(function, dataA);
		}
		return null;
	}

	public static EasingData NewOrNull(Func<float, float> function)
	{
		if (function != null)
		{
			return Pool.New().Initialize(function);
		}
		return null;
	}

	public static void Free(EasingData data)
	{
		Pool.Free(data);
	}

	private EasingData()
	{
	}

	public EasingData Initialize(Func<float, float, float, float> function, float dataA, float dataB)
	{
		if (function == null)
		{
			throw new NullReferenceException("function can not be null");
		}
		_functionC = function;
		_dataA = dataA;
		_dataB = dataB;
		return this;
	}

	public EasingData Initialize(Func<float, float, float> function, float dataA)
	{
		if (function == null)
		{
			throw new NullReferenceException("function can not be null");
		}
		_functionB = function;
		_dataA = dataA;
		return this;
	}

	public EasingData Initialize(Func<float, float> function)
	{
		if (function == null)
		{
			throw new NullReferenceException("function can not be null");
		}
		_functionA = function;
		return this;
	}

	public float Ease(float ratio)
	{
		if (_functionA != null)
		{
			return _functionA(ratio);
		}
		if (_functionB != null)
		{
			return _functionB(ratio, _dataA);
		}
		if (_functionC != null)
		{
			return _functionC(ratio, _dataA, _dataB);
		}
		throw new Exception("No ease function selected");
	}

	public void Clean()
	{
		_functionA = null;
		_functionB = null;
		_functionC = null;
	}
}

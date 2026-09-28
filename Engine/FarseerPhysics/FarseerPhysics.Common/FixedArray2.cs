using System;

namespace FarseerPhysics.Common;

public struct FixedArray2<T>
{
	private T _value0;

	private T _value1;

	public T this[int index]
	{
		get
		{
			return index switch
			{
				0 => _value0, 
				1 => _value1, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		set
		{
			switch (index)
			{
			case 0:
				_value0 = value;
				break;
			case 1:
				_value1 = value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}
}

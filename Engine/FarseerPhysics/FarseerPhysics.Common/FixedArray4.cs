using System;

namespace FarseerPhysics.Common;

public struct FixedArray4<T>
{
	private T _value0;

	private T _value1;

	private T _value2;

	private T _value3;

	public T this[int index]
	{
		get
		{
			return index switch
			{
				0 => _value0, 
				1 => _value1, 
				2 => _value2, 
				3 => _value3, 
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
			case 2:
				_value2 = value;
				break;
			case 3:
				_value3 = value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}
}

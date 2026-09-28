using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual.Util;

public class VertexPositionColorTextureListAdapter : IList<Vector2>, ICollection<Vector2>, IEnumerable<Vector2>, IEnumerable
{
	private VertexPositionColorTexture[] source;

	public int Count => source.Length;

	public bool IsReadOnly
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public Vector2 this[int index]
	{
		get
		{
			return source[index].Position.ToVector2();
		}
		set
		{
			source[index].Position = value.ToVector3();
		}
	}

	public VertexPositionColorTextureListAdapter(VertexPositionColorTexture[] source)
	{
		this.source = source;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<Vector2> GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public void Add(Vector2 item)
	{
		throw new NotImplementedException();
	}

	public void Clear()
	{
		throw new NotImplementedException();
	}

	public bool Contains(Vector2 item)
	{
		throw new NotImplementedException();
	}

	public void CopyTo(Vector2[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public bool Remove(Vector2 item)
	{
		throw new NotImplementedException();
	}

	public int IndexOf(Vector2 item)
	{
		throw new NotImplementedException();
	}

	public void Insert(int index, Vector2 item)
	{
		throw new NotImplementedException();
	}

	public void RemoveAt(int index)
	{
		throw new NotImplementedException();
	}
}

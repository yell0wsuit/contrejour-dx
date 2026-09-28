using System;
using Microsoft.Xna.Framework;
using Mokus2D.Data;
using Mokus2D.Effects.Tweening;
using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.Effects.Transitions;

public class FadeTransition : ITween, ICleanable, IUpdatable
{
	private static readonly Pool<FadeTransition> pool = new Pool<FadeTransition>(() => new FadeTransition());

	private Node toRemove;

	private Func<Node> nodeFactory;

	private bool currentActionFinished;

	private LayerColor layer;

	private bool fadingOut;

	private bool replaced;

	private float hideSeconds;

	private float showSeconds;

	private int framesToWait;

	private int currentFrame;

	private bool dispatch;

	public LayerColor Layer => layer;

	public bool Finished { get; private set; }

	public event Action MiddleEvent;

	private void OnCurrentActionFinished()
	{
		currentActionFinished = true;
	}

	public FadeTransition()
	{
		throw new NotImplementedException();
	}

	public FadeTransition(string spriteId, float seconds, Node toRemove, Func<Node> nodeFactory, int framesToWait = 0)
		: this(spriteId, seconds / 2f, seconds / 2f, toRemove, nodeFactory, framesToWait)
	{
	}

	public FadeTransition(string spriteId, float hideSeconds, float showSeconds, Node toRemove, Func<Node> nodeFactory, int framesToWait = 0)
	{
		Initialize(spriteId, hideSeconds, showSeconds, toRemove, nodeFactory, framesToWait);
	}

	public FadeTransition Initialize(string spriteId, float hideSeconds, float showSeconds, Node toRemove, Func<Node> nodeFactory, int framesToWait = 0)
	{
		Clean();
		layer = new LayerColor(Color.Black, spriteId)
		{
			OpacityFloat = 0f
		};
		this.framesToWait = framesToWait;
		this.toRemove = toRemove;
		this.nodeFactory = nodeFactory;
		this.hideSeconds = hideSeconds;
		this.showSeconds = showSeconds;
		return this;
	}

	internal void Start(float time)
	{
		layer.FadeIn(hideSeconds).OnComplete(OnCurrentActionFinished);
	}

	public void Update(float time)
	{
		if (dispatch)
		{
			dispatch = false;
			this.MiddleEvent.Dispatch();
		}
		if (currentActionFinished)
		{
			if (currentFrame >= framesToWait)
			{
				layer.FadeOut(showSeconds).OnComplete(OnCurrentActionFinished);
				currentActionFinished = false;
			}
			currentFrame++;
		}
		else if (currentActionFinished)
		{
			if (fadingOut && replaced)
			{
				Finished = true;
				Finish();
			}
			else if (fadingOut && !replaced)
			{
				replaced = true;
				Node node = nodeFactory();
				toRemove.Parent.AddChild(node, toRemove.Layer);
				toRemove.RemoveFromParent();
				toRemove = null;
				currentFrame = 0;
				dispatch = true;
			}
			else if (!fadingOut)
			{
				fadingOut = true;
			}
		}
	}

	protected void Finish()
	{
		layer.RemoveFromParent();
	}

	public void Clean()
	{
		toRemove = null;
		nodeFactory = null;
		this.MiddleEvent = null;
		dispatch = false;
		currentFrame = 0;
		replaced = false;
		fadingOut = false;
		currentActionFinished = false;
	}

	public void Free()
	{
		pool.Free(this);
	}

	public void Reset()
	{
		throw new NotImplementedException();
	}

	public ITween OnComplete(Action<object> action)
	{
		throw new NotImplementedException();
	}

	public ITween OnComplete(Action action)
	{
		throw new NotImplementedException();
	}

	public static FadeTransition New(string spriteId, float hideSeconds, float showSeconds, Node toRemove, Func<Node> nodeFactory, int framesToWait = 0)
	{
		return pool.New().Initialize(spriteId, hideSeconds, showSeconds, toRemove, nodeFactory, framesToWait);
	}
}

using Mokus2D.Visual;

namespace Default.Namespace;

public class SpriteFader
{
	protected ushort disabledOpacity;

	protected float duration;

	protected bool enabled;

	protected ushort enabledOpacity;

	protected Node target;

	public ushort EnabledOpacity
	{
		get
		{
			return enabledOpacity;
		}
		set
		{
			enabledOpacity = value;
		}
	}

	public ushort DisabledOpacity
	{
		get
		{
			return disabledOpacity;
		}
		set
		{
			disabledOpacity = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			if (enabled != value)
			{
				enabled = value;
				target.Tweener.StartSequence(duration).Tween(NodeValues.OpacityFloat, (int)(enabled ? enabledOpacity : disabledOpacity));
			}
		}
	}

	public float Duration
	{
		get
		{
			return duration;
		}
		set
		{
			duration = value;
		}
	}

	public SpriteFader(Node _target)
	{
		target = _target;
		enabledOpacity = 0;
		disabledOpacity = 255;
		duration = 0.15f;
	}
}

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public abstract class MagnetIntervalActionBase : MagnetAction
{
	private readonly float _timeout;

	private float _elapsed;

	public override bool Finished => _elapsed >= _timeout;

	protected MagnetIntervalActionBase(GridMagnetBase gridMagnet, float timeout)
		: base(gridMagnet)
	{
		_timeout = timeout;
	}

	protected abstract void UpdateMagnet(float ratio);

	public override void Update(float time)
	{
		base.Update(time);
		_elapsed += time;
		float ratio = (_elapsed / _timeout).Clamp(0f, 1f);
		UpdateMagnet(ratio);
	}
}

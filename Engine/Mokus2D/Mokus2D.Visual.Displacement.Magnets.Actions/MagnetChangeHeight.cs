using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetChangeHeight : MagnetIntervalActionBase
{
	private readonly float _startHeight;

	private readonly float _endHeight;

	private readonly LineMagnet _lineMagnet;

	public MagnetChangeHeight(LineMagnet gridMagnet, float timeout, float startHeight, float endHeight)
		: base(gridMagnet, timeout)
	{
		_startHeight = startHeight;
		_endHeight = endHeight;
		_lineMagnet = gridMagnet;
		_lineMagnet.SetHeight(_startHeight);
	}

	protected override void UpdateMagnet(float ratio)
	{
		_lineMagnet.SetHeight(ratio.Lerp(_startHeight, _endHeight));
	}
}

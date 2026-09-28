namespace Mokus2D.Util;

public class Flag
{
	private bool _on;

	public Flag(bool on = true)
	{
		_on = on;
	}

	public bool Use()
	{
		if (_on)
		{
			_on = false;
			return true;
		}
		return false;
	}

	public void SetOn()
	{
		_on = true;
	}
}

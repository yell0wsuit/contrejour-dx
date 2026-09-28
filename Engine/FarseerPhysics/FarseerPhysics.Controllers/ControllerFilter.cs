namespace FarseerPhysics.Controllers;

public struct ControllerFilter
{
	public ControllerType ControllerFlags;

	public void IgnoreController(ControllerType controller)
	{
		ControllerFlags |= controller;
	}

	public void RestoreController(ControllerType controller)
	{
		ControllerFlags &= ~controller;
	}

	public bool IsControllerIgnored(ControllerType controller)
	{
		return (ControllerFlags & controller) == controller;
	}
}

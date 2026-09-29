namespace FarseerPhysics.Controllers
{
    public struct ControllerFilter
    {
        private ControllerType ControllerFlags;

        public void IgnoreController(ControllerType controller)
        {
            ControllerFlags |= controller;
        }

        public void RestoreController(ControllerType controller)
        {
            ControllerFlags &= ~controller;
        }

        public readonly bool IsControllerIgnored(ControllerType controller)
        {
            return (ControllerFlags & controller) == controller;
        }
    }
}

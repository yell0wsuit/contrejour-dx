namespace Mokus2D.Visual.Animation
{
    internal static class AnimationNodePlayers
    {
        internal static readonly IAnimationNodePlayer Smooth = new SmoothAnimationPlayer();

        internal static readonly IAnimationNodePlayer Discrete = new SafeDiscreteNodePlayer();
    }
}

using Mokus2D.Visual.Shaders.NormalMap;

namespace DubstepRun.Rendering.Shaders;

public class NormalMapEffect : NormalMapEffectBase
{
    public new const int MaxLightsCount = 30;

    public NormalMapEffect(int lightsCount)
        : base("Mokus2D.Shaders.NormalMap", 30)
    {
        LightsCount.Value = lightsCount;
    }
}

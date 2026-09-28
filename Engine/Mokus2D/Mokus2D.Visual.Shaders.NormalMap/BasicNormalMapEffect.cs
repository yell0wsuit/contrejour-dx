using Mokus2D.Visual.ShaderSupport.Parameters;

namespace Mokus2D.Visual.Shaders.NormalMap;

public class BasicNormalMapEffect : NormalMapEffectBase
{
    public new const int MaxLightsCount = 5;

    public readonly ShaderParameterFloat LightTextureColorRatio;

    public BasicNormalMapEffect(int lightsCount)
        : base("Mokus2D.Shaders.BasicNormalMap", 5)
    {
        LightsCount.Value = lightsCount;
        LightTextureColorRatio = new ShaderParameterFloat(Parameters, "LightTextureColorRatio");
    }
}

namespace Mokus2D.Graphics
{
    // How a draw's source color src combines with the target color dst. Colors are premultiplied.
    public enum BlendMode
    {
        // dst = src + dst·(1 − src.a)
        AlphaBlend,

        // dst = src·src.a + dst
        Additive,

        // dst = src·src.a + dst·(1 − src.a)
        NonPremultiplied
    }

    // Bilinear filtering without mipmaps; texture coordinates outside 0-1 clamp to the edge or repeat.
    public enum SamplerMode
    {
        LinearClamp,
        LinearWrap
    }

    // How the texture sample tex (premultiplied) and the interpolated vertex color c (straight
    // alpha, channels 0-1) make the source color.
    public enum ColorMode
    {
        // Texture required; Opacity ignored. rgb = tex.rgb·c.rgb·c.a, a = tex.a·c.a.
        Sprite,

        // Texture optional (null means tex = 1). All four channels: tex·c·Opacity.
        Primitive
    }

    public readonly record struct DrawState(ITexture Texture, BlendMode Blend, SamplerMode Sampler, ColorMode ColorMode, float Opacity = 1f);
}

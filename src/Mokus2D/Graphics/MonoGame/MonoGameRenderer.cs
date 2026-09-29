using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.FileSystem;

namespace Mokus2D.Graphics.MonoGame
{
    // IRenderer on a MonoGame GraphicsDevice. Sprites use the original Mokus2D sprite shader with
    // tinting off; primitives use a BasicEffect, as the engine drew them before this renderer
    // existed. It lives in the engine only while the rendering seam is being built; it moves into the
    // desktop host once nothing in the engine needs MonoGame graphics.
    public sealed class MonoGameRenderer : IRenderer, IDisposable
    {
        private const string SpriteShaderPath = "Mokus2D/Shaders/SpriteShader.ogl.mgfxo";

        private readonly GraphicsDevice _device;

        private readonly Effect _spriteEffect;

        private readonly EffectParameter _spriteTexture;

        private readonly EffectParameter _spriteMatrix;

        private readonly BasicEffect _primitiveEffect;

        public MonoGameRenderer(GraphicsDevice device, IFileLoader files)
        {
            _device = device;
            _spriteEffect = LoadEffect(device, files, SpriteShaderPath);
            _spriteEffect.Parameters["TintEnabled"].SetValue(false);
            _spriteTexture = _spriteEffect.Parameters["Texture"];
            _spriteMatrix = _spriteEffect.Parameters["Matrix"];
            _primitiveEffect = new BasicEffect(device)
            {
                World = Matrix.Identity,
                View = Matrix.Identity,
                VertexColorEnabled = true
            };
        }

        public ITexture CreateTexture(Stream stream)
        {
            // Sprites are drawn with premultiplied-alpha blending, as MonoGame's content loader
            // prepared raw image files; straight alpha shows white fringes around soft edges.
            return new MonoGameTexture(Texture2D.FromStream(_device, stream, DefaultColorProcessors.PremultiplyAlpha));
        }

        public void DrawTriangles(Vertex[] vertices, int vertexCount, short[] indices, int indexCount, in Matrix transform, in DrawState state)
        {
            // States first, then the effect, as the engine always did: the effect pass may override them.
            _device.BlendState = ToBlendState(state.Blend);
            _device.SamplerStates[0] = ToSamplerState(state.Sampler);
            Texture2D texture = MonoGameTexture.Unwrap(state.Texture);
            if (state.ColorMode == ColorMode.Sprite)
            {
                _spriteTexture.SetValue(texture);
                _spriteMatrix.SetValue(transform);
                _spriteEffect.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                _primitiveEffect.Projection = transform;
                _primitiveEffect.Alpha = state.Opacity;
                _primitiveEffect.TextureEnabled = texture != null;
                if (texture != null)
                {
                    _primitiveEffect.Texture = texture;
                }
                _primitiveEffect.CurrentTechnique.Passes[0].Apply();
            }
            _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, indexCount / 3, VertexPositionColorTexture.VertexDeclaration);
        }

        public void Dispose()
        {
            _spriteEffect.Dispose();
            _primitiveEffect.Dispose();
        }

        private static BlendState ToBlendState(BlendMode blend)
        {
            return blend switch
            {
                BlendMode.AlphaBlend => BlendState.AlphaBlend,
                BlendMode.Additive => BlendState.Additive,
                BlendMode.NonPremultiplied => BlendState.NonPremultiplied,
                _ => throw new ArgumentOutOfRangeException(nameof(blend), blend, null)
            };
        }

        private static SamplerState ToSamplerState(SamplerMode sampler)
        {
            return sampler == SamplerMode.LinearWrap ? SamplerState.LinearWrap : SamplerState.LinearClamp;
        }

        private static Effect LoadEffect(GraphicsDevice device, IFileLoader files, string path)
        {
            using Stream stream = files.OpenFile(path);
            byte[] bytes = new byte[stream.Length];
            stream.ReadExactly(bytes);
            return new Effect(device, bytes);
        }
    }
}

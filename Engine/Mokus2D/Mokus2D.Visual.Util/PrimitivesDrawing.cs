using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Util;

public struct PrimitivesDrawing : IDisposable
{
    private static BasicEffect Effect;

    private BlendState oldState = BlendState.AlphaBlend;

    public static void Clear()
    {
        Effect = null;
    }

    public PrimitivesDrawing(VisualState state)
        : this(state, state.Matrix)
    {
    }

    public PrimitivesDrawing(VisualState state, Matrix matrix, Texture2D texture = null)
    {
        BeginDrawPrimitives(state, matrix, texture);
    }

    public void Dispose()
    {
        EndDrawPrimitives();
    }

    public void BeginDrawPrimitives(VisualState state)
    {
        BeginDrawPrimitives(state, state.Matrix);
    }

    public void BeginDrawPrimitives(VisualState state, Matrix matrix, Texture2D texture)
    {
        if (texture != null)
        {
            Mokus2DGame.Device.SamplerStates[0] = SamplerState.LinearWrap;
        }
        oldState = Mokus2DGame.Device.BlendState;
        Mokus2DGame.Device.BlendState = BlendState.NonPremultiplied;
        if (Effect == null)
        {
            Effect = new BasicEffect(Mokus2DGame.Device);
        }
        Effect.Projection = matrix;
        Effect.World = Matrix.Identity;
        Effect.View = Matrix.Identity;
        Effect.Alpha = state.Opacity;
        Effect.VertexColorEnabled = true;
        Effect.TextureEnabled = texture != null;
        if (texture != null)
        {
            Effect.Texture = texture;
        }
        Effect.CurrentTechnique.Passes[0].Apply();
    }

    public void BeginDrawPrimitives(VisualState state, Matrix matrix)
    {
        BeginDrawPrimitives(state, matrix, null);
    }

    public void EndDrawPrimitives()
    {
        Mokus2DGame.Device.BlendState = oldState;
    }
}

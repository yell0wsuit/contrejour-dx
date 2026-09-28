using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Particles.Grid;

public class GridSystem : Node
{
    private readonly Vector2 gridSize;

    private readonly Vector2 cellSize;

    private readonly MovieClipData data;

    public Vector2 GridSize => gridSize;

    public static GridSystem CreateGrid(ISizeNode source, Vector2 gridSize, RootNode root = null)
    {
        return CreateGrid((Node)source, source.Size, gridSize);
    }

    public static GridSystem CreateGrid(Node source, Vector2 gridSize, RenderSprite renderSprite)
    {
        Node parent = source.Parent;
        int layer = source.Layer;
        source.RemoveFromParent();
        renderSprite.AddChild(source);
        renderSprite.UpdateAndDraw();
        renderSprite.RemoveChild(source);
        parent?.AddChild(source, layer);
        return new GridSystem(renderSprite.Texture, gridSize);
    }

    public static GridSystem CreateGrid(Node source, Vector2 size, Vector2 gridSize, RootNode root = null)
    {
        RenderSprite renderSprite = (root != null) ? new RenderSprite(size, root) : new RenderSprite(size);
        return CreateGrid(source, gridSize, renderSprite);
    }

    private static MovieClipData CreateGrid(Texture2D texture, Vector2 gridSize)
    {
        gridSize = gridSize.ToIntVector();
        MovieClipData movieClipData = new(null)
        {
            Texture = texture
        };
        Vector2 size = GetCellSize(texture, gridSize);
        for (int i = 0; i < gridSize.Y; i++)
        {
            for (int j = 0; j < gridSize.X; j++)
            {
                FrameData item = new()
                {
                    Anchor = new Vector2(0.5f),
                    Rect = new Rectangle((int)(j * size.X), (int)(i * size.Y), (int)size.X, (int)size.Y)
                };
                movieClipData.Frames.Add(item);
            }
        }
        movieClipData.Size = size;
        return movieClipData;
    }

    private static Vector2 GetCellSize(Texture2D texture, Vector2 gridSize)
    {
        return new Vector2((float)Math.Ceiling(texture.Width / (float)(int)gridSize.X), (float)Math.Ceiling(texture.Height / (float)(int)gridSize.Y));
    }

    public GridSystem(Texture2D texture, Vector2 gridSize, bool createParticles = true)
    {
        data = CreateGrid(texture, gridSize);
        this.gridSize = gridSize.ToIntVector();
        cellSize = GetCellSize(texture, gridSize);
        if (createParticles)
        {
            CreateParticles();
        }
    }

    public Node GetParticle(int x, int y)
    {
        return Children[(int)((y * gridSize.X) + x)];
    }

    public void CreateParticles()
    {
        for (int i = 0; i < gridSize.Y; i++)
        {
            for (int j = 0; j < gridSize.X; j++)
            {
                Node particle = AddParticle(j, i);
                ResetParticlePosition(particle, j, i);
            }
        }
    }

    public void ResetTransformations()
    {
        for (int i = 0; i < gridSize.Y; i++)
        {
            for (int j = 0; j < gridSize.X; j++)
            {
                Node particle = GetParticle(j, i);
                particle.ScaleVec = Vector2.One;
                particle.OpacityFloat = 1f;
                particle.RotationRadians = 0f;
                ResetParticlePosition(particle, j, i);
            }
        }
    }

    private void ResetParticlePosition(Node particle, int x, int y)
    {
        particle.Position = new Vector2(x * cellSize.X, y * cellSize.Y);
    }

    public Node AddParticle(int x, int y)
    {
        return AddParticle((int)((y * gridSize.X) + x));
    }

    public virtual Node AddParticle(int frame)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(frame, data.Frames.Count);
        OneFrameSprite oneFrameSprite = new(data, frame);
        AddChild(oneFrameSprite);
        return oneFrameSprite;
    }

    public Vector2 GetParticlePosition(int index)
    {
        return new Vector2(index % gridSize.X, (int)(index / gridSize.X) % gridSize.Y);
    }
}

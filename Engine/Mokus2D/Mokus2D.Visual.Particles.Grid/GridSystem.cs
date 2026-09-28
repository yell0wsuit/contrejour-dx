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

    private readonly IMovieClipData data;

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
        RenderSprite renderSprite = ((root != null) ? new RenderSprite(size, root) : new RenderSprite(size));
        return CreateGrid(source, gridSize, renderSprite);
    }

    private static IMovieClipData CreateGrid(Texture2D texture, Vector2 gridSize)
    {
        gridSize = gridSize.ToIntVector();
        MovieClipData movieClipData = new MovieClipData(null);
        movieClipData.Texture = texture;
        Vector2 size = GetCellSize(texture, gridSize);
        for (int i = 0; (float)i < gridSize.Y; i++)
        {
            for (int j = 0; (float)j < gridSize.X; j++)
            {
                FrameData item = new FrameData
                {
                    Anchor = new Vector2(0.5f),
                    Rect = new Rectangle((int)((float)j * size.X), (int)((float)i * size.Y), (int)size.X, (int)size.Y)
                };
                movieClipData.Frames.Add(item);
            }
        }
        movieClipData.Size = size;
        return movieClipData;
    }

    private static Vector2 GetCellSize(Texture2D texture, Vector2 gridSize)
    {
        return new Vector2((float)Math.Ceiling((float)texture.Width / (float)(int)gridSize.X), (float)Math.Ceiling((float)texture.Height / (float)(int)gridSize.Y));
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
        return base.Children[(int)((float)y * gridSize.X + (float)x)];
    }

    public void CreateParticles()
    {
        for (int i = 0; (float)i < gridSize.Y; i++)
        {
            for (int j = 0; (float)j < gridSize.X; j++)
            {
                Node particle = AddParticle(j, i);
                ResetParticlePosition(particle, j, i);
            }
        }
    }

    public void ResetTransformations()
    {
        for (int i = 0; (float)i < gridSize.Y; i++)
        {
            for (int j = 0; (float)j < gridSize.X; j++)
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
        particle.Position = new Vector2((float)x * cellSize.X, (float)y * cellSize.Y);
    }

    public Node AddParticle(int x, int y)
    {
        return AddParticle((int)((float)y * gridSize.X + (float)x));
    }

    public virtual Node AddParticle(int frame)
    {
        if (frame >= data.Frames.Count)
        {
            throw new ArgumentOutOfRangeException("frame");
        }
        OneFrameSprite oneFrameSprite = new OneFrameSprite(data, frame);
        AddChild(oneFrameSprite);
        return oneFrameSprite;
    }

    public Vector2 GetParticlePosition(int index)
    {
        return new Vector2((float)index % gridSize.X, (float)(int)((float)index / gridSize.X) % gridSize.Y);
    }
}

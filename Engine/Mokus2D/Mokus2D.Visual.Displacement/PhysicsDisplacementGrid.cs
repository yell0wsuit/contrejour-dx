using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Displacement;

public class PhysicsDisplacementGrid : DisplacementGrid
{
    private PhysicsNodeData[,] _nodesData;

    private float InteractionCoeff = 1f;

    private float RecoveryVelocity;

    private Vector2 _defaultDistances;

    private float _diagonalDistance;

    public float DampingRatio
    {
        get;
        set
        {
            if (!value.Between(0f, 1f))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Damping should be between 0 and 1");
            }
            field = value;
        }
    }

    public PhysicsDisplacementGrid(string name, Size gridSize)
        : base(name, gridSize)
    {
        Initialize(gridSize);
    }

    public PhysicsDisplacementGrid(ISpriteData data, Size gridSize)
        : base(data, gridSize)
    {
        Initialize(gridSize);
    }

    public PhysicsDisplacementGrid(Texture2D texture, Size gridSize, float scaleFactor = 1f)
        : base(texture, gridSize, scaleFactor)
    {
        Initialize(gridSize);
    }

    public PhysicsDisplacementGrid(Texture2D texture, Rectangle textureRect, Size gridSize, float scaleFactor = 1f)
        : base(texture, textureRect, gridSize, scaleFactor)
    {
        Initialize(gridSize);
    }

    public override void Update(float time)
    {
        base.Update(time);
        UpdatePositions(time);
        CalculateAccelerations();
        ApplyAccelerations(time);
    }

    private void ApplyAccelerations(float time)
    {
        for (int i = 1; i < GridSize.Width - 1; i++)
        {
            for (int j = 1; j < GridSize.Height - 1; j++)
            {
                _nodesData[i, j].Velocity += _nodesData[i, j].Acceleration * time * (1f - DampingRatio);
            }
        }
    }

    private void CalculateAccelerations()
    {
        for (int i = 0; i < GridSize.Width; i++)
        {
            for (int j = 0; j < GridSize.Height - 1; j++)
            {
                PhysicsNodeData currentData = _nodesData[i, j];
                Vector2 currentPosition = base[i, j];
                if (i < GridSize.Width - 1)
                {
                    CalculateInteraction(i, j, i + 1, j, currentPosition, currentData, cleanAcceleration: false, _defaultDistances.X);
                    CalculateInteraction(i, j, i + 1, j + 1, currentPosition, currentData, cleanAcceleration: true, _diagonalDistance);
                }
                CalculateInteraction(i, j, i, j + 1, currentPosition, currentData, cleanAcceleration: false, _defaultDistances.Y);
                if (i > 0)
                {
                    CalculateInteraction(i, j, i - 1, j + 1, currentPosition, currentData, cleanAcceleration: false, _diagonalDistance);
                }
            }
        }
    }

    private void CalculateInteraction(int w, int h, int nextW, int nextH, Vector2 currentPosition, PhysicsNodeData currentData, bool cleanAcceleration, float targetDistance)
    {
        PhysicsNodeData physicsNodeData = _nodesData[nextW, nextH];
        if (cleanAcceleration)
        {
            physicsNodeData.Acceleration = Vector2.Zero;
        }
        Vector2 vector = base[nextW, nextH];
        Vector2 vector2 = vector - currentPosition;
        Vector2 vector3 = vector2.Normalize(targetDistance);
        Vector2 vector4 = vector3 - vector2;
        if ((nextW - w).Sign() == (currentPosition.X - vector.X).Sign())
        {
            vector4.X = 0f;
        }
        if ((nextH - h).Sign() == (currentPosition.Y - vector.Y).Sign())
        {
            vector4.Y = 0f;
        }
        Vector2 vector5 = vector4 * InteractionCoeff;
        currentData.Acceleration -= vector5;
        physicsNodeData.Acceleration += vector5;
    }

    private void UpdatePositions(float time)
    {
        for (int i = 1; i < GridSize.Width - 1; i++)
        {
            for (int j = 1; j < GridSize.Height - 1; j++)
            {
                Vector2 source = base[i, j];
                PhysicsNodeData physicsNodeData = _nodesData[i, j];
                source += physicsNodeData.Velocity * time;
                source = source.StepTo(physicsNodeData.DefaultPosition, RecoveryVelocity * time);
                base[i, j] = source;
            }
        }
    }

    private void Initialize(Size gridSize)
    {
        _nodesData = new PhysicsNodeData[gridSize.Width, gridSize.Height];
        _defaultDistances = TextureRect.Size() / (gridSize - Vector2.One);
        _diagonalDistance = _defaultDistances.Length();
        for (int i = 0; i < GridSize.Width; i++)
        {
            for (int j = 0; j < GridSize.Height; j++)
            {
                _nodesData[i, j] = new PhysicsNodeData(base[i, j]);
            }
        }
    }
}

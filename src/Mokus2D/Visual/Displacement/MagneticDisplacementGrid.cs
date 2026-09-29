using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Displacement.Magnets;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Displacement
{
    public class MagneticDisplacementGrid : DisplacementGrid
    {
        private readonly float Velocity = 100f;

        private readonly bool StaticBorders = true;

        public Vector2 MagnetsOffset { get; set; }

        private readonly float PowerMult = 1f;

        private MagneticNodeData[,] _nodesData;

        private readonly List<IGridMagnet> _magnets = new(64);

        private readonly List<IGridMagnet> _toRemove = new(64);

        private bool _targetsDirty;

        public MagneticDisplacementGrid(string name, Size gridSize)
            : base(name, gridSize)
        {
            Initialize();
        }

        public MagneticDisplacementGrid(ISpriteData data, Size gridSize)
            : base(data, gridSize)
        {
            Initialize();
        }

        public MagneticDisplacementGrid(Texture2D texture, Size gridSize, float scaleFactor = 1f)
            : base(texture, gridSize, scaleFactor)
        {
            Initialize();
        }

        public MagneticDisplacementGrid(Texture2D texture, Rectangle textureRect, Size gridSize, float scaleFactor = 1f)
            : base(texture, textureRect, gridSize, scaleFactor)
        {
            Initialize();
        }

        public void AddMagnet(IGridMagnet magnet)
        {
            _magnets.Add(magnet);
        }

        public void RemoveMagnet(IGridMagnet magnet)
        {
            _ = _magnets.Remove(magnet);
        }

        public void ClearMagnets()
        {
            _magnets.Clear();
        }

        public override void Update(float time)
        {
            base.Update(time);
            CleanTargetPositions();
            foreach (IGridMagnet magnet in _magnets)
            {
                magnet.Update(time);
                if (!magnet.HasRemove)
                {
                    CalculateMagnetForces(magnet);
                }
                else
                {
                    _toRemove.Add(magnet);
                }
            }
            ApplyForces(time);
            _magnets.RemoveListNoGarbage(_toRemove);
            _toRemove.Clear();
        }

        private void ApplyForces(float time)
        {
            float step = Velocity * time;
            for (int i = 0; i < GridSize.Width; i++)
            {
                for (int j = 0; j < GridSize.Height; j++)
                {
                    base[i, j] = base[i, j].StepTo(_nodesData[i, j].TargetPosition, step);
                }
            }
        }

        private void CalculateMagnetForces(IGridMagnet magnet)
        {
            Vector2 vector = magnet.Position + MagnetsOffset;
            Vector2 position = VectorExtensions.Ceiling((vector + magnet.Bounds.LeftTop()) / NodeSize);
            Vector2 position2 = VectorExtensions.Floor((vector + magnet.Bounds.RightBottom()) / NodeSize);
            int num = StaticBorders ? 1 : 0;
            int num2 = (!StaticBorders) ? 1 : 2;
            position = position.Clamp(new Vector2(num), GridSize);
            position2 = position2.Clamp(Vector2.Zero, GridSize - new Vector2(num2));
            for (int i = (int)position.X; i <= position2.X; i++)
            {
                for (int j = (int)position.Y; j <= position2.Y; j++)
                {
                    MagneticNodeData magneticNodeData = _nodesData[i, j];
                    Vector2 vector2 = magnet.GetForce(magneticNodeData.DefaultPosition - vector) * PowerMult;
                    magneticNodeData.TargetPosition += vector2;
                }
            }
        }

        private void CleanTargetPositions()
        {
            if (_targetsDirty)
            {
                MagneticNodeData[,] nodesData = _nodesData;
                foreach (MagneticNodeData magneticNodeData in nodesData)
                {
                    magneticNodeData.Clean();
                }
            }
            _targetsDirty = _magnets.Count > 0;
        }

        private void Initialize()
        {
            _nodesData = new MagneticNodeData[GridSize.Width, GridSize.Height];
            for (int i = 0; i < GridSize.Width; i++)
            {
                for (int j = 0; j < GridSize.Height; j++)
                {
                    _nodesData[i, j] = new MagneticNodeData(base[i, j]);
                }
            }
        }
    }
}

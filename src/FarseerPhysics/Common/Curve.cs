// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// Ported from MonoGame 3.8.5.1 for Farseer's AbstractForceController.

using System;

namespace FarseerPhysics.Common
{
    /// <summary>
    /// Contains a collection of <see cref="CurveKey"/> points in 2D space and provides methods for evaluating features of the curve they define.
    /// </summary>
    public class Curve
    {
        #region Private Fields


        #endregion

        #region Public Properties

        /// <summary>
        /// Returns <c>true</c> if this curve is constant (has zero or one points); <c>false</c> otherwise.
        /// </summary>
        public bool IsConstant => Keys.Count <= 1;

        /// <summary>
        /// Defines how to handle weighting values that are less than the first control point in the curve.
        /// </summary>
        public CurveLoopType PreLoop { get; set; }

        /// <summary>
        /// Defines how to handle weighting values that are greater than the last control point in the curve.
        /// </summary>
        public CurveLoopType PostLoop { get; set; }

        /// <summary>
        /// The collection of curve keys.
        /// </summary>
        public CurveKeyCollection Keys { get; private set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Constructs a curve.
        /// </summary>
        public Curve()
        {
            Keys = [];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a copy of this curve.
        /// </summary>
        /// <returns>A copy of this curve.</returns>
        public Curve Clone()
        {
            Curve curve = new()
            {
                Keys = Keys.Clone(),
                PreLoop = PreLoop,
                PostLoop = PostLoop
            };

            return curve;
        }

        /// <summary>
        /// Evaluate the value at a position of this <see cref="Curve"/>.
        /// </summary>
        /// <param name="position">The position on this <see cref="Curve"/>.</param>
        /// <returns>Value at the position on this <see cref="Curve"/>.</returns>
        public float Evaluate(float position)
        {
            if (Keys.Count == 0)
            {
                return 0f;
            }

            if (Keys.Count == 1)
            {
                return Keys[0].Value;
            }

            CurveKey first = Keys[0];
            CurveKey last = Keys[^1];

            if (position < first.Position)
            {
                switch (PreLoop)
                {
                    case CurveLoopType.Constant:
                        //constant
                        return first.Value;

                    case CurveLoopType.Linear:
                        // linear y = a*x +b with a tangeant of last point
                        return first.Value - (first.TangentIn * (first.Position - position));

                    case CurveLoopType.Cycle:
                        //start -> end / start -> end
                        int cycle = GetNumberOfCycle(position);
                        float virtualPos = position - (cycle * (last.Position - first.Position));
                        return GetCurvePosition(virtualPos);

                    case CurveLoopType.CycleOffset:
                        //make the curve continue (with no step) so must up the curve each cycle of delta(value)
                        cycle = GetNumberOfCycle(position);
                        virtualPos = position - (cycle * (last.Position - first.Position));
                        return GetCurvePosition(virtualPos) + (cycle * (last.Value - first.Value));

                    case CurveLoopType.Oscillate:
                        //go back on curve from end and target start 
                        // start-> end / end -> start
                        cycle = GetNumberOfCycle(position);
                        virtualPos = 0 == cycle % 2f
                            ? position - (cycle * (last.Position - first.Position))
                            : last.Position - position + first.Position + (cycle * (last.Position - first.Position));

                        return GetCurvePosition(virtualPos);
                    default:
                        break;
                }
            }
            else if (position > last.Position)
            {
                int cycle;
                switch (PostLoop)
                {
                    case CurveLoopType.Constant:
                        //constant
                        return last.Value;

                    case CurveLoopType.Linear:
                        // linear y = a*x +b with a tangeant of last point
                        return last.Value + (first.TangentOut * (position - last.Position));

                    case CurveLoopType.Cycle:
                        //start -> end / start -> end
                        cycle = GetNumberOfCycle(position);
                        float virtualPos = position - (cycle * (last.Position - first.Position));
                        return GetCurvePosition(virtualPos);

                    case CurveLoopType.CycleOffset:
                        //make the curve continue (with no step) so must up the curve each cycle of delta(value)
                        cycle = GetNumberOfCycle(position);
                        virtualPos = position - (cycle * (last.Position - first.Position));
                        return GetCurvePosition(virtualPos) + (cycle * (last.Value - first.Value));

                    case CurveLoopType.Oscillate:
                        //go back on curve from end and target start 
                        // start-> end / end -> start
                        cycle = GetNumberOfCycle(position);
                        _ = position - (cycle * (last.Position - first.Position));
                        virtualPos = 0 == cycle % 2f
                            ? position - (cycle * (last.Position - first.Position))
                            : last.Position - position + first.Position + (cycle * (last.Position - first.Position));

                        return GetCurvePosition(virtualPos);
                    default:
                        break;
                }
            }

            //in curve
            return GetCurvePosition(position);
        }

        /// <summary>
        /// Computes tangents for all keys in the collection.
        /// </summary>
        /// <param name="tangentType">The tangent type for both in and out.</param>
		public void ComputeTangents(CurveTangent tangentType)
        {
            ComputeTangents(tangentType, tangentType);
        }

        /// <summary>
        /// Computes tangents for all keys in the collection.
        /// </summary>
        /// <param name="tangentInType">The tangent in-type. <see cref="CurveKey.TangentIn"/> for more details.</param>
        /// <param name="tangentOutType">The tangent out-type. <see cref="CurveKey.TangentOut"/> for more details.</param>
        public void ComputeTangents(CurveTangent tangentInType, CurveTangent tangentOutType)
        {
            for (int i = 0; i < Keys.Count; ++i)
            {
                ComputeTangent(i, tangentInType, tangentOutType);
            }
        }

        /// <summary>
        /// Computes tangent for the specific key in the collection.
        /// </summary>
        /// <param name="keyIndex">The index of a key in the collection.</param>
        /// <param name="tangentType">The tangent type for both in and out.</param>
        public void ComputeTangent(int keyIndex, CurveTangent tangentType)
        {
            ComputeTangent(keyIndex, tangentType, tangentType);
        }

        /// <summary>
        /// Computes tangent for the specific key in the collection.
        /// </summary>
        /// <param name="keyIndex">The index of key in the collection.</param>
        /// <param name="tangentInType">The tangent in-type. <see cref="CurveKey.TangentIn"/> for more details.</param>
        /// <param name="tangentOutType">The tangent out-type. <see cref="CurveKey.TangentOut"/> for more details.</param>
        public void ComputeTangent(int keyIndex, CurveTangent tangentInType, CurveTangent tangentOutType)
        {
            // See http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.curvetangent.aspx

            CurveKey key = Keys[keyIndex];

            float p0, p, p1;
            p0 = p = p1 = key.Position;

            float v0, v, v1;
            v0 = v = v1 = key.Value;

            if (keyIndex > 0)
            {
                p0 = Keys[keyIndex - 1].Position;
                v0 = Keys[keyIndex - 1].Value;
            }

            if (keyIndex < Keys.Count - 1)
            {
                p1 = Keys[keyIndex + 1].Position;
                v1 = Keys[keyIndex + 1].Value;
            }

            switch (tangentInType)
            {
                case CurveTangent.Flat:
                    key.TangentIn = 0;
                    break;
                case CurveTangent.Linear:
                    key.TangentIn = v - v0;
                    break;
                case CurveTangent.Smooth:
                    float pn = p1 - p0;
                    key.TangentIn = Math.Abs(pn) < float.Epsilon ? 0 : (v1 - v0) * ((p - p0) / pn);

                    break;
                default:
                    break;
            }

            switch (tangentOutType)
            {
                case CurveTangent.Flat:
                    key.TangentOut = 0;
                    break;
                case CurveTangent.Linear:
                    key.TangentOut = v1 - v;
                    break;
                case CurveTangent.Smooth:
                    float pn = p1 - p0;
                    key.TangentOut = Math.Abs(pn) < float.Epsilon ? 0 : (v1 - v0) * ((p1 - p) / pn);

                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Private Methods

        private int GetNumberOfCycle(float position)
        {
            float cycle = (position - Keys[0].Position) / (Keys[^1].Position - Keys[0].Position);
            if (cycle < 0f)
            {
                cycle--;
            }

            return (int)cycle;
        }

        private float GetCurvePosition(float position)
        {
            //only for position in curve
            int nextIndex = Keys.IndexAtPosition(position);
            if (nextIndex < 0)
            {
                nextIndex = ~nextIndex;
            }

            nextIndex = Math.Max(nextIndex, 1);
            CurveKey prev = Keys[nextIndex - 1];
            CurveKey next = Keys[nextIndex];
            if (prev.Continuity == CurveContinuity.Step)
            {
                return position >= 1f ? next.Value : prev.Value;
            }
            float t = (position - prev.Position) / (next.Position - prev.Position);//to have t in [0,1]
            float ts = t * t;
            float tss = ts * t;
            //After a lot of search on internet I have found all about spline function
            // and bezier (phi'sss ancien) but finaly use hermite curve 
            //http://en.wikipedia.org/wiki/Cubic_Hermite_spline
            //P(t) = (2*t^3 - 3t^2 + 1)*P0 + (t^3 - 2t^2 + t)m0 + (-2t^3 + 3t^2)P1 + (t^3-t^2)m1
            //with P0.value = prev.value , m0 = prev.tangentOut, P1= next.value, m1 = next.TangentIn
            return (((2 * tss) - (3 * ts) + 1f) * prev.Value) + ((tss - (2 * ts) + t) * prev.TangentOut) + (((3 * ts) - (2 * tss)) * next.Value) + ((tss - ts) * next.TangentIn);
        }

        #endregion
    }

}

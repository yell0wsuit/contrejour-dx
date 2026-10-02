// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// Ported from MonoGame 3.8.5.1 for Farseer's AbstractForceController.

using System;
using System.Diagnostics.CodeAnalysis;

namespace FarseerPhysics.Common
{
    /// <summary>
    /// Key point on the <see cref="Curve"/>.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="CurveKey"/> class.
    /// </remarks>
    /// <param name="position">Position on the curve.</param>
    /// <param name="value">Value of the control point.</param>
    /// <param name="tangentIn">Tangent approaching point from the previous point on the curve.</param>
    /// <param name="tangentOut">Tangent leaving point toward next point on the curve.</param>
    /// <param name="continuity">Indicates whether the curve is discrete or continuous.</param>
    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "Ported MonoGame API: keys are only ordered through CompareTo.")]
    public class CurveKey(float position, float value, float tangentIn, float tangentOut, CurveContinuity continuity) : IEquatable<CurveKey>, IComparable<CurveKey>
    {
        #region Private Fields

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the indicator whether the segment between this point and the next point on the curve is discrete or continuous.
        /// </summary>
        public CurveContinuity Continuity { get; set; } = continuity;

        /// <summary>
        /// Gets a position of the key on the curve.
        /// </summary>
        public float Position { get; } = position;

        /// <summary>
        /// Gets or sets a tangent when approaching this point from the previous point on the curve.
        /// </summary>
        public float TangentIn { get; set; } = tangentIn;

        /// <summary>
        /// Gets or sets a tangent when leaving this point to the next point on the curve.
        /// </summary>
        public float TangentOut { get; set; } = tangentOut;

        /// <summary>
        /// Gets a value of this point.
        /// </summary>
        public float Value { get; set; } = value;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CurveKey"/> class with position: 0 and value: 0.
        /// </summary>
        public CurveKey() : this(0, 0)
        {
            // This parameterless constructor is needed for correct serialization of CurveKeyCollection and CurveKey.
        }

        /// <summary>
        /// Creates a new instance of <see cref="CurveKey"/> class.
        /// </summary>
        /// <param name="position">Position on the curve.</param>
        /// <param name="value">Value of the control point.</param>
        public CurveKey(float position, float value)
            : this(position, value, 0, 0, CurveContinuity.Smooth)
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="CurveKey"/> class.
        /// </summary>
        /// <param name="position">Position on the curve.</param>
        /// <param name="value">Value of the control point.</param>
        /// <param name="tangentIn">Tangent approaching point from the previous point on the curve.</param>
        /// <param name="tangentOut">Tangent leaving point toward next point on the curve.</param>
        public CurveKey(float position, float value, float tangentIn, float tangentOut)
            : this(position, value, tangentIn, tangentOut, CurveContinuity.Smooth)
        {

        }

        #endregion

        /// <summary>
        /// 
        /// Compares whether two <see cref="CurveKey"/> instances are not equal.
        /// </summary>
        /// <param name="value1"><see cref="CurveKey"/> instance on the left of the not equal sign.</param>
        /// <param name="value2"><see cref="CurveKey"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
        public static bool operator !=(CurveKey value1, CurveKey value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Compares whether two <see cref="CurveKey"/> instances are equal.
        /// </summary>
        /// <param name="value1"><see cref="CurveKey"/> instance on the left of the equal sign.</param>
        /// <param name="value2"><see cref="CurveKey"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(CurveKey value1, CurveKey value2)
        {
            return Equals(value1, null)
                ? Equals(value2, null)
                : Equals(value2, null)
                ? Equals(value1, null)
                : (value1.Position == value2.Position)
                && (value1.Value == value2.Value)
                && (value1.TangentIn == value2.TangentIn)
                && (value1.TangentOut == value2.TangentOut)
                && (value1.Continuity == value2.Continuity);
        }

        /// <summary>
        /// Creates a copy of this key.
        /// </summary>
        /// <returns>A copy of this key.</returns>
        public CurveKey Clone()
        {
            return new CurveKey(Position, Value, TangentIn, TangentOut, Continuity);
        }

        #region Inherited Methods

        /// <inheritdoc/>
        public int CompareTo(CurveKey other)
        {
            return Position.CompareTo(other.Position);
        }

        /// <inheritdoc/>
        public bool Equals(CurveKey other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return (obj as CurveKey) != null && Equals((CurveKey)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Value.GetHashCode() ^ TangentIn.GetHashCode() ^
                TangentOut.GetHashCode() ^ Continuity.GetHashCode();
        }

        #endregion
    }
}

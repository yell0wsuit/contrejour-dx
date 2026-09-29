// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// Ported from MonoGame 3.8.5.1 for Farseer's AbstractForceController.

namespace FarseerPhysics.Common
{
    /// <summary>
    /// Defines the different tangent types to be calculated for <see cref="CurveKey"/> points in a <see cref="Curve"/>.
    /// </summary>
	public enum CurveTangent
    {
        /// <summary>
        /// The tangent which always has a value equal to zero. 
        /// </summary>
		Flat,
        /// <summary>
        /// The tangent which contains a difference between current tangent value and the tangent value from the previous <see cref="CurveKey"/>. 
        /// </summary>
		Linear,
        /// <summary>
        /// The smoouth tangent which contains the inflection between <see cref="CurveKey.TangentIn"/> and <see cref="CurveKey.TangentOut"/> by taking into account the values of both neighbors of the <see cref="CurveKey"/>.
        /// </summary>
		Smooth
    }
}

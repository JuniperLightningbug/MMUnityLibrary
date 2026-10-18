using System.Runtime.CompilerServices;
using UnityEngine;

namespace MM
{
	public static class MathsUtils
	{
#region Constants
		// Maths constants
		public const float kSqrt2_Reciprocal = 0.7071f;
		
		// Utility constants
		public const float kFloatEpsilon = 0.0001f;
#endregion
		
#region Comparison
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static float Difference( float a, float b ) =>
			Mathf.Abs( a - b );
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool Approximately( float a, float b, float epsilon = kFloatEpsilon ) =>
			Mathf.Abs( a - b ) <= epsilon;
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool Approximately( Vector2 a, Vector2 b, float epsilon = kFloatEpsilon ) =>
			Difference(a.x, b.x) +
			Difference(a.y, b.y) < kFloatEpsilon;
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool ApproximatelyAll( Vector2 a, Vector2 b, float epsilon = kFloatEpsilon ) =>
			Approximately(a.x, b.x, kFloatEpsilon) &&
			Approximately(a.y, b.y, kFloatEpsilon);
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool Approximately( Vector3 a, Vector3 b, float epsilon = kFloatEpsilon ) =>
			Difference(a.x, b.x) +
			Difference(a.y, b.y) +
			Difference(a.z, b.z) < kFloatEpsilon;		
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool ApproximatelyAll( Vector3 a, Vector3 b, float epsilon = kFloatEpsilon ) =>
			Approximately(a.x, b.x, kFloatEpsilon) &&
			Approximately(a.y, b.y, kFloatEpsilon) &&
			Approximately(a.z, b.z, kFloatEpsilon);		
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool Approximately( Quaternion a, Quaternion b, float epsilon = kFloatEpsilon ) =>
			Difference(a.x, b.x) +
			Difference(a.y, b.y) +
			Difference(a.z, b.z) +
			Difference(a.w, b.w) < kFloatEpsilon;
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool ApproximatelyAll( Quaternion a, Quaternion b, float epsilon = kFloatEpsilon ) =>
			Approximately(a.x, b.x, kFloatEpsilon) &&
			Approximately(a.y, b.y, kFloatEpsilon) &&
			Approximately(a.z, b.z, kFloatEpsilon) &&
			Approximately(a.w, b.w, kFloatEpsilon);

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool Approximately( Matrix4x4 a, Matrix4x4 b, float epsilon = kFloatEpsilon ) =>
			Difference( a.m00, b.m00 ) +
			Difference( a.m01, b.m01 ) +
			Difference( a.m02, b.m02 ) +
			Difference( a.m03, b.m03 ) +
			Difference( a.m10, b.m10 ) +
			Difference( a.m11, b.m11 ) +
			Difference( a.m12, b.m12 ) +
			Difference( a.m13, b.m13 ) +
			Difference( a.m20, b.m20 ) +
			Difference( a.m21, b.m21 ) +
			Difference( a.m22, b.m22 ) +
			Difference( a.m23, b.m23 ) +
			Difference( a.m30, b.m30 ) +
			Difference( a.m31, b.m31 ) +
			Difference( a.m32, b.m32 ) +
			Difference( a.m33, b.m33 ) < epsilon;
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool ApproximatelyAll( Matrix4x4 a, Matrix4x4 b, float epsilon = kFloatEpsilon ) =>
			Approximately( a.m00, b.m00, kFloatEpsilon ) &&
			Approximately( a.m01, b.m01, kFloatEpsilon ) &&
			Approximately( a.m02, b.m02, kFloatEpsilon ) &&
			Approximately( a.m03, b.m03, kFloatEpsilon ) &&
			Approximately( a.m10, b.m10, kFloatEpsilon ) &&
			Approximately( a.m11, b.m11, kFloatEpsilon ) &&
			Approximately( a.m12, b.m12, kFloatEpsilon ) &&
			Approximately( a.m13, b.m13, kFloatEpsilon ) &&
			Approximately( a.m20, b.m20, kFloatEpsilon ) &&
			Approximately( a.m21, b.m21, kFloatEpsilon ) &&
			Approximately( a.m22, b.m22, kFloatEpsilon ) &&
			Approximately( a.m23, b.m23, kFloatEpsilon ) &&
			Approximately( a.m30, b.m30, kFloatEpsilon ) &&
			Approximately( a.m31, b.m31, kFloatEpsilon ) &&
			Approximately( a.m32, b.m32, kFloatEpsilon ) &&
			Approximately( a.m33, b.m33, kFloatEpsilon );
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool PositiveNonZero( float a, float epsilon = kFloatEpsilon ) =>
			a > epsilon;
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static float Max( Vector3 v ) => Mathf.Max(v.x, v.y, v.z);
		
#endregion

#region Int Operations

		/// <summary>
		/// Int variant of Mathf.Repeat
		/// 
		/// Similar to int modulo, but repeats input -ve numbers on a positive output range.
		/// Default % behaviour: -1 % 4 = -1; -5 % 4 = -1
		/// RepeatInt behaviour: -1 "%" 4 = 3; -5 "%" 4 = 3
		/// </summary>
		/// <param name="value"></param>
		/// <param name="length">Exclusive upper bound; Must be > 0</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static int RepeatInt( int value, int length )
		{
			int remainder = value % length;
			return remainder < 0 ? remainder + length : remainder;
		}

		/// <summary>
		/// Variant of Mathf.Repeat with bounded range [lower, upper)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min">Inclusive lower bound</param>
		/// <param name="max">Exclusive upper bound; Must be > 'min'</param>
		public static float Repeat( float value, float min, float max ) =>
			min + Mathf.Repeat(value - min, max - min);
		
		/// <summary>
		/// Int variant of Mathf.Repeat with bounded range [lower, upper)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min">Inclusive lower bound</param>
		/// <param name="max">Exclusive upper bound; Must be > 'min'</param>
		public static int Repeat( int value, int min, int max ) =>
			min + RepeatInt(value - min, max - min);
		
		/// <summary>
		/// Floor-division for ints, rounding downwards towards negative infinity
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static int DivideFloor( int value, int divisor ) =>
			(value % divisor != 0) &&		// Non-zero remainder
			(value < 0 != divisor < 0) ?	// -ve quotient
				value / divisor - 1 :		// Round -ve values down
				value / divisor;

		/// <summary>
		/// Ceil-division for ints, rounding upwards towards positive infinity without float casts
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static int DivideCeil( int value, int divisor ) =>
			(value % divisor != 0) &&		// Non-zero remainder
			(value < 0 == divisor < 0) ?	// +ve quotient
				value / divisor + 1 :		// Round +ve values up
				value / divisor;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static float Overflow( float value, float min, float max ) => value - Mathf.Clamp( value, min, max );
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static float Overflow01( float value ) => value - Mathf.Clamp01( value );

#endregion
	}
}
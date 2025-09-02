using System.Collections.Generic;
using UnityEngine;

namespace MM
{
	public static class MathsUtils
	{
#region Constants
		// Maths constants
		public const float kOneOverRootTwo = 0.7071f;
		
		// Utility constants
		public const float kFloatEpsilon = 0.0001f;
#endregion
		
#region Comparison
		public static float Difference( float a, float b ) =>
			Mathf.Abs( a - b );
		
		public static bool Approximately( float a, float b, float epsilon = kFloatEpsilon ) =>
			Mathf.Abs( a - b ) <= epsilon;
		
		public static bool Approximately( Vector2 a, Vector2 b, float epsilon = kFloatEpsilon ) =>
			Difference(a.x, b.x) +
			Difference(a.y, b.y) < kFloatEpsilon;
		
		public static bool ApproximatelyAll( Vector2 a, Vector2 b, float epsilon = kFloatEpsilon ) =>
			Approximately(a.x, b.x, kFloatEpsilon) &&
			Approximately(a.y, b.y, kFloatEpsilon);
		
		public static bool Approximately( Vector3 a, Vector3 b, float epsilon = kFloatEpsilon ) =>
			Difference(a.x, b.x) +
			Difference(a.y, b.y) +
			Difference(a.z, b.z) < kFloatEpsilon;		
		
		public static bool ApproximatelyAll( Vector3 a, Vector3 b, float epsilon = kFloatEpsilon ) =>
			Approximately(a.x, b.x, kFloatEpsilon) &&
			Approximately(a.y, b.y, kFloatEpsilon) &&
			Approximately(a.z, b.z, kFloatEpsilon);		
		
		public static bool Approximately( Quaternion a, Quaternion b, float epsilon = kFloatEpsilon ) =>
			Difference(a.x, b.x) +
			Difference(a.y, b.y) +
			Difference(a.z, b.z) +
			Difference(a.w, b.w) < kFloatEpsilon;
		
		public static bool ApproximatelyAll( Quaternion a, Quaternion b, float epsilon = kFloatEpsilon ) =>
			Approximately(a.x, b.x, kFloatEpsilon) &&
			Approximately(a.y, b.y, kFloatEpsilon) &&
			Approximately(a.z, b.z, kFloatEpsilon) &&
			Approximately(a.w, b.w, kFloatEpsilon);

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
		
		public static float Max( Vector3 v ) => Mathf.Max(v.x, v.y, v.z);
#endregion
		
#region Polyline Generation
		public static List<Vector3> GetArcPolylinePoints(
			Vector3 centre,
			float radius,
			int fullCircleLineSegments,
			float minAngle = 0.0f,
			float maxAngle = 2 * Mathf.PI )
		{
			float arcAngle = maxAngle - minAngle;
			if( arcAngle < 0.0f )
			{
				arcAngle += (2.0f * Mathf.PI);
			}

			List<Vector3> outVertices;

			if( Mathf.Approximately( arcAngle, 0.0f ) )
			{
				outVertices = new List<Vector3>();
			}
			else if( Mathf.Approximately( radius, 0.0f ) )
			{
				outVertices = new List<Vector3>()
				{
					centre
				};
			}
			else
			{
				float rotationRatio = arcAngle / (2 * Mathf.PI);
				int numSegments = Mathf.Max( Mathf.CeilToInt( fullCircleLineSegments * rotationRatio ), 1 );
				float segmentAngle =
					arcAngle /
					(float)numSegments; // This won't necessarily be exactly the same as 2PI/fullCircleLineSegments

				outVertices = new List<Vector3>( numSegments );

				for( int i = 0; i < numSegments; ++i )
				{
					float pointAngle = minAngle + segmentAngle * i;
					outVertices.Add( centre + new Vector3(
						Mathf.Cos( pointAngle ) * radius,
						0.0f,
						Mathf.Sin( pointAngle ) * radius ) );
				}
			}

			return outVertices;
		}

		public static List<Vector3> GetRoundedPolygonPolylinePoints(
			Vector3[] vertices,
			float radius,
			int fullCircleLineSegments)
		{

			List<Vector3> outOutlineLineVertices = new List<Vector3>();

			if( vertices != null )
			{
				if( vertices.Length == 1 )
				{
					// Single vertex - a sphere (simple case)
					outOutlineLineVertices = GetArcPolylinePoints(
						vertices[0],
						radius,
						fullCircleLineSegments );
				}
				else if( vertices.Length > 1 )
				{
					// Multiple vertices - a convex hull
					// (NOTE: Vertices need to be ordered)
					int numVertices = vertices.Length;
					for( int i = 0; i < numVertices; ++i )
					{
						int prevIdx = Mathf.Abs( (i + numVertices - 1) % numVertices );
						int nextIdx = Mathf.Abs( (i + numVertices + 1) % numVertices );
						Vector3 fromPrev = vertices[i] - vertices[prevIdx];
						Vector3 fromPrevOrthogonal = Vector3.Cross( Vector3.up, fromPrev );
						fromPrevOrthogonal.y = 0.0f;
						fromPrevOrthogonal.Normalize();
						Vector3 toNext = vertices[nextIdx] - vertices[i];
						Vector3 toNextOrthogonal = Vector3.Cross( Vector3.up, toNext );
						toNextOrthogonal.y = 0.0f;
						toNextOrthogonal.Normalize();

						float angleFrom = Mathf.Acos( fromPrevOrthogonal.x ) * Mathf.Sign( fromPrevOrthogonal.z );
						float angleTo = Mathf.Acos( toNextOrthogonal.x ) * Mathf.Sign( toNextOrthogonal.z );

						outOutlineLineVertices.AddRange(
							GetArcPolylinePoints(
								vertices[i],
								radius,
								fullCircleLineSegments,
								angleFrom,
								angleTo ) );
					}
				}
			}

			return outOutlineLineVertices;
		}
#endregion
		
	}
}
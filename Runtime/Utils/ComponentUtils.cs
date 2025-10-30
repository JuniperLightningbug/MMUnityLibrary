#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MM
{
	public static class ComponentUtils
	{
		public static Material GetMaterialInstancePlaymodeSafe( MeshRenderer inRenderer )
		{
			if( !inRenderer )
			{
				return null;
			}

#if UNITY_EDITOR
			return EditorApplication.isPlaying ? inRenderer.material : inRenderer.sharedMaterial;

#else
			return inRenderer.material;
#endif
		}

		public static void DestroyPlaymodeSafe( Object inObject )
		{
			if( !inObject )
			{
				return;
			}

#if UNITY_EDITOR
			if( EditorApplication.isPlaying )
			{
				Object.Destroy( inObject );
			}
			else
			{
				Object.DestroyImmediate( inObject );
			}
#else
			Object.Destroy( inObject );
#endif
			inObject = null;
		}

		/**
		 * If an object tries to destroy other objects during its OnDestroy callback, this can lead to null reference
		 * errors outside play mode.
		 * Solution: Wait for the next editor frame (if relevant), and then post the destroy event delayed.
		 */
		public static void DestroyFromOnDestroyCallbackPlaymodeSafe( Object inObject )
		{
			if( !inObject )
			{
				return;
			}

#if UNITY_EDITOR
			if( EditorApplication.isPlaying )
			{
				Object.Destroy( inObject );
			}
			else
			{
				EditorApplication.delayCall += () => { DestroyPlaymodeSafe( inObject ); };
			}
#else
			Object.Destroy( inObject );
#endif
		}
		
	}
}
using System.Collections.Generic;
using MM;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor( typeof( SEvent_Base ), true )]
public class Inspector_SO_Event_Base : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		SEvent_Base component = (SEvent_Base)target;

		EditorGUILayout.Space();

		if( Application.isPlaying )
		{
			EditorGUILayout.LabelField( "[Runtime Data:]", EditorStyles.boldLabel );
			
			// Debug view of listeners
			List<object> listeners = component.ForInspector_Listeners;
			for( int i = 0; i < listeners.Count; ++i )
			{
				if( listeners[i] is UnityEngine.Object unityObj )
				{
					EditorGUILayout.ObjectField( unityObj.name, unityObj, unityObj.GetType(), true );
				}
				else if( listeners[i] != null )
				{
					EditorGUILayout.LabelField( listeners[i].ToString() );
				}
				else
				{
					EditorGUILayout.LabelField( $"Null listener at: {i}" );
				}
			}

			// Debug invoker
			if( GUILayout.Button( "Debug: Invoke (Runtime)" ) )
			{
				component.ForInspector_Invoke();
			}
		}
	}
}

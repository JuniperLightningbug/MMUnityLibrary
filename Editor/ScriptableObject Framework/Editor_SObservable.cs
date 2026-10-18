using UnityEditor;
using UnityEngine;

namespace MM
{
	[CustomEditor( typeof( SObservableBase ), true )]
	public class Editor_SObservable : Editor
	{
		private const string kProperty_Script = "m_Script";
		private const string kProperty_DefaultValue = "_defaultValue";
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(
				serializedObject.FindProperty( kProperty_DefaultValue ),
				true
			);

			if( !MM.ApplicationUtils.BIsPlaying )
			{
				return;
			}

			// Play mode only
			EditorGUILayout.Space();

			using( new EditorGUI.IndentLevelScope() )
			{
				using( new EditorGUILayout.VerticalScope( EditorStyles.helpBox ) )
				{
					EditorGUILayout.LabelField( "Runtime Value", EditorStyles.boldLabel );
					DrawPropertiesExcluding( serializedObject, kProperty_DefaultValue, kProperty_Script );
				}
			}

			EditorGUILayout.Space();

			if( target is SObservableBase observable )
			{
				if( GUILayout.Button( "Store Runtime [Current] as new [Default]" ) )
				{
					observable.DebugStoreCurrentValueAsDefault();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}

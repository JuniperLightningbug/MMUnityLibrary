using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MM
{
	[CustomEditor( typeof( SEventSignature<> ), true )]
	public class Editor_SEvent : Editor
	{
		private const string kProperty_Guard = "_guard";
		private const string kProperty_DebugArgPrefix = "_defaultPayload";
		private const int kDebugArgCountMax = 3;

		private bool _bShowListeners = false;

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			bool bPlayMode = MM.ApplicationUtils.BIsPlaying;
			SEventBase eventBase = (SEventBase)target;

			// Show default args
			for( int i = 0; i <= kDebugArgCountMax; ++i )
			{
				SerializedProperty arg = serializedObject.FindProperty( $"{kProperty_DebugArgPrefix}{i}" );

				if( arg != null )
				{
					EditorGUILayout.PropertyField( arg, true );
				}
				else
				{
					break;
				}
			}

			if( bPlayMode )
			{
				// Show Ownership Guard (Play mode only)
				SerializedProperty guard = serializedObject.FindProperty( kProperty_Guard );
				if( guard != null )
				{
					EditorGUILayout.PropertyField( guard, true );
				}

				// Show Listeners
				if( eventBase )
				{
					Delegate[] listeners = eventBase.DebugGetListeners;

					_bShowListeners = EditorGUILayout.Foldout(
						_bShowListeners,
						$"Listeners ({listeners.Length}){(_bShowListeners ? ":" : "")}",
						true
					);

					if( !_bShowListeners )
					{
						return;
					}

					EditorGUI.indentLevel++;

					foreach( Delegate listener in listeners )
					{
						Object listenerObj = listener.Target as Object;

						string listenerString = listener.Target switch
						{
							Component c => $"{c.gameObject.name}: {c.GetType().Name}",
							GameObject go => go.name,
							UnityEngine.Object obj => obj.name,
							_ => listener.Target?.GetType().Name ?? "Static",
						};

						EditorGUILayout.LabelField( listenerString );
					}

					EditorGUI.indentLevel--;
				}
			}

			serializedObject.ApplyModifiedProperties();

			if( bPlayMode && eventBase )
			{
				// Show debug invoke button (Play mode only)
				if( GUILayout.Button( "Debug Invoke" ) )
				{
					eventBase.DebugInvoke();
				}
			}
		}
	}
}

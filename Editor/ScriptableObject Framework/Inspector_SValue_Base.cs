using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MM
{
	[CanEditMultipleObjects]
	[CustomEditor( typeof( SValue_Base ), true )]
    public class Inspector_SValue_Base : Editor
    {
	    public override void OnInspectorGUI()
	    {
		    DrawDefaultInspector();

		    SValue_Base component = (SValue_Base)target;
		    IForInspector_ObservableValue iObservable = component as IForInspector_ObservableValue;

		    EditorGUILayout.Space();

		    if( Application.isPlaying )
		    {
			    EditorGUILayout.LabelField( "[Runtime Payload:]", EditorStyles.boldLabel );
			    
			    // Reset default buttons
			    if( GUILayout.Button( "Debug: Reset To Default Value" ) )
			    {
				    component.ResetToDefaultValue();
			    }
			    if( GUILayout.Button( "Debug: Apply Current Value As Default [-Permanent-]" ) )
			    {
				    Undo.RecordObject( component, "Apply Current Value As Default" );
				    component.ForInspector_StoreValueAsDefault();
				    EditorUtility.SetDirty( component );
			    }

			    if( iObservable != null )
			    {
				    // Debug view of listeners
				    void ShowListenerObjects( List<object> objects )
				    {
					    for( int i = 0; i < objects.Count; ++i )
					    {
						    if( objects[i] is UnityEngine.Object unityObj )
						    {
							    EditorGUILayout.ObjectField( unityObj.name, unityObj, unityObj.GetType(), true );
						    }
						    else if( objects[i] != null )
						    {
							    EditorGUILayout.LabelField( objects[i].ToString() );
						    }
						    else
						    {
							    EditorGUILayout.LabelField( $"Null listener at: {i}" );
						    }
					    }
				    }
				    
				    EditorGUILayout.LabelField( "Pre-Changed Listeners:" );
				    ShowListenerObjects( iObservable.ForInspector_GetOnPreChangedListeners() );
				    EditorGUILayout.LabelField( "Post-Changed Listeners:" );
				    ShowListenerObjects( iObservable.ForInspector_GetOnPostChangedListeners() );

				    // Debug invokers
				    if( GUILayout.Button( "Debug Simulate Changed: As Current (Runtime)" ) )
				    {
					    iObservable.ForInspector_SimulateCallbacks_AsCurrentValue();
				    }
				    if( GUILayout.Button( "Debug Simulate Changed: Current -> Default (Runtime)" ) )
				    {
					    iObservable.ForInspector_SimulateCallbacks_CurrentToDefault();
				    }
				    if( GUILayout.Button( "Debug Simulate Changed: Default -> Current (Runtime)" ) )
				    {
					    iObservable.ForInspector_SimulateCallbacks_DefaultToCurrent();
				    }
			    }
		    }
	    }
    }
}

using System;
using UnityEditor;
using UnityEngine;

namespace MM
{
	[CustomPropertyDrawer( typeof( Observable<> ), true )]
	public class PropertyDrawer_Observable : PropertyDrawer
	{
		private bool _bShowRuntimeDebugger = false;
		private bool _bShowListeners = false;
		private const float kPaddingV = 8.0f;
		private const float kPaddingH = 14.0f;
		private const float kSpacingUnderDebugUtils = 4.0f;
		
		private const string kProperty_Value = "_value";
		private const string kProperty_PreviousValue = "_previousValue";
		private const string kProperty_Guard = "_guard";

		public override void OnGUI(
			Rect rect,
			SerializedProperty property,
			GUIContent label )
		{
			bool bPlayMode = MM.ApplicationUtils.BIsPlaying;
			//UnityEngine.Object thisObj = property.serializedObject.targetObject;

			if( fieldInfo.GetValue(
				   property.serializedObject.targetObject
			   ) is not ObservableBase observable )
			{
				return;
			}

			float h = EditorGUIUtility.singleLineHeight;
			float gap = EditorGUIUtility.standardVerticalSpacing;
			
			SerializedProperty value = property.FindPropertyRelative( kProperty_Value );
			float valueH = EditorGUI.GetPropertyHeight( value, label, true );
			SerializedProperty prevValue = property.FindPropertyRelative( kProperty_PreviousValue );
			float prevValueH = EditorGUI.GetPropertyHeight( prevValue, label, true );
			SerializedProperty ownershipGuard = property.FindPropertyRelative( kProperty_Guard );
			float guardH = EditorGUI.GetPropertyHeight( ownershipGuard, label, true );

			Rect valueBox = new Rect( rect.x, rect.y, rect.width, valueH );

			EditorGUI.PropertyField(
				valueBox,
				value,
				new GUIContent( $"{property.displayName}" ),
				true
			);

			if( !bPlayMode )
			{
				// Stop here - no need for runtime info or buttons
				return;
			}

			Rect debuggerBox = new Rect( rect.x, valueBox.yMax + gap, rect.width, rect.yMax - (valueBox.yMax + gap + kSpacingUnderDebugUtils) );
			GUI.Box( debuggerBox, GUIContent.none, EditorStyles.helpBox );

			Rect headerBox = new Rect( rect.x, debuggerBox.y, rect.width, h + gap );

			if( GUI.Button( headerBox, $"Runtime Debugger {(_bShowRuntimeDebugger ? '\u25bc' : '\u25b6')}" ) )
			{
				_bShowRuntimeDebugger = !_bShowRuntimeDebugger;
			}

			if( !_bShowRuntimeDebugger )
			{
				return;
			}
			
			// Previous Value
			Rect rowBox = new Rect( rect.x + kPaddingH, headerBox.yMax + kPaddingV, rect.width - kPaddingH * 2.0f, prevValueH );
			
			EditorGUI.PropertyField(
				rowBox,
				prevValue,
				new GUIContent( $"Previously:" ),
				true
			);
			
			// Ownership
			rowBox.y = rowBox.yMax + gap;
			rowBox.height = guardH;
			EditorGUI.PropertyField(
				rowBox,
				ownershipGuard,
				true
			);
			
			// Listeners
			rowBox.y = rowBox.yMax + gap;
			rowBox.height = h;
			_bShowListeners = EditorGUI.Foldout(
				EditorGUI.IndentedRect( rowBox ),
				_bShowListeners,
				$"Listeners ({observable.DebugGetListenerCount()}):",
				true
			);

			if( _bShowListeners )
			{
				EditorGUI.indentLevel++;

				rowBox.y += h + gap;
				EditorHelpers.DrawActionListeners( ref rowBox, "Value Listeners", observable.DebugGetListeners );

				rowBox.y += h + gap;
				EditorHelpers.DrawActionListeners( ref rowBox, "Notify Listeners", observable.DebugGetListeners_NoParams );

				EditorGUI.indentLevel--;
			}

			rowBox.y += h + gap;
			if( GUI.Button( EditorGUI.IndentedRect( rowBox ), "Invoke All Callbacks" ) )
			{
				observable.DebugInvoke();
			}
		}

		public override float GetPropertyHeight(
			SerializedProperty property,
			GUIContent label )
		{
			SerializedProperty value = property.FindPropertyRelative( kProperty_Value );
			
			float height = value == null ?
				EditorHelpers.FullLineHeight :
				EditorGUI.GetPropertyHeight( value, label, true );

			if( Application.isPlaying &&
			    fieldInfo.GetValue(
				    property.serializedObject.targetObject
			    ) is ObservableBase observable )
			{
				height += EditorHelpers.FullLineHeight + // Runtime Debugger Header
				          EditorGUIUtility.standardVerticalSpacing; // Gap between Value & Debugger Header

				if( _bShowRuntimeDebugger )
				{
					SerializedProperty prevValue = property.FindPropertyRelative( kProperty_PreviousValue );
					float prevValueHeight = prevValue == null ?
						EditorHelpers.FullLineHeight :
						EditorGUI.GetPropertyHeight( prevValue, label, true );
					
					SerializedProperty ownershipGuard = property.FindPropertyRelative( kProperty_Guard );
					float guardH = EditorGUI.GetPropertyHeight( ownershipGuard, label, true );

					height += kPaddingV * 2 + // Runtime debugger group padding
					          EditorHelpers.FullLineHeight * 2 + // Listeners Header, Button
					          prevValueHeight + // Previous runtime value before last callback invoke
					          guardH + // Ownership guard
					          // Add a slight extra gap under large field (not accounted for in the runtime debugger)
					          kSpacingUnderDebugUtils;

					if( _bShowListeners )
					{
						height += EditorHelpers.GetActionListenersHeight( observable.DebugGetListeners ) +
						          EditorHelpers.GetActionListenersHeight( observable.DebugGetListeners_NoParams );
					}
				}
			}

			return height;
		}
	}
}
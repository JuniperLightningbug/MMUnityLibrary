using UnityEngine;
using UnityEditor;

namespace MM
{
	[CustomPropertyDrawer( typeof( SObservableBase ), true )]
	public class PropertyDrawer_SObservable : PropertyDrawer
	{
		private const float kPaddingV = 8.0f;
		private const float kPaddingH = 14.0f;
		
		public override void OnGUI(
			Rect rect,
			SerializedProperty property,
			GUIContent label )
		{
			float h = EditorGUIUtility.singleLineHeight;
			float gap = EditorGUIUtility.standardVerticalSpacing;
			
			GUI.Box( rect, GUIContent.none, EditorStyles.helpBox );
			rect.xMin += kPaddingH;
			rect.xMax -= kPaddingH;
			rect.yMin += kPaddingV;
			rect.yMax -= kPaddingH;
			
			Rect row = new Rect( rect.x, rect.y, rect.width, h );
			EditorGUI.PropertyField( row, property, label );
			row.y += h + gap;

			if( property.objectReferenceValue is not SObservableBase observable )
			{
				return;
			}

			// Editable value from the referenced asset.
			SerializedObject so = new SerializedObject( observable );
			so.Update();
			
			// Default value
			SerializedProperty defaultValue = so.FindProperty( "_defaultValue" );
			if( defaultValue != null )
			{
				row.height = EditorGUI.GetPropertyHeight( defaultValue, true );
				EditorGUI.PropertyField( row, defaultValue, true );
				row.y += row.height + gap;
			}
			
			// Current value
			SerializedProperty currentValue = so.FindProperty( "_current" );
			if( MM.ApplicationUtils.BIsPlaying && currentValue != null )
			{
				row.height = EditorGUI.GetPropertyHeight( currentValue, true );
				EditorGUI.PropertyField( row, currentValue, true );
				row.y += row.height + gap;
			}
			
			so.ApplyModifiedProperties();
		}

		public override float GetPropertyHeight(
			SerializedProperty property,
			GUIContent label )
		{
			float height = EditorGUIUtility.singleLineHeight +
			               kPaddingV * 2.0f;

			if( property.objectReferenceValue is SObservableBase observable )
			{
				SerializedObject so = new SerializedObject( observable );
				SerializedProperty defaultValue = so.FindProperty( "_defaultValue" );
				SerializedProperty currentValue = so.FindProperty( "_current" );

				if( defaultValue != null )
				{
					height += EditorGUIUtility.standardVerticalSpacing
					          + EditorGUI.GetPropertyHeight( defaultValue, true );
				}

				if( MM.ApplicationUtils.BIsPlaying && currentValue != null )
				{
					height += EditorGUIUtility.standardVerticalSpacing
					          + EditorGUI.GetPropertyHeight( currentValue, true );
				}
			}

			return height;
		}
	}
}
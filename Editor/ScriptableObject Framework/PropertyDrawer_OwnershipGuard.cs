using UnityEditor;
using UnityEngine;

namespace MM
{
	[CustomPropertyDrawer( typeof( OwnershipGuard ), true )]
	public class PropertyDrawer_OwnershipGuard : PropertyDrawer
	{
		private const string kProperty_Owner = "_owner";

		public override void OnGUI(
			Rect rect,
			SerializedProperty property,
			GUIContent label )
		{
			OwnershipGuard guard = (OwnershipGuard)property.boxedValue;
			
			float h = EditorGUIUtility.singleLineHeight;
			float gap = EditorGUIUtility.standardVerticalSpacing;

			Rect row = new Rect( rect.x, rect.y, rect.width, h );
			
			/*
			 * Note: Can't just access the property.
			 * We might want to attach this to a ScriptableObject (see SObservable), where scene objects assigned would
			 * cause a type mismatch, but we'd still want to be able to view the current owner at runtime.
			 */
			if (EditorUtility.IsPersistent(property.serializedObject.targetObject))
			{
				// Asset wrappers: show the read-only scene object reference (valid at runtime)
				using (new EditorGUI.DisabledScope(true))
				{
					EditorGUI.ObjectField(
						row, "Owner", guard.Owner, typeof(Object), true
					);
				}
			}
			else
			{
				// Scene Object property: editable, serialised owner
				EditorGUI.PropertyField(
					row,
					property.FindPropertyRelative(kProperty_Owner),
					new GUIContent("Owner")
				);
				
				// Show whether the object drawing this property is the current owner.
				// Note: Skip this entirely for asset wrappers, which will never be assigned as a runtime owner.
				using( new EditorGUI.DisabledScope( true ) )
				{
					if( guard.BLocked )
					{
						row.y += h + gap;

						EditorGUI.indentLevel++;
						EditorGUI.Toggle(
							row,
							"Has Write Access",
							guard.HasWriteAccess( property.serializedObject.targetObject ) );
						EditorGUI.indentLevel--;
					}
				}
			}
		}

		public override float GetPropertyHeight(
			SerializedProperty property,
			GUIContent label )
		{
			float height = 0.0f;
			OwnershipGuard guard = (OwnershipGuard)property.boxedValue;
			height += EditorGUIUtility.singleLineHeight;
			
			if( !EditorUtility.IsPersistent(property.serializedObject.targetObject) && // Scene Object inspector
			    guard.BLocked ) // Has an owner to display (otherwise HasWriteAccess() is always true)
			{
				// Line showing local write access
				height += EditorGUIUtility.singleLineHeight +
				          EditorGUIUtility.standardVerticalSpacing;
			}

			return height;
		}
	}
}

using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MM
{
	[CustomPropertyDrawer( typeof( SingletonComponentsInitInfo ) )]
	public class SingletonComponentsInitInfo_PropertyDrawer : PropertyDrawer
	{
		const string kConfigsListBindingPath = "_configs";

		public override VisualElement CreatePropertyGUI( SerializedProperty property )
		{
			VisualElement root = new VisualElement();

			// Note: We don't want to display this as a list. The internal list has a hard-coded execution order.
			SerializedProperty configsProperty = property.FindPropertyRelative( kConfigsListBindingPath );
			VisualElement initInfoReadonlyArray = new VisualElement();

			Color borderColor = Color.black;
			for( int i = 0; i < configsProperty.arraySize; ++i )
			{
				SerializedProperty configProperty = configsProperty.GetArrayElementAtIndex( i );
				PropertyField initInfo = new PropertyField( configProperty, "" );
				VisualElement configElement = new VisualElement()
				{
					style =
					{
						borderLeftWidth = 1,
						borderRightWidth = 1,
						borderTopWidth = 1,
						borderBottomWidth = 1,
						borderLeftColor = borderColor,
						borderRightColor = borderColor,
						borderTopColor = borderColor,
						borderBottomColor = borderColor,
						paddingTop = 3,
						paddingBottom = 3,
					}
				};
				configElement.Add( initInfo );
				initInfoReadonlyArray.Add( configElement );
			}

			root.Add( initInfoReadonlyArray );

			return root;
		}
	}
}
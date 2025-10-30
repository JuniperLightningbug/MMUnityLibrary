using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MM
{
	[CustomEditor( typeof( SingletonHub ) )]
	public class SingletonHub_Inspector : Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			VisualElement root = new VisualElement();

			// Draw default fields first
			InspectorElement.FillDefaultInspector( root, serializedObject, this );

			// Add a custom “Force Refresh” button at the bottom
			Button refreshButton = new Button(
				() =>
				{
					SingletonHub hub = (SingletonHub)target;
					hub.TryRefreshInspectorData();
				} )
			{
				text = "Force Refresh"
			};
			root.Add( refreshButton );

			return root;
		}
	}
}

using System;
using UnityEngine;
using UnityEditor;

namespace MM
{
	public static class EditorHelpers
	{
		public static void DrawActionListeners(
			ref Rect row,
			string label,
			Delegate[] listeners )
		{
			EditorGUI.LabelField( row, $"{label} ({listeners.Length}):" );

			EditorGUI.indentLevel++;

			foreach( Delegate listener in listeners )
			{
				row.y += FullLineHeight;

				string target = listener.Target switch
				{
					Component c => $"{c.gameObject.name}: {c.GetType().Name}",
					GameObject go => go.name,
					UnityEngine.Object obj => obj.name,
					_ => listener.Target?.GetType().Name ?? "Static",
				};
				string method = listener.Method.Name;

				EditorGUI.LabelField( row, $"{target}.{method}" );
			}

			EditorGUI.indentLevel--;
		}

		public static float GetActionListenersHeight( Delegate[] listeners ) =>
			(1 + // Label
			 listeners.Length // Content
			) * FullLineHeight;

		public static float FullLineHeight =>
			EditorGUIUtility.singleLineHeight +
			EditorGUIUtility.standardVerticalSpacing;
	}
}

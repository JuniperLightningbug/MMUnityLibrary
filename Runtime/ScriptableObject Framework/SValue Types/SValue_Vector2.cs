using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Vector2",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Vector2",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SValue_Vector2 : SValue<Vector2> {}
}

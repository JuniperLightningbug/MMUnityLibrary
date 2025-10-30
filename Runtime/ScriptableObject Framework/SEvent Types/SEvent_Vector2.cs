using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Vector2",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Vector2",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SEvent_Vector2 : SEvent_Typed<Vector2> {}
}

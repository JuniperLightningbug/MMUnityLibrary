using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Vector3",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Vector3",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SEvent_Vector3 : SEvent_Typed<Vector3> {}
}

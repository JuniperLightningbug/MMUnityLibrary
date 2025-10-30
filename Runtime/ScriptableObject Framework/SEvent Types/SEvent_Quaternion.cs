using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Quaternion",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Quaternion",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SEvent_Quaternion : SEvent_Typed<Quaternion> {}
}

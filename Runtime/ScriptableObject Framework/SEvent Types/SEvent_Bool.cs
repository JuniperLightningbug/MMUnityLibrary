using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Bool",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Bool",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SEvent_Bool : SEvent_Typed<bool> {}
}

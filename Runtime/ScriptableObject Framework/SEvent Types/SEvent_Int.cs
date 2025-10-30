using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Int",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Int",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SEvent_Int : SEvent_Typed<int> {}
}

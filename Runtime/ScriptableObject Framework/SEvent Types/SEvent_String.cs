using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "String",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "String",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SEvent_String : SEvent_Typed<string> {}
}

using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Float",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Float",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SEvent_Float : SEvent_Typed<float> {}
}

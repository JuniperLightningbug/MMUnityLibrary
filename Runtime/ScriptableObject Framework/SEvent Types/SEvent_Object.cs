using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Object",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Object",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SEvent_Object : SEvent_Typed<Object> {}
}

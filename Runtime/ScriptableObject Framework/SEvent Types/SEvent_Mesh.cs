using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Mesh",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Mesh",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SEvent_Mesh : SEvent_Typed<Mesh> {}
}

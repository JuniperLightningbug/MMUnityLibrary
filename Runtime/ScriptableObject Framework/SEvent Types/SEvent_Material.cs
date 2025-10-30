using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Material",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Material",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SEvent_Material : SEvent_Typed<Material> {}
}

using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Texture",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Texture",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SEvent_Texture : SEvent_Typed<Texture> {}
}

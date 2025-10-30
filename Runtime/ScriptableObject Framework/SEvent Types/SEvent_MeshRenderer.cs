using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "MeshRenderer",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "MeshRenderer",
		order = ScriptableObjectUtils.kMenuOrder_SceneRef )]
	public class SEvent_MeshRenderer : SEvent_Typed<MeshRenderer> {}
}

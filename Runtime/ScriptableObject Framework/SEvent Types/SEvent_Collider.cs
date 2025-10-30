using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Collider",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Collider",
		order = ScriptableObjectUtils.kMenuOrder_SceneRef )]
	public class SEvent_Collider : SEvent_Typed<Collider> {}
}

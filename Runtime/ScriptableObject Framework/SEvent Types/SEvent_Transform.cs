using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Transform",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Transform",
		order = ScriptableObjectUtils.kMenuOrder_SceneRef )]
	public class SEvent_Transform : SEvent_Typed<Transform> {}
}

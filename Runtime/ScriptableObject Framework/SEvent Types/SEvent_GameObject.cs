using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "GameObject",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "GameObject",
		order = ScriptableObjectUtils.kMenuOrder_SceneRef )]
	public class SEvent_GameObject : SEvent_Typed<GameObject> {}
}

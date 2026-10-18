using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Transform",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Transform",
		order = ScriptableObjectUtils.kMenuOrder_SceneRef )]
	public class SObservable_Transform : SObservable<Transform> {}
}

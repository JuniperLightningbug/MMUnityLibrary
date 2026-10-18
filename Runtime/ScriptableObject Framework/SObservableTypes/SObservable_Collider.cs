using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Collider",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Collider",
		order = ScriptableObjectUtils.kMenuOrder_SceneRef )]
	public class SObservable_Collider : SObservable<Collider> {}
}

using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "GameObject",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "GameObject",
		order = ScriptableObjectUtils.kMenuOrder_SceneRef )]
	public class SObservable_GameObject : SObservable<GameObject> {}
}

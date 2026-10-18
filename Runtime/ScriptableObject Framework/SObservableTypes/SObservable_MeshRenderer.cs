using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "MeshRenderer",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "MeshRenderer",
		order = ScriptableObjectUtils.kMenuOrder_SceneRef )]
	public class SObservable_MeshRenderer : SObservable<MeshRenderer> {}
}

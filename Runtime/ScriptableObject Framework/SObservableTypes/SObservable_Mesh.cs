using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Mesh",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Mesh",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SObservable_Mesh : SObservable<Mesh> {}
}

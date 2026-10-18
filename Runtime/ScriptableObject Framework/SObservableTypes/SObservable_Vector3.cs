using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Vector3",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Vector3",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SObservable_Vector3 : SObservable<Vector3> {}
}

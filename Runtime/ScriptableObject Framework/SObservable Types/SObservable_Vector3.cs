using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableValueNamePrefix + "Vector3",
		menuName = ScriptableObjectUtils.kObservableValueMenuPrefix + "Vector3",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SObservable_Vector3 : SObservable<Vector3> {}
}

using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableValueNamePrefix + "Quaternion",
		menuName = ScriptableObjectUtils.kObservableValueMenuPrefix + "Quaternion",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SObservable_Quaternion : SObservable<Quaternion> {}
}

using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Quaternion",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Quaternion",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SObservable_Quaternion : SObservable<Quaternion> {}
}

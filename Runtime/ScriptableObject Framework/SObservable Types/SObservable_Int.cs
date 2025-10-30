using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableValueNamePrefix + "Int",
		menuName = ScriptableObjectUtils.kObservableValueMenuPrefix + "Int",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SObservable_Int : SObservable<int> {}
}

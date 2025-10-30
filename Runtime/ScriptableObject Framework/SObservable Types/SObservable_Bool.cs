using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableValueNamePrefix + "Bool",
		menuName = ScriptableObjectUtils.kObservableValueMenuPrefix + "Bool",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SObservable_Bool : SObservable<bool> {}
}

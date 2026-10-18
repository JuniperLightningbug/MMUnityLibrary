using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Bool",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Bool",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SObservable_Bool : SObservable<bool> {}
}

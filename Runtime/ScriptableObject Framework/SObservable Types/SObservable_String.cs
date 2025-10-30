using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableValueNamePrefix + "String",
		menuName = ScriptableObjectUtils.kObservableValueMenuPrefix + "String",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SObservable_String : SObservable<string> {}
}

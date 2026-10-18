using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "String",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "String",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SObservable_String : SObservable<string> {}
}

using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Int",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Int",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SObservable_Int : SObservable<int> {}
}

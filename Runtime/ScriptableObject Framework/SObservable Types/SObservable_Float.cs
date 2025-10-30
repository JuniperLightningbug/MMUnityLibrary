using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableValueNamePrefix + "Float",
		menuName = ScriptableObjectUtils.kObservableValueMenuPrefix + "Float",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SObservable_Float : SObservable<float> {}
}

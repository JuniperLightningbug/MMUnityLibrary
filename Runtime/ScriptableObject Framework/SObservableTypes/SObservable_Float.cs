using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Float",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Float",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SObservable_Float : SObservable<float> {}
}

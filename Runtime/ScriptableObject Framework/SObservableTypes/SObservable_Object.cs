using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Object",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Object",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SObservable_Object : SObservable<Object> {}
}

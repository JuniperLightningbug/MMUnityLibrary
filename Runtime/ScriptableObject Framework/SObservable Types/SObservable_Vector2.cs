using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableValueNamePrefix + "Vector2",
		menuName = ScriptableObjectUtils.kObservableValueMenuPrefix + "Vector2",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SObservable_Vector2 : SObservable<Vector2> {}
}

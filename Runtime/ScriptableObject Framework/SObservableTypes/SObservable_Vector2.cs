using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Vector2",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Vector2",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SObservable_Vector2 : SObservable<Vector2> {}
}

using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableValueNamePrefix + "Colour",
		menuName = ScriptableObjectUtils.kObservableValueMenuPrefix + "Colour",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SObservable_Colour : SObservable<Color> {}
}

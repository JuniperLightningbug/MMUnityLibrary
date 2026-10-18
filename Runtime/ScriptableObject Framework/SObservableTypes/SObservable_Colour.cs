using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Colour",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Colour",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SObservable_Colour : SObservable<Color> {}
}

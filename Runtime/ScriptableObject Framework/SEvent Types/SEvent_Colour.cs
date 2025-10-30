using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNamePrefix + "Colour",
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Colour",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SEvent_Colour : SEvent_Typed<Color> {}
}

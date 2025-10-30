using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Colour",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Colour",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SValue_Colour : SValue<Color> {}
}

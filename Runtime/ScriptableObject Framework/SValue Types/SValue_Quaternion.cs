using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Quaternion",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Quaternion",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SValue_Quaternion : SValue<Quaternion> {}
}

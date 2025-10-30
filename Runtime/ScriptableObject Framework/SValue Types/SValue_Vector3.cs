using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Vector3",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Vector3",
		order = ScriptableObjectUtils.kMenuOrder_UnityComposite )]
	public class SValue_Vector3 : SValue<Vector3> {}
}

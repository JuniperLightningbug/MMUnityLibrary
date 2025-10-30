using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Float",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Float",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SValue_Float : SValue<float> {}
}

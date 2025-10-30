using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Bool",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Bool",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SValue_Bool : SValue<bool> {}
}

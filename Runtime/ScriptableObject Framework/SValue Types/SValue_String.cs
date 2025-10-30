using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "String",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "String",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SValue_String : SValue<string> {}
}

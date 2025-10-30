using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Int",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Int",
		order = ScriptableObjectUtils.kMenuOrder_Primitive )]
	public class SValue_Int : SValue<int> {}
}

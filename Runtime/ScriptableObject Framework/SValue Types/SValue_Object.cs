using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Object",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Object",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SValue_Object : SValue<Object> {}
}

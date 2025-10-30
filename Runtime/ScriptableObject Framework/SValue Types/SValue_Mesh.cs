using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Mesh",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Mesh",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SValue_Mesh : SValue<Mesh> {}
}

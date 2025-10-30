using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Material",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Material",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SValue_Material : SValue<Material> {}
}

using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kValueNamePrefix + "Texture",
		menuName = ScriptableObjectUtils.kValueMenuPrefix + "Texture",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SValue_Texture : SValue<Texture> {}
}

using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Texture",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Texture",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SObservable_Texture : SObservable<Texture> {}
}

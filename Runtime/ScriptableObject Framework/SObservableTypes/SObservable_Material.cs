using UnityEngine;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kObservableNamePrefix + "Material",
		menuName = ScriptableObjectUtils.kObservableMenuPrefix + "Material",
		order = ScriptableObjectUtils.kMenuOrder_Asset )]
	public class SObservable_Material : SObservable<Material> {}
}

using UnityEngine;

namespace MM
{
	public struct SEventData_Empty
	{
		public static SEventData_Empty Default => default;
	}

	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNameNoParams,
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Event",
		order = ScriptableObjectUtils.kMenuOrder_Empty )]
	public class SEvent : SEvent_Typed<SEventData_Empty>
	{
	}
}
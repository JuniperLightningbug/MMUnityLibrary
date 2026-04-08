using UnityEngine;

namespace MM
{
	public struct SEventPayload_Empty
	{
		public static SEventPayload_Empty Default => default;
		public override string ToString() => "[NO PAYLOAD]";
	}

	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNameNoParams,
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Event",
		order = ScriptableObjectUtils.kMenuOrder_Empty )]
	public class SEvent : SEvent_Typed<SEventPayload_Empty>
	{
	}
}
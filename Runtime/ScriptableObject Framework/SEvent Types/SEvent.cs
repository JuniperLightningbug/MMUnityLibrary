using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNameNoParams,
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Event",
		order = ScriptableObjectUtils.kMenuOrder_Empty )]
	public class SEvent : SEventSignature<Action>
	{
		public void Invoke( Object from = null )
		{
			if( HasWriteAccess( from ) )
			{
				InvokeWithAuthority();
			}
		}

		public void InvokeWithAuthority() { _listeners?.Invoke(); }
		
#if UNITY_EDITOR
		public override void DebugInvoke() =>
			InvokeWithAuthority();
#endif
	}
}

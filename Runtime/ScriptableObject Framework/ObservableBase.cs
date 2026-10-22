using System;
using UnityEngine;

namespace MM
{
	/**
	 * Strongly typed base class for custom inspector interface & shared enum types
	 */
	public abstract class ObservableBase
	{
#region Shared Types

		public enum ECallbackMode
		{
			Auto, // Execute callbacks if the new value differs from the current value
			AlwaysInvoke,
			Suppress,
		}

#endregion

#region Inspector Access

		public abstract int DebugGetListenerCount();
		public abstract Delegate[] DebugGetListeners { get; }
		public abstract Delegate[] DebugGetListeners_NoParams { get; }

		public abstract void DebugInvoke();
#endregion

#region Dynamic access

		public abstract string DisplayValue { get; }
		public abstract void StartListeningNotify( Action listener );
		public abstract void StopListeningNotify( Action listener );

#endregion
	}
}

using System;
using UnityEngine;

namespace MM
{
	/// <summary>
	/// `ScriptableObject` that behaves as a singleton at runtime with dependable initialisation behaviour and order
	/// <see cref="SingletonHub"/>
	/// </summary>
	public abstract class SingletonComponent : ScriptableObject, IControlled
	{
		public enum ESingletonComponentLazyUpdateMode
		{
			None,
			OnPoll,
			OnPollOncePerFrame,
		}
		
#region SingletonHub accessor interface
		
		private bool _bInitialised = false;
		public bool BInitialised => _bInitialised;
		
		public void Initialise( bool bForceReInitialise = false )
		{
			if( bForceReInitialise || !_bInitialised )
			{
				InitialiseInternal();
				_bInitialised = true;
				
				MonoBehaviourController.Instance?.Register( this );
			}
		}

#endregion

#region Virtual/Abstract
		public virtual ESingletonComponentLazyUpdateMode LazyUpdateMode => ESingletonComponentLazyUpdateMode.None;
		
		protected abstract void InitialiseInternal();

		public virtual void LazyUpdate()
		{
		}

#endregion

#region ScriptableObject
		private void OnDisable()
		{
			MonoBehaviourController.Instance?.Unregister( this );
		}

#endregion

// TODO: If we don't want this to be an SO (we could still inject config data from an SO)
// #region IDisposable
// 		public void Dispose()
// 		{
// 			MonoBehaviourController.TryGetInstance?.Unregister( this );
// 		}
// #endregion
	}
}
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MM
{

#region 0, 1, 2, 3 Arg Variants

	/*
	 * Intermediates branched here for shared Invoke() definitions, which need the argument #s.
	 * Note: 0-arg has its own file for CreateAssetMenu.
	 */

	// 1 Param
	public abstract class SEvent<T> : SEventSignature<Action<T>>
	{
		public void Invoke( T arg0, Object from = null )
		{
			if( HasWriteAccess( from ) )
			{
				InvokeWithAuthority( arg0 );
			}
		}

		public void InvokeWithAuthority( T arg0 ) { _listeners?.Invoke( arg0 ); }

#if UNITY_EDITOR
		// Used for inspector / debug broadcasts
		[SerializeField] private T _defaultPayload0;
		public override void DebugInvoke() =>
			InvokeWithAuthority( _defaultPayload0 );
#endif
	}

	// 2 Params
	public abstract class SEvent<T0, T1> : SEventSignature<Action<T0, T1>>
	{
		public void Invoke( T0 arg0, T1 arg1, Object from = null )
		{
			if( HasWriteAccess( from ) )
			{
				InvokeWithAuthority( arg0, arg1 );
			}
		}

		public void InvokeWithAuthority( T0 arg0, T1 arg1 ) { _listeners?.Invoke( arg0, arg1 ); }

#if UNITY_EDITOR
		// Used for inspector / debug broadcasts
		[SerializeField] private T0 _defaultPayload0;
		[SerializeField] private T1 _defaultPayload1;
		public override void DebugInvoke() =>
			InvokeWithAuthority( _defaultPayload0, _defaultPayload1 );
#endif
	}

	// 3 Params
	public abstract class SEvent<T0, T1, T2> : SEventSignature<Action<T0, T1, T2>>
	{
		public void Invoke( T0 arg0, T1 arg1, T2 arg2, Object from = null )
		{
			if( HasWriteAccess( from ) )
			{
				InvokeWithAuthority( arg0, arg1, arg2 );
			}
		}

		public void InvokeWithAuthority( T0 arg0, T1 arg1, T2 arg2 ) { _listeners?.Invoke( arg0, arg1, arg2 ); }

#if UNITY_EDITOR
		// Used for inspector / debug broadcasts
		[SerializeField] private T0 _defaultPayload0;
		[SerializeField] private T1 _defaultPayload1;
		[SerializeField] private T2 _defaultPayload2;
		public override void DebugInvoke() =>
			InvokeWithAuthority( _defaultPayload0, _defaultPayload1, _defaultPayload2 );
#endif
	}

#endregion

	/**
	 * Generic base class. Branch here for different #s of arguments in TAction.
	 * e.g. SEvent<T> : SEventSignature<Action<T>>
	 * Note: We can't include Invoke() interface here without knowing the #s/types of arguments.
	 */
	[System.Serializable]
	public abstract class SEventSignature<TAction> : SEventBase where TAction : Delegate
	{
		protected TAction _listeners;

#region Ownership Guard

		[SerializeField] protected OwnershipGuard _guard;
		public OwnershipGuard Guard => _guard;

		public Object Owner => Guard?.Owner;
		public bool BLocked => Guard?.BLocked == true;
		public bool HasWriteAccess( Object from ) => Guard?.HasWriteAccess( from ) != false;
		public void Lock( Object owner, bool bForce = false ) => Guard?.Lock( owner, bForce );
		public void Release( Object owner, bool bForce = false ) => Guard?.Release( owner, bForce );

#endregion

#region Read Access

		public void StartListening( TAction listener ) =>
			_listeners = (TAction)Delegate.Combine( _listeners, listener );

		public void StopListening( TAction listener ) =>
			_listeners = (TAction)Delegate.Remove( _listeners, listener );

#endregion

#region Write Access

		public void ClearAllListeners( Object from )
		{
			if( HasWriteAccess( from ) )
			{
				ClearAllListenersWithAuthority();
			}
		}

		public void ClearAllListenersWithAuthority() { _listeners = null; }

#endregion

#region ScriptableObject

		protected void OnEnable() { _guard = new OwnershipGuard(); }

		protected void OnDisable()
		{
			_guard = null;
			ClearAllListenersWithAuthority();
		}

#endregion

#region SEventBase

		public override int DebugGetListenerCount() => DebugGetListeners.Length;
		public override Delegate[] DebugGetListeners => _listeners?.GetInvocationList() ?? Array.Empty<Delegate>();

		public override Type ListenerType => typeof( TAction );

		public override void StartListeningDynamic( Delegate listener ) =>
			StartListening( (TAction)listener );

		public override void StopListeningDynamic( Delegate listener ) =>
			StopListening( (TAction)listener );

#endregion

	}

	/**
	 * Strongly-typed base class for custom editors / property drawers
	 */
	[System.Serializable]
	public abstract class SEventBase : ScriptableObject
	{
#region Editor

		public abstract int DebugGetListenerCount();
		public abstract Delegate[] DebugGetListeners { get; }
		public abstract void DebugInvoke();

#endregion

#region Type Access

		public abstract Type ListenerType { get; }
		public abstract void StartListeningDynamic( Delegate listener );
		public abstract void StopListeningDynamic( Delegate listener );

#endregion
	}
}
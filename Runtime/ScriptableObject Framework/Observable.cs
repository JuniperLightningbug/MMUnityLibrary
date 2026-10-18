using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MM
{
	[System.Serializable]
	public class Observable<T> : ObservableBase
	{
		[SerializeField] protected T _value;
		[SerializeField] protected T _previousValue;

		public Observable( T initialValue, bool bAllowExternalChanges = false )
		{
			_guard = new OwnershipGuard();
			_value = initialValue;
		}

		public Observable() {}

		public void ClearAllListeners()
		{
			_onChanged = null;
			_onChangedNotify = null;
		}

#region Lock / Release

		[SerializeField] protected OwnershipGuard _guard = new OwnershipGuard();
		public OwnershipGuard Guard => _guard;
		public bool BLocked => Guard.BLocked;
		public Object Owner => Guard.Owner;
		public bool HasWriteAccess( Object from ) => Guard.HasWriteAccess( from );
		public void Lock( Object owner, bool bForce = false ) => Guard.Lock( owner, bForce );
		public void Release( Object owner, bool bForce = false ) => Guard.Release( owner, bForce );

#endregion

#region Value & Callbacks

		private Action<T, T> _onChanged;
		private Action _onChangedNotify;

		public bool HasChangedSinceCallbacks() => !EqualityComparer<T>.Default.Equals( _previousValue, _value );

		public T Get() => _value;

		public void Set( T inValue, Object from = null, ECallbackMode callbackMode = ECallbackMode.Auto )
		{
			if( HasWriteAccess( from ) )
			{
				SetWithAuthority( inValue, callbackMode );
			}
		}

		public void SetWithAuthority( T inValue, ECallbackMode callbackMode = ECallbackMode.Auto )
		{
			_value = inValue;

			if( BCallbacksEnabled( _value, inValue, callbackMode ) )
			{
				InvokeOnChangedWithAuthority();
			}
		}

		/**
		 * Useful for:
		 * 1. Batching multiple changes and executing callbacks once, manually
		 * 2. Debug callback triggers from changes made in the inspector
		 * 3. Object typed targets, whose internal changes might not be caught by a default equality comparer
		 */
		public void InvokeOnChangedWithAuthority()
		{
			_onChanged?.Invoke( _previousValue, _value );
			_onChangedNotify?.Invoke();

			// Record all changes since the last callback invocation
			_previousValue = _value;
		}

		public void InvokeIfChangedWithAuthority()
		{
			if( HasChangedSinceCallbacks() )
			{
				InvokeOnChangedWithAuthority();
			}
		}

		public void InvokeOnChanged( Object from = null )
		{
			if( Guard.HasWriteAccess( from ) )
			{
				InvokeOnChangedWithAuthority();
			}
		}

		public void InvokeIfChanged( Object from = null )
		{
			if( Guard.HasWriteAccess( from ) && HasChangedSinceCallbacks() )
			{
				InvokeOnChangedWithAuthority();
			}
		}

		// Hide all changes since the last broadcast
		public void SkipCallbacks() => _previousValue = _value;

		public bool BCallbacksEnabled( T fromValue, T toValue, ECallbackMode callbackMode = ECallbackMode.Auto ) =>
			callbackMode switch
			{
				ECallbackMode.Auto => !EqualityComparer<T>.Default.Equals( fromValue, toValue ),
				ECallbackMode.AlwaysInvoke => true,
				ECallbackMode.Suppress => false,
				_ => throw new ArgumentOutOfRangeException( nameof( callbackMode ), callbackMode, null )
			};

#endregion

#region Observers

		public void StartListening( Action<T, T> callback ) => _onChanged += callback;

		public void StartListening( Action callback ) => _onChangedNotify += callback;

		public void StopListening( Action<T, T> callback ) => _onChanged -= callback;

		public void StopListening( Action callback ) => _onChangedNotify -= callback;

#endregion

#region ObservableBase: Dynamic Access

		// Override this for type-specific views (see 'SObservableTMP')
		public override string DisplayValue => _value?.ToString() ?? string.Empty;

		public override void StartListeningNotify( Action listener ) => StartListening( listener );

		public override void StopListeningNotify( Action listener ) => StopListening( listener );

#endregion

#region ObservableBase: Debug

		public override int DebugGetListenerCount() =>
			DebugGetListeners.Length +
			DebugGetListeners_NoParams.Length;

		public override Delegate[] DebugGetListeners => _onChanged?.GetInvocationList() ?? Array.Empty<Delegate>();

		public override Delegate[] DebugGetListeners_NoParams =>
			_onChangedNotify?.GetInvocationList() ?? Array.Empty<Delegate>();

		public override void DebugInvoke() => InvokeOnChangedWithAuthority();

		public override void DebugInvoke( object from, object to )
		{
			if( from is T fromTyped && to is T toTyped )
			{
				_previousValue = fromTyped;
				_value = toTyped;
				InvokeOnChangedWithAuthority();
			}
		}

#endregion
	}

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
		public abstract void DebugInvoke( object from, object to );

#endregion

#region Dynamic access

		public abstract string DisplayValue { get; }
		public abstract void StartListeningNotify( Action listener );
		public abstract void StopListeningNotify( Action listener );

#endregion
	}
}

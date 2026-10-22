using System;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MM
{
	/**
	 * Holds a sharable value (Observable), with a default value from edit-time.
	 * Used for loading values from assets / asset paths, and reducing scene dependencies / race conditions.
	 */
	public abstract class SObservable<T> : SObservableBase
	{
		[SerializeField] protected Observable<T> _current;
		public Observable<T> Current => _current ??= new Observable<T>(_defaultValue);
		public T _defaultValue;

		public void ResetToDefault( Object from = null ) => Current.Set( _defaultValue, from );
		
#region Observable Routing

		// Access (might need prior validation - T can be null)
		public T Get() => Current.Get();

		// Ownership Guard
		public Object Owner => Current.Guard.Owner;
		public bool BLocked => Current.Guard.BLocked;
		public bool HasWriteAccess( Object from ) => Current.Guard.HasWriteAccess( from );
		public void Lock( Object owner, bool bForce = false ) => Current.Guard.Lock( owner, bForce );
		public void Release( Object owner, bool bForce = false ) => Current.Guard.Release( owner, bForce );


		// Modify
		public void ClearAllListeners() => Current?.ClearAllListeners();
		public bool HasChangedSinceCallbacks() => Current.HasChangedSinceCallbacks();

		public void Set(
			T inValue,
			Object from = null,
			ObservableBase.ECallbackMode callbackMode = ObservableBase.ECallbackMode.Auto ) =>
			Current?.Set( inValue, from, callbackMode );

		public void SetWithAuthority(
			T inValue,
			ObservableBase.ECallbackMode callbackMode = ObservableBase.ECallbackMode.Auto ) =>
			Current?.SetWithAuthority( inValue, callbackMode );

		public void InvokeOnChangedWithAuthority() => Current?.InvokeOnChangedWithAuthority();
		public void InvokeIfChangedWithAuthority() => Current?.InvokeIfChangedWithAuthority();
		public void InvokeOnChanged( Object from = null ) => Current?.InvokeOnChanged( from );
		public void InvokeIfChanged( Object from = null ) => Current?.InvokeIfChanged( from );

		// Observe
		public void StartListening( Action<T, T> callback ) => Current?.StartListening( callback );
		public void StartListening( Action callback ) => Current?.StartListening( callback );
		public void StopListening( Action<T, T> callback ) => Current?.StopListening( callback );
		public void StopListening( Action callback ) => Current?.StopListening( callback );

#endregion

#region Unity Callbacks

		protected virtual void OnDisable()
		{
			// Prevent invokes from other objects holding a stale reference
			Current?.ClearAllListeners();
			_current = null;
		}

		protected virtual void OnEnable()
		{
			// Ensure initially set to default value
			Current.SetWithAuthority( _defaultValue, ObservableBase.ECallbackMode.Suppress );

			// No listeners yet - don't flag this change for callbacks
			_current?.SkipCallbacks();
		}

#endregion

#region SObservableBase

		// Override this for type-specific views (see 'SObservableTMP')
		public override string DisplayValue => _current?.ToString() ?? string.Empty;

		public override void StartListeningNotify( Action listener ) =>
			_current?.StartListeningNotify( listener );

		public override void StopListeningNotify( Action listener ) =>
			_current?.StopListeningNotify( listener );

#if UNITY_EDITOR
		public override void DebugStoreCurrentValueAsDefault()
		{
			Undo.RecordObject( this, "SValue: Store Current Value As Default" );
			_defaultValue = Current.Get();
		}
#endif

#endregion

	}

	public abstract class SObservableBase : ScriptableObject
	{
#if UNITY_EDITOR
		public abstract void DebugStoreCurrentValueAsDefault();
#endif

#region Dynamic Access

		public abstract string DisplayValue { get; }
		public abstract void StartListeningNotify( Action listener );
		public abstract void StopListeningNotify( Action listener );

#endregion
	}
}

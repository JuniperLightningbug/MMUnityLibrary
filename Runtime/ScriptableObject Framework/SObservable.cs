using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MM
{
	/// <summary>
	/// Variant of <see cref="SValue{T}"/> with observable pre- and post-change callbacks.
	/// Callbacks are automatic for atomic value types and composite/object type reassignments.
	/// If composite/object types expose their content for modifications by external classes, then those classes are
	/// responsible for posting the callbacks manually (see 'DirectInvokePreChange' and 'DirectInvokePostChange').
	/// </summary>
	public abstract class SObservable<T> : SValue<T>, IForInspector_ObservableValue
	{
		// Mainly for debugging tools - sometimes we want to force the callbacks to invoke, or suppress them
		public enum ECallbackMode
		{
			Auto, // Execute callbacks if the new value differs from the current value
			AlwaysInvoke,
			Suppress,
		}

		// Value-type change callbacks and object-type reassignments (automatic)
		private readonly IndexedHashSet<Action<T, T>> _onPreChangedListeners = new IndexedHashSet<Action<T, T>>();
		private readonly IndexedHashSet<Action<T, T>> _onPostChangedListeners = new IndexedHashSet<Action<T, T>>();
		private readonly IndexedHashSet<Action<SObservable<T>>> _onChangedListeners = new IndexedHashSet<Action<SObservable<T>>>();

#region Interface

		public void SetSkipCallbacks( T inValue ) => _value = inValue;
		public override void Set( T inValue ) => Set( inValue, ECallbackMode.Auto );

		public void Set( T inValue, ECallbackMode callbackMode )
		{
			bool bInvokeCallbacks = callbackMode switch
			{
				ECallbackMode.Auto => !EqualityComparer<T>.Default.Equals( _value, inValue ),
				ECallbackMode.AlwaysInvoke => true,
				ECallbackMode.Suppress => false,
				_ => throw new ArgumentOutOfRangeException( nameof( callbackMode ), callbackMode, null )
			};
			
			if( bInvokeCallbacks )
			{
				T previousValue = _value;
				InvokeOnPreChangedCallbacksInternal( previousValue, inValue );
				_value = inValue;
				InvokeOnChangedCallbacksInternal();
				InvokeOnPostChangedCallbacksInternal( previousValue, _value );
			}
			else
			{
				_value = inValue;
			}
		}

		public void StartListening( Action<SObservable<T>> listener ) => _onChangedListeners?.Add( listener );
		public void StopListening( Action<SObservable<T>> listener ) => _onChangedListeners?.Remove( listener );
		public void StartListeningPreChange( Action<T, T> listener ) => _onPreChangedListeners?.Add( listener );
		public void StopListeningPreChange( Action<T, T> listener ) => _onPreChangedListeners?.Remove( listener );
		public void StartListeningPostChange( Action<T, T> listener ) => _onPostChangedListeners?.Add( listener );
		public void StopListeningPostChange( Action<T, T> listener ) => _onPostChangedListeners?.Remove( listener );

		public void BroadcastChange()
		{
			InvokeOnPreChangedCallbacksInternal( _value, _value );
			InvokeOnChangedCallbacksInternal();
			InvokeOnPostChangedCallbacksInternal(_value, _value );
		}
		public void BroadcastPreChange() => InvokeOnPreChangedCallbacksInternal( _value, _value );
		public void BroadcastPostChange() => InvokeOnPostChangedCallbacksInternal( _value, _value );

#endregion

#region Automatic Callbacks

		private void InvokeOnChangedCallbacksInternal()
		{
			if( _onChangedListeners != null )
			{
				for( int i = _onChangedListeners.Count - 1; i >= 0; --i )
				{
					_onChangedListeners[i]?.Invoke( this );
				}
			}
		}
		
		private void InvokeOnPreChangedCallbacksInternal( T currentValue, T nextValue )
		{
			if( _onPreChangedListeners != null )
			{
				for( int i = _onPreChangedListeners.Count - 1; i >= 0; --i )
				{
					_onPreChangedListeners[i]?.Invoke( currentValue, nextValue );
				}
			}
		}

		private void InvokeOnPostChangedCallbacksInternal( T previousValue, T currentValue )
		{
			if( _onPostChangedListeners != null )
			{
				for( int i = _onPostChangedListeners.Count - 1; i >= 0; --i )
				{
					_onPostChangedListeners[i]?.Invoke( previousValue, currentValue );
				}
			}
		}

#endregion

#region SValue<T>

		protected override void ResetInternal()
		{
			base.ResetInternal();
			_onChangedListeners?.Clear();
			_onPreChangedListeners?.Clear();
			_onPostChangedListeners?.Clear();
		}

#endregion

#region IForInspector_ObservableValue

		public List<object> ForInspector_GetOnChangedListeners() =>
			_onChangedListeners.GetList().Select( listener => listener.Target ).ToList();
		
		public List<object> ForInspector_GetOnPreChangedListeners() =>
			_onPreChangedListeners.GetList().Select( listener => listener.Target ).ToList();

		public List<object> ForInspector_GetOnPostChangedListeners() =>
			_onPostChangedListeners.GetList().Select( listener => listener.Target ).ToList();

		/*
		 * If the value has been set in the inspector, but we want to simulate the change, try to undo it and
		 * then process it through the usual runtime pipeline to trigger the relevant callbacks
		 */

		public void ForInspector_SimulateCallbacks_AsCurrentValue()
		{
#if UNITY_EDITOR
			if( EditorApplication.isPlaying )
			{
				BroadcastChange();
			}
#endif
		}

		public void ForInspector_SimulateCallbacks_CurrentToDefault()
		{
#if UNITY_EDITOR
			if( EditorApplication.isPlaying )
			{
				Set( _defaultValue );
			}
#endif
		}

		public void ForInspector_SimulateCallbacks_DefaultToCurrent()
		{
#if UNITY_EDITOR
			if( EditorApplication.isPlaying )
			{
				T toValue = _value;
				SetSkipCallbacks( _defaultValue );
				Set( toValue );
			}
#endif
		}

#endregion
	}

	/// <summary>
	/// Interface helps serialize data for inspector of generic typed SValue subclass
	/// </summary>
	public interface IForInspector_ObservableValue
	{
		public List<object> ForInspector_GetOnChangedListeners();
		public List<object> ForInspector_GetOnPreChangedListeners();
		public List<object> ForInspector_GetOnPostChangedListeners();

		public void ForInspector_SimulateCallbacks_AsCurrentValue();
		public void ForInspector_SimulateCallbacks_CurrentToDefault();
		public void ForInspector_SimulateCallbacks_DefaultToCurrent();
	}
}
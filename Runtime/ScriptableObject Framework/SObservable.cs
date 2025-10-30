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
		// Value-type change callbacks and object-type reassignments (automatic)
		private readonly IndexedHashSet<Action<T, T>> _onPreChangedListeners = new IndexedHashSet<Action<T, T>>();
		private readonly IndexedHashSet<Action<T, T>> _onPostChangedListeners = new IndexedHashSet<Action<T, T>>();
		
#region Interface
		public void SetSkipCallbacks( T inValue ) => _value = inValue;
		public override void Set( T inValue ) => Set( inValue, false );
		public void Set( T inValue, bool bNotifyIfUnchanged )
		{
			if( !bNotifyIfUnchanged &&
			    EqualityComparer<T>.Default.Equals( _value, inValue ) )
			{
				return;
			}

			InvokePreChange( _value, inValue );
			T previousValue = _value;
			_value = inValue;
			InvokePostChange( previousValue, _value );
		}

		public void StartListeningPreChange( Action<T, T> listener ) => _onPreChangedListeners?.Add( listener );
		public void StopListeningPreChange( Action<T, T> listener ) => _onPreChangedListeners?.Remove( listener );
		public void StartListeningPostChange( Action<T, T> listener ) => _onPostChangedListeners?.Add( listener );
		public void StopListeningPostChange( Action<T, T> listener) => _onPostChangedListeners?.Remove( listener );
		
		public void DirectInvokePreChange() => InvokePreChange( _value, _value );
		public void DirectInvokePostChange() => InvokePostChange( _value, _value );
#endregion

#region Automatic Callbacks
		private void InvokePreChange( T currentValue, T nextValue )
		{
			if( _onPreChangedListeners != null )
			{
				for( int i = _onPreChangedListeners.Count - 1; i >= 0; --i )
				{
					_onPreChangedListeners[i]?.Invoke( currentValue, nextValue );
				}
			}
		}

		private void InvokePostChange( T previousValue, T currentValue )
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
			_onPreChangedListeners?.Clear();
			_onPostChangedListeners?.Clear();
		}
#endregion

#region IForInspector_ObservableValue

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
				Set( _value, true );
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

		/**
		 * If the value has been set in the inspector, but we want to simulate the change, try to undo it and
		 * then process it through the usual runtime pipeline to trigger the relevant callbacks
		 */
		public void ForInspector_SimulateOnChangedCallbacks()
		{
#if UNITY_EDITOR
			if( Application.isPlaying )
			{
				Set( _value );
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
		public List<object> ForInspector_GetOnPreChangedListeners();
		public List<object> ForInspector_GetOnPostChangedListeners();
		
		public void ForInspector_SimulateCallbacks_AsCurrentValue();
		public void ForInspector_SimulateCallbacks_CurrentToDefault();
		public void ForInspector_SimulateCallbacks_DefaultToCurrent();
	}
}

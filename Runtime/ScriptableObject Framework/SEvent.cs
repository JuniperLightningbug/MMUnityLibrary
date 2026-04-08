using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace MM
{
	public abstract class SEvent_Typed<T> : SEvent_Base
	{
		// Use this for null calls & debug invoke triggers
		[SerializeField] protected T _defaultPayload;
		[SerializeField] private bool _bDebugLogInvokes = false;

		protected readonly IndexedHashSet<Action<SEvent_Typed<T>, T>> _listeners =
			new IndexedHashSet<Action<SEvent_Typed<T>, T>>();

		private void InvokeInternal( T eventPayload )
		{
			for( int i = _listeners.Count - 1; i >= 0; --i )
			{
				_listeners[i]?.Invoke( this, eventPayload );
			}
		}

#region C# Interface

		public void ClearListeners() { _listeners.Clear(); }

		public void Invoke()
		{
			if( _bDebugLogInvokes )
			{
				Debug_LogInvoke( _defaultPayload, false );
			}

			InvokeInternal( _defaultPayload );
		}

		public void Invoke( T eventPayload )
		{
			if( _bDebugLogInvokes )
			{
				Debug_LogInvoke( _defaultPayload, true );
			}

			InvokeInternal( eventPayload );
		}

		public void StartListening( Action<SEvent_Typed<T>, T> listener ) => _listeners.Add( listener );
		public void StopListening( Action<SEvent_Typed<T>, T> listener ) => _listeners.Remove( listener );

#endregion

#region Unity Callbacks

		void OnDisable() { ClearListeners(); }

#endregion

#region SO_Event_Base

		public IEnumerable<object> ListenerObjects =>
			_listeners.GetList()
				.Select( listener => listener.Target );

		public IEnumerable<object> Listener =>
			ListenerObjects.Where( target => target != null );

		public sealed override List<object> ForInspector_Listeners => ListenerObjects.ToList();

		public sealed override void ForInspector_Invoke()
		{
#if UNITY_EDITOR
			if( Application.isPlaying )
			{
				Invoke();
			}
#endif
		}

		public sealed override Type PayloadType => typeof( T );

#endregion

#region Debug

		private IEnumerable<string> ListenerStrings =>
			ListenerObjects.Select( listener => listener == null ? "NULL" : listener.ToString() );

		public override string ToString() =>
			$"SEvent: {name} ({GetType()})" +
			$"\n[{_listeners.Count}] Listeners: {string.Join( ", ", ListenerStrings )}" +
			$"\nDefault payload: {_defaultPayload}";

		private string Debug_DescribeInvoke( T eventPayload, bool bUsedCustomPayload ) =>
			$"Invoked event: {name}" +
			$"\n[{_listeners.Count}] Listeners: {string.Join( ", ", ListenerStrings )}" +
			$"\nWith {(bUsedCustomPayload ? "[custom]" : "[default]")} payload:\n{(eventPayload == null ? "NULL" : eventPayload.ToString())}";

		[Conditional( "DEBUG" )]
		private void Debug_LogInvoke( T eventPayload, bool bUsedCustomPayload ) =>
			Debug.Log( Debug_DescribeInvoke( eventPayload, bUsedCustomPayload ) );

#endregion
	}

	/// <summary>
	/// Strongly-typed base class to help serialise properties
	/// </summary>
	public abstract class SEvent_Base : ScriptableObject
	{
		// For editor inspector usage only
		public abstract List<object> ForInspector_Listeners { get; }
		public abstract void ForInspector_Invoke();

		// Simplifies generic accessors
		public abstract Type PayloadType { get; }
	}
}
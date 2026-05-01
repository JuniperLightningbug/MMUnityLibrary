using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MM
{
	[CreateAssetMenu(
		fileName = ScriptableObjectUtils.kEventNameNoParams,
		menuName = ScriptableObjectUtils.kEventMenuPrefix + "Event",
		order = ScriptableObjectUtils.kMenuOrder_Empty )]
	public class SEvent : SEvent_Base
	{
		protected readonly IndexedHashSet<Action<SEvent>> _listeners =
			new IndexedHashSet<Action<SEvent>>();

#region C# Interface

		public override void ClearListeners() { _listeners.Clear(); }

		public override void Invoke()
		{
			if( _bDebugLogInvokes )
			{
				Debug_LogInvoke();
			}

			for( int i = _listeners.Count - 1; i >= 0; --i )
			{
				_listeners[i]?.Invoke( this );
			}
		}

		public void StartListening( Action<SEvent> listener ) => _listeners.Add( listener );
		public void StopListening( Action<SEvent> listener ) => _listeners.Remove( listener );

#endregion

#region SO_Event_Base

		public override IEnumerable<object> ListenerObjects =>
			_listeners.GetList()
				.Select( listener => listener.Target );
		
		public sealed override void ForInspector_Invoke()
		{
#if UNITY_EDITOR
			if( Application.isPlaying )
			{
				Invoke();
			}
#endif
		}

		public override Type PayloadType => null;

#endregion

#region Debug
		public override string ToString() =>
			$"SEvent: {name}" +
			$"\n[{_listeners.Count}] Listeners: {string.Join( ", ", ListenerStrings )}";

		private string Debug_DescribeInvoke() => ToString();

		[Conditional( "DEBUG" )]
		private void Debug_LogInvoke() =>
			Debug.Log( Debug_DescribeInvoke() );

#endregion
	}
	
	public abstract class SEvent_Typed<T> : SEvent_Base
	{
		// Use this for null calls & debug invoke triggers
		[SerializeField] protected T _defaultPayload;

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
		public override void ClearListeners() { _listeners.Clear(); }

		public override void Invoke()
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
				Debug_LogInvoke( eventPayload, true );
			}

			InvokeInternal( eventPayload );
		}

		public void StartListening( Action<SEvent_Typed<T>, T> listener ) => _listeners.Add( listener );
		public void StopListening( Action<SEvent_Typed<T>, T> listener ) => _listeners.Remove( listener );

#endregion

#region SO_Event_Base

		public override IEnumerable<object> ListenerObjects =>
			_listeners.GetList()
				.Select( listener => listener.Target );
		
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
		public override string ToString() =>
			$"SEvent: {name} ({GetType()})" +
			$"\n[{_listeners.Count}] Listeners: {string.Join( ", ", ListenerStrings )}" +
			$"\nDefault payload: {_defaultPayload}";

		private string Debug_DescribeInvoke( T eventPayload, bool bUsedCustomPayload ) =>
			$"Invoked event: {name} with {(bUsedCustomPayload ? "[custom]" : "[default]")} payload:" +
			$"\n{(eventPayload == null ? "NULL" : eventPayload.ToString())}" +
			$"\n[{_listeners.Count}] Listeners: {string.Join( ", ", ListenerStrings )}";

		[Conditional( "DEBUG" )]
		private void Debug_LogInvoke( T eventPayload, bool bUsedCustomPayload ) =>
			Debug.Log( Debug_DescribeInvoke( eventPayload, bUsedCustomPayload ) );
#endregion
	}

	/// <summary>
	/// Strongly-typed base class to help serialise properties and custom inspector features.
	/// Contains functionality common to typed and untyped ("empty") SEvents.
	/// </summary>
	public abstract class SEvent_Base : ScriptableObject
	{
		// For editor inspector usage only
		public virtual List<object> ForInspector_Listeners => ListenerObjects.ToList();
		public abstract void ForInspector_Invoke();
		
		
		// Simplifies generic accessors
		public abstract Type PayloadType { get; }
		
		
		// Type-independent features
		[SerializeField] protected bool _bDebugLogInvokes = false;

		public virtual IEnumerable<object> ListenerObjects => null;

		public IEnumerable<object> Listeners =>
			ListenerObjects.Where( target => target != null );

		public abstract void ClearListeners();

		public abstract void Invoke();
		
		protected IEnumerable<string> ListenerStrings =>
			ListenerObjects.Select( listener => listener == null ? "NULL" : listener.ToString() );
		
		// Unity Callbacks
		void OnDisable() { ClearListeners(); }

	}
}
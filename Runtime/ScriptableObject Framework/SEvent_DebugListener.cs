using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MM
{
	/// <summary>
	/// Generic debug monobehaviour to listen for inspector-assigned events.
	/// Override to add code-based event subscriptions for bespoke testing.
	/// </summary>
	public class SEvent_DebugListener : MonoBehaviour
	{
		[SerializeReference] private List<SEventBase> _events = new List<SEventBase>();

		// Store the delegate instance so that we can stop listening on disable
		private readonly Dictionary<SEventBase, Delegate> _listeners = new Dictionary<SEventBase, Delegate>();

		protected string EventsString => $"[{_events.Count}] events: \n\t{string.Join( ",\n\t", _events )}";

		void OnEnable() { StartAllListeners(); }

		void OnDisable() { StopAllListeners(); }

		[ContextMenu( "Refresh Listeners" )]
		public void RefreshListeners()
		{
			StopAllListeners();
			StartAllListeners();
		}

		public void StartAllListeners()
		{
			if( _events != null )
			{
				Debug.Log( $"DEBUG -- Subscribing to {EventsString}" );

				for( int i = 0; i < _events.Count; ++i )
				{
					if( _events[i] && !_listeners.ContainsKey( _events[i] ) )
					{
						StartListening( _events[i] );
					}
				}
			}
		}

		public void StopAllListeners()
		{
			Debug.Log( $"DEBUG -- Unsubscribing from {EventsString}" );

			foreach( KeyValuePair<SEventBase, Delegate> pair in _listeners )
			{
				if( pair.Key )
				{
					pair.Key.StopListeningDynamic( pair.Value );
				}
			}

			_listeners.Clear();
		}

		private void StartListening( SEventBase eventBase )
		{
			Type actionType = eventBase.ListenerType;

			// Parameterless event.
			if( actionType == typeof( Action ) )
			{
				Action listener = () => OnInvokeUntyped( eventBase );

				eventBase.StartListeningDynamic( listener );
				_listeners.Add( eventBase, listener );
				return;
			}

			// Extract the argument types from Action<T...>.
			if( !actionType.IsGenericType )
			{
				return;
			}

			Type[] args = actionType.GetGenericArguments();

			string methodName = args.Length switch
			{
				1 => nameof( TryStartListening1 ),
				2 => nameof( TryStartListening2 ),
				3 => nameof( TryStartListening3 ),
				_ => null
			};

			if( methodName == null )
			{
				Debug.LogWarning( $"Unsupported event signature: {eventBase}", this );
				return;
			}

			MethodInfo method = typeof( SEvent_DebugListener ).GetMethod(
				methodName,
				BindingFlags.Instance | BindingFlags.NonPublic
			);

			method?.MakeGenericMethod( args ).Invoke( this, new object[] { eventBase } );
		}

		private void TryStartListening1<T>( SEventBase eventBase )
		{
			if( eventBase is SEvent<T> typed )
			{
				Action<T> listener = value => OnInvokeTyped( eventBase, value );

				if( _listeners.TryAdd( eventBase, listener ) )
				{
					typed.StartListening( listener );
				}
			}
		}

		private void TryStartListening2<T1, T2>( SEventBase eventBase )
		{
			if( eventBase is SEvent<T1, T2> typed )
			{
				Action<T1, T2> listener =
					( a, b ) => OnInvokeTyped( eventBase, a, b );

				if( _listeners.TryAdd( eventBase, listener ) )
				{
					typed.StartListening( listener );
				}
			}
		}

		private void TryStartListening3<T1, T2, T3>( SEventBase eventBase )
		{
			if( eventBase is SEvent<T1, T2, T3> typed )
			{
				Action<T1, T2, T3> listener =
					( a, b, c ) => OnInvokeTyped( eventBase, a, b, c );

				if( _listeners.TryAdd( eventBase, listener ) )
				{
					typed.StartListening( listener );
				}
			}
		}

		// Override these for custom debug output for different types
		protected virtual void OnInvokeUntyped( SEventBase eventBase ) =>
			Debug.Log( $"Received event: {eventBase.name}", eventBase );

		protected virtual void OnInvokeTyped<T>( SEventBase eventBase, T a ) =>
			LogEvent( eventBase, a );

		protected virtual void OnInvokeTyped<T1, T2>(
			SEventBase eventBase,
			T1 a,
			T2 b ) =>
			LogEvent( eventBase, a, b );

		protected virtual void OnInvokeTyped<T1, T2, T3>(
			SEventBase eventBase,
			T1 a,
			T2 b,
			T3 c ) =>
			LogEvent( eventBase, a, b, c );

		private void LogEvent( SEventBase eventBase, params object[] args )
		{
			Debug.Log(
				$"Received event: {eventBase.name}\n" +
				$"Arguments: {string.Join( ", ", args )}",
				eventBase
			);
		}
	}
}
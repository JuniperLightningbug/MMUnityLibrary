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
		[SerializeReference] private List<SEvent_Base> _events;

		// Store the delegate instance so that we can stop listening on disable
		private readonly Dictionary<SEvent_Base, Delegate> _listeners = new Dictionary<SEvent_Base, Delegate>();

		protected string EventsString => $"[{_events.Count}] events: \n\t{string.Join( ",\n\t", _events )}";

		void OnEnable()
		{
			if( _events != null )
			{
				Debug.Log( $"DEBUG -- Subscribing to {EventsString}" );

				for( int i = 0; i < _events.Count; ++i )
				{
					if( _events[i] != null )
					{
						StartListening( _events[i] );
					}
				}
			}
		}

		void OnDisable()
		{
			if( _events != null )
			{
				Debug.Log( $"DEBUG -- Unsubscribing from {EventsString}" );

				for( int i = 0; i < _events.Count; ++i )
				{
					if( _events[i] != null )
					{
						StopListening( _events[i] );
					}
				}
			}
		}

		private void StartListening( SEvent_Base eventBase )
		{
			if( eventBase != null )
			{
				if( eventBase.PayloadType != null )
				{
					MethodInfo method = GetType().GetMethod(
						"TryStartListeningTyped",
						BindingFlags.Instance | BindingFlags.NonPublic );

					MethodInfo genericMethod = method?.MakeGenericMethod( eventBase.PayloadType );
					genericMethod?.Invoke( this, new object[] { eventBase } );
				}
			}
		}

		private void StopListening( SEvent_Base eventBase )
		{
			if( eventBase != null )
			{
				if( eventBase.PayloadType != null )
				{
					MethodInfo method = GetType().GetMethod(
						"TryStopListeningTyped",
						BindingFlags.Instance | BindingFlags.NonPublic );

					MethodInfo genericMethod = method?.MakeGenericMethod( eventBase.PayloadType );
					genericMethod?.Invoke( this, new object[] { eventBase } );
				}
			}
		}

		private void TryStartListeningTyped<T>( SEvent_Base eventBase )
		{
			if( eventBase is SEvent_Typed<T> eventTyped )
			{
				Action<SEvent_Typed<T>, T> listener = OnInvokeTyped<T>;

				if( !_listeners.TryAdd( eventBase, listener ) )
				{
					Debug.LogWarning( $"Duplicate event ignored: {eventBase}" );
				}
				else
				{
					eventTyped.StartListening( listener );
				}
			}
		}

		private void TryStopListeningTyped<T>( SEvent_Base eventBase )
		{
			if( eventBase is SEvent_Typed<T> eventTyped )
			{
				if( _listeners.TryGetValue( eventBase, out Delegate listener ) )
				{
					eventTyped.StopListening( (Action<SEvent_Typed<T>, T>)listener );
					_listeners.Remove( eventBase );
				}
			}
		}

		protected virtual void OnInvokeTyped<T>( SEvent_Base eventBase, T eventPayload ) =>
			Debug.Log( $"Received event: {eventBase}\nWith payload: {eventPayload.ToString()}" );

	}
}
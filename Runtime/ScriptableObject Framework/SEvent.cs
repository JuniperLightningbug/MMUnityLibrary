using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MM
{
	public abstract class SEvent_Typed<T> : SEvent_Base
	{
		// Use this for null calls & debug invoke triggers
		[SerializeField] protected T _defaultData;
		
		protected readonly IndexedHashSet<Action<T>> _listeners = new IndexedHashSet<Action<T>>();

#region C# Interface

		public void ClearListeners()
		{
			_listeners.Clear();
		}

		public virtual void Invoke()
		{
			Invoke( _defaultData );
		}

		public void Invoke( T eventData )
		{
			for( int i = _listeners.Count - 1; i >= 0; --i )
			{
				_listeners[i]?.Invoke(eventData);
			}
		}
		public void StartListening( Action<T> listener ) => _listeners.Add( listener );
		public void StopListening( Action<T> listener ) => _listeners.Remove( listener );

#endregion
		
#region Unity Callbacks

		void OnDisable()
		{
			ClearListeners();
		}

#endregion

#region SO_Event_Base

		public override List<object> ForInspector_Listeners =>
			_listeners.GetList().Select( listener => listener.Target ).ToList();

		public override void ForInspector_Invoke()
		{
#if UNITY_EDITOR
			if( Application.isPlaying )
			{
				Invoke();
			}
#endif
		}
		
#endregion
	}

	/// <summary>
	/// Strongly-typed base class to help serialise properties to the inspector
	/// </summary>
	public abstract class SEvent_Base : ScriptableObject
	{
		public abstract List<object> ForInspector_Listeners { get; }
		public abstract void ForInspector_Invoke();
	}
}
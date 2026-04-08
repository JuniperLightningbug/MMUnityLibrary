using System;
using System.Linq;
using UnityEngine;

namespace MM
{
	/// <summary>
	/// A centralized controller for executing game loop update functions in a fixed, predefined order
	/// <see cref="ControlledUpdateOrderProvider"/>.
	/// </summary>
	[DefaultExecutionOrder( -1 )]
	public class MonoBehaviourController : StandaloneSingleton<MonoBehaviourController>
	{
		private static Type[] s_updateOrder; // Assigned on load via reflection

		private ControlledUpdateGroup _updateGroup;
		private ControlledUpdateGroup _lateUpdateGroup;
		private ControlledUpdateGroup _fixedUpdateGroup;
		private ControlledUpdateGroup _debugUpdateGroup;

		[SerializeField] private bool _bSuppressWarnings = false;

		public static bool OnLoad_TryAssignUpdateOrder( Type[] inOrder )
		{
			if( s_updateOrder != null )
			{
				return false;
			}

			s_updateOrder = inOrder.ToArray();
			return true;
		}

#region StandaloneSingleton

		protected override void InitialiseSingleton()
		{
			_updateGroup = new ControlledUpdateGroup( s_updateOrder, typeof( IControlledUpdate ) );
			_lateUpdateGroup = new ControlledUpdateGroup( s_updateOrder, typeof( IControlledLateUpdate ) );
			_fixedUpdateGroup = new ControlledUpdateGroup( s_updateOrder, typeof( IControlledFixedUpdate ) );
#if DEBUG
			_debugUpdateGroup = new ControlledUpdateGroup( s_updateOrder, typeof( IControlledDebugUpdate ) );
#endif
		}

#endregion

#region IControlled Injection

		public void Register( IControlled inControlled )
		{
			if( inControlled == null )
			{
				return;
			}

			if( !_bSuppressWarnings && s_updateOrder == null )
			{
				Debug.LogWarningFormat( "Type order not assigned for '{0}'.\n" +
				                        "Create a subclass of '{1}' for controlled updates to function as intended.\n" +
				                        "Otherwise, updates will invoke in arbitrary orders.",
					GetType().FullName,
					typeof( ControlledUpdateOrderProvider ).FullName );
				_bSuppressWarnings = true;
			}

			Type inType = inControlled.GetType();

			if( inControlled is IControlledUpdate inControlledUpdate )
			{
				_updateGroup.Add( inType, inControlledUpdate.ControlledUpdate );
			}

			if( inControlled is IControlledLateUpdate inControlledLateUpdate )
			{
				_lateUpdateGroup.Add( inType, inControlledLateUpdate.ControlledLateUpdate );
			}

			if( inControlled is IControlledFixedUpdate inControlledFixedUpdate )
			{
				_fixedUpdateGroup.Add( inType, inControlledFixedUpdate.ControlledFixedUpdate );
			}

#if DEBUG
			if( inControlled is IControlledDebugUpdate inControlledDebugUpdate )
			{
				_debugUpdateGroup.Add( inType, inControlledDebugUpdate.ControlledDebugUpdate );
			}
#endif
		}

		public void Unregister( IControlled inControlled )
		{
			if( inControlled == null )
			{
				return;
			}

			Type inType = inControlled.GetType();

			if( inControlled is IControlledUpdate inControlledUpdate )
			{
				_updateGroup.Remove( inType, inControlledUpdate.ControlledUpdate );
			}

			if( inControlled is IControlledLateUpdate inControlledLateUpdate )
			{
				_lateUpdateGroup.Remove( inType, inControlledLateUpdate.ControlledLateUpdate );
			}

			if( inControlled is IControlledFixedUpdate inControlledFixedUpdate )
			{
				_fixedUpdateGroup.Remove( inType, inControlledFixedUpdate.ControlledFixedUpdate );
			}

#if DEBUG
			if( inControlled is IControlledDebugUpdate inControlledDebugUpdate )
			{
				_debugUpdateGroup.Remove( inType, inControlledDebugUpdate.ControlledDebugUpdate );
			}
#endif
		}

#endregion

#region MonoBehaviour Invokes

		protected void Update()
		{
			_updateGroup?.InvokeUpdates( Time.deltaTime );
#if DEBUG
			_debugUpdateGroup?.InvokeUpdates( Time.deltaTime );
#endif
		}

		protected void LateUpdate()
		{
			_lateUpdateGroup?.InvokeUpdates( Time.deltaTime );
		}

		protected void FixedUpdate()
		{
			_fixedUpdateGroup?.InvokeUpdates( Time.fixedDeltaTime );
		}

#endregion
	}
}
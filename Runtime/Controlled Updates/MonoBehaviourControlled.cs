using UnityEngine;

namespace MM
{
	/// <summary>
	/// `MonoBehaviour` with a configurable and dependable update order.
	/// Designed to be managed by a corresponding <see cref="MonoBehaviourController"/>.
	/// </summary>
	public abstract class MonoBehaviourControlled : MonoBehaviour, IControlled
	{
		protected virtual void OnEnable()
		{
			MonoBehaviourController controller = MonoBehaviourController.GetOrMakeInstance();
			if( controller )
			{
				controller.Register( this );
			}
		}

		protected virtual void OnDisable()
		{
			if( MonoBehaviourController.TryGetInstance( out MonoBehaviourController controller ) )
			{
				controller.Unregister( this );
			}
		}
	}
}
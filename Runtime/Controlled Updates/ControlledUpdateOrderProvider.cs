using System;
using System.Linq;
using UnityEngine;

namespace MM
{
	/// <summary>
	/// Update order injection handler for <see cref="MonoBehaviourControlled"/>.
	/// Override this class for project-specific update orders.
	/// </summary>
	public abstract class ControlledUpdateOrderProvider
	{
		protected abstract Type[] ControlledUpdateOrder { get; }

		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		private static void OnLoadInjectFirstControlledUpdateOrder()
		{
			// Find the first subclass of this type
			Type providerType = typeof( ControlledUpdateOrderProvider );
			Type subclassType = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany( assembly => assembly.GetTypes() )
				.FirstOrDefault( type => !type.IsAbstract && providerType.IsAssignableFrom( type ) );

			if( subclassType == null )
			{
				return;
			}

			ControlledUpdateOrderProvider instance =
				Activator.CreateInstance( subclassType ) as ControlledUpdateOrderProvider;

			if( !MonoBehaviourController.OnLoad_TryAssignUpdateOrder( instance?.ControlledUpdateOrder ) )
			{
				Debug.LogWarningFormat(
					"More than one {0} subclasses found in the project. Only the first " +
					"update order will be included.",
					providerType.Name );
			}
		}
	}
}

using System;
using System.Linq;
using UnityEngine;

namespace MM
{
	/// <summary>
	/// Initialisation order injection handler for <see cref="SingletonComponent"/>.
	/// Override this class for project-specific initialisation orders.
	/// </summary>
	public abstract class SingletonInitOrderProvider
	{
		protected abstract Type[] InitialisationOrder { get; }

		public static Type[] GetFirstChildInitOrder()
		{
			// Find the first subclass of this type
			Type providerType = typeof( SingletonInitOrderProvider );
			Type subclassType = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany( assembly => assembly.GetTypes() )
				.FirstOrDefault( type => !type.IsAbstract && providerType.IsAssignableFrom( type ) );

			if( subclassType == null )
			{
				return Array.Empty<Type>();
			}

			SingletonInitOrderProvider instance =
				Activator.CreateInstance( subclassType ) as SingletonInitOrderProvider;

			if( instance == null )
			{
				return Array.Empty<Type>();
			}

			return instance.InitialisationOrder;
		}

		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		private static void InjectInitialisationOrderFromFirstConcreteChildClass()
		{
			Type[] initialisationOrder = GetFirstChildInitOrder();

			if( !SingletonHub.OnLoad_TryInjectInitialisationOrder( initialisationOrder ) )
			{
				Debug.LogWarningFormat(
					"More than one {0} subclasses found in the project. Only the first " +
					"initialisation order will be included.",
					typeof( SingletonInitOrderProvider ).FullName );
			}
		}
	}
}

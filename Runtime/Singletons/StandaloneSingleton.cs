using UnityEngine;

namespace MM
{
	/// <summary>
	/// A simple singleton base class, with optional override for scene persistence.
	/// </summary>
	public abstract class StandaloneSingleton<T> : MonoBehaviour where T : StandaloneSingleton<T>
	{
		/*
		 * This is an independent singleton instance - no external influence over initialisation, update order, mode, etc.
		 * (although it's possible to add IControlled functionality manually in child monobehaviours).
		 *
		 * The intended usage is to set up the two fundamental singletons in a project:
		 * - The singleton manager (`SingletonHub`)
		 * - The MonoBehaviour controller (`MonoBehaviourController`)
		 * Other singletons ideally derive from `SingletonComponent` to benefit from the structure of `SingletonHub`.
		 *
		 * That said, it's also useful in the general case for rapid testing and prototyping before race conditions
		 * become problematic.
		 */
		
		protected bool _bSingletonInitialised = false;
		private static T s_instance;

#region Interface

		public static T TryGetInstance => s_instance;

		public static T GetOrMakeInstance
		{
			get
			{
				if( !s_instance && ApplicationUtils.BIsPlaying )
				{
					GameObject obj = new GameObject( $"{typeof( T ).FullName} [runtime-generated]", typeof( T ) );
					s_instance = obj.GetComponent<T>();
				}

				return s_instance;
			}
		}

		public static T GetInstance( bool bCreateIfNull = true ) => bCreateIfNull ? GetOrMakeInstance : TryGetInstance;

#endregion

#region Initialisation

		private void Awake() { InitialiseSingletonInternal(); }

		private void InitialiseSingletonInternal()
		{
			if( _bSingletonInitialised ||
			    !ApplicationUtils.BIsPlaying )
			{
				return;
			}

			if( s_instance != null && s_instance != this )
			{
				OnBeforeDestroyForExistingInstance( s_instance );
				MM.ComponentUtils.DestroyPlaymodeSafe( gameObject );
			}
			else
			{
				s_instance = this as T;
				if( BPersistent )
				{
					DontDestroyOnLoad( this );
				}

				InitialiseSingleton();
				_bSingletonInitialised = true;
			}
		}

#endregion

#region Virtual

		protected virtual bool BPersistent => false;

		protected virtual void InitialiseSingleton()
		{

		}

		/**
		 * Use this if we need to pass information to an existing singleton before destroying self
		 */
		protected virtual void OnBeforeDestroyForExistingInstance( T existingInstance )
		{

		}

#endregion

	}
}
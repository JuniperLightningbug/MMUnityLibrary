using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MM
{
	public enum ESingletonComponentInitialisationMode
	{
		OnAwake,
		OnPoll,
		
		// e.g.: Testing scene - we might want to avoid clutter, but also to catch missed dependency null refs
		OnPollWithWarning,
	}
	
	/// <summary>
	/// An instantiable central manager for active singleton components.
	/// Provides a single source of truth for initialization order, 
	/// inspector options, and safe access to active singletons.
	/// Supports optional lazy updates per component.
	/// <see cref="SingletonComponent"/>
	/// </summary>
	public class SingletonHub : StandaloneSingleton<SingletonHub>, ISerializationCallbackReceiver
	{
		/*
		 * TODO (future scope):
		 *
		 * Support merging multiple `SingletonHub` instances across scenes:
		 * - Scene A hub initializes subset SC(A)
		 * - Scene B hub initializes subset SC(B)
		 * - At runtime, Scene B’s hub passes its data to Scene A’s hub before being discarded.
		 *
		 * Pros: Each scene manages its own singleton dependencies independently.
		 * Cons: May complicate (or obfuscate) initialization order due to scene load sequence.
		 */

		private Dictionary<Type, float> _lastLazyUpdateTimes;
		
		/*
		 * This is the source of truth for `SingletonComponent types` allowed and their initialisation order at runtime.
		 * Assigned on load by injection from `SingletonInitOrderProvider`
		 */
		private static Type[] s_initialisationOrder;
		// For edit-time inspector data assignments, look for relevant types on poll using reflection
		private Type[] GetEditTimeInitialisationOrder() => SingletonInitOrderProvider.GetFirstChildInitOrder();

#region Static Interface

		public static bool OnLoad_TryInjectInitialisationOrder( Type[] inOrder )
		{
			if( s_initialisationOrder != null )
			{
				return false;
			}

			s_initialisationOrder = inOrder.ToArray();
			return true;
		}

#endregion
		
#region StandaloneSingleton

		protected override bool BPersistent => true;

		protected override void InitialiseSingleton()
		{
			CacheInitialisationInfos();
			CreateSingletonComponentsOnAwake();
		}

#endregion

#region SingletonComponent Inspector Data

		[SerializeField]
		public SingletonComponentsInitInfo _inspectorComponentsInitInfo;

		public void TryRefreshInspectorData()
		{
#if UNITY_EDITOR
			if( !InspectorDataNeedsRefresh() )
			{
				return;
			}

			Debug.Log( $"Updating SingletonComponent Info list (from: {_inspectorComponentsInitInfo.DebugString()})" );
			
			List<SingletonComponentInitInfo> previousList = _inspectorComponentsInitInfo._configs.ToList();
			int previousCount = previousList.Count;

			// Make a new list - this way, we can easily ensure that the update order is visible in the inspector
			List<SingletonComponentInitInfo> newSingletonComponentList =
				new List<SingletonComponentInitInfo>();

			List<string> addedList = new List<string>();

			Type[] initialisationOrder = GetEditTimeInitialisationOrder();

			for( int updateOrderIdx = 0; updateOrderIdx < initialisationOrder.Length; ++updateOrderIdx )
			{
				bool bExists = false;

				for( int inspectorListIdx = previousList.Count - 1; inspectorListIdx >= 0; --inspectorListIdx )
				{
					if( previousList[inspectorListIdx]._typeString ==
					    initialisationOrder[updateOrderIdx].AssemblyQualifiedName )
					{
						// Copy an existing item into the new list
						bExists = true;
						
						// In case type names were changed - don't skip this update
						previousList[inspectorListIdx].InitialiseReadableTypeFields();
						
						newSingletonComponentList.Add( previousList[inspectorListIdx] );
						previousList.RemoveAt( inspectorListIdx );
						break;
					}
				}

				if( !bExists )
				{
					// Add a missing entry to the new list
					SingletonComponentInitInfo newInitInfo = new SingletonComponentInitInfo()
					{
						_typeString = initialisationOrder[updateOrderIdx].AssemblyQualifiedName,
						_bActive = false,
						_initialisationMode = ESingletonComponentInitialisationMode.OnPollWithWarning,
					};
					newInitInfo.InitialiseReadableTypeFields();
					newSingletonComponentList.Add( newInitInfo );
					addedList.Add( newInitInfo._typeString );
				}
			}

			Debug.LogFormat( "\tSingletonComponent Info list updated: " +
			                 "\n\tPreviously: {0} entries. Now: {1} entries." +
			                 " {2} Added: [{3}];" +
			                 " {4} Removed: [{5}]",
				previousCount, newSingletonComponentList.Count,
				addedList.Count, string.Join( ", ", addedList.ToArray() ),
				previousList.Count, string.Join( ", ", previousList.Select( x => x._typeString ).ToArray() ) );

			_inspectorComponentsInitInfo._configs = newSingletonComponentList.ToArray();
			
			Debug.Log( $"Finished updating SingletonComponent Info list (now: {_inspectorComponentsInitInfo.DebugString()})" );
#endif
		}

		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR

#endif
		}

		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			TryRefreshInspectorData();
#endif
		}

		/**
		 * Returns true if more singleton types have been added in code since the config data
		 * was assigned in the inspector
		 */
		public bool InspectorDataNeedsRefresh()
		{
			Type[] initialisationOrder = GetEditTimeInitialisationOrder();

			if( initialisationOrder == null )
			{
				return false;
			}
			
			if( initialisationOrder.Length != _inspectorComponentsInitInfo._configs.Length )
			{
				return true;
			}

			for( int i = 0; i < _inspectorComponentsInitInfo._configs.Length; ++i )
			{
				if( _inspectorComponentsInitInfo._configs[i]._type != initialisationOrder[i] )
				{
					return true;
				}
			}

			return false;
		}

#endregion

#region SingletonComponent Runtime Initialisations

		// Lookup table for runtime initialisations (created and validated on init)
		private Dictionary<Type, SingletonComponentInitInfo> _initialisationInfoDictionary;

		// Lookup table for all currently active and initialised singleton components
		private Dictionary<Type, SingletonComponent> _activeSingletonComponents;
		
		private void CacheInitialisationInfos()
		{
			_initialisationInfoDictionary = new Dictionary<Type, SingletonComponentInitInfo>();
			for( int i = 0; i < _inspectorComponentsInitInfo._configs.Length; ++i )
			{
				if( _inspectorComponentsInitInfo._configs[i]._bActive )
				{
					// Can't serialise `Type`
					_inspectorComponentsInitInfo._configs[i].InitialiseTypeField();
					
					_initialisationInfoDictionary.Add(
						_inspectorComponentsInitInfo._configs[i]._type,
						_inspectorComponentsInitInfo._configs[i] );
				}
			}
		}

		private void CreateSingletonComponentsOnAwake()
		{
			if( _activeSingletonComponents == null )
			{
				_activeSingletonComponents = new Dictionary<Type, SingletonComponent>();
			}

			Debug.Log( "Creating SingletonComponent instances..." );
			for( int i = 0; i < s_initialisationOrder.Length; ++i )
			{
				if( _initialisationInfoDictionary.TryGetValue( s_initialisationOrder[i],
					   out SingletonComponentInitInfo initInfo ) )
				{
					if( initInfo._bActive && initInfo._initialisationMode ==
					   ESingletonComponentInitialisationMode.OnAwake )
					{
						SingletonComponent newComponent = InitialiseSingletonComponent( initInfo );
						_activeSingletonComponents.Add( initInfo._type, newComponent );
					}
				}
			}

			Debug.LogFormat(
				"Finished creating SingletonComponent instances. SingletonComponents active: {0}",
				_activeSingletonComponents.Count );
		}

		private SingletonComponent InitialiseSingletonComponent( Type singletonComponentType )
		{
			if( _initialisationInfoDictionary.TryGetValue( singletonComponentType,
				   out SingletonComponentInitInfo initInfo ) )
			{
				return InitialiseSingletonComponent( initInfo );
			}

			return null;
		}

		private SingletonComponent InitialiseSingletonComponent( in SingletonComponentInitInfo initInfo )
		{
			if( _activeSingletonComponents.ContainsKey( initInfo._type ) )
			{
				Debug.LogWarningFormat( "\tUnable to create SingletonComponent instance: [{0}] - type already exists.",
					initInfo._typeString );
				return null;
			}

			Debug.LogFormat( "\tCreating SingletonComponent instance: [{0}]",
				initInfo._typeString );

			SingletonComponent newSingletonComponent;
			if( initInfo._presetConfig )
			{
				newSingletonComponent = ScriptableObject.Instantiate( initInfo._presetConfig );
			}
			else
			{
				newSingletonComponent = ScriptableObject.CreateInstance( initInfo._type ) as SingletonComponent;
			}

			if( newSingletonComponent )
			{
				newSingletonComponent.Initialise();

				if( newSingletonComponent.LazyUpdateMode ==
				    SingletonComponent.ESingletonComponentLazyUpdateMode.OnPollOncePerFrame )
				{
					_lastLazyUpdateTimes.Add( initInfo._type, Time.time );
				}
			}

			return newSingletonComponent;
		}
		
#endregion

#region SingletonComponent Accessors

		/**
		 * Use this to avoid all initialisations on poll; output might be null
		 */
		public T TryGet<T>() where T : SingletonComponent
		{
			Type tType = typeof(T);
			if( _activeSingletonComponents.TryGetValue( tType, out SingletonComponent outComponent ) )
			{
				if( outComponent.BInitialised )
				{
					TryLazyUpdate( tType, outComponent );
					return (T)outComponent;
				}
			}

			return null;
		}

		/**
		 * Use this to try to initialise an inactive singleton component if it's inactive
		 */
		public T Get<T>() where T : SingletonComponent
		{
			Type tType = typeof(T);
			SingletonComponent outComponent;
			if( _activeSingletonComponents.TryGetValue( typeof( T ), out outComponent ) )
			{
				// Singleton component exists
				if( !outComponent.BInitialised )
				{
					outComponent.Initialise();
				}
			}
			else if( _initialisationInfoDictionary.TryGetValue( typeof( T ), out SingletonComponentInitInfo initInfo ) &&
			         initInfo._bActive)
			{
				// Singleton component doesn't exist - create one
				if( initInfo._initialisationMode == ESingletonComponentInitialisationMode.OnPoll ||
				    initInfo._initialisationMode == ESingletonComponentInitialisationMode.OnPollWithWarning )
				{
					if( initInfo._initialisationMode == ESingletonComponentInitialisationMode.OnPollWithWarning )
					{
						Debug.LogWarningFormat( "Initialising SingletonComponent of type: [{0}]", initInfo._typeString );
					}
					outComponent = InitialiseSingletonComponent( initInfo );
				}
			}

			if( outComponent )
			{
				TryLazyUpdate( tType, outComponent );
				return (T)outComponent;
			}
			return null;
		}
		
		private void TryLazyUpdate( Type type, SingletonComponent singletonComponent )
		{
			switch( singletonComponent.LazyUpdateMode )
			{
				case SingletonComponent.ESingletonComponentLazyUpdateMode.OnPoll:
				{
					singletonComponent.LazyUpdate();
					break;
				}
				case SingletonComponent.ESingletonComponentLazyUpdateMode.OnPollOncePerFrame:
				{
					if( _lastLazyUpdateTimes.TryGetValue( type, out float lastUpdateTime ) )
					{
						float time = Time.time;
						if( !MathsUtils.Approximately( lastUpdateTime, time ) )
						{
							_lastLazyUpdateTimes[type] = time;
							singletonComponent.LazyUpdate();
						}
					}
					break;
				}
			}
		}

#endregion
	}
}
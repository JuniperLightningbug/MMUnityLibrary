using System;
using System.Collections.Generic;
using System.Linq;

namespace MM
{
	/// <summary>
	/// Handles bookkeeping for active <see cref="IControlled"/> implementations, exposing update actions and handling
	/// O(1) adds and removes. It's intended use-case is for a project's <see cref="MonoBehaviourController"/>
	/// implementation, but can be repurposed for sub-controllers as needed.
	/// </summary>
	public class ControlledUpdateGroup
	{
		private readonly Dictionary<Type, IndexedHashSet<Action<float>>> _ordered;
		private readonly IndexedHashSet<Action<float>> _unordered;

		/*
		 * Normally, an IndexedHashSet order is not guaranteed, because it can change when adding or removing
		 * elements. In this case, we're initialising once, and not making any further changes, so the order
		 * will be persistent and dependable, while providing O(1) lookups.
		 * Use this (as a list) as the ordered keys for the `_ordered` collection lookups.
		 */
		private readonly IndexedHashSet<Type> _cachedOrder;

		public bool BHasOrderedContent => _ordered.Count > 0;
		public bool BHasUnorderedContent => _unordered.Count > 0;
		private bool BGetIsOrderedType( Type type ) => _cachedOrder.Contains( type );

		/// <summary>
		/// Optionally provide a filter to create a subset of types in the ordered list.
		/// For a hot code path like the update controller, this can save a lot of lookups every frame for a small
		/// initialisation cost. TODO: This filter could be applied in edit time.
		/// </summary>
		/// <param name="typeOrder">List of types prescribing update order</param>
		/// <param name="typeFilter">Filter out anything not assignable from this type from input list</param>
		public ControlledUpdateGroup( Type[] typeOrder, Type typeFilter )
		{
			_cachedOrder = new IndexedHashSet<Type>();
			if( typeOrder != null )
			{
				for( int i = 0; i < typeOrder.Length; i++ )
				{
					if( typeOrder[i].IsAssignableFrom( typeFilter ) )
					{
						// Filter inputs (e.g. provided an ordered list of IControlled, filter for IControlledUpdate)
						_cachedOrder.Add( typeOrder[i] );
					}
				}
			}

			_ordered = new Dictionary<Type, IndexedHashSet<Action<float>>>();
			_unordered = new IndexedHashSet<Action<float>>();
		}

		public ControlledUpdateGroup( Type[] typeOrder )
		{
			_cachedOrder = typeOrder != null ?
				new IndexedHashSet<Type>( typeOrder ) :
				new IndexedHashSet<Type>();

			_ordered = new Dictionary<Type, IndexedHashSet<Action<float>>>();
			_unordered = new IndexedHashSet<Action<float>>();
		}

		public ControlledUpdateGroup()
		{
			_cachedOrder = new IndexedHashSet<Type>();
			_ordered = new Dictionary<Type, IndexedHashSet<Action<float>>>();
			_unordered = new IndexedHashSet<Action<float>>();
		}

#region Interface

		public void InvokeUpdates( float inDeltaTime )
		{
			if( BHasOrderedContent )
			{
				ExecuteControlledUpdatesOrdered( inDeltaTime );
			}

			if( BHasUnorderedContent )
			{
				ExecuteControlledUpdatesUnordered( inDeltaTime );
			}
		}

		public void Clear()
		{
			_ordered.Clear();
			_unordered.Clear();
		}

		public void Add( Type inType, Action<float> action )
		{
			if( BGetIsOrderedType( inType ) )
			{
				AddOrderedInternal( inType, action );
			}
			else
			{
				AddUnorderedInternal( action );
			}
		}

		public void Remove( Type inType, Action<float> action )
		{
			if( BGetIsOrderedType( inType ) )
			{
				RemoveOrderedInternal( inType, action );
			}
			else
			{
				RemoveUnorderedInternal( action );
			}
		}

#endregion

#region Internal Bookkeeping

		private void AddOrderedInternal( Type inType, Action<float> action )
		{
			if( !_ordered.TryGetValue( inType, out IndexedHashSet<Action<float>> outSet ) )
			{
				outSet = new IndexedHashSet<Action<float>>();
				_ordered.Add( inType, outSet );
			}

			outSet.Add( action );
		}

		private void AddUnorderedInternal( Action<float> action ) { _unordered.Add( action ); }

		private void RemoveOrderedInternal( Type inType, Action<float> action )
		{
			if( _ordered != null && _ordered.TryGetValue( inType, out IndexedHashSet<Action<float>> outSet ) )
			{
				outSet.Remove( action );
				if( outSet.Count == 0 )
				{
					_ordered.Remove( inType );
				}
			}
		}

		private void RemoveUnorderedInternal( Action<float> action ) { _unordered.Remove( action ); }

		private void ExecuteControlledUpdatesOrdered( float inDeltaTime )
		{
			for( int typeIdx = 0; typeIdx < _cachedOrder.Count; ++typeIdx )
			{
				if( _ordered.TryGetValue(
					   _cachedOrder[typeIdx],
					   out IndexedHashSet<Action<float>> targets ) )
				{
					for( int targetIdx = targets.Count - 1; targetIdx >= 0; --targetIdx )
					{
						targets[targetIdx]( inDeltaTime );
					}
				}
			}
		}

		private void ExecuteControlledUpdatesUnordered( float inDeltaTime )
		{
			for( int targetIdx = _unordered.Count - 1; targetIdx >= 0; --targetIdx )
			{
				_unordered[targetIdx]( inDeltaTime );
			}
		}

#endregion
	}
}
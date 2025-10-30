using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MM
{
	/// <summary>
	/// Enumerable dictionary/list combination where:
	/// - Each item can only be represented once
	/// - Add and Remove are both O(1) (ish, like a hash set)
	/// - Indexed iteration / enumeration is fast (dense array)
	/// - Order is not guaranteed after Remove is first called (quick swap remove)
	/// </summary>
	[System.Serializable]
	public class IndexedHashSet<T> : IReadOnlyList<T>, ISerializationCallbackReceiver
	{
		[SerializeField] private List<T> _list;
		private readonly Dictionary<T, int> _indices;

#region Constructors (if data provided on initialisation - not required)

		public IndexedHashSet()
		{
			_list = new List<T>();
			_indices = new Dictionary<T, int>();
		}

		public IndexedHashSet( T[] fromArray )
		{
			_list = new List<T>( fromArray );
			_indices = new Dictionary<T, int>( fromArray.Length );

#if UNITY_EDITOR
			if( Application.isPlaying )
#endif
				RecalculateIndexDictionary();
		}

		public IndexedHashSet( List<T> fromList )
		{
			_list = new List<T>( fromList );
			_indices = new Dictionary<T, int>( fromList.Count );

#if UNITY_EDITOR
			if( Application.isPlaying )
#endif
				RecalculateIndexDictionary();
		}

#endregion

#region Interface

		public string ContentString()
		{
			return string.Join( ", ", _list.ToArray() );
		}

		public bool Add( T item )
		{
			if( _indices.TryAdd( item, _list.Count ) )
			{
				_list.Add( item );
				return true;
			}

			return false;
		}

		public bool Remove( T item )
		{
			if( !_indices.TryGetValue( item, out int removeIdx ) ||
			    removeIdx >= _list.Count )
			{
				return false;
			}

			// Swap with the last item
			_list[removeIdx] = _list[_list.Count - 1];
			_indices[_list[removeIdx]] = removeIdx;

			// Now, remove the last list item
			_list.RemoveAt( _list.Count - 1 );
			_indices.Remove( item );
			return true;
		}

		public void Clear()
		{
			_list.Clear();
			_indices.Clear();
		}

		public T[] ToArray() => _list.ToArray();

		public IReadOnlyList<T> GetList() => _list.AsReadOnly();

#endregion

#region IReadOnlyList

		public int Count => _list.Count;

		public T this[ int idx ] => (idx < _list.Count && idx >= 0) ? _list[idx] : default;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

#endregion

#region ISerializationCallbackReceiver

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			/*
			 * The list element is serialised, and is the source of truth for serialised modifications.
			 * On entering play mode, or on changing the serialised list during playmode, we need to
			 * regenerate the index dictionary.
			 */

			// Sanity-check input list first
			for( int i = _list.Count - 1; i >= 0; --i )
			{
				if( _list[i] == null )
				{
					_list.RemoveAt( i );
				}
			}

			// Regenerate dictionary content from list
			RecalculateIndexDictionary();
		}

#endregion

#region Bookkeeping

		private void RecalculateIndexDictionary()
		{
			_indices.Clear();

			// Second pass for assignments (reverse-order removal could affect assigned indices)
			for( int i = 0; i < _list.Count; ++i )
			{
				if( !_indices.TryAdd( _list[i], i ) )
				{
					Debug.LogWarningFormat(
						"Unable to add item {0} at {1} to index dictionary - entry already exists!",
						_list[i].ToString(), i );
				}
			}
		}

#endregion

	}
}
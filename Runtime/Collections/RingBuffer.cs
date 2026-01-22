using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MM
{

	// TODO: Add custom inspector that either shows the content in chronological order or highlights the "Head" entry

	/// <summary>
	/// Serialisable ring buffer.
	///
	/// Naming convention:
	/// "Head" means the most recent entry. 
	/// "Next" means the "Head" entry + 1 (the next index which will be replaced by a push call). 
	/// "Tail" means the least recent entry (when the buffer has been filled, this will be equivalent to "Next").
	/// </summary>
	[System.Serializable]
	public class RingBuffer<T> : IReadOnlyList<T>
	{
		public const int kInvalidIdx = -1;

		[SerializeField] private int _count;
		[SerializeField] private T[] _array;

		[SerializeField] private int _headIdx;

#region Index Bookkeeping

		private bool IsValid => _array.Length > 0;
		private bool IsEmpty => !IsValid || _headIdx == kInvalidIdx || _count == 0;
		private bool IsFull => IsValid && _count == _array.Length;

		private int WrapIdx( int index ) => IsValid ? MathsUtils.RepeatInt( index, _array.Length ) : kInvalidIdx;

		// The least recent entry
		private int TailIdx => WrapIdx( _headIdx - (_count - 1) );

		// The next idx to be overwritten (for a full buffer, this is equivalent to Tail)
		private int NextIdx => WrapIdx( _headIdx + 1 );
		
		private int GetInternalArrayIdx( int index ) => WrapIdx( _headIdx - (_count - 1) + index );

		// Spliced array segments to access full chronological order without making a copy
		private ArraySegment<T>[] GetSplicedInternalArray()
		{
			if( IsEmpty )
			{
				return Array.Empty<ArraySegment<T>>();
			}
			int tailIdx = TailIdx;
			
			/*
			 * We can construct the full chronological order in at most 2 segments.
			 * The first: tail (oldest, externally [0]) -> earliest of either the internal capacity or head
			 * The second: from the internal 0 to the first of either the head (externally [count-1]) or the tail
			 *
			 * Note:
			 * - if _count == 0 (i.e. tailIdx == _headIdx == kInvalidIdx), both segments will be empty
			 * - if _count == 1 (i.e. tailIdx == _headIdx != kInvalidIdx), the second segment will be empty
			 */
			
			int tailSegmentLength = Mathf.Min( tailIdx + _count, _array.Length ) - tailIdx;
			return new ArraySegment<T>[]
			{
				// Tail segment: tail idx -> either head idx (inclusive) if no wrap, else array end
				new ArraySegment<T>( _array, TailIdx, tailSegmentLength ),
				// Head segment: array 0 -> head idx if wrap
				new ArraySegment<T>( _array, 0, _count - tailSegmentLength ),
			};
		}

#endregion

#region ctor

		public RingBuffer( int capacity ) { Reset( capacity ); }

		public RingBuffer( int capacity, T defaultValue ) : this( capacity ) { Fill( defaultValue ); }

		public RingBuffer( int capacity, IEnumerable<T> initialContent ) : this( capacity )
		{
			T[] inputArray = initialContent as T[] ?? initialContent?.ToArray() ?? System.Array.Empty<T>();
			int copyInputFrom = Mathf.Max( 0, inputArray.Length - capacity );
			int copyInputCount = Mathf.Min( inputArray.Length, capacity );
			System.Array.Copy( inputArray, copyInputFrom, _array, 0, copyInputCount );

			_count = copyInputCount;
			_headIdx = copyInputCount - 1;
		}

#endregion

#region Interface

		public void Clear()
		{
			_count = 0;
			_headIdx = kInvalidIdx;
		}

		public void Clear( T defaultValue )
		{
			Clear();
			Fill( defaultValue );
		}

		public void Reset( int capacity )
		{
			Clear();

			if( _array == null || capacity != _array.Length )
			{
				// If we're not changing the capacity, we don't have to create a new array.
				// Setting the count to 0 will flag all entries as invalid.

				if( capacity <= 0 )
				{
					throw new System.ArgumentException( "Ring buffer capacity must be > 0", nameof( capacity ) );
				}

				_array = new T[capacity];
			}
		}

		public void Fill( T fillWith )
		{
			if( _array != null )
			{
				for( int i = 0; i < _array.Length; ++i )
				{
					_array[i] = fillWith;
				}

				_count = _array.Length;
				_headIdx = _array.Length - 1;
			}
		}

		public void Push( T value )
		{
			if( IsValid )
			{
				_headIdx = IsEmpty ? 0 : NextIdx;
				_array[_headIdx] = value;
				_count = Mathf.Min( _count + 1, _array.Length );
			}
		}

		// Try to add a value but don't overwrite anything
		public void TryPush( T value )
		{
			if( IsValid && !IsFull )
			{
				Push( value );
			}
		}

		/// <summary>
		/// Copies the content of the ring buffer to a new array, in order of assignment
		/// </summary>
		/// <returns>A copy of the buffer content, in order of assignment</returns>
		public T[] ToArray()
		{
			ArraySegment<T>[] internalArraySegments = GetSplicedInternalArray();
			T[] chronologicalArray = new T[Count];
			int copiedEntries = 0;
			for( int i = 0; i < internalArraySegments.Length; ++i )
			{
				internalArraySegments[i].CopyTo( chronologicalArray, copiedEntries );
				copiedEntries += internalArraySegments[i].Count;
			}
			return chronologicalArray;
		}
		
		public T Newest => IsEmpty ? default : _array[_headIdx];
		public T Oldest => IsEmpty ? default : _array[TailIdx];

#endregion

#region IReadOnlyList

		public IEnumerator<T> GetEnumerator()
		{
			ArraySegment<T>[] internalArraySegments = GetSplicedInternalArray();
			for( int segmentIdx = 0; segmentIdx < internalArraySegments.Length; ++segmentIdx )
			{
				for( int localIdx = 0; localIdx < internalArraySegments[segmentIdx].Count; ++localIdx )
				{
					yield return internalArraySegments[segmentIdx][localIdx];
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public int Count => _count;
		public int Capacity => _array.Length;

		public T this[ int index ]
		{
			get
			{
				if( index < 0 || index >= _count )
				{
					throw new System.IndexOutOfRangeException();
				}

				return _array[GetInternalArrayIdx( index )];
			}
		}

#endregion

	}
}

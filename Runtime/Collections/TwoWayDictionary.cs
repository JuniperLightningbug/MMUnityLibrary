using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MM
{
	/// <summary>
	/// Dictionary that enforces uniqueness of both key & value, and provides O(1) lookups both ways
	/// </summary>
	public class TwoWayDictionary<TLeft, TRight> : IEnumerable<KeyValuePair<TLeft, TRight>>
	{
		private readonly Dictionary<TLeft, TRight> _dictionaryLR = new Dictionary<TLeft, TRight>();
		private readonly Dictionary<TRight, TLeft> _dictionaryRL = new Dictionary<TRight, TLeft>();

#region Interface

		public int Count => _dictionaryLR.Count;

		public bool TryAdd( TLeft left, TRight right )
		{
			if( _dictionaryLR.ContainsKey( left ) || _dictionaryRL.ContainsKey( right ) )
			{
				return false;
			}

			AddInternal( left, right );
			return true;
		}
		
		public bool TryAdd( KeyValuePair<TLeft, TRight> pair ) => TryAdd( pair.Key, pair.Value );

		public void Add( TLeft left, TRight right )
		{
			if( _dictionaryLR.ContainsKey( left ) )
			{
				throw new ArgumentException( $"Duplicate [left] key: {left}" );
			}

			if( _dictionaryRL.ContainsKey( right ) )
			{
				throw new ArgumentException( $"Duplicate [right] key: {right}" );
			}

			AddInternal( left, right );
		}
		
		public void Add( KeyValuePair<TLeft, TRight> pair ) => Add( pair.Key, pair.Value );

		public bool RemoveLeft( TLeft left, out TRight outRight )
		{
			if( _dictionaryLR.TryGetValue( left, out outRight ) )
			{
				RemoveInternal( left, outRight );
				return true;
			}

			return false;
		}
		
		public bool RemoveLeft( TLeft left ) => RemoveLeft( left, out TRight _ );
		
		public bool RemoveRight( TRight right, out TLeft outLeft )
		{
			if( _dictionaryRL.TryGetValue( right, out outLeft ) )
			{
				RemoveInternal( outLeft, right );
				return true;
			}

			return false;
		}
		
		public bool RemoveRight( TRight right ) => RemoveRight( right, out TLeft _ );

		public bool Remove( TLeft left, TRight right )
		{
			if( _dictionaryLR.TryGetValue( left, out TRight checkRight ) &&
			    checkRight.Equals( right ) &&
			    _dictionaryRL.ContainsKey( right ) )
			{
				RemoveInternal( left, right );
				return true;
			}

			return false;
		}

		public void Clear()
		{
			_dictionaryLR.Clear();
			_dictionaryRL.Clear();
		}

		public bool TryGetRight( TLeft left, out TRight right ) => _dictionaryLR.TryGetValue( left, out right );
		public bool TryGetLeft( TRight right, out TLeft left ) => _dictionaryRL.TryGetValue( right, out left );
		public TRight GetRight( TLeft left ) => _dictionaryLR[left];
		public TRight GetLeft( TLeft right ) => _dictionaryLR[right];
		public bool ContainsLeft( TLeft left ) => _dictionaryLR.ContainsKey( left );
		public bool ContainsRight( TRight right ) => _dictionaryRL.ContainsKey( right );

		public KeyValuePair<TLeft, TRight>[] ToArray() => _dictionaryLR.ToArray();
		public List<KeyValuePair<TLeft, TRight>> ToList() => _dictionaryLR.ToList();
		public IEnumerable<TLeft> Lefts => _dictionaryLR.Keys;
		public IEnumerable<TRight> Rights => _dictionaryRL.Keys;

#endregion

#region Internal

		// Prior error checking is assumed

		private void AddInternal( TLeft left, TRight right )
		{
			_dictionaryLR.Add( left, right );
			_dictionaryRL.Add( right, left );
		}

		private void RemoveInternal( TLeft left, TRight right )
		{
			_dictionaryLR.Remove( left );
			_dictionaryRL.Remove( right );
		}

#endregion

#region IEnumerable

		public IEnumerator<KeyValuePair<TLeft, TRight>> GetEnumerator() => _dictionaryLR.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#endregion

	}
}

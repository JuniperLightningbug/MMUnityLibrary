using UnityEngine;

namespace MM
{
	/**
	 * SEvents, SObservables, Observables, etc. are assignable int the inspector, and the first 2 are saved as assets.
	 * At runtime, we can assign an 'owner' Object (likely MonoBehaviour or GameObject).
	 * Other objects can listen for callbacks, but not post events or make changes.
	 * This behaviour is opt-in, and turned off by default.
	 */
	[System.Serializable]
    public class OwnershipGuard
    {
	    [SerializeField]
	    private Object _owner;
	    
	    public Object Owner => _owner;
	    
	    public bool BLocked => Owner;

	    public OwnershipGuard( Object withOwner = null ) { _owner = withOwner; }

	    public bool HasWriteAccess( Object from ) =>
		    !BLocked || Owner == from || Owner == (from as MonoBehaviour)?.gameObject;
	    
	    public void Lock( Object from, bool bForce = false )
	    {
		    if( !from || from == Owner )
		    {
			    return;
		    }

		    if( !HasWriteAccess( from ) )
		    {
			    if( !bForce )
			    {
				    Debug.LogWarning(
					    $"Skipping [{GetType()}] ownership request from '{from.name}':\n" +
					    $"Observable is already locked by '{Owner.name}'." );
				    return;
			    }

			    Debug.LogWarning(
				    $"Overwriting [{GetType()}] ownership: '{Owner.name}' -> '{from.name}'" );
		    }

		    _owner = from;
	    }

	    public void Release( Object from, bool bForce = false )
	    {
		    if( HasWriteAccess( from ) )
		    {
			    _owner = null;
		    }
	    }
    }
}

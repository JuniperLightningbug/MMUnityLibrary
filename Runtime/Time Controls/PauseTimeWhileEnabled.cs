using UnityEngine;

namespace MM
{
	/**
	 * Simple runtime implementation of 'GameTimeState' system: an object which pauses time while active and enabled.
	 * Multiple objects can use this functionality at the same time, and timescale will be 0 for as long as ANY of
	 * them are active and enabled.
	 *
	 * Example usage:
	 * 1. A time-sensitive tutorial UI pop-up
	 * 2. A dialogue system that pauses time for the player to read
	 * 3. The game's default pause menu
	 * 4. Controller disconnection warnings
	 */
	public class PauseTimeWhileEnabled : MonoBehaviour
	{
		void OnEnable()
		{
			GameTimeState.StartPauseTime( this );
		}

		void OnDisable()
		{
			GameTimeState.StopPauseTime( this );
		}
	}
}

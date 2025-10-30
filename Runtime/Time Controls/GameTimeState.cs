using System.Collections.Generic;
using UnityEngine;

namespace MM
{
	/**
	 * Static class to allow control of time from runtime objects.
	 * Singleton-like behaviour - data lifecycle is tied to the application play state.
	 */
    public static class GameTimeState
    {
	    private static float _defaultTimeScale;
	    private static HashSet<Object> _activeTimePauseSources;
	    private static bool _bTimeIsPaused;
	    
	    private static void RefreshTimeScale()
	    {
		    bool bTimeIsPausedNew = _activeTimePauseSources != null && _activeTimePauseSources.Count > 0;
		    if( bTimeIsPausedNew != _bTimeIsPaused )
		    {
			    _bTimeIsPaused = bTimeIsPausedNew;
			    Time.timeScale = bTimeIsPausedNew ? 0.0f : _defaultTimeScale;
		    }
	    }

#region Interface
	    
	    public static bool BTimeIsPaused => _bTimeIsPaused;

	    public static void StartPauseTime( Object source )
	    {
		    if( ApplicationUtils.BIsPlaying && source )
		    {
			    _activeTimePauseSources.Add( source );
			    RefreshTimeScale();
		    }
	    }
	    
	    public static void StopPauseTime( Object source )
	    {
		    if( ApplicationUtils.BIsPlaying && source )
		    {
			    _activeTimePauseSources.Remove( source );
			    RefreshTimeScale();
		    }
	    }

#endregion

#region Callbacks

	    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	    private static void OnRuntimeInitialise()
	    {
		    _activeTimePauseSources = new HashSet<Object>();
		    _defaultTimeScale = Time.timeScale;
		    _bTimeIsPaused = false;
		    
		    Application.quitting += OnQuit;
	    }

	    private static void OnQuit()
	    {
		    _activeTimePauseSources.Clear();
	    }
	    
#endregion
	    
    }
}

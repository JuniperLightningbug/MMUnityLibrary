using UnityEngine;

namespace MM
{
	public static class ApplicationUtils
	{
		public static bool BIsQuitting { get; private set; }
		public static bool BIsPlaying => Application.isPlaying && !BIsQuitting;
		
		static void OnQuit()
		{
			BIsQuitting = true;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] 
		static void OnRuntimeInitialise()
		{
			BIsQuitting = false;
			Application.quitting += OnQuit;
		} 
	}
}
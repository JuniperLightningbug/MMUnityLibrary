using UnityEngine;

namespace MM
{
	/**
	 * Exposes functions from generic typed 'SingletonHubBase' to inspector PropertyDrawers.
	 */
	public interface ISingletonHubBaseEditorUtility
	{
		public void TryRefreshInspectorData();
	}
}
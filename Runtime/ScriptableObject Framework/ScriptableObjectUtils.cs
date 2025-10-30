namespace MM
{
	public static class ScriptableObjectUtils
	{
		public const string kEventNameNoParams = "SEvent";
		public const string kEventNamePrefix = "SEvent_";
		public const string kEventMenuPrefix = "SEvent/";
		
		public const string kValueNamePrefix = "SValue_";
		public const string kValueMenuPrefix = "SValue/";
		public const string kObservableValueNamePrefix = "SObservable_";
		public const string kObservableValueMenuPrefix = "SObservable/";
		
		// Default menu orders
		// (Note: separation of 100 adds a dividing line)
		public const int kMenuOrder_Custom = -100;			// Project-specific type or collection
		public const int kMenuOrder_Empty = 0;				// e.g. SEvent with no argument
		public const int kMenuOrder_Primitive = 100;		// e.g. int, bool, float string
		public const int kMenuOrder_UnityComposite = 200;	// e.g. Vector3, Vector2, Color, Quaternion
		public const int kMenuOrder_Asset = 300;			// e.g. Mesh, Material, Prefab (as Object)
		public const int kMenuOrder_SceneRef = 400;			// e.g. GameObject, Collider, Transform, MeshRenderer
	}
}

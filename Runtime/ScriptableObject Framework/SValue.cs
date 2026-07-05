using UnityEngine;

namespace MM
{
	public class SValue<T> : SValue_Base
	{
		[SerializeField] protected T _value;
		[SerializeField] protected T _defaultValue;

#region SValue_Base
		public override void ResetToDefaultValue() => _value = _defaultValue;
		public override void ForInspector_StoreValueAsDefault() => _defaultValue = _value;
#endregion
		
#region Interface
		public virtual void Set( T inValue ) => _value = inValue;
		public virtual T Get() => _value;
		public virtual void Clear() => ResetInternal();
#endregion
	}
	
	
	/// <summary>
	/// Strongly-typed base class to help serialise properties to the inspector
	/// </summary>
	public abstract class SValue_Base : ScriptableObject
	{
		public abstract void ResetToDefaultValue();

		[ContextMenu( "Reset to Default Value" )]
		protected virtual void ResetInternal()
		{
			ResetToDefaultValue();
		}
		
		public abstract void ForInspector_StoreValueAsDefault();

#region Unity Callbacks
		protected virtual void OnDisable() => ResetInternal();
		protected virtual void OnEnable() => ResetInternal();
#endregion
	}
}

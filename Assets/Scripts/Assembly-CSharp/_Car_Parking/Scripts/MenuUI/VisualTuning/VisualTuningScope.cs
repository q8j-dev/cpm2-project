using System;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public abstract class VisualTuningScope
	{
		public Action LastSelectOperation;

		public Action<int> ItemSet;

		public Action<int> ItemSelected;

		public event Action OnValuesChanged;

		protected void InvokeOnValuesChanged()
		{
			OnValuesChanged?.Invoke();
		}

		public abstract void Init();

		public abstract void Apply();

		public abstract void Revert();

		public abstract void Select(int index);

		public virtual void SetDefault()
		{
		}

		public virtual void OnChangeScope()
		{
		}

		public virtual void Discard()
		{
			Revert();
		}

		public virtual CombinedPrice GetBoughtItemPrice()
		{
			return null;
		}
	}
}

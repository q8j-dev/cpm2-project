using System;
using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public abstract class EditableHUDElementIsActiveGetter : MonoBehaviour
	{
		public abstract bool IsActive { get; }

		public event Action<EditableHUDElementIsActiveGetter> OnIsActiveChanged;

		public abstract void Initialization();

		protected void OnOnIsActiveChangedEvent()
		{
			OnIsActiveChanged?.Invoke(this);
		}

		protected virtual void SetRefs()
		{
		}

		protected virtual void OnDestroy()
		{
			OnIsActiveChanged = null;
		}
	}
}

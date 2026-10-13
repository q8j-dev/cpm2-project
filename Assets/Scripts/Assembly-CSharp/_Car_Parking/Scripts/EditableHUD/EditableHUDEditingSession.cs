using System;
using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public static class EditableHUDEditingSession
	{
		private static bool _isOpen;

		public static bool IsOpen => _isOpen;

		public static event Action OnChanged;

		public static void SetOpen(bool isOpen)
		{
			if (_isOpen != isOpen)
			{
				_isOpen = isOpen;
				OnChanged?.Invoke();
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetState()
		{
			_isOpen = false;
			OnChanged = null;
		}
	}
}

using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	public class PhotoModeObserver
	{
		public event Action OnPhotoModeClosed;

		public void OnClosePhotoMode()
		{
			OnPhotoModeClosed?.Invoke();
		}
	}
}

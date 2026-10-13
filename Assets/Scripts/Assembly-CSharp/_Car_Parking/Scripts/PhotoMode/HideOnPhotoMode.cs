using System;
using UnityEngine;

namespace _Car_Parking.Scripts.PhotoMode
{
	public class HideOnPhotoMode : MonoBehaviour
	{
		private void Start()
		{
			V.OnPhotoModeChanged = (Action<bool>)Delegate.Combine(V.OnPhotoModeChanged, new Action<bool>(OnPhotoModeChanged));
			base.gameObject.SetActive(!FlagsStorage.IsPhotoMode);
		}

		private void OnDestroy()
		{
			V.OnPhotoModeChanged = (Action<bool>)Delegate.Remove(V.OnPhotoModeChanged, new Action<bool>(OnPhotoModeChanged));
		}

		private void OnPhotoModeChanged(bool isPhotoMode)
		{
			base.gameObject.SetActive(!isPhotoMode);
		}
	}
}

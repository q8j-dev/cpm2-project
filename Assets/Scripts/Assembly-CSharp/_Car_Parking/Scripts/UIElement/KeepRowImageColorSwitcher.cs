using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.UIElement.HUD;

namespace _Car_Parking.Scripts.UIElement
{
	public class KeepRowImageColorSwitcher : MonoBehaviour
	{
		[SerializeField]
		private Image ToggleIcon = default;

		private readonly Color _onColor = Color.green;

		private FreeDriveProvider _freeDriveProvider;

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		private void Start()
		{
			FreeDriveProvider freeDriveProvider = _freeDriveProvider;
			freeDriveProvider.OnKeepRowStateChanged = (Action<bool>)Delegate.Combine(freeDriveProvider.OnKeepRowStateChanged, new Action<bool>(SetColor));
		}

		private void OnDestroy()
		{
			FreeDriveProvider freeDriveProvider = _freeDriveProvider;
			freeDriveProvider.OnKeepRowStateChanged = (Action<bool>)Delegate.Remove(freeDriveProvider.OnKeepRowStateChanged, new Action<bool>(SetColor));
		}

		private void SetColor(bool isOn)
		{
			Color color = ((PlayerPrefs.GetInt("DarkButtons", 0) == 1) ? HudColorChanger.DarkColor : HudColorChanger.DefaultColor);
			color.a = 0.5f;
			ToggleIcon.color = (isOn ? _onColor : color);
		}
	}
}

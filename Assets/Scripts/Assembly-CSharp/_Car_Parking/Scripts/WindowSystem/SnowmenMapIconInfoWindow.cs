using CPMEngine;
using MVPFree.MapIcons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class SnowmenMapIconInfoWindow : WindowScript
	{
		private const string DestroyTextKey = "SnowmanMapWindow.Destroy";

		private const string SnowMensWithingKey = "SnowmanMapWindow.SnowmensWithin";

		private const string MinTextKey = "SnowmanMapWindow.Min";

		[Inject]
		private TimedActivityZoneController _timedActivityZoneController;

		[Inject]
		private FreeDriveProvider _freeDriveProvider;

		[Inject]
		private MapIconsStarter _mapIconsStarter;

		[SerializeField]
		private Button InfoButton;

		[SerializeField]
		private TextMeshProUGUI InfoLimitsText;

		public void Init(int index)
		{
			ZoneConfigData zoneConfigData = _mapIconsStarter.EventZoneCollectiblesPosDatas.AllZoneConfigDatas[index];
			string text = new LocalizedString("SnowmanMapWindow.Destroy");
			string text2 = new LocalizedString("SnowmanMapWindow.SnowmensWithin");
			string text3 = new LocalizedString("SnowmanMapWindow.Min");
			InfoLimitsText.text = $"{text} {zoneConfigData.ActiveCollectiblesCount} {text2} {Utils.ConvertToTime(zoneConfigData.TotalPassTime)} {text3}";
			InfoButton.onClick.AddListener(ShowEventBanner);
		}

		private void Update()
		{
			if (_freeDriveProvider.MapCamera.beganTouching || !_freeDriveProvider.MapCamera.ShowingBigMap)
			{
				Close();
			}
		}

		private void ShowEventBanner()
		{
			_managerUI.ShowEventViewWindow(_Car_Parking.Scripts.EventSystem.EventType.NewYearSnowManAttack, showOnTop: true);
		}

		protected override void OnDestroy()
		{
			InfoButton.onClick.RemoveAllListeners();
			base.OnDestroy();
		}
	}
}

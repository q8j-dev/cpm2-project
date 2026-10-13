using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Car_Parking.Scripts.Racing_Scripts;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class RallyModeBannerView : NotEventViewBase
	{
		[SerializeField]
		private Button StartButton;

		protected override void Start()
		{
			base.Start();
			StartButton.onClick.AddListener(StartRally);
		}

		private void StartRally()
		{
			V.CurrentRaceModeType = RacingModeType.Rally;
			_managerUI.ShowWaitingWindow(set: true).Forget();
			RallyController.LoadFromBanner = true;
			SceneManager.LoadSceneAsync(41);
		}
	}
}

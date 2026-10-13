using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class RallyOffroadBannerView : EventView, IBigBannerView
	{
		[SerializeField]
		private string BackgroundImagePath = default;

		[SerializeField]
		protected RawImageLoadAsync RawImageLoadAsync;

		[SerializeField]
		private UICupElement[] CupElements = default;

		[SerializeField]
		private Button StartButton = default;

		private bool _isDestroyed;

		private ManagerUI _managerUI;

		[Inject]
		public void Construct(ManagerUI managerUI)
		{
			_managerUI = managerUI;
		}

		private void Start()
		{
			StartButton.onClick.AddListener(StartRally);
		}

		public override void Init(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			base.Init(@event);
			RawImageLoadAsync.LoadFromSDN(BackgroundImagePath);
			_ = UIInitialize();
			SetupBanner().Forget();
		}

		private async UniTaskVoid UIInitialize()
		{
			RallyOffroadEventData rallyOffroadEventData = base.Event.Data as RallyOffroadEventData;
			if (rallyOffroadEventData?.Cups == null)
			{
				return;
			}
			int count = Mathf.Min(CupElements.Length, rallyOffroadEventData.Cups.Length);
			for (int i = 0; i < count; i++)
			{
				CupElements[i].SetProgress(rallyOffroadEventData.Cups[i], base.Event.Status == EventStatus.Claimed);
				await UniTask.Delay(400);
				if (_isDestroyed)
				{
					break;
				}
			}
		}

		private void StartRally()
		{
			V.CurrentRaceModeType = RacingModeType.Rally;
			_managerUI.ShowWaitingWindow(set: true).Forget();
			RallyController.LoadFromBanner = true;
			SceneManager.LoadSceneAsync(41);
		}

		private void OnDestroy()
		{
			_isDestroyed = true;
		}
	}
}

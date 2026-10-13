using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class NotEventViewBase : MonoBehaviour, IBigBannerView, IBackableWindow
	{
		[SerializeField]
		private string BackgroundImagePath = default;

		[SerializeField]
		protected RawImageLoadAsync RawImageLoadAsync;

		[SerializeField]
		private Button CloseButton = default;

		private int _index;

		protected ManagerUI _managerUI;

		protected bool _isClosing;

		[Inject]
		public void Construct(ManagerUI managerUI)
		{
			_managerUI = managerUI;
		}

		protected virtual void Start()
		{
			CloseButton.onClick.AddListener(Close);
			InitBackButton();
		}

		private void Close()
		{
			if (!_isClosing)
			{
				_managerUI.CloseEventViewWindow();
				_isClosing = true;
			}
		}

		public void InitBanner(IBigBannerView.BannerData bannerData)
		{
			_index = bannerData.BannerID;
			RawImageLoadAsync.LoadFromSDN(BackgroundImagePath);
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			Close();
		}

		public bool IsCorrectPrefab(IBigBannerView.BannerData bannerData)
		{
			return _index == bannerData.BannerID;
		}

		public void Shown()
		{
		}
	}
}

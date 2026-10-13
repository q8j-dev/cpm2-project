using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	public class OtherSmallBannerItemBase : MonoBehaviour
	{
		[SerializeField]
		private string imagePath;

		[SerializeField]
		protected RawImageLoadAsync RawImageLoadAsync;

		[SerializeField]
		private Button Button;

		protected ManagerUI _managerUI;

		private int _index;

		[Inject]
		public void Construct(ManagerUI managerUI)
		{
			_managerUI = managerUI;
		}

		private void Start()
		{
			Button.onClick.AddListener(OnClick);
		}

		public void Init(int index)
		{
			_index = index;
			RawImageLoadAsync.LoadFromSDN(imagePath);
		}

		protected virtual void OnClick()
		{
			EventViewWindow.AdditionalData value = new EventViewWindow.AdditionalData(new IBigBannerView.BannerData(_index));
			_managerUI.ShowWindow("EventViewWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, JsonConvert.SerializeObject(value)).Forget();
		}
	}
}

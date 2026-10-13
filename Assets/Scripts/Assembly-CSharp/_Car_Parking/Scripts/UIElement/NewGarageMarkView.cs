using CPM.Scripts.Garage.GarageBuyer;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	public class NewGarageMarkView : MonoBehaviour
	{
		private IGarageBuyService _garageBuyService;

		[Inject]
		public void Construct(IGarageBuyService garageBuyService)
		{
			_garageBuyService = garageBuyService;
		}

		private void Start()
		{
			bool flag = true;
			for (int i = 0; i < _garageBuyService.BuyingGaragesStatus.Count; i++)
			{
				if (!_garageBuyService.GetGarageStatus(i))
				{
					flag = false;
				}
			}
			int num = ObscuredPrefs.GetInt("GarageButtonImportantClicks");
			base.gameObject.SetActive(num == 0 && !flag);
		}
	}
}

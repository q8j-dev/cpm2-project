using MVPFree.MapIcons.Enum;
using UnityEngine;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.PlayerProperty.Processor;

namespace _Car_Parking.Scripts.PlayerProperty.Trigger
{
	public class HouseBuyTrigger : PropertyTrigger
	{
		[SerializeField]
		private int ID;

		[SerializeField]
		private GameObject[] ExtraEnabledGos;

		public int HouseId => ID;

		protected override void Awake()
		{
			bool flag = ApartmentsBuyer.IsHomeBought(ID);
			bool flag2 = !flag;
			base.Awake();
			base.gameObject.SetActive(flag2);
			UpdateExtraEnabledGo(flag);
			SetEnabled(flag2);
			InitializeProcessor(new HouseBuyProcessor(ID));
			if (flag2)
			{
				SetMainImage(IconType.BuyHome);
			}
			ApartmentsBuyer.HouseBoughtFree += OnHouseBought;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ApartmentsBuyer.HouseBoughtFree -= OnHouseBought;
		}

		private void OnHouseBought(int houseId)
		{
			if (houseId == ID)
			{
				SetEnabled(value: false);
				HideTriggerVisual();
				UpdateExtraEnabledGo(bought: true);
			}
		}

		private void UpdateExtraEnabledGo(bool bought)
		{
			GameObject[] extraEnabledGos = ExtraEnabledGos;
			foreach (GameObject gameObject in extraEnabledGos)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(!bought);
				}
			}
		}
	}
}

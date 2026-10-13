using CodeStage.AntiCheat.Storage;
using MVPFree.MapIcons.Enum;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.WindowSystem.PropertyPopupWindow
{
	public class HouseMapPopupWindow : PropertyPopupWindow
	{
		[SerializeField]
		private Transform OwnedObject;

		private HomesConfig _homesConfig;

		private int _index;

		[Inject]
		public void Construct(HomesConfig homesConfig)
		{
			_homesConfig = homesConfig;
		}

		public void Initialize(int index, IconType iconType)
		{
			_index = index;
			UpdateOwnershipState(IsBought());
			HomesConfig.HomeDataItem homeDataItem = _homesConfig.Get(index);
			Init(new LocalizedString(homeDataItem.Name), new LocalizedString(homeDataItem.Location.GetEnumMemberValue()), iconType, homeDataItem.GetGlobalCost);
		}

		private void UpdateOwnershipState(bool isBought)
		{
			base.CostContainer.gameObject.SetActive(!isBought);
			OwnedObject.gameObject.SetActive(isBought);
		}

		private bool IsBought()
		{
			return ObscuredPrefs.GetInt(GetHomePrefKey()) == 1;
		}

		private string GetHomePrefKey()
		{
			return "homeOpen" + _index;
		}
	}
}

using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Realty;

namespace _Car_Parking.Scripts.UIElement
{
	public class RealtyPlaceUIItem : PlaceUIItem<RealtyConfig, RealtyData>
	{
		[SerializeField]
		private Button ActionButton = default;

		[SerializeField]
		private Image MainImage = default;

		[SerializeField]
		private Image Icon = default;

		[SerializeField]
		private TextMeshProUGUI Name = default;

		public override void Init()
		{
			base.Init();
			MainImage.sprite = base.DataItem.MainImage;
			Icon.sprite = base.DataItem.Icon;
			Name.text = new LocalizedString(base.DataItem.RealtyNameKey);
			ActionButton.onClick.AddListener(OnRealtyClick);
		}

		protected override RealtyData GetData(int index)
		{
			return config.RealtyData[index];
		}

		private void OnRealtyClick()
		{
			string realtyWindowName = base.DataItem.RealtyWindowName;
			if (realtyWindowName == "GarageSelectWindow")
			{
				V.CoreGameManager.ManagerUI.ShowGarageSelectWindow();
			}
			else
			{
				V.CoreGameManager.ManagerUI.ShowWindow(realtyWindowName).Forget();
			}
		}
	}
}

using Newtonsoft.Json;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class BigOfferSkinsWindow : BigOfferWindowBase
	{
		protected override void ButtonViewInShopClickCallback()
		{
			base.ButtonViewInShopClickCallback();
			PrepareToClose();
			_coreGameManager.menuControl.ShowSelectDriver(JsonConvert.SerializeObject(new PersonWindowAdditionalData
			{
				ParentWindow = PersonWindowAdditionalData.ParentWindowType.MainMenu,
				PersonClothsEventType = PersonClothsEventType.NewYear
			}));
			_managerUI.ShowUserBalanceWindow();
		}
	}
}

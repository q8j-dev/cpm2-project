using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class EmojiWindow : WindowScript, IBackableWindow
	{
		private MenuLevelParent _menuLevelParent;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.CarOther;

		[Inject]
		public void Construct(MenuLevelParent menuLevelParent)
		{
			_menuLevelParent = menuLevelParent;
		}

		protected override void Start()
		{
			InitBackButton();
			base.Start();
		}

		protected override void OnEnable()
		{
			UiInputSetup.EnableUiNavigation(base.gameObject);
			UISelectionCursor.ShowCursor(null, force: true);
			UiInputSetup.AutoSelectFirstInteractable(base.gameObject).Forget();
			base.OnEnable();
		}

		private void OnDisable()
		{
			UiInputSetup.DisableUiNavigation(base.gameObject);
		}

		public void Back()
		{
			_menuLevelParent.HideEmoji();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}
	}
}

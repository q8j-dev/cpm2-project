using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Animations.Emotes.Locker;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.AnimationUI
{
	public class MenuGarageAnimationPanel : MonoBehaviour
	{
		[SerializeField]
		private Button EmojiButton = default;

		[SerializeField]
		private Toggle AnimationPresetShowToggle = default;

		[SerializeField]
		private GameObject AnimationsPreset = default;

		[SerializeField]
		private Button AnimationShopButton = default;

		[SerializeField]
		private Button AnimationsPresetCloseButton = default;

		[SerializeField]
		private MenuGarageNavigationWindow MenuGarageWindow = default;

		[SerializeField]
		private GameObject OnImage = default;

		[SerializeField]
		private GameObject OffImage = default;

		private ManagerUI _managerUI;

		private CoreGameManager _coreGameManager;

		private AnimationShopOpenRequest _openRequest;

		private EmoteLockerOpenRequest _lockerOpenRequest;

		[Inject]
		public void Construct(ManagerUI managerUI, CoreGameManager coreGameManager, AnimationShopOpenRequest openRequest, EmoteLockerOpenRequest lockerOpenRequest)
		{
			_managerUI = managerUI;
			_coreGameManager = coreGameManager;
			_openRequest = openRequest;
			_lockerOpenRequest = lockerOpenRequest;
		}

		private void Start()
		{
			if ((bool)EmojiButton)
			{
				EmojiButton.onClick.AddListener(OpenEmoteLocker);
			}
			if ((bool)AnimationShopButton)
			{
				AnimationShopButton.onClick.AddListener(OpenAnimationShop);
			}
			if ((bool)AnimationsPresetCloseButton)
			{
				AnimationsPresetCloseButton.onClick.AddListener(ClosePanel);
			}
			HidePresetPanel();
		}

		private void OnDisable()
		{
			HidePresetPanel();
		}

		private void ClosePanel()
		{
			HidePresetPanel();
		}

		private void OpenEmoteLocker()
		{
			if (CanOpenLockerNow())
			{
				_lockerOpenRequest.Set(new EmoteLockerContext
				{
					ExitWalkForBanner = ExitWalkForBanner
				});
				_managerUI.ShowEmoteLockerWindow();
			}
		}

		private bool ExitWalkForBanner()
		{
			if (!MenuGarageWindow)
			{
				return false;
			}
			return MenuGarageWindow.TryExitWalkForAnimationShop();
		}

		public void OpenAnimationShopExternal()
		{
			OpenAnimationShop();
		}

		public void OpenEmoteLockerExternal()
		{
			OpenEmoteLocker();
		}

		private void OpenAnimationShop()
		{
			if (CanUseAnimationsNow() && (bool)MenuGarageWindow && MenuGarageWindow.TryExitWalkForAnimationShop())
			{
				HidePresetPanel();
				_openRequest.Set(AnimationShopTab.Poses);
				_coreGameManager.menuControl.ShowSelectDriver(JsonConvert.SerializeObject(CreateShopOpenData()));
				_managerUI.ShowUserBalanceWindow();
			}
		}

		private void HidePresetPanel()
		{
			if ((bool)AnimationsPreset)
			{
				AnimationsPreset.SetActive(value: false);
			}
			if ((bool)OnImage)
			{
				OnImage.SetActive(value: false);
			}
			if ((bool)OffImage)
			{
				OffImage.SetActive(value: true);
			}
			if ((bool)AnimationPresetShowToggle && AnimationPresetShowToggle.isOn)
			{
				AnimationPresetShowToggle.SetIsOnWithoutNotify(value: false);
			}
		}

		private bool CanUseAnimationsNow()
		{
			if (!TryReadPersonState(out var ragdolled, out var animatingPlayer))
			{
				return false;
			}
			return CanUseAnimations(FlagsStorage.OutOfCar, ragdolled, animatingPlayer);
		}

		private bool CanOpenLockerNow()
		{
			if (!TryReadPersonState(out var ragdolled, out var animatingPlayer))
			{
				return false;
			}
			return CanOpenLocker(ragdolled, animatingPlayer);
		}

		private static bool TryReadPersonState(out bool ragdolled, out bool animatingPlayer)
		{
			ragdolled = false;
			animatingPlayer = false;
			if (!V.DriverPerson)
			{
				return false;
			}
			DriverActionController component = V.DriverPerson.GetComponent<DriverActionController>();
			RagdollCPM ragdollCPM = (component ? component.ragdollCpm : null);
			MainPerson component2 = V.DriverPerson.GetComponent<MainPerson>();
			ragdolled = ragdollCPM != null && (ragdollCPM.isRagdoll || ragdollCPM.isStadningUp);
			animatingPlayer = (bool)component2 && (bool)component2.PersonAnimationsController && component2.PersonAnimationsController.AnimationService.IsAnimatingPlayer();
			return true;
		}

		public static bool CanUseAnimations(bool outOfCar, bool ragdolled, bool animatingPlayer)
		{
			if (outOfCar && !ragdolled)
			{
				return !animatingPlayer;
			}
			return false;
		}

		public static bool CanOpenLocker(bool ragdolled, bool animatingPlayer)
		{
			if (!ragdolled)
			{
				return !animatingPlayer;
			}
			return false;
		}

		public static PersonWindowAdditionalData CreateShopOpenData()
		{
			return new PersonWindowAdditionalData
			{
				ParentWindow = PersonWindowAdditionalData.ParentWindowType.MainMenu,
				OpenCategory = PersonWindowCategoryType.Poses
			};
		}
	}
}

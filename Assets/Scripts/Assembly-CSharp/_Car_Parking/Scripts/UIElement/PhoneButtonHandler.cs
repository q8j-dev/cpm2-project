using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement
{
	public class PhoneButtonHandler : MonoBehaviour
	{
		[SerializeField]
		private Button PhoneButton = default;

		private ManagerUI _managerUI;

		private LevelScript _levelScript;

		private OptionalDependency<LevelScript> _levelScriptDep;

		private InputItemsProvider _inputItemsProvider;

		private OptionalDependency<InputItemsProvider> _inputItemsProviderDep;

		private FreeTimeAttackController _freeTimeAttackController;

		private OptionalDependency<FreeTimeAttackController> _freeTimeAttackControllerDep;

		private EventRegistry _eventRegistry;

		private RegistryHandle<ScriptableEvent> _openPhoneHandler = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OpenPhone);

		[Inject]
		public void Construct(ManagerUI managerUI, EventRegistry eventRegistry, OptionalDependency<LevelScript> levelScript, OptionalDependency<ChatControl> chatControl, OptionalDependency<InputItemsProvider> inputItemsProvider, OptionalDependency<FreeTimeAttackController> freeTimeAttackController)
		{
			_levelScriptDep = levelScript;
			_managerUI = managerUI;
			_eventRegistry = eventRegistry;
			_inputItemsProviderDep = inputItemsProvider;
			_freeTimeAttackControllerDep = freeTimeAttackController;
		}

		private void Start()
		{
			_levelScript = _levelScriptDep?.Value;
			_inputItemsProvider = _inputItemsProviderDep?.Value;
			_freeTimeAttackController = _freeTimeAttackControllerDep?.Value;
			PhoneButton.onClick.AddListener(OpenPhone);
			_openPhoneHandler.Get(_eventRegistry).Subscribe(OpenPhone);
		}

		private void OnDestroy()
		{
			_openPhoneHandler.Get(_eventRegistry).Unsubscribe(OpenPhone);
		}

		private void OpenPhone()
		{
			MainPerson component;
			if (_freeTimeAttackController != null && _freeTimeAttackController.IsRaceInProgress())
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("FreeRace.WaitTimeAttack"), 3f).Forget();
			}
			else if (!V.DriverPerson.TryGetComponent<MainPerson>(out component) || !component.PersonAnimationsController || (!component.PersonAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate && !component.PersonAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate))
			{
				SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
				_managerUI.ShowPhoneWindowAsync().Forget();
				if ((bool)_levelScript)
				{
					_levelScript.CleanScreen();
				}
				if (_freeTimeAttackController != null && _freeTimeAttackController.IsRaceInProgress())
				{
					_managerUI.CloseMultiplayerRacePlayWindow();
				}
				if (FlagsStorage.IsMultiFree)
				{
					_Car_Parking.Scripts.Audio.Audio.SetAllVolumeBySavedVolume(0.5f);
				}
				Time.timeScale = 1f;
				if (_inputItemsProvider != null && (bool)_inputItemsProvider.InputTouch && (bool)_inputItemsProvider.InputTouch.TouchField)
				{
					_inputItemsProvider.InputTouch.TouchField.OnPointerUp(null);
				}
			}
		}
	}
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Database;
using CPM.Scripts.Garage;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Windows.FirstGameWindow;

namespace _Car_Parking.Scripts.FirstGameSetup.Presets
{
	public class PersonPresetsView : MonoBehaviour
	{
		[SerializeField]
		private PersonPresetToggle PresetPrefab = default;

		private MainPerson _mainPerson;

		private Dictionary<CharacterEquipmentPreset, RectTransform> _presetsTransforms = new Dictionary<CharacterEquipmentPreset, RectTransform>();

		private Dictionary<CharacterEquipmentPreset, PersonPresetToggle> _presetsToggles = new Dictionary<CharacterEquipmentPreset, PersonPresetToggle>();

		private CancellationTokenSource _cancellationTokenScrollTo;

		private bool _isScrollTo;

		private CharactersService _charactersService;

		private IPrefabFactory _prefabFactory;

		private IGarageSelectorService _garageSelectorService;

		private GaragePositionsConfig _garagePositionsConfig;

		private CameraManager _cameraManager;

		private OptionalDependency<CameraManager> _cameraManagerDep;

		private ICharacterEquipmentDataService _characterEquipmentDataService;

		private EquipmentsData _equipmentsData;

		private FirstGameWindow _firstGameWindow;

		private CharacterEquipmentPreset _pendingPreset;

		private PresetGrantSession _grantSession;

		private List<List<int>> _presetsEquipments = new List<List<int>>();

		[field: SerializeField]
		public ScrollRect ScrollRect { get; private set; }

		[field: SerializeField]
		public float ScrollSpeed { get; private set; } = 5f;

		[field: SerializeField]
		public ClickDetector ClickDetector { get; private set; }

		[field: SerializeField]
		public RectTransform ParentForItems { get; private set; }

		[field: SerializeField]
		public ToggleGroup ToggleGroup { get; private set; }

		[field: SerializeField]
		public CharacterEquipmentPresetsSo CharacterEquipmentPresetsSo { get; private set; }

		public bool IsDiscardChangesPanelVisible { get; set; }

		private bool IsCheckPresets { get; set; } = true;

		[Inject]
		public void Construct(CharactersService charactersService, IPrefabFactory prefabFactory, IGarageSelectorService garageSelectorService, GaragePositionsConfig garagePositionsConfig, ICharacterEquipmentDataService characterEquipmentDataService, PresetGrantSession grantSession, OptionalDependency<CameraManager> cameraManager)
		{
			_charactersService = charactersService;
			_prefabFactory = prefabFactory;
			_garageSelectorService = garageSelectorService;
			_garagePositionsConfig = garagePositionsConfig;
			_cameraManagerDep = cameraManager;
			_characterEquipmentDataService = characterEquipmentDataService;
			_grantSession = grantSession;
		}

		private void Awake()
		{
			_cameraManager = _cameraManagerDep?.Value;
		}

		public void Init(FirstGameWindow firstGameWindow)
		{
			_firstGameWindow = firstGameWindow;
			_firstGameWindow.KeepButton.onClick.AddListener(KeepKits);
			_firstGameWindow.ResetButton.onClick.AddListener(ResetKits);
			if ((bool)V.DriverPerson)
			{
				_mainPerson = V.DriverPerson.GetComponent<MainPerson>();
			}
			for (int i = 0; i < CharacterEquipmentPresetsSo.Presets.Length; i++)
			{
				PersonPresetToggle personPresetToggle = Object.Instantiate(PresetPrefab, ParentForItems);
				personPresetToggle.Init(CharacterEquipmentPresetsSo.Presets[i], ToggleGroup);
				personPresetToggle.OnSelected += OnSelectedPreset;
				personPresetToggle.ClickDetector.OnPressed += (bool value) =>
				{
					if (value && _isScrollTo)
					{
						CancelToken();
					}
				};
				_presetsTransforms.Add(CharacterEquipmentPresetsSo.Presets[i], personPresetToggle.GetComponent<RectTransform>());
				_presetsToggles.Add(CharacterEquipmentPresetsSo.Presets[i], personPresetToggle);
			}
			if (!ToggleGroup.allowSwitchOff && ParentForItems.childCount > 0 && ParentForItems.GetChild(0).TryGetComponent<PersonPresetToggle>(out var component))
			{
				component.Toggle.isOn = true;
			}
			ClickDetector.OnPressed += (bool value) =>
			{
				if (value && _isScrollTo)
				{
					CancelToken();
				}
			};
			foreach (CharacterEquipmentPreset key in _presetsTransforms.Keys)
			{
				List<int> list = new List<int>();
				list.Add(key.GetId(KitType.Hair));
				list.Add(key.GetId(KitType.Face));
				list.Add(key.GetId(KitType.Beard));
				list.Add(key.GetId(KitType.Caps));
				list.Add(key.GetId(KitType.Mask));
				list.Add(key.GetId(KitType.Top));
				list.Add(key.GetId(KitType.Gloves));
				list.Add(key.GetId(KitType.Bag));
				list.Add(key.GetId(KitType.Pants));
				list.Add(key.GetId(KitType.Shoes));
				list.Add(key.GetId(KitType.Glasses));
				_presetsEquipments.Add(list);
			}
		}

		private void CancelToken()
		{
			_cancellationTokenScrollTo?.Cancel();
		}

		private void ScrollToPreset(CharacterEquipmentPreset preset)
		{
			CancelToken();
			_cancellationTokenScrollTo = new CancellationTokenSource();
			ScrollToAsync(_presetsTransforms[preset], _cancellationTokenScrollTo.Token).Forget();
		}

		private async UniTask ScrollToAsync(RectTransform target, CancellationToken token)
		{
			_isScrollTo = true;
			RectTransform content = ScrollRect.content;
			RectTransform viewport = ScrollRect.viewport;
			Vector2 center = viewport.rect.center;
			Vector2 vector = content.InverseTransformPoint(target.position);
			Vector2 vector2 = content.InverseTransformPoint(viewport.TransformPoint(center));
			float num = vector.y - vector2.y;
			float num2 = content.rect.height - viewport.rect.height;
			if (num2 <= 0f)
			{
				return;
			}
			float normalizedTarget = Mathf.Clamp01(ScrollRect.verticalNormalizedPosition + num / num2);
			while (!token.IsCancellationRequested)
			{
				ScrollRect.verticalNormalizedPosition = Mathf.Lerp(ScrollRect.verticalNormalizedPosition, normalizedTarget, Time.deltaTime * ScrollSpeed);
				if (Mathf.Abs(ScrollRect.verticalNormalizedPosition - normalizedTarget) < 0.001f)
				{
					ScrollRect.verticalNormalizedPosition = normalizedTarget;
					break;
				}
				await UniTask.Yield(token);
			}
			_isScrollTo = false;
		}

		private void OnSelectedPreset(CharacterEquipmentPreset preset)
		{
			if (ToggleGroup.allowSwitchOff && IsCheckPresets && !IsCurrentKitsEqualsPreset())
			{
				_pendingPreset = preset;
				_firstGameWindow.DiscardChangesPanel.SetActive(value: true);
				_firstGameWindow.MainPanel.SetActive(value: false);
				_presetsToggles[preset].Toggle.SetIsOnWithoutNotify(value: false);
				_presetsToggles[preset].UpdateVisible(selected: false);
				return;
			}
			if ((int)_mainPerson.Gender != preset.Gender)
			{
				Object.Destroy(V.DriverPerson);
				V.DriverPerson = _prefabFactory.Instantiate(Resources.Load("MenuPerson/person" + preset.Gender, typeof(GameObject)) as GameObject);
				V.DriverPerson.transform.position = _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].PersonCustomizationPosition.Position;
				V.DriverPerson.transform.rotation = _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].PersonCustomizationPosition.Rotation;
				_mainPerson = V.DriverPerson.GetComponent<MainPerson>();
				_cameraManager.ChangeTarget(_mainPerson.middle);
				_characterEquipmentDataService.Gender = preset.Gender;
				NewMoveController component = V.DriverPerson.GetComponent<NewMoveController>();
				if ((bool)_mainPerson && (bool)_mainPerson.PersonAnimationsController)
				{
					PersonAnimationsController personAnimationsController = _mainPerson.PersonAnimationsController;
					personAnimationsController.Animator = personAnimationsController.GetComponent<Animator>();
					_mainPerson.PersonAnimationsController.Init(component);
				}
			}
			_equipmentsData = _charactersService.GetEquipmentsData(_mainPerson.Gender);
			ApplyPresetOwnership(preset);
			SetEquipment(KitType.Hair, preset.GetId(KitType.Hair));
			SetEquipment(KitType.Face, preset.GetId(KitType.Face));
			SetEquipment(KitType.Beard, preset.GetId(KitType.Beard));
			SetEquipment(KitType.Caps, preset.GetId(KitType.Caps));
			SetEquipment(KitType.Mask, preset.GetId(KitType.Mask));
			SetEquipment(KitType.Top, preset.GetId(KitType.Top));
			SetEquipment(KitType.Gloves, preset.GetId(KitType.Gloves));
			SetEquipment(KitType.Bag, preset.GetId(KitType.Bag));
			SetEquipment(KitType.Pants, preset.GetId(KitType.Pants));
			SetEquipment(KitType.Shoes, preset.GetId(KitType.Shoes));
			SetEquipment(KitType.Glasses, preset.GetId(KitType.Glasses));
			_charactersService.LoadAllMask(V.DriverPerson);
			ToggleGroup.allowSwitchOff = false;
			ScrollToPreset(preset);
		}

		private void ApplyPresetOwnership(CharacterEquipmentPreset preset)
		{
			if (_grantSession.IsActive)
			{
				RevokeItemsOwnership(_charactersService.GetEquipmentsData(_grantSession.Gender), _grantSession.Items);
			}
			_grantSession.Begin(preset.Gender);
			GrantPresetOwnership(preset, _charactersService.GetEquipmentsData(preset.Gender), _grantSession.Items);
		}

		public static void GrantPresetOwnership(CharacterEquipmentPreset preset, EquipmentsData equipmentsData, List<(int type, int id)> newlyGranted = null)
		{
			for (int i = 0; i < 11; i++)
			{
				int id = preset.GetId((KitType)i);
				if (id >= 0 && !equipmentsData.IsItemBought(i, id))
				{
					equipmentsData.BuyItem(i, id);
					newlyGranted?.Add((i, id));
				}
			}
		}

		public static void RevokeItemsOwnership(EquipmentsData equipmentsData, List<(int type, int id)> items)
		{
			foreach (var (typeId, item) in items)
			{
				equipmentsData.GetBoughtList(typeId)?.Remove(item);
			}
			items.Clear();
		}

		private void SetEquipment(KitType kitType, int id)
		{
			if ((bool)_mainPerson && _equipmentsData.SelectedEquipments[(int)kitType] != id)
			{
				_charactersService.LoadPersonEquipment(V.DriverPerson, (int)kitType, id);
				_equipmentsData.SetSelectedItem((int)kitType, id);
			}
		}

		private bool IsCurrentKitsEqualsPreset()
		{
			List<int> selectedEquipments = _charactersService.GetEquipmentsData(_mainPerson.Gender).SelectedEquipments;
			foreach (List<int> presetsEquipment in _presetsEquipments)
			{
				if (selectedEquipments.SequenceEqual(presetsEquipment))
				{
					return true;
				}
			}
			return false;
		}

		private void KeepKits()
		{
			_firstGameWindow.MainPanel.SetActive(value: true);
			_firstGameWindow.DiscardChangesPanel.SetActive(value: false);
		}

		private void ResetKits()
		{
			_firstGameWindow.MainPanel.SetActive(value: true);
			_firstGameWindow.DiscardChangesPanel.SetActive(value: false);
			IsCheckPresets = false;
			_presetsToggles[_pendingPreset].Toggle.isOn = true;
		}

		private void OnDestroy()
		{
			CancelToken();
			if ((bool)_firstGameWindow)
			{
				if ((bool)_firstGameWindow.KeepButton)
				{
					_firstGameWindow.KeepButton.onClick.RemoveAllListeners();
				}
				if ((bool)_firstGameWindow.ResetButton)
				{
					_firstGameWindow.ResetButton.onClick.RemoveAllListeners();
				}
			}
		}
	}
}

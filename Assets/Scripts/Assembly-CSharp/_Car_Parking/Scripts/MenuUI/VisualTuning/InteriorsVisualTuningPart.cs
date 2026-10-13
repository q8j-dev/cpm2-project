using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Configs;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Interior;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class InteriorsVisualTuningPart
	{
		public const string InteriorsInfoKey = "InteriorsInfo/";

		private const int NoInteriorId = 0;

		private Vector3 _offsetForCameraIntoCarRotation = new Vector3(10f, 40f, 0f);

		private VehicleData _vehicleData;

		private DriverController _driverController;

		private VisualItemElement _uiElementPrefab;

		private InteriorInfoConfig _config;

		private VisualTuningWindow _window;

		private Texture _icon;

		private DownloadAssetButton _downloadAssetButtonPrefab;

		private IPrefabFactory _prefabFactory;

		private CameraManager _cameraManager;

		private AssetBundleManager _assetBundleManager;

		private CancellationTokenSource _downloadInteriorCancellationTokenSource;

		private Quaternion _initRotationCamInsideView;

		private readonly List<VisualItemElement> _uiElements = new List<VisualItemElement>();

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, CameraManager cameraManager, AssetBundleManager assetBundleManager)
		{
			_prefabFactory = prefabFactory;
			_cameraManager = cameraManager;
			_assetBundleManager = assetBundleManager;
		}

		public void Init(VisualTuningWindow window, VehicleData vehicleData, VisualItemElement uiElementPrefab, Texture icon, DownloadAssetButton downloadAssetButton)
		{
			_vehicleData = vehicleData;
			_uiElementPrefab = uiElementPrefab;
			_window = window;
			_icon = icon;
			_downloadAssetButtonPrefab = downloadAssetButton;
			_driverController = (_vehicleData ? _vehicleData.GetComponent<DriverController>() : null);
			_initRotationCamInsideView = (_driverController ? _driverController.camInsideView.transform.rotation : Quaternion.identity);
		}

		public void OpenPanel()
		{
			_config = _vehicleData.InteriorsComposition.InteriorInfoConfig;
			_uiElements.Clear();
			_window.VisualTuningElementSpawner.SpawnList(_config.InteriorsInfos, _uiElementPrefab, InitInteriorUiItem);
			if ((bool)_driverController)
			{
				Vector3 offsetForCameraIntoCarRotation = _offsetForCameraIntoCarRotation;
				offsetForCameraIntoCarRotation.y = (_vehicleData.RightHanded ? (0f - offsetForCameraIntoCarRotation.y) : offsetForCameraIntoCarRotation.y);
				Vector3 euler = _driverController.camInsideView.transform.rotation.eulerAngles + offsetForCameraIntoCarRotation;
				_driverController.camInsideView.transform.rotation = Quaternion.Euler(euler);
			}
			_cameraManager.ActivateCamera(RacingCameraType.InCar, V.LastUsedCar.GetComponent<DriverController>().camInsideView.transform);
			_downloadInteriorCancellationTokenSource = new CancellationTokenSource();
		}

		public void ClosePanel()
		{
			_window.SetActiveControlButtons(isActive: false);
			_vehicleData.InteriorsComposition.ResetInterior();
			_vehicleData.InteriorsComposition.TryRestoreSavedInterior();
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, _vehicleData.transform);
			if ((bool)_driverController)
			{
				_driverController.camInsideView.transform.rotation = _initRotationCamInsideView;
			}
			if (_downloadInteriorCancellationTokenSource != null && !_downloadInteriorCancellationTokenSource.Token.IsCancellationRequested)
			{
				_downloadInteriorCancellationTokenSource?.Cancel();
				_downloadInteriorCancellationTokenSource?.Dispose();
				_downloadInteriorCancellationTokenSource = null;
			}
		}

		private void InitInteriorUiItem(GameObject uiElement, object info)
		{
			VisualItemElement visualItemElement = uiElement.GetComponent<VisualItemElement>();
			InteriorInfo interiorInfo = info as InteriorInfo;
			if (!visualItemElement || interiorInfo == null)
			{
				return;
			}
			visualItemElement.RectTransformElementImage.anchorMin = new Vector2(0.5f, 0.5f);
			visualItemElement.RectTransformElementImage.anchorMax = new Vector2(0.5f, 0.5f);
			visualItemElement.elementImage.texture = _icon;
			visualItemElement.elementImage.SetNativeSize();
			visualItemElement.AspectRatioFitterElementImage.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
			visualItemElement.AspectRatioFitterElementImage.aspectRatio = 0.6f;
			UIHelper.DelayedRebuildLayout(visualItemElement.RectTransformElementImage).Forget();
			_uiElements.Add(visualItemElement);
			visualItemElement.index = interiorInfo.Number;
			visualItemElement.bodyKitType = BodyKitType.Interior;
			visualItemElement.Button.group = _window.toggleGroup;
			visualItemElement.Button.onValueChanged.AddListener((bool isOn) =>
			{
				if (isOn)
				{
					OnSelectInteriorUiItem(visualItemElement, interiorInfo);
				}
			});
			visualItemElement.Button.SetIsOnWithoutNotify(interiorInfo.Number == _vehicleData.carInfo.InstalledBodyKits.InteriorId);
			visualItemElement.IsBought = _vehicleData.carInfo.BoughtBodyKits.GetBodyKitPaidStatus(BodyKitType.Interior, interiorInfo.Number) || interiorInfo.Number == 1;
			visualItemElement.elementPrice.text = (visualItemElement.IsBought ? "0" : interiorInfo.Price.ToString());
			int version = InteriorVersionStorage.GetVersionOfInterior(_vehicleData.CarId) + 6;
			string name = ((interiorInfo.Number > 1) ? $"{_vehicleData.CarId}i_{interiorInfo.Number}" : $"{_vehicleData.CarId}i");
			if (_assetBundleManager.IsCachedAssetBundle(name, version) || CarIdStorage.LoadsBuiltInInteriorViaAddressables(_vehicleData.CarId, interiorInfo.Number))
			{
				return;
			}
			DownloadAssetButton downloadAssetButton = _prefabFactory.Instantiate(_downloadAssetButtonPrefab, visualItemElement.Button.transform);
			downloadAssetButton.Initialize(interiorInfo.AssetSize);
			visualItemElement.Button.interactable = false;
			downloadAssetButton.OnDownloadCompleted += () =>
			{
				visualItemElement.Button.interactable = true;
			};
			downloadAssetButton.Button.onClick.AddListener(() =>
			{
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("GarageMenuWindow.NoInternetConnection"), 2f);
				}
				else
				{
					downloadAssetButton.StartDownload(downloadAssetButton.CancellationTokenSource.Token).Forget();
					DownloadInteriorBundle(name, version, downloadAssetButton).Forget();
				}
			});
		}

		private async UniTaskVoid DownloadInteriorBundle(string name, int version, DownloadAssetButton button)
		{
			CancellationToken token = _downloadInteriorCancellationTokenSource.Token;
			AssetBundle assetBundle = await _assetBundleManager.DownloadInteriorAssetBundle(name, version, token);
			if (!token.IsCancellationRequested && assetBundle != null)
			{
				button.ContinueDownload();
			}
		}

		private void OnSelectInteriorUiItem(VisualItemElement uiElement, InteriorInfo interiorInfo)
		{
			_window.SetSelectedVisualItemElement(uiElement);
			_window.BodyKitsVisualScope.Cart.AddSmart(BodyKitType.Interior, interiorInfo.Number);
			_window.SetActiveControlButtons(isActive: true);
			_vehicleData.InteriorsComposition.TrySetInterior(interiorInfo.Number, save: false).Forget();
		}

		public void Apply()
		{
			_window.BodyKitsVisualScope.Apply();
		}

		public void Discard()
		{
			if ((bool)_vehicleData)
			{
				BodyKitsCartData cart = _window.BodyKitsVisualScope.Cart;
				int interiorId = _vehicleData.carInfo.InstalledBodyKits.InteriorId;
				if (cart.BodyKitType == BodyKitType.Interior && cart.BodyKitIndex == interiorId)
				{
					_vehicleData.carInfo.InstalledBodyKits.InteriorId = 0;
					_vehicleData.carInfo.Save();
					cart.Clear();
					_vehicleData.InteriorsComposition.ResetInterior();
					RefreshSelection();
				}
				else
				{
					cart.Clear();
					_vehicleData.InteriorsComposition.ResetInterior();
					_vehicleData.InteriorsComposition.TryRestoreSavedInterior();
					RefreshSelection();
				}
			}
		}

		private void RefreshSelection()
		{
			_window.SetSelectedVisualItemElement(null);
			_window.toggleGroup.SetAllTogglesOff(sendCallback: false);
			int interiorId = _vehicleData.carInfo.InstalledBodyKits.InteriorId;
			foreach (VisualItemElement uiElement in _uiElements)
			{
				if ((bool)uiElement && uiElement.index == interiorId)
				{
					uiElement.Button.SetIsOnWithoutNotify(value: true);
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Providers;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class DirtComposition
	{
		private readonly VehicleData _vehicle;

		private Color dirtColor = Color.white;

		public const float MaxDirtIntensity = 0.95f;

		public const float DirtIntensityUpdateValue = 0.005f;

		public const float SlippingDirtUpdateValue = 0.005f;

		private readonly int _dirtIntensityId = Shader.PropertyToID("_MaskValue");

		private readonly int _dirtColorId = Shader.PropertyToID("_MaskColor");

		private readonly int _specColor = Shader.PropertyToID("_SpecColor");

		private readonly int _decalTexPropID = Shader.PropertyToID("_DecalTex");

		private MultiPlayer.PlayerConnectedDelegate _passDirtToNewlyConnectedPlayer;

		private Color _lastDirtColor = Color.white;

		private float _lastDirtIntensity;

		private readonly float _minCarGetDirtySpeed = 10f;

		private readonly float _dirtUpdateFreq = 0.75f;

		private int _firstViewID = -1;

		private bool _isUpdatingDirt;

		private UniTask _updateDirtTask;

		private bool _isDestroyed;

		public Material SpoilerMat;

		public Material AirIntakeRoofMat;

		public Material AirIntakeHoodMat;

		private FreeDriveProvider _freeDriveProvider;

		private Surface[] _wheelSurfaces = Array.Empty<Surface>();

		public bool SkipWrite;

		private readonly MaterialAssetStateGuard _materialGuard = new MaterialAssetStateGuard();

		public float DirtIntensity { get; private set; }

		public Color DirtColor => dirtColor;

		private string PlayerPrefsDirtIntensityKey
		{
			get
			{
				if (!_vehicle)
				{
					return "CarDirtIntensity";
				}
				return "CarDirtIntensity" + _vehicle.carInfo.CarInstanceId;
			}
		}

		private string PlayerPrefsDirtColorKey
		{
			get
			{
				if (!_vehicle)
				{
					return "CarDirtColor";
				}
				return "CarDirtColor" + _vehicle.carInfo.CarInstanceId;
			}
		}

		public DirtComposition(VehicleData vehicle)
		{
			_vehicle = vehicle;
			_freeDriveProvider = _vehicle.FreeDriveProvider;
			_isDestroyed = false;
			_vehicle.OnDestroyAsObservable().Subscribe((Unit _) =>
			{
				OnDestroy();
			});
		}

		private void OnDestroy()
		{
			_isDestroyed = true;
			if (_passDirtToNewlyConnectedPlayer != null && _freeDriveProvider != null && (bool)_freeDriveProvider.MultiPlayer)
			{
				_freeDriveProvider.MultiPlayer.OnPlayerConnected -= _passDirtToNewlyConnectedPlayer;
			}
			CommonExtensions.DestroyMat(ref SpoilerMat);
			CommonExtensions.DestroyMat(ref AirIntakeRoofMat);
			CommonExtensions.DestroyMat(ref AirIntakeHoodMat);
			_materialGuard.Restore();
		}

		public async UniTaskVoid UpdateDirtMultiplayer(float dirtInt, int viewId, Color? dirtCol = null)
		{
			while (!_vehicle || _vehicle.photonView == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
				if (_isDestroyed)
				{
					return;
				}
			}
			if (_vehicle.photonView.ViewID == viewId)
			{
				DirtIntensity = dirtInt;
				dirtColor = dirtCol ?? dirtColor;
				SetDirtMatProps();
			}
			if (_firstViewID == -1)
			{
				while (!_vehicle.FreeDriveProvider.FirstCarMultiPlayer)
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
					if (_isDestroyed)
					{
						return;
					}
				}
				PhotonView component = _vehicle.FreeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>();
				while (component == null)
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
					if (_isDestroyed)
					{
						return;
					}
					component = _vehicle.FreeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>();
				}
				_firstViewID = component.ViewID;
			}
			if (_firstViewID == viewId)
			{
				SetDirtPlayerPrefs();
			}
		}

		public void SetDirtMatProps(bool updateWheelManually = false, bool updateWheelDirtAmount = true)
		{
			SetDirtProps(_vehicle.WindowComposition.WindowMat);
			SetDirtProps(_vehicle.WheelComposition.WheelMatLod);
			SetDirtProps(_vehicle.WheelComposition.WheelMatFront1);
			SetDirtProps(_vehicle.WheelComposition.WheelMatFront2);
			SetDirtProps(_vehicle.WheelComposition.WheelMatRear1);
			SetDirtProps(_vehicle.WheelComposition.WheelMatRear2);
			IReadOnlyList<Material> runtimeWheelMaterials = _vehicle.WheelComposition.RuntimeWheelMaterials;
			for (int i = 0; i < runtimeWheelMaterials.Count; i++)
			{
				SetDirtProps(runtimeWheelMaterials[i]);
			}
			if ((bool)_vehicle.Axles & updateWheelDirtAmount)
			{
				foreach (WheelProvider allWheel in _vehicle.Axles.AllWheels)
				{
					allWheel.DirtAmount = DirtIntensity;
				}
			}
			if ((bool)_vehicle.CarMat)
			{
				SetDirtProps(_vehicle.CarMat);
				SetDirtProps(SpoilerMat);
				if (_vehicle.BodyLoadingComposition.HasAirRoof)
				{
					SetDirtProps(AirIntakeRoofMat);
				}
				if (_vehicle.BodyLoadingComposition.HasAirHood)
				{
					SetDirtProps(AirIntakeHoodMat);
				}
			}
		}

		private void SetDirtProps(Material material)
		{
			if ((bool)material)
			{
				_materialGuard.Capture(material);
				material.SetColor(_dirtColorId, dirtColor);
				material.SetFloat(_dirtIntensityId, DirtIntensity);
			}
		}

		public void SaveDirtToPrefs()
		{
			SetDirtPlayerPrefs();
		}

		private void SetDirtPlayerPrefs()
		{
			if (!SkipWrite)
			{
				PlayerDatas.SetColor(PlayerPrefsDirtColorKey, dirtColor);
				PlayerPrefs.SetFloat(PlayerPrefsDirtIntensityKey, DirtIntensity);
			}
		}

		private Surface MostFrequentSurface(Surface[] array, int count)
		{
			if (array == null || count <= 0)
			{
				return Surface.None;
			}
			if (count > array.Length)
			{
				count = array.Length;
			}
			Surface result = array[0];
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				Surface surface = array[i];
				int num2 = 0;
				for (int j = 0; j < count; j++)
				{
					if (surface == array[j])
					{
						num2++;
					}
				}
				if (num2 > num)
				{
					result = surface;
					num = num2;
				}
			}
			return result;
		}

		private int CombineIntensityAndColor(float intensity, Color color)
		{
			byte num = (byte)(color.r * 100f);
			byte b = (byte)(color.g * 100f);
			byte b2 = (byte)(color.b * 100f);
			byte b3 = (byte)(intensity * 100f);
			return (num << 24) | (b << 16) | (b2 << 8) | b3;
		}

		public (float intensity, Color color) SplitIntensityAndColor(int combined)
		{
			byte b = (byte)((combined >> 24) & 0xFF);
			byte b2 = (byte)((combined >> 16) & 0xFF);
			byte b3 = (byte)((combined >> 8) & 0xFF);
			return (intensity: (float)(int)(byte)(combined & 0xFF) * 0.01f, color: new Color((float)(int)b * 0.01f, (float)(int)b2 * 0.01f, (float)(int)b3 * 0.01f));
		}

		public void GetCarDirt()
		{
			if (!_vehicle.CarMat || (!FlagsStorage.IsMainMenu && PhotonNetwork.IsConnected && (bool)_vehicle.photonView && !_vehicle.photonView.IsMine))
			{
				return;
			}
			Colors colors = _vehicle.carInfo?.Colors;
			if (colors != null && colors.DirtAmount > 0f)
			{
				DirtIntensity = colors.DirtAmount;
				dirtColor = colors.DirtColor;
				SetDirtPlayerPrefs();
			}
			else
			{
				DirtIntensity = PlayerPrefs.GetFloat(PlayerPrefsDirtIntensityKey, 0f);
				dirtColor = PlayerDatas.GetColor(PlayerPrefsDirtColorKey, Color.white);
			}
			_lastDirtIntensity = DirtIntensity;
			_lastDirtColor = dirtColor;
			SetDirtMatProps(updateWheelManually: true);
			if (PhotonNetwork.IsConnected && _freeDriveProvider.MultiPlayer != null && !_vehicle.isGarageCar)
			{
				int combined = CombineIntensityAndColor(DirtIntensity, dirtColor);
				_vehicle.photonView.InterestGroupRPC("UpdateCarDirtRPC", _vehicle.photonView.Group, RpcTarget.All, combined);
				if (_passDirtToNewlyConnectedPlayer != null)
				{
					_freeDriveProvider.MultiPlayer.OnPlayerConnected -= _passDirtToNewlyConnectedPlayer;
				}
				_passDirtToNewlyConnectedPlayer = (Player player) =>
				{
					_vehicle.photonView.RPC("UpdateCarDirtRPC", player, combined);
				};
				_freeDriveProvider.MultiPlayer.OnPlayerConnected += _passDirtToNewlyConnectedPlayer;
			}
		}

		public void TrySetDirtUpdate(bool isActive)
		{
			if (_vehicle.isTraffiCar || (PhotonNetwork.IsConnected && (!_vehicle.photonView || !_vehicle.photonView.IsMine)))
			{
				return;
			}
			if (isActive)
			{
				_isUpdatingDirt = true;
				if (_updateDirtTask.Status != UniTaskStatus.Pending)
				{
					_updateDirtTask = UpdateDirtLocal();
				}
			}
			else
			{
				_isUpdatingDirt = false;
			}
		}

		public void ForceUpdateDirt(float deltaIntensity, Color newColor)
		{
			if (!_vehicle || !_vehicle.CarMat || !_vehicle.Axles || (PhotonNetwork.IsConnected && !_vehicle.photonView.IsMine))
			{
				return;
			}
			dirtColor = Color.Lerp(dirtColor, newColor, 0.25f);
			if (DirtIntensity < 0.95f)
			{
				DirtIntensity += deltaIntensity;
			}
			else
			{
				DirtIntensity = 0.95f;
			}
			if (PhotonNetwork.IsConnected)
			{
				if (dirtColor.AlmostEqual(_lastDirtColor, 0.025f))
				{
					_vehicle.photonView.InterestGroupRPC("UpdateCarDirtIntensityRPC", _vehicle.photonView.Group, RpcTarget.All, (byte)(DirtIntensity * 100f));
				}
				else
				{
					int num = CombineIntensityAndColor(DirtIntensity, dirtColor);
					_vehicle.photonView.InterestGroupRPC("UpdateCarDirtRPC", _vehicle.photonView.Group, RpcTarget.All, num);
				}
				_lastDirtColor = dirtColor;
				_lastDirtIntensity = DirtIntensity;
			}
			else
			{
				SetDirtMatProps();
				SetDirtPlayerPrefs();
			}
		}

		private async UniTask UpdateDirtLocal()
		{
			while (_isUpdatingDirt)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(_dirtUpdateFreq));
				if (_isDestroyed)
				{
					break;
				}
				if (!_vehicle || !_vehicle.CarMat || !_vehicle.Axles || (PhotonNetwork.IsConnected && !_vehicle.photonView.IsMine))
				{
					continue;
				}
				int num = 0;
				float num2 = 0f;
				int count = _vehicle.Axles.AllWheels.Count;
				if (_wheelSurfaces.Length < count)
				{
					Array.Resize(ref _wheelSurfaces, count);
				}
				for (int i = 0; i < count; i++)
				{
					WheelProvider wheelProvider = _vehicle.Axles.AllWheels[i];
					if ((bool)wheelProvider && wheelProvider.WheelSurface != Surface.None)
					{
						_wheelSurfaces[i] = wheelProvider.WheelSurface;
						num += (wheelProvider.Wheel.GetSmoke() ? 1 : 0);
					}
				}
				if (!WheelSurfaceConfigContainer.TryGetDirtColor(MostFrequentSurface(_wheelSurfaces, count), out var colorOut))
				{
					continue;
				}
				num2 += (float)num * 0.005f;
				num2 += ((_vehicle.GetPowertrain.SpeedMps >= _minCarGetDirtySpeed) ? 0.005f : 0f);
				if (num2.AlmostEquals(0f, float.Epsilon))
				{
					continue;
				}
				dirtColor = Color.Lerp(dirtColor, colorOut, 0.25f);
				if (DirtIntensity < 0.95f)
				{
					DirtIntensity += num2;
				}
				else
				{
					DirtIntensity = 0.95f;
				}
				if (PhotonNetwork.IsConnected)
				{
					int combined = CombineIntensityAndColor(DirtIntensity, dirtColor);
					if (_passDirtToNewlyConnectedPlayer != null)
					{
						_freeDriveProvider.MultiPlayer.OnPlayerConnected -= _passDirtToNewlyConnectedPlayer;
					}
					_passDirtToNewlyConnectedPlayer = (Player player) =>
					{
						_vehicle.photonView.RPC("UpdateCarDirtRPC", player, combined);
					};
					_freeDriveProvider.MultiPlayer.OnPlayerConnected += _passDirtToNewlyConnectedPlayer;
					if (!DirtIntensity.AlmostEquals(_lastDirtIntensity, 0.01f) || !dirtColor.AlmostEqual(_lastDirtColor, 0.025f))
					{
						if (!dirtColor.AlmostEqual(_lastDirtColor, 0.025f))
						{
							_vehicle.photonView.InterestGroupRPC("UpdateCarDirtRPC", _vehicle.photonView.Group, RpcTarget.All, combined);
						}
						else
						{
							_vehicle.photonView.InterestGroupRPC("UpdateCarDirtIntensityRPC", _vehicle.photonView.Group, RpcTarget.All, (byte)(DirtIntensity * 100f));
						}
						_lastDirtColor = dirtColor;
						_lastDirtIntensity = DirtIntensity;
					}
				}
				else
				{
					SetDirtMatProps(updateWheelManually: false, updateWheelDirtAmount: false);
					SetDirtPlayerPrefs();
				}
			}
		}

		public void ReduceCarDirtIntensity(float reduceValue)
		{
			if (!_vehicle.CarMat || (PhotonNetwork.IsConnected && !_vehicle.photonView.IsMine))
			{
				return;
			}
			DirtIntensity -= reduceValue;
			if (DirtIntensity < 0f)
			{
				dirtColor = Color.white;
				DirtIntensity = 0f;
			}
			if (PhotonNetwork.IsConnected)
			{
				if (dirtColor.AlmostEqual(_lastDirtColor, 0.025f))
				{
					_vehicle.photonView.InterestGroupRPC("UpdateCarDirtIntensityRPC", _vehicle.photonView.Group, RpcTarget.All, (byte)(DirtIntensity * 100f));
				}
				else
				{
					int num = CombineIntensityAndColor(DirtIntensity, dirtColor);
					_vehicle.photonView.InterestGroupRPC("UpdateCarDirtRPC", _vehicle.photonView.Group, RpcTarget.All, num);
				}
				_lastDirtColor = dirtColor;
			}
			else
			{
				SetDirtMatProps();
				SetDirtPlayerPrefs();
			}
		}

		public void ResetCarDirt()
		{
			if ((bool)_vehicle.CarMat && (!PhotonNetwork.IsConnected || _vehicle.photonView.IsMine))
			{
				DirtIntensity = 0f;
				dirtColor = Color.white;
				if (PhotonNetwork.IsConnected)
				{
					int num = CombineIntensityAndColor(0f, Color.white);
					_vehicle.photonView.InterestGroupRPC("UpdateCarDirtRPC", _vehicle.photonView.Group, RpcTarget.All, num);
				}
				else
				{
					SetDirtMatProps(updateWheelManually: true);
					SetDirtPlayerPrefs();
				}
			}
		}
	}
}

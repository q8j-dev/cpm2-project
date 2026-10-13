using System;
using System.Collections.Generic;
using CPM.Database;
using Photon.Pun;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	public class FsoStrobeLights : MonoBehaviour
	{
		[Serializable]
		public class SpawnPoint
		{
			public Transform[] points;

			public SpawnPoint(Transform[] points)
			{
				this.points = points;
			}
		}

		public const bool FeatureEnabled = false;

		public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

		private static FsoStrobeLightsConfig _config;

		private Color _colorLeft;

		private Color _colorRight;

		private bool _colorWasSet;

		private GameObject[] _spawnedPrefabs;

		private List<SpawnPoint> _bodyKitSpawnPoints;

		private VehicleData _vehicleData;

		private ISlotService _slotService;

		private ICarInfoService _carInfoService;

		private FsoDataFileMediator _fsoDataFileMediator;

		private MenuLevelParent _menuLevelParent;

		private FreeDriveProvider _freeDriveProvider;

		private const float DisableDistance = 35f;

		public List<SpawnPoint> BodyKitSpawnPoints
		{
			get
			{
				return _bodyKitSpawnPoints;
			}
			set
			{
				_bodyKitSpawnPoints = value;
				UpdateSpawn();
			}
		}

		private List<SpawnPoint> CurrentSpawnPoints
		{
			get
			{
				if (BodyKitSpawnPoints != null)
				{
					return BodyKitSpawnPoints;
				}
				return spawnPoints;
			}
		}

		public bool HasFso => CurrentPrefab != null;

		public int CurrentSpawnPointIndex { get; set; }

		public GameObject CurrentPrefab { get; set; }

		public Color ColorLeft => _colorLeft;

		public Color ColorRight => _colorRight;

		private int CurrentToggleModeIndex { get; set; } = 1;

		public void SetColors(Color colorLeft, Color colorRight)
		{
			_colorLeft = colorLeft;
			_colorRight = colorRight;
			_colorWasSet = true;
			UpdateColor();
		}

		public void SetColorLeft(Color colorLeftRight, bool isSameColors)
		{
			if (isSameColors)
			{
				SetColors(colorLeftRight, colorLeftRight);
				return;
			}
			_colorLeft = colorLeftRight;
			UpdateColor();
		}

		public void SetColorRight(Color colorRight)
		{
			_colorRight = colorRight;
			UpdateColor();
		}

		[Inject]
		public void Construct(ISlotService slotService, ICarInfoService carInfoService, MenuLevelParent menuLevelParent, FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
			_menuLevelParent = menuLevelParent;
			_carInfoService = carInfoService;
			_slotService = slotService;
		}

		public void ToggleFromButton()
		{
			int num = ToggleFso();
			if (num != -1 && PhotonNetwork.IsConnected)
			{
				SendFsoRpc(num);
			}
		}

		public void SpawnFromPrefs(CarInfo carInfo = null)
		{
		}

		public void SendModeByRpc()
		{
			SendFsoRpc(CurrentToggleModeIndex);
		}

		public void SendModeByRpcTo(int actorNumber)
		{
			if (PhotonNetwork.IsConnected && HasFso)
			{
				int viewID = V.Car.GetComponent<PhotonView>().ViewID;
				_freeDriveProvider.MultiPlayer.SendValueStrToPlayer(53, viewID.ToString(), CurrentToggleModeIndex.ToString(), actorNumber);
			}
		}

		public void SpawnStrobeFromId(int fsoPrefabIndex, int fsoPositionIndex, Color fsoColorLeft, Color fsoColorRight)
		{
		}

		public void ChangePositionPoints(Direction direction)
		{
			switch (direction)
			{
			case Direction.Right:
				NextSpawnPoints();
				break;
			case Direction.Left:
				PrevSpawnPoints();
				break;
			}
		}

		public void UpdateSpawn()
		{
		}

		public void ReceiveRpcForFsoToggle(int modeIndex)
		{
			if (CurrentToggleModeIndex != modeIndex)
			{
				CurrentToggleModeIndex = modeIndex;
				if (_spawnedPrefabs == null)
				{
					Debug.Log("No Spawned prefabs of fso.");
				}
				else
				{
					ToggleByInt(CurrentToggleModeIndex);
				}
			}
		}

		private void Init()
		{
			LoadConfig();
			_vehicleData = GetComponentInParent<VehicleData>();
		}

		private void LoadDataFromPrefs(CarInfo carInfo)
		{
			_fsoDataFileMediator = new FsoDataFileMediator(carInfo);
			int fsoIndex = _fsoDataFileMediator.GetFsoIndex();
			CurrentSpawnPointIndex = _fsoDataFileMediator.GetFsoPosition();
			Color fsoColorLeft = _fsoDataFileMediator.GetFsoColorLeft();
			Color fsoColorRight = _fsoDataFileMediator.GetFsoColorRight();
			SetColors(fsoColorLeft, fsoColorRight);
			CurrentPrefab = FindPrefab(fsoIndex);
		}

		private void LoadConfig()
		{
			if (_config == null)
			{
				_config = FsoStrobeLightsConfig.LoadConfigFromResources();
			}
		}

		private GameObject FindPrefab(int fsoPrefabIndex)
		{
			if (fsoPrefabIndex == 0)
			{
				fsoPrefabIndex = -1;
			}
			return _config.data.Find((OneFsoItemData x) => x.index == fsoPrefabIndex).prefab;
		}

		private void TryLoadColor()
		{
			if (!_colorWasSet && _spawnedPrefabs[0].TryGetComponentInChildren<FsoLightAnimation>(out var _, includeInactive: true))
			{
				Color fsoColorLeft = _fsoDataFileMediator.GetFsoColorLeft();
				Color fsoColorRight = _fsoDataFileMediator.GetFsoColorRight();
				SetColors(fsoColorLeft, fsoColorRight);
			}
		}

		private void NextSpawnPoints()
		{
			if (CurrentSpawnPointIndex + 1 <= CurrentSpawnPoints.Count - 1)
			{
				CurrentSpawnPointIndex++;
			}
			else
			{
				CurrentSpawnPointIndex = 0;
			}
			UpdateSpawn();
		}

		private void PrevSpawnPoints()
		{
			if (CurrentSpawnPointIndex - 1 >= 0)
			{
				CurrentSpawnPointIndex--;
			}
			else
			{
				CurrentSpawnPointIndex = CurrentSpawnPoints.Count - 1;
			}
			UpdateSpawn();
		}

		private void UpdateColor()
		{
			if (_spawnedPrefabs == null || _spawnedPrefabs.Length == 0)
			{
				return;
			}
			for (int i = 0; i < _spawnedPrefabs.Length; i++)
			{
				if (_spawnedPrefabs[i].TryGetComponentInChildren<FsoLightAnimation>(out var component, includeInactive: true))
				{
					if (i % 2 == 0)
					{
						component.FsoColor = _colorLeft;
					}
					else
					{
						component.FsoColor = _colorRight;
					}
				}
				else
				{
					Debug.LogWarning("No fso light animation component in children on prefab. Check fso prefab.");
				}
			}
		}

		private void Spawn(SpawnPoint points, GameObject prefab)
		{
			RemoveObjects();
			if (prefab == null)
			{
				_spawnedPrefabs = null;
				return;
			}
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < points.points.Length; i++)
			{
				Transform parent = points.points[i];
				GameObject gameObject = UnityEngine.Object.Instantiate(prefab, parent);
				gameObject.transform.parent = base.transform;
				if (gameObject.TryGetComponent<FsoLightAnimation>(out var component))
				{
					component.Init(i, points.points.Length, _config, this);
				}
				list.Add(gameObject);
			}
			_spawnedPrefabs = list.ToArray();
			TryLoadColor();
			UpdateColor();
		}

		private void RemoveObjects()
		{
			if (_spawnedPrefabs != null)
			{
				Array.ForEach(_spawnedPrefabs, (GameObject x) =>
				{
					UnityEngine.Object.Destroy(x);
				});
			}
		}

		private int ToggleFso()
		{
			if (_spawnedPrefabs == null)
			{
				return -1;
			}
			if (CurrentToggleModeIndex < 9)
			{
				CurrentToggleModeIndex++;
			}
			else
			{
				CurrentToggleModeIndex = 1;
			}
			ToggleByInt(CurrentToggleModeIndex);
			return CurrentToggleModeIndex;
		}

		private void ToggleByInt(int toggleModeIndex)
		{
			if (_spawnedPrefabs == null)
			{
				return;
			}
			GameObject[] spawnedPrefabs = _spawnedPrefabs;
			for (int i = 0; i < spawnedPrefabs.Length; i++)
			{
				if (spawnedPrefabs[i].TryGetComponentInChildren<FsoLightAnimation>(out var component, includeInactive: true))
				{
					component.Toggle(toggleModeIndex);
				}
			}
		}

		private void SendFsoRpc(int toggleFso)
		{
			if (PhotonNetwork.IsConnected && HasFso)
			{
				int viewID = V.Car.GetComponent<PhotonView>().ViewID;
				_freeDriveProvider.MultiPlayer.SendValueStr(53, viewID.ToString(), toggleFso.ToString());
			}
		}

		public bool EnabledCheck()
		{
			if (_vehicleData == null)
			{
				return false;
			}
			if (_vehicleData.DistanceFromCamera < 35f)
			{
				if (!_menuLevelParent.isDroneEnabled)
				{
					if (_vehicleData.IsMeInCar)
					{
						return V.CurrentCamIndex != 0;
					}
					return true;
				}
				return true;
			}
			return false;
		}

		public void SetFsoData(int lastSelectedIndex)
		{
		}
	}
}

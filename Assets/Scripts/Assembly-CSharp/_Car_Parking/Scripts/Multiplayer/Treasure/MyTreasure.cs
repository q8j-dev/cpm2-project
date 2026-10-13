using System;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Treasure
{
	public class MyTreasure : MonoBehaviour
	{
		private const float TreasureIconScale = 40f;

		private const double TreasureIconBlinkInterval = 0.5;

		[SerializeField]
		private SpawnPointsScriptableObject TreasurePointsSO = default;

		[SerializeField]
		private IconMapView _treasureIcon = default;

		private MyTreasureManager _treasureManager;

		private MapControl _mapControl;

		private int _index = -1;

		private int _showOnMapTimeSec = 10;

		private IDisposable _showOnMapDispose;

		private GlobalCollectionsProvider _globalCollectionsProvider;

		private MapIconsModel _mapIconsModel;

		[Inject]
		public void Construct(MapControl mapControl, GlobalCollectionsProvider globalCollectionsProvider, MapIconsModel mapIconsModel)
		{
			_mapControl = mapControl;
			_globalCollectionsProvider = globalCollectionsProvider;
			_mapIconsModel = mapIconsModel;
		}

		public void Init(MyTreasureManager treasureManager)
		{
			_treasureManager = treasureManager;
			try
			{
				if ((bool)_treasureIcon)
				{
					_treasureIcon.gameObject.layer = LayerViaName.Map;
					_treasureIcon.transform.localScale = new Vector3(40f, 40f, 1f);
					_mapControl.treasure = _treasureIcon.transform;
					_mapControl.showTreasure = false;
					Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
					{
						Blink();
					}).AddTo(this);
					_mapIconsModel?.Icons.Add(_treasureIcon);
					_mapIconsModel?.AddMapIconModel(_treasureIcon, IconType.Treasure);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("MyTreasure Init: " + ex.Message);
			}
			SetToNewPointOrDisable(-1);
		}

		private void Blink()
		{
			if ((bool)_treasureIcon)
			{
				_treasureIcon.SetActive(!_treasureIcon.gameObject.activeSelf);
			}
		}

		public int GetRandomPositionIndex()
		{
			int count = TreasurePointsSO.treasurePoints.pointPositions.Count;
			int num;
			do
			{
				num = UnityEngine.Random.Range(0, count);
			}
			while (num == _index);
			return num;
		}

		public void SetToNewPointOrDisable(int pointIndex)
		{
			_showOnMapDispose?.Dispose();
			if (pointIndex < 0)
			{
				base.gameObject.SetActive(value: false);
				_mapControl.showTreasure = false;
				_index = -1;
				return;
			}
			if (pointIndex >= TreasurePointsSO.treasurePoints.pointPositions.Count || pointIndex == _index)
			{
				pointIndex = GetRandomPositionIndex();
			}
			_index = pointIndex;
			base.transform.position = TreasurePointsSO.treasurePoints.pointPositions[_index] + V.GlobalOffset;
			base.gameObject.SetActive(value: true);
			_mapControl.showTreasure = true;
			_showOnMapDispose = Observable.Timer(TimeSpan.FromSeconds(_showOnMapTimeSec)).Subscribe((long _) =>
			{
				_mapControl.showTreasure = false;
			}).AddTo(this);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Car") || other.CompareTag("OnlineCar"))
			{
				VehicleData componentInParent = other.GetComponentInParent<VehicleData>();
				if (!componentInParent || componentInParent.gameObject != V.Car)
				{
					return;
				}
				_treasureManager.Collided();
			}
			if (other.CompareTag("DriverPerson") && V.DriverPerson != null && V.DriverPerson == other.gameObject)
			{
				_treasureManager.Collided();
			}
		}

		private void OnDestroy()
		{
			if (_mapIconsModel != null && _mapIconsModel.Icons.Contains(_treasureIcon))
			{
				_mapIconsModel.Icons.Remove(_treasureIcon);
				_mapIconsModel.RemoveMapIconModel(_treasureIcon);
			}
		}
	}
}

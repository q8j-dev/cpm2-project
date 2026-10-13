using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.ArtistTools;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.EventSystem
{
	public class EventZoneCollectible : MonoBehaviour, IMovableCollectible
	{
		public int Index;

		[SerializeField]
		private VfxSpawner VfxSpawner = default;

		[SerializeField]
		private GameObject MeshObject = default;

		private MapCamera _mapCamera;

		private MapIconsModel _mapIconsModel;

		private IconMapView _iconMapView;

		private bool _isDestroyed;

		private FreeDriveProvider _freeDriveProvider;

		private bool _cantCollide;

		private float _snowDirtDelta = 0.1f;

		private Color _snowColor = Color.white;

		private IEventZone _activityZoneTrigger;

		[Inject]
		public void Construct(MapCamera mapCamera, MapIconsModel mapIconsModel, FreeDriveProvider freeDriveProvider)
		{
			_mapCamera = mapCamera;
			_mapIconsModel = mapIconsModel;
			_freeDriveProvider = freeDriveProvider;
		}

		private void Start()
		{
			CreateMapIcon();
			SetIconState(state: false);
		}

		public void Init(IEventZone activityZoneTrigger, int index)
		{
			_activityZoneTrigger = activityZoneTrigger;
			Index = index;
		}

		private void CreateMapIcon()
		{
			_iconMapView = _mapIconsModel.CreateMapIcon(base.gameObject, IconType.EventCollectible, Index);
			V.CoreGameManager.MapIconsModel.AddMapIconModel(_iconMapView, IconType.EventCollectible);
			_iconMapView.transform.SetParent(base.transform);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.attachedRigidbody || _cantCollide || (!other.CompareTag("Car") && !other.CompareTag("OnlineCar")))
			{
				return;
			}
			GameObject gameObject = other.attachedRigidbody.gameObject;
			if (!(V.CoreGameManager.FreeDriveProvider.FirstCarMultiPlayer != gameObject) && gameObject.TryGetComponent<VehicleData>(out var component) && (!component.Driver || !(component.Driver != V.DriverPerson)))
			{
				_cantCollide = true;
				AudioWrapper.PostEvent("Play_Snowman_Hit", base.gameObject);
				_activityZoneTrigger.OnCollect();
				if ((bool)VfxSpawner)
				{
					VfxSpawner.TryActivateVfxEffect();
				}
				if ((bool)MeshObject)
				{
					MeshObject.SetActive(value: false);
				}
				SetIconState(state: false);
			}
		}

		public void SetIconState(bool state)
		{
			if ((bool)_iconMapView)
			{
				if (state && (bool)MeshObject && !MeshObject.activeSelf)
				{
					state = false;
				}
				_iconMapView.SetActive(state);
			}
		}

		private void OnDestroy()
		{
			_isDestroyed = true;
		}

		public void MoveCollectible(Vector3 position, Quaternion rotation)
		{
			base.transform.localPosition = position;
			base.transform.localRotation = rotation;
			_cantCollide = false;
		}

		public void ChangeState(bool state)
		{
			base.gameObject.SetActive(state);
			if (!state && (bool)MeshObject)
			{
				MeshObject.SetActive(value: true);
			}
		}
	}
}

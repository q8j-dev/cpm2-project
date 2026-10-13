using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace _Car_Parking.Scripts.Car
{
	public class VehicleMembersController : IDisposable
	{
		public enum TrySitPlaceResult
		{
			Success = 0,
			CarIsBusy = 1,
			CarIsLocked = 2
		}

		private const int DriverPlaceIndex = 0;

		private const int FirstPassengerIndex = 1;

		private GameObject[] _members = Array.Empty<GameObject>();

		private readonly VehicleData _vehicleData;

		private readonly CustomPropertiesCarSits _customPropertiesCarSits;

		private CancellationTokenSource _carLockedCTS;

		public IReadOnlyList<GameObject> Members => _members;

		public int PlacesCount => _members.Length;

		public bool HasPlaceForPassenger => FirstFreePlaceForPassenger >= 0;

		public int FirstFreePlaceForPassenger
		{
			get
			{
				for (int i = 1; i < _members.Length; i++)
				{
					if (!_members[i])
					{
						return i;
					}
				}
				return -1;
			}
		}

		public bool HasPassenger
		{
			get
			{
				GameObject passenger;
				return TryGetFirstPassenger(out passenger);
			}
		}

		public GameObject Driver
		{
			get
			{
				if (_members.Length == 0)
				{
					return null;
				}
				return _members[0];
			}
		}

		public bool IsFirstCarMultiplayer
		{
			get
			{
				if ((bool)_vehicleData && (bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null)
				{
					return _vehicleData.gameObject == V.CoreGameManager.FreeDriveProvider.FirstCarMultiPlayer;
				}
				return false;
			}
		}

		public bool IsLocked { get; private set; }

		public void SetLocked(bool value)
		{
			IsLocked = value;
			if (IsLocked)
			{
				OnCarLocked();
			}
		}

		public VehicleMembersController(VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
			_carLockedCTS = new CancellationTokenSource();
			_customPropertiesCarSits = new CustomPropertiesCarSits(this);
			V.OnPlayerDisconnected += PlayerDisconnectedCallback;
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _carLockedCTS);
			_customPropertiesCarSits.Dispose();
			V.OnPlayerDisconnected -= PlayerDisconnectedCallback;
		}

		public void InitIfNeeds(int placeCount = 4)
		{
			if (PlacesCount == 0)
			{
				Init(placeCount);
			}
		}

		public void Init(int placeCount)
		{
			_members = new GameObject[placeCount];
		}

		public GameObject GetMember(int placeIndex)
		{
			if (placeIndex < 0 || placeIndex >= _members.Length)
			{
				return null;
			}
			return _members[placeIndex];
		}

		public bool HaveMember(GameObject person)
		{
			return GetPlaceOnCar(person) >= 0;
		}

		public int GetPlaceOnCar(GameObject person)
		{
			for (int i = 0; i < _members.Length; i++)
			{
				if (_members[i] == person)
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsPassenger(GameObject person)
		{
			for (int i = 1; i < _members.Length; i++)
			{
				if (_members[i] == person)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsPlaceFree(int place)
		{
			if (place >= 0 && place < _members.Length)
			{
				return !_members[place];
			}
			return false;
		}

		public bool SetDriver(GameObject person)
		{
			return SetMember(0, person);
		}

		public int GetLocalProperty(int place)
		{
			return _customPropertiesCarSits.GetPlace(place);
		}

		public bool SetMember(int place, GameObject person)
		{
			if (place >= 0 && place < _members.Length)
			{
				GameObject gameObject = _members[place];
				_members[place] = person;
				PhotonView photonView2;
				if ((bool)gameObject && !person && gameObject.TryGetPhotonView(out var photonView))
				{
					_customPropertiesCarSits.RemoveCustomPropertiesCarSits(photonView.CreatorActorNr);
				}
				else if ((bool)person && person.TryGetPhotonView(out photonView2))
				{
					_customPropertiesCarSits.SetPlace(place, photonView2.CreatorActorNr);
				}
				if ((bool)person && person == V.DriverPerson && IsLocked)
				{
					OnCarLockedAsync(_carLockedCTS.Token).Forget();
				}
				return true;
			}
			return false;
		}

		public bool TryGetFirstMember(out GameObject passenger)
		{
			return TryGetFirstMember(0, out passenger);
		}

		public bool TryGetFirstPassenger(out GameObject passenger)
		{
			return TryGetFirstMember(1, out passenger);
		}

		public void SetActiveMember(bool isActive)
		{
			for (int i = 0; i < _members.Length; i++)
			{
				if ((bool)_members[i])
				{
					_members[i].SetActive(isActive);
				}
			}
		}

		public TrySitPlaceResult TrySitPlace(int place, int actorNumber)
		{
			if (place == 0 && ((bool)Driver || GetLocalProperty(0) != -1))
			{
				return TrySitPlaceResult.CarIsBusy;
			}
			if ((bool)GetMember(place))
			{
				return TrySitPlaceResult.CarIsBusy;
			}
			if (IsLocked && actorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
			{
				return TrySitPlaceResult.CarIsLocked;
			}
			if (_customPropertiesCarSits.GetPlace(place) == -1)
			{
				_customPropertiesCarSits.SetPlace(place, actorNumber);
				return TrySitPlaceResult.Success;
			}
			return TrySitPlaceResult.CarIsBusy;
		}

		public void OnCarLocked()
		{
			if (!V.CoreGameManager || V.CoreGameManager.FreeDriveProvider == null || _vehicleData.gameObject == V.CoreGameManager.FreeDriveProvider.FirstCarMultiPlayer)
			{
				return;
			}
			for (int i = 0; i < _members.Length; i++)
			{
				if ((bool)_members[i] && _members[i].gameObject == V.DriverPerson)
				{
					V.CoreGameManager.FreeDriveProvider.MultiPlayer.KickedFromCar();
				}
			}
		}

		private bool TryGetFirstMember(int firstIndex, out GameObject passenger)
		{
			passenger = null;
			for (int i = firstIndex; i < _members.Length; i++)
			{
				passenger = _members[i];
				if ((bool)passenger)
				{
					return true;
				}
			}
			return false;
		}

		private void PlayerDisconnectedCallback(Player player)
		{
			_customPropertiesCarSits.RemoveCustomPropertiesCarSits(player.ActorNumber);
		}

		private async UniTask OnCarLockedAsync(CancellationToken cancellationToken)
		{
			await UniTask.Yield();
			if (!cancellationToken.IsCancellationRequested)
			{
				OnCarLocked();
			}
		}
	}
}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;

namespace _Car_Parking.Scripts.Car
{
	public class CustomPropertiesCarSits : IDisposable
	{
		public const string PhotonCustomPropertiesCarSeatsKey = "CarSeats";

		public const int SitIsEmptyKey = -1;

		private readonly VehicleMembersController _vehicleMembersController;

		private int[] _localCarSeats;

		private int[] _localCarSeatsCurrent;

		private CancellationTokenSource _updateCarSeatsCTS;

		public CustomPropertiesCarSits(VehicleMembersController vehicleMembersController)
		{
			_vehicleMembersController = vehicleMembersController;
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _updateCarSeatsCTS);
		}

		public int GetPlace(int place)
		{
			InitCarSeatsIfNeeds();
			if (place >= _localCarSeats.Length)
			{
				return -1;
			}
			return _localCarSeats[place];
		}

		public void RemoveCustomPropertiesCarSits(int actorNumber)
		{
			InitCarSeatsIfNeeds();
			for (int i = 0; i < _localCarSeats.Length; i++)
			{
				if (_localCarSeats[i] == actorNumber)
				{
					_localCarSeats[i] = -1;
				}
			}
			UpdateCarSeats();
		}

		public void SetPlace(int place, int actorNumber)
		{
			InitCarSeatsIfNeeds();
			if (place < _localCarSeats.Length)
			{
				_localCarSeats[place] = actorNumber;
			}
			UpdateCarSeats();
		}

		private void InitCarSeatsIfNeeds()
		{
			if (_localCarSeats != null && _localCarSeats.Length >= _vehicleMembersController.Members.Count)
			{
				return;
			}
			if (!_vehicleMembersController.IsFirstCarMultiplayer)
			{
				if (_localCarSeats == null)
				{
					_localCarSeats = Array.Empty<int>();
				}
				if (_localCarSeatsCurrent == null)
				{
					_localCarSeatsCurrent = Array.Empty<int>();
				}
				return;
			}
			_localCarSeats = new int[_vehicleMembersController.Members.Count];
			_localCarSeatsCurrent = new int[_vehicleMembersController.Members.Count];
			for (int i = 0; i < _vehicleMembersController.Members.Count; i++)
			{
				_localCarSeatsCurrent[i] = -1;
				if ((bool)_vehicleMembersController.Members[i] && _vehicleMembersController.Members[i].TryGetPhotonView(out var photonView))
				{
					_localCarSeats[i] = photonView.CreatorActorNr;
				}
				else
				{
					_localCarSeats[i] = -1;
				}
			}
		}

		private void UpdateCarSeats()
		{
			if (_vehicleMembersController.IsFirstCarMultiplayer && !FlagsStorage.InGarageFree)
			{
				CommonExtensions.CancelAndDisposeToken(ref _updateCarSeatsCTS);
				_updateCarSeatsCTS = new CancellationTokenSource();
				UpdateCarSeatsAsync(_updateCarSeatsCTS.Token).Forget();
			}
		}

		private async UniTask UpdateCarSeatsAsync(CancellationToken cancellationToken)
		{
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			InitCarSeatsIfNeeds();
			bool flag = false;
			for (int i = 0; i < _localCarSeats.Length; i++)
			{
				if (_localCarSeats[i] != _localCarSeatsCurrent[i])
				{
					_localCarSeatsCurrent[i] = _localCarSeats[i];
					flag = true;
				}
			}
			if (flag)
			{
				PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { ["CarSeats"] = (int[])_localCarSeats.Clone() });
			}
		}
	}
}

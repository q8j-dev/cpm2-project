using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl
{
	public class InterestGroupManager : IDisposable
	{
		public const string InterestGroupKey = "IG";

		private const float DistanceToSubscribe = 200f;

		private const float DistanceToUnsubscribe = 250f;

		private const float UpdateInterval = 0.5f;

		private static ReactiveProperty<byte> MyInterestGroup = new ReactiveProperty<byte>();

		private readonly float[] _onlineCarsDistances = new float[32];

		private readonly HashSet<byte> _subscribedGroups = new HashSet<byte>();

		private Transform _cameraTransform;

		private IDisposable _checkDistanceOperation;

		private PhotonView _myPhotonView;

		[Inject]
		private OnlineCollectionsProvider _onlineCollections;

		[Inject]
		private FreeDriveProvider _freeDriveProvider;

		public static byte CarInterestGroup => (byte)Mathf.Clamp(MyInterestGroup.Value * 2 - 1, 0, 255);

		public static byte PersonInterestGroup => (byte)Mathf.Clamp(MyInterestGroup.Value * 2, 0, 255);

		public HashSet<byte> InterestGroups => _subscribedGroups;

		public void Dispose()
		{
			_checkDistanceOperation?.Dispose();
			MyInterestGroup?.Dispose();
			_subscribedGroups.Clear();
			_cameraTransform = null;
			_myPhotonView = null;
		}

		public void Initialize()
		{
			Dispose();
			MyInterestGroup = new ReactiveProperty<byte>();
			_cameraTransform = V.CoreGameManager.transform;
			if (FlagsStorage.IsMultiFree)
			{
				_checkDistanceOperation = Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe(FillDistances).AddTo(V.CoreGameManager);
			}
		}

		private void SubscribeToInterestGroups(byte groupId)
		{
			if (groupId != 0 && _subscribedGroups.Add(groupId))
			{
				PhotonNetwork.SetInterestGroups(groupId, enabled: true);
			}
		}

		private void UpdateIgnoreCarsColliders(MultiPlayerPosControl otherCar)
		{
			bool checkAgain;
			if ((bool)V.Car && V.Car.TryGetComponent<MultiPlayerPosControl>(out var component))
			{
				PlayerDatas.IgnoreCollisionWithOtherCars(V.Car, ignore: true, _onlineCollections, out checkAgain);
				component.ignoreCollision = true;
			}
			if ((bool)otherCar)
			{
				PlayerDatas.IgnoreCollisionWithOtherCars(otherCar.gameObject, ignore: true, _onlineCollections, out checkAgain);
				otherCar.ignoreCollision = true;
			}
		}

		private void UnsubscribeFromInterestGroups(byte groupId)
		{
			if (_subscribedGroups.Remove(groupId) && groupId != 0)
			{
				PhotonNetwork.SetInterestGroups(groupId, enabled: false);
			}
		}

		public bool IsSubscribedToInterestGroup(byte groupId)
		{
			return _subscribedGroups.Contains(groupId);
		}

		public bool IsOtherInInterestGroups()
		{
			if (_subscribedGroups.Count == 0)
			{
				return false;
			}
			if (_subscribedGroups.Count > 2)
			{
				return true;
			}
			if (!_myPhotonView)
			{
				GameObject gameObject = (_freeDriveProvider.FirstCarMultiPlayer ? _freeDriveProvider.FirstCarMultiPlayer : V.Car);
				if (!gameObject)
				{
					return false;
				}
				_myPhotonView = gameObject.GetComponent<PhotonView>();
			}
			return _subscribedGroups.Any(CheckGroup);
		}

		private bool CheckGroup(byte g)
		{
			if (g != PersonInterestGroup)
			{
				if (g == CarInterestGroup)
				{
					return !_myPhotonView.IsMine;
				}
				return true;
			}
			return false;
		}

		public float GetDistanceToCar(MultiPlayerPosControl car)
		{
			int num = Array.IndexOf(_onlineCollections.CarsOnlineNow, car);
			if (num < 0)
			{
				return float.MaxValue;
			}
			return _onlineCarsDistances[num];
		}

		public bool TryReserveInterestGroupID(out byte groupId)
		{
			groupId = 0;
			if (!PhotonNetwork.IsConnected)
			{
				return false;
			}
			List<byte> list = new List<byte>();
			foreach (Player value2 in PhotonNetwork.CurrentRoom.Players.Values)
			{
				if (!PhotonNetwork.LocalPlayer.Equals(value2) && value2.CustomProperties.TryGetValue("IG", out var value))
				{
					list.Add((byte)value);
				}
			}
			int num = 0;
			do
			{
				groupId = (byte)UnityEngine.Random.Range(1, 126);
				num++;
			}
			while (list.Contains(groupId) && num <= 10);
			MyInterestGroup.Value = groupId;
			return PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "IG", groupId } });
		}

		private void FillDistances(long _ = 0L)
		{
			Vector3 localPosition = _cameraTransform.localPosition;
			for (int i = 0; i < 32; i++)
			{
				MultiPlayerPosControl multiPlayerPosControl = _onlineCollections.CarsOnlineNow[i];
				if ((bool)multiPlayerPosControl)
				{
					Vector3 localPosition2 = multiPlayerPosControl.transform.localPosition;
					_onlineCarsDistances[i] = Vector3.Distance(localPosition, localPosition2);
					localPosition2 = multiPlayerPosControl.LastReceivedRawPosition;
					Vector3 a = localPosition;
					a.y = localPosition2.y;
					float num = Vector3.Distance(a, localPosition2);
					if (num < 200f)
					{
						if (!multiPlayerPosControl.photonView.IsMine && !_subscribedGroups.Contains(multiPlayerPosControl.InterestGroup))
						{
							multiPlayerPosControl.RequestDataFromInterestGroup();
							multiPlayerPosControl.SkipPosLerp();
							UpdateIgnoreCarsColliders(multiPlayerPosControl);
						}
						SubscribeToInterestGroups(multiPlayerPosControl.InterestGroup);
					}
					else if (num > 250f)
					{
						UnsubscribeFromInterestGroups(multiPlayerPosControl.InterestGroup);
					}
				}
				else
				{
					_onlineCarsDistances[i] = float.MaxValue;
				}
				MultiPersonControl multiPersonControl = _onlineCollections.PlayersOnlineNow[i];
				if (!multiPersonControl)
				{
					continue;
				}
				Vector3 a2 = localPosition;
				a2.y = multiPersonControl.LastReceivedRawPosition.y;
				float num2 = Vector3.Distance(a2, multiPersonControl.LastReceivedRawPosition);
				if (num2 < 200f)
				{
					if (!multiPersonControl.photonView.IsMine && !_subscribedGroups.Contains(multiPersonControl.InterestGroup))
					{
						multiPersonControl.RequestPersonData();
					}
					SubscribeToInterestGroups(multiPersonControl.InterestGroup);
				}
				else if (num2 > 250f)
				{
					if (_subscribedGroups.Contains(multiPersonControl.InterestGroup))
					{
						multiPersonControl.WorldInteraction?.ClearRemotePoseLocal();
					}
					UnsubscribeFromInterestGroups(multiPersonControl.InterestGroup);
				}
			}
		}
	}
}

using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Providers;

public class Emoji : MonoBehaviourPunCallbacks
{
	public GameObject _emojiPoint;

	public List<GameObject> emojies;

	private GameObject passenger;

	private int _zoneNumber;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	[Inject]
	public void Construct(OnlineCollectionsProvider onlineCollectionsProvider)
	{
		_onlineCollectionsProvider = onlineCollectionsProvider;
	}

	private void Start()
	{
		GetIk();
	}

	public void GetIk()
	{
		if ((bool)GetComponent<MainPerson>() && GetComponent<MainPerson>().ik != null)
		{
			_emojiPoint = GetComponent<MainPerson>().ik.references.head.gameObject;
		}
	}

	public void EmojiController(int _emojiNumber)
	{
		if (PhotonNetwork.IsConnected)
		{
			if (base.photonView.IsMine)
			{
				int num;
				if (V.Car != null)
				{
					ShowEmoji(_emojiNumber, 1);
					num = 1;
				}
				else if (FlagsStorage.IsPassenger)
				{
					ShowEmoji(_emojiNumber, 2);
					num = 2;
				}
				else
				{
					ShowEmoji(_emojiNumber, 3);
					num = 3;
				}
				byte b = (byte)(num | (_emojiNumber << 2));
				base.photonView.InterestGroupRPC("EmojiRPC", base.photonView.Group, RpcTarget.Others, b);
			}
		}
		else if (V.Car != null)
		{
			ShowEmoji(_emojiNumber, 1);
		}
		else if (FlagsStorage.IsPassenger)
		{
			ShowEmoji(_emojiNumber, 2);
		}
		else
		{
			ShowEmoji(_emojiNumber, 3);
		}
	}

	private void ShowEmoji(int _emojiNumber, int personStatusCondition)
	{
		if (_emojiPoint == null)
		{
			GetIk();
		}
		GameObject gameObject = Object.Instantiate(emojies[_emojiNumber++]);
		gameObject.GetComponent<Renderer>().material.renderQueue = 3001;
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
		switch (personStatusCondition)
		{
		case 3:
			gameObject.transform.position = base.transform.position;
			gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2.3f, gameObject.transform.position.z);
			break;
		case 1:
			if ((bool)_emojiPoint)
			{
				gameObject.transform.position = _emojiPoint.transform.position;
				float num = 0.75f;
				int carId = GetComponent<DriverActionController>().Car.GetComponent<VehicleData>().CarId;
				if (carId == 70 || carId == 170 || carId == 171)
				{
					num = 1.75f;
				}
				gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + num, gameObject.transform.position.z);
			}
			break;
		default:
			if ((bool)_emojiPoint)
			{
				gameObject.transform.position = _emojiPoint.transform.position;
				gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.75f, gameObject.transform.position.z);
			}
			break;
		}
		Object.Destroy(gameObject, 2f);
	}

	[PunRPC]
	private void EmojiRPC(byte data, PhotonMessageInfo info)
	{
		if (PhotonNetwork.IsConnected && !FlagsStorage.IsPhotoMode)
		{
			int personStatusCondition = data & 3;
			int emojiNumber = data >> 2;
			if (info.photonView.ViewID == base.photonView.ViewID)
			{
				ShowEmoji(emojiNumber, personStatusCondition);
			}
		}
	}

	public void CallRPC_NewClient(int zoneNumber)
	{
		if (PhotonNetwork.IsConnected && (bool)base.photonView)
		{
			base.photonView.RpcToDefaultGroup("CreateNewClient", RpcTarget.All, zoneNumber);
		}
	}

	[PunRPC]
	private void CreateNewClient(int zoneNumber)
	{
		if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient && base.photonView.IsMine)
		{
			TaxiController._taxiController.ReCreateByRPC(zoneNumber);
		}
	}

	private void RemovePassenger()
	{
		PhotonNetwork.Destroy(passenger);
		TaxiController._taxiController.ReCreateByRPC(_zoneNumber);
	}

	private GameObject GetCarByOwnerID(int ownerId)
	{
		return _onlineCollectionsProvider.GetCarByOwnerID(ownerId);
	}

	[PunRPC]
	public void TrafficLightRPC()
	{
		if ((bool)TrafficLightController2._object)
		{
			TrafficLightController._object._greenCounter = 0;
			TrafficLightController._object.timer = 0f;
		}
	}
}

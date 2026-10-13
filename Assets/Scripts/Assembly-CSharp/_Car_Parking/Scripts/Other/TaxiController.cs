using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class TaxiController : MonoBehaviourPunCallbacks
	{
		public static TaxiController _taxiController;

		public GameObject taxiZone;

		public GameObject finishZone;

		public GameObject createdFinishZone;

		public List<GameObject> pointsList;

		private List<Transform> startPointList;

		[HideInInspector]
		public GameObject createdTaxiZone;

		public GameObject createTaxiZone_City;

		public GameObject createTaxiZone_Mountain;

		public GameObject createTaxiZone_Island;

		public GameObject createTaxiZone_Desert;

		public GameObject createTaxiZone_RaceTrack;

		public GameObject createTaxiZone_NewCity;

		private GameObject taxiPrefab;

		private IDisposable _createDisposable;

		private void Start()
		{
			_taxiController = this;
			if (!PhotonNetwork.IsConnected)
			{
				Create();
			}
		}

		public void ReCreateByRPC(int _zoneNumber)
		{
			instantiate(pointsList[_zoneNumber], _zoneNumber);
		}

		public void Create()
		{
			if (!(createdTaxiZone == null))
			{
				return;
			}
			int num = 0;
			foreach (GameObject points in pointsList)
			{
				if (num == 0 && createTaxiZone_City == null)
				{
					createTaxiZone_City = instantiate(points, num);
				}
				if (num == 1 && createTaxiZone_Mountain == null)
				{
					createTaxiZone_Mountain = instantiate(points, num);
				}
				if (num == 2 && createTaxiZone_Island == null)
				{
					createTaxiZone_Island = instantiate(points, num);
				}
				if (num == 3 && createTaxiZone_Desert == null)
				{
					createTaxiZone_Desert = instantiate(points, num);
				}
				if (num == 4 && createTaxiZone_RaceTrack == null)
				{
					createTaxiZone_RaceTrack = instantiate(points, num);
				}
				if (num == 5 && createTaxiZone_NewCity == null)
				{
					createTaxiZone_NewCity = instantiate(points, num);
				}
				num++;
			}
		}

		private GameObject instantiate(GameObject item, int counter)
		{
			if (FlagsStorage.IsMultiFree)
			{
				return null;
			}
			GetChilds(item.transform);
			Transform transform = startPointList[GetRandomPosition(startPointList)];
			taxiPrefab = UnityEngine.Object.Instantiate(Resources.Load("taxiperson" + 1, typeof(GameObject)), new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), Quaternion.identity) as GameObject;
			taxiPrefab.transform.parent = V.GlobalPoint;
			return taxiPrefab;
		}

		private void GetChilds(Transform parent)
		{
			startPointList = new List<Transform>();
			int childCount = parent.childCount;
			for (int i = 0; i < childCount; i++)
			{
				startPointList.Add(parent.GetChild(i));
			}
		}

		public override void OnJoinedRoom()
		{
			if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
			{
				_createDisposable = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
				{
					Create();
				}).AddTo(this);
			}
		}

		public int GetRewardByDistance(Vector3 _startPoint, Vector3 _endPoint)
		{
			int value = ObscuredPrefs.GetInt("ordercount") + 1;
			ObscuredPrefs.SetInt("ordercount", value);
			float num = Vector3.Distance(_startPoint, _endPoint);
			int value2 = ObscuredPrefs.GetInt("taxicashcount") + (int)num * 2;
			ObscuredPrefs.SetInt("taxicashcount", value2);
			ObscuredPrefs.Save();
			return (int)num * 2;
		}

		private int GetRandomPosition(List<Transform> list)
		{
			if (list.Count > 0)
			{
				return UnityEngine.Random.Range(0, list.Count);
			}
			return -1;
		}

		private void OnDestroy()
		{
			_taxiController = null;
		}

		public void CreateFinishZone(Transform _startPoint, int zoneNumber)
		{
			GetChilds(pointsList[zoneNumber].transform);
			Transform transform = startPointList[GetRandomPosition(startPointList)];
			while (Vector3.Distance(_startPoint.position, transform.position) < 100f)
			{
				transform = startPointList[GetRandomPosition(startPointList)];
			}
			if (createdFinishZone != null)
			{
				UnityEngine.Object.Destroy(createdFinishZone);
			}
			Vector3 position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
			GameObject gameObject = UnityEngine.Object.Instantiate(finishZone, position, Quaternion.identity);
			gameObject.transform.parent = V.GlobalPoint.transform;
			gameObject.transform.position = position;
			createdFinishZone = gameObject;
			if ((bool)gameObject.GetComponent<TaxiGetReward>())
			{
				gameObject.GetComponent<TaxiGetReward>().reward = GetRewardByDistance(_startPoint.position, gameObject.transform.position);
			}
		}
	}
}

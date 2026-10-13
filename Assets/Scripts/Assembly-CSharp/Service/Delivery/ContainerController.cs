using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Service.Manager;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts;

namespace Service.Delivery
{
	public class ContainerController : MonoBehaviour
	{
		public List<ItemController> items = new List<ItemController>();

		public List<int> onBoard = new List<int>();

		[HideInInspector]
		public TrailerController trailer;

		public Collider palet;

		private ExitGames.Client.Photon.Hashtable _hash = new ExitGames.Client.Photon.Hashtable();

		private Player _owner;

		private LayerMask _loadLayer;

		private LayerMask _onlineLoadLayer;

		private readonly ObscuredFloat _maxValueToCheck = 100000f;

		private readonly TaskManager _taskManager = Global.TaskManager;

		private void Start()
		{
			_loadLayer = LayerViaName.LoadLayer;
			_onlineLoadLayer = LayerViaName.OnlineLoadLayer;
			if (!PhotonNetwork.IsConnected)
			{
				return;
			}
			Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				Send();
			}).AddTo(this);
			foreach (ItemController item in items)
			{
				if (trailer.photonView.Owner.CustomProperties.ContainsKey($"item_{item.item.id}"))
				{
					Vector3 vector = (Vector3)trailer.photonView.Owner.CustomProperties[$"item_{item.item.id}"];
					vector = CheckVectorLength(vector);
					if (vector == Vector3.zero)
					{
						item.Relocate();
					}
					else
					{
						item.SetLocation(vector);
					}
				}
			}
			if ((bool)palet)
			{
				trailer.colliders.Add(palet);
			}
			Observable.EveryFixedUpdate().Subscribe((long _) =>
			{
				UpdateGroundLightAndItemStatus();
			}).AddTo(this);
		}

		public void Init(TrailerController trailerController)
		{
			items = new List<ItemController>();
			onBoard = new List<int>();
			_hash = new ExitGames.Client.Photon.Hashtable();
			int num = 0;
			foreach (Transform item in base.transform)
			{
				if ((bool)item.gameObject.GetComponent<Rigidbody>())
				{
					ItemController itemController = item.gameObject.GetComponent<ItemController>();
					if (!itemController)
					{
						itemController = item.gameObject.AddComponent<ItemController>();
					}
					itemController.Init(num, item, trailerController.Owner == V.DriverPerson);
					num++;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			ItemController component = other.GetComponent<ItemController>();
			if ((bool)component && items.Contains(component))
			{
				component.item.status = true;
				component.item.onGroud = false;
				if (!onBoard.Contains(component.item.id))
				{
					onBoard.Add(component.item.id);
				}
				if ((bool)component.groundLight)
				{
					component.groundLight.SetActive(value: false);
				}
				if (trailer.Owner == V.DriverPerson)
				{
					_taskManager.Task.Order.Price += (uint)component.item.price;
					_taskManager.Task.CurrentItemsCount = onBoard.Count;
					_taskManager.Task.TotalItemsCount = items.Count;
					_taskManager.Task.OnItemsCountChanged?.Invoke();
				}
				if (!trailer.photonView.IsMine)
				{
					component.Rigidbody.useGravity = !PhotonNetwork.IsConnected;
				}
				else
				{
					_hash[$"item_{component.item.id}"] = Vector3.zero;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			ItemController component = other.GetComponent<ItemController>();
			if ((bool)component && items.Contains(component))
			{
				component.item.status = false;
				component.transform.parent = V.GlobalPoint;
				if (onBoard.Contains(component.item.id))
				{
					onBoard.Remove(component.item.id);
				}
				if (trailer.Owner == V.DriverPerson)
				{
					_taskManager.Task.Order.Price -= (uint)component.item.price;
					_taskManager.Task.CurrentItemsCount = onBoard.Count;
					_taskManager.Task.TotalItemsCount = items.Count;
					_taskManager.Task.OnItemsCountChanged?.Invoke();
				}
				if (!trailer.photonView.IsMine)
				{
					component.Rigidbody.useGravity = true;
				}
			}
		}

		public void Relocate(params ItemController[] newItemControllers)
		{
			for (int i = 0; i < newItemControllers.Length; i++)
			{
				newItemControllers[i].Relocate();
			}
			foreach (int item in onBoard)
			{
				items[item].Relocate();
			}
		}

		private void Send()
		{
			if (_hash.Count >= 1 && (bool)trailer && (bool)trailer.Owner)
			{
				if (_owner == null)
				{
					_owner = trailer.Owner.GetComponent<PhotonView>().Owner;
				}
				_owner.SetCustomProperties(_hash);
				_hash.Clear();
			}
		}

		public void EnablePhysics()
		{
			foreach (ItemController item in items)
			{
				item.Rigidbody.useGravity = true;
				item.SetConstraints(freeze: false);
			}
		}

		public void DisablePhysics()
		{
			foreach (ItemController item in items)
			{
				if (item.item.status)
				{
					item.Rigidbody.useGravity = false;
				}
				item.transform.localRotation = Quaternion.identity;
				item.SetConstraints(item.item.status);
			}
		}

		public void UpdatePhysics()
		{
			foreach (int item in onBoard)
			{
				items[item].transform.SetParent(base.transform);
			}
		}

		private void ResetPhysics(Rigidbody rb)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.Sleep();
		}

		private void UpdateGroundLightAndItemStatus()
		{
			if (trailer.Owner == V.DriverPerson)
			{
				foreach (ItemController item in items)
				{
					if (!item.item.status)
					{
						float num = Mathf.Abs(item.Rigidbody.velocity.magnitude);
						float num2 = Mathf.Abs(item.Rigidbody.angularVelocity.magnitude);
						if (num > 0.1f && num2 > 0.1f)
						{
							item.groundLight?.SetActive(value: false);
							continue;
						}
						Vector3 min = item.Collider.bounds.min;
						Vector3 center = item.Collider.bounds.center;
						item.CreateGroundLight(new Vector3(center.x, min.y, center.z));
					}
				}
			}
			if (!trailer.photonView.IsMine && PhotonNetwork.IsConnected)
			{
				foreach (ItemController item2 in items)
				{
					if (item2.item.status)
					{
						item2.transform.localRotation = item2.item.rotation;
						ResetPhysics(item2.Rigidbody);
					}
				}
				return;
			}
			foreach (ItemController item3 in items)
			{
				if (item3.item.status)
				{
					continue;
				}
				float num3 = Mathf.Abs(item3.Rigidbody.velocity.magnitude);
				float num4 = Mathf.Abs(item3.Rigidbody.angularVelocity.magnitude);
				if (num3 < 0.1f && !item3.item.onGroud && num4 < 0.1f)
				{
					if (PhotonNetwork.IsConnected)
					{
						item3.transform.localPosition = CheckVectorLength(item3.transform.localPosition);
						_hash[$"item_{item3.item.id}"] = item3.transform.localPosition;
					}
					item3.item.onGroud = true;
				}
			}
		}

		public void ResetCustomProperties()
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			foreach (ItemController item in items)
			{
				hashtable[$"item_{item.item.id}"] = null;
			}
			trailer.photonView.Owner.SetCustomProperties(hashtable);
		}

		private void OnDestroy()
		{
			foreach (ItemController item in items)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}

		private void OnEnable()
		{
			foreach (int item in onBoard)
			{
				items[item].Rigidbody.constraints = RigidbodyConstraints.None;
			}
		}

		public void EnableItems()
		{
			foreach (int item in onBoard)
			{
				items[item].gameObject.layer = _loadLayer;
			}
		}

		public void DisableItems()
		{
			foreach (int item in onBoard)
			{
				items[item].gameObject.layer = _onlineLoadLayer;
			}
		}

		public void SetItemsLocation(ExitGames.Client.Photon.Hashtable changedProps, bool state)
		{
			foreach (DictionaryEntry changedProp in changedProps)
			{
				if (changedProp.Value == null)
				{
					break;
				}
				Vector3 vector = (Vector3)changedProp.Value;
				int index = int.Parse(((string)changedProp.Key).Split('_')[1]);
				if (vector == Vector3.zero)
				{
					items[index].Relocate();
				}
				else
				{
					items[index].SetLocation(vector);
				}
			}
		}

		private Vector3 CheckVectorLength(Vector3 vector3)
		{
			if (vector3.magnitude > (float)_maxValueToCheck)
			{
				string text = base.name;
				Vector3 vector4 = vector3;
				Debug.LogError(text + ":CheckVectorLength was clamped. " + vector4.ToString());
				vector3 = Vector3.zero;
			}
			return vector3;
		}
	}
}

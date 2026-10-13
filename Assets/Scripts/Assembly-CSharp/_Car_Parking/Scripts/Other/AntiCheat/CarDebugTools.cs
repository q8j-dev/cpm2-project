using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CPM.PlayerData;
using Newtonsoft.Json;
using Photon.Pun;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.ClosedBeta;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Other.AntiCheat
{
	public class CarDebugTools : MonoBehaviour
	{
		private string _json2;

		private string _json1;

		public string dif;

		public bool use1;

		public bool use2;

		public bool use3;

		public bool use4;

		public static bool showtext = true;

		private PhotonView _photonView;

		private PlayerDataStorage _playerDataStorage;

		private FreeDriveProvider _freeDriveProvider;

		public GameObject DebugText;

		[Inject]
		public void Construct(PlayerDataStorage playerDataStorage, FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
			_playerDataStorage = playerDataStorage;
		}

		private void Start()
		{
			Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
			{
				_photonView = GetPhotonView(_freeDriveProvider);
			}).AddTo(this);
			_ = ClosedBetaData.IsClosedBetaBuild;
		}

		private void ShowDebugText()
		{
			VehicleData component = GetComponent<VehicleData>();
			DebugText = AddDebugCarIdText(component.CarId.ToString(), base.transform, component.RoofPosition + 0.25f);
		}

		public static GameObject AddDebugCarIdText(string carId, Transform newParent, float y = 1f)
		{
			GameObject obj = new GameObject("debug text");
			obj.transform.parent = newParent;
			obj.transform.localPosition = new Vector3(0f, y, 0f);
			obj.transform.localScale = new Vector3(-1f, 1f, 1f);
			GameObject debugText = obj;
			TextMeshPro textMeshPro = debugText.AddComponent<TextMeshPro>();
			textMeshPro.text = "CAR ID:" + carId;
			textMeshPro.fontSize = 2f;
			textMeshPro.color = Color.red;
			textMeshPro.alignment = TextAlignmentOptions.Center;
			textMeshPro.enableWordWrapping = false;
			textMeshPro.overflowMode = TextOverflowModes.Overflow;
			textMeshPro.rectTransform.sizeDelta = new Vector2(20f, 6f);
			(from _ in Observable.EveryUpdate()
				where (bool)V.CoreGameManager && debugText.activeInHierarchy
				select _).Subscribe((long _) =>
			{
				Vector3 forward = V.CoreGameManager.transform.position - debugText.transform.position;
				forward.y = 0f;
				if (forward.sqrMagnitude > 0.0001f)
				{
					debugText.transform.rotation = Quaternion.LookRotation(forward);
				}
			}).AddTo(debugText);
			return debugText;
		}

		private void RequestFirstJson()
		{
			_photonView.RPC("CreateFirstJsonScan", RpcTarget.Others);
		}

		private void RequestSecondJson()
		{
			_photonView.RPC("CreateSecondJsonScan", RpcTarget.Others);
		}

		[PunRPC]
		public void CreateFirstJsonScan()
		{
			_json1 = GetJson();
		}

		[PunRPC]
		public void CreateSecondJsonScan(PhotonMessageInfo photonMessageInfo)
		{
			_json2 = GetJson();
			dif = FindStringDifferences(_json1, _json2);
			Debug.Log("Difference: " + dif);
			_photonView.RPC("ReceiveScanDifferences", RpcTarget.Others, dif);
			_freeDriveProvider.FreeDriveControl.chatControl.SendPersonalMessage(dif, photonMessageInfo.Sender.UserId);
		}

		[PunRPC]
		public void ReceiveScanDifferences(string data, PhotonMessageInfo photonMessageInfo)
		{
			Debug.LogFormat("Differences player: {0} \n {1}", photonMessageInfo.Sender.NickName, data);
		}

		private static PhotonView GetPhotonView(FreeDriveProvider freeDriveProvider)
		{
			if (!PhotonNetwork.IsConnected)
			{
				return null;
			}
			PhotonView result = null;
			if ((bool)V.Car)
			{
				PhotonView component = V.Car.GetComponent<PhotonView>();
				if ((bool)component)
				{
					result = PhotonView.Get(component);
				}
			}
			else if (freeDriveProvider != null && (bool)freeDriveProvider.FirstCarMultiPlayer)
			{
				result = PhotonView.Get(freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>());
			}
			return result;
		}

		public static string FindStringDifferences(string str1, string str2)
		{
			StringBuilder stringBuilder = new StringBuilder(Math.Max(str1.Length, str2.Length));
			string[] array = str1.Split(",", a: false);
			string[] array2 = str2.Split(",", a: false);
			Debug.Log("count equals:" + (array.Length == array2.Length));
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string text2 = array2[i];
				if (text != text2)
				{
					stringBuilder.Append("[");
					stringBuilder.Append(text);
					stringBuilder.Append("]_[");
					stringBuilder.Append(text2);
					stringBuilder.Append("]");
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}

		private string GetJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Component[] components = GetComponents<Component>();
			foreach (Component component in components)
			{
				FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fields)
				{
					string arg = fieldInfo.Name;
					object value = fieldInfo.GetValue(component);
					if (fieldInfo.IsDefined(typeof(SerializeField), inherit: false) && (fieldInfo.FieldType.IsArray || typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType)))
					{
						if (fieldInfo.FieldType.IsArray)
						{
							IList list = (IList)value;
							if (list != null && list.Count > 0)
							{
								for (int k = 0; k < list.Count; k++)
								{
									object o = list[k];
									OneField(o, component, dictionary);
								}
							}
						}
						else if (value != null)
						{
							OneField(fieldInfo, component, dictionary);
						}
					}
					else
					{
						OneField(fieldInfo, component, dictionary);
						Debug.Log($"{arg} - {value}");
					}
				}
			}
			string text = JsonConvert.SerializeObject(dictionary);
			Debug.Log(text);
			return text;
		}

		private void OneField(FieldInfo f, Component vComponent, Dictionary<string, object> allData)
		{
			switch (f.FieldType.Name)
			{
			case "Vector3":
			case "Vector2":
			case "Int32":
			case "Int16":
			case "Single":
			case "Byte":
			{
				string text = vComponent.GetType().Name + "_" + f.Name;
				if (!allData.ContainsKey(text))
				{
					allData.Add(text, f.GetValue(vComponent));
				}
				else
				{
					allData.Add(text + 1, f.GetValue(vComponent));
				}
				break;
			}
			}
		}

		private void OneField(object o, Component vComponent, Dictionary<string, object> allData)
		{
			string text = o.GetType().Name;
			switch (text)
			{
			case "Vector3":
			case "Vector2":
			case "Int32":
			case "Int16":
			case "Single":
			case "Byte":
			{
				string text2 = vComponent.GetType().Name + "_" + text;
				if (!allData.ContainsKey(text2))
				{
					allData.Add(text2, text);
				}
				else
				{
					allData.Add(text2 + 1, text);
				}
				break;
			}
			}
		}
	}
}

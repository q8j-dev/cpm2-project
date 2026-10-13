using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Service;
using UnityEngine;
using _Car_Parking;

namespace GPS
{
	public static class GPSTracker
	{
		public enum NavType
		{
			DEPARTURE = 0,
			DESTINATION = 1,
			SERVICE = 2,
			MAP = 3
		}

		public static bool isDrawing;

		private static bool aStarLock;

		public static Dictionary<uint, float> children = new Dictionary<uint, float>(128);

		public static Dictionary<uint, uint> cameFrom = new Dictionary<uint, uint>(2048);

		public static Dictionary<uint, float> distance = new Dictionary<uint, float>(1024);

		public static WayPointsSO scriptableObject;

		private static int depID = -1;

		private static int destID = -1;

		private static readonly Color lineColor = (Color.yellow + Color.red) / 2f;

		private static GameObject player;

		private static LineRenderer lineRenderer;

		private static GameObject _depQuad;

		private static GameObject _destQuad;

		private static Vector3 departure;

		private static Vector3 cashedDest;

		private static NavType cashedNavType;

		private static int previousWayPointID = 0;

		private static Queue<CancellationTokenSource> GPSToken = new Queue<CancellationTokenSource>();

		private static int[] _adjs = new int[4];

		public static List<WayPointNode> nodes => scriptableObject.nodes;

		public static async UniTaskVoid DrawGPS(Vector3 destination, MapCamera mapCamera, NavType navType = NavType.MAP)
		{
			if (navType != NavType.SERVICE)
			{
				cashedDest = destination;
				cashedNavType = navType;
			}
			isDrawing = true;
			if (GPSToken.Count > 0)
			{
				GPSToken.Dequeue().Cancel();
			}
			CancellationTokenSource token = new CancellationTokenSource();
			GPSToken.Enqueue(token);
			while (FlagsStorage.IsFreeDriveMode && !token.IsCancellationRequested)
			{
				if (mapCamera.serviceMapIsOpen)
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(1000.0));
					continue;
				}
				if (V.DriverPerson != null && !aStarLock)
				{
					aStarLock = true;
					SetGPS(destination, navType).Forget();
				}
				await UniTask.Delay(TimeSpan.FromMilliseconds(250.0));
			}
		}

		public static async UniTaskVoid SetGPS(Vector3 destination, NavType navType)
		{
			await EnsureInit();
			depID = FindClosestPoint(player.transform.localPosition);
			if (depID == -1)
			{
				Debug.Log("FAIL: No Dep close point found");
				CancelGPSDraw();
				return;
			}
			destID = FindClosestPoint(destination);
			if (destID == -1)
			{
				Debug.Log("FAIL: No DEST close point found");
				CancelGPSDraw();
				return;
			}
			NodePosition position = nodes[depID].position;
			NodePosition position2 = nodes[destID].position;
			if ((destID == depID || Vector3.Distance(new Vector3(position.x, position.y, position.z), new Vector3(position2.x, position2.y, position2.z)) < 15f) && navType == NavType.MAP)
			{
				CancelGPSDraw();
			}
			else
			{
				AStarSearch(destination, navType).Forget();
			}
		}

		private static async UniTask EnsureInit()
		{
			while (!V.DriverPerson)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(8.0));
			}
			player = V.DriverPerson;
			if ((bool)scriptableObject && (bool)lineRenderer)
			{
				return;
			}
			if ((object)scriptableObject == null)
			{
				ResourceRequest rr = Resources.LoadAsync("GPS/Scriptables/WayPoints");
				while (!rr.isDone)
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(8.0));
				}
				scriptableObject = rr.asset as WayPointsSO;
			}
			if (!lineRenderer)
			{
				lineRenderer = new GameObject("GPS")
				{
					layer = 13
				}.AddComponent<LineRenderer>();
				LineRenderer obj = lineRenderer;
				float startWidth = (lineRenderer.endWidth = 5f);
				obj.startWidth = startWidth;
				Material material = lineRenderer.material;
				material.EnableKeyword("_EMISSION");
				material.SetColor(84, lineColor);
				material.shader = Shader.Find("Unlit/Color");
				material.color = lineColor;
				material.renderQueue = 3000;
				lineRenderer.material = material;
			}
			if (!_destQuad)
			{
				_destQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				_destQuad.layer = 13;
				_destQuad.transform.rotation = Quaternion.Euler(90f, 45f, 0f);
				_destQuad.transform.localScale = new Vector3(30f, 30f, 30f);
				_destQuad.GetComponent<MeshCollider>().enabled = false;
				Material material2 = _destQuad.GetComponent<Renderer>().material;
				material2.shader = Shader.Find("Unlit/Color");
				material2.renderQueue = 3002;
				material2.color = Color.red;
			}
			if (!_depQuad)
			{
				_depQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				_depQuad.SetActive(value: false);
				_depQuad.layer = 13;
				_depQuad.transform.rotation = Quaternion.Euler(90f, 45f, 0f);
				_depQuad.transform.localScale = new Vector3(30f, 30f, 30f);
				_depQuad.GetComponent<MeshCollider>().enabled = false;
				Material material3 = _depQuad.GetComponent<Renderer>().material;
				material3.shader = Shader.Find("Unlit/Color");
				material3.renderQueue = 3002;
				material3.color = Color.red;
			}
		}

		public static int FindClosestPoint(Vector3 pos)
		{
			int num = GetAreaID(pos);
			if ((object)scriptableObject == null)
			{
				Debug.LogError("NO SO found");
				return -1;
			}
			if (scriptableObject.grid != null && !scriptableObject.grid.ContainsKey(num))
			{
				num = GetNeighborAreaID(num);
				if (num == -1)
				{
					return -1;
				}
			}
			Grid grid = scriptableObject.grid[num];
			int num2 = -1;
			float? num3 = null;
			foreach (uint id in grid.ids)
			{
				NodePosition position = nodes[(int)id].position;
				float num4 = Distance(new Vector3(position.x, position.y, position.z), pos);
				if (num2 == -1)
				{
					num2 = (int)id;
					num3 = num4;
				}
				else if (num3 > num4)
				{
					num2 = (int)id;
					num3 = num4;
				}
			}
			return num2;
		}

		public static int FindRandomClosestPoint(Vector3 pos)
		{
			int num = GetAreaID(pos);
			if ((object)scriptableObject == null)
			{
				scriptableObject = Resources.LoadAsync("GPS/Scriptables/WayPoints").asset as WayPointsSO;
			}
			if ((object)scriptableObject == null)
			{
				Debug.LogError("NO SO found");
				return -1;
			}
			if (scriptableObject.grid != null && !scriptableObject.grid.ContainsKey(num))
			{
				num = GetNeighborAreaID(num);
				if (num == -1)
				{
					return -1;
				}
			}
			Grid grid = scriptableObject.grid[num];
			int num2 = -1;
			int num3 = -1;
			float? num4 = null;
			foreach (uint id in grid.ids)
			{
				NodePosition position = nodes[(int)id].position;
				float num5 = Distance(new Vector3(position.x, position.y, position.z), pos);
				if (num2 == -1)
				{
					num2 = (int)id;
					num4 = num5;
				}
				else if (num4 > num5)
				{
					num3 = num2;
					num2 = (int)id;
					num4 = num5;
				}
			}
			if (num2 == previousWayPointID && num3 != -1)
			{
				num2 = num3;
			}
			previousWayPointID = num2;
			return num2;
		}

		private static async UniTaskVoid AStarSearch(Vector3 destination, NavType navType = NavType.MAP)
		{
			uint id = (uint)depID;
			cameFrom.Clear();
			children.Clear();
			NodePosition position = nodes[destID].position;
			Vector3 destPos = new Vector3(position.x, position.y, position.z);
			children.Add(id, nodes[(int)id].CalcHeuristic(destPos));
			if (!distance.ContainsKey(id))
			{
				distance.Add(id, 0f);
			}
			int count = 1;
			while (children != null && children.Count > 0 && id != (uint)destID)
			{
				if (nodes[(int)id].ClosestPath(destPos) == 0)
				{
					return;
				}
				children.Remove(id);
				id = NextNodeID();
				count++;
				if (count % 500 == 0)
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(8.0));
				}
				if (!isDrawing)
				{
					return;
				}
			}
			if (id == (uint)destID)
			{
				DrawLine(destination, navType).Forget();
			}
			else
			{
				Debug.LogError($"GPS no path from {depID} to {destID}");
			}
			aStarLock = false;
		}

		public static float Distance(Vector3 pos1, Vector3 pos2)
		{
			pos2 -= pos1;
			return Mathf.Abs(pos2.x) + Mathf.Abs(pos2.y) + Mathf.Abs(pos2.z);
		}

		public static int GetAreaID(Vector3 pos)
		{
			int num = Mathf.FloorToInt((pos.x + 18000f) / 300f);
			return Mathf.FloorToInt((pos.z + 18000f) / 300f) * 360 + num;
		}

		private static int GetNeighborAreaID(int id)
		{
			int item = id;
			HashSet<int> hashSet = new HashSet<int>();
			Queue<int> queue = new Queue<int>();
			hashSet.Add(item);
			queue.Enqueue(item);
			float num = 100000f;
			int result = -1;
			int num2 = 0;
			while (num2 < 256 && queue.Count > 0)
			{
				num2++;
				item = queue.Dequeue();
				if (scriptableObject.grid.ContainsKey(item))
				{
					float magnitude = (GetVectorFromGrid(id) - GetVectorFromGrid(item)).magnitude;
					if (magnitude < num)
					{
						num = magnitude;
						result = item;
					}
				}
				_adjs[0] = item + 1;
				_adjs[1] = item - 1;
				_adjs[2] = (item - item % 360) / 360 * 359 + item % 360;
				_adjs[3] = (item - item % 360) / 360 * 361 + item % 360;
				int[] adjs = _adjs;
				foreach (int item2 in adjs)
				{
					if (!hashSet.Contains(item2))
					{
						hashSet.Add(item2);
						queue.Enqueue(item2);
					}
				}
			}
			return result;
		}

		private static Vector3 GetVectorFromGrid(int i)
		{
			float z = i % 360 * 100 - 18000;
			return new Vector3((i - i % 360) * 100 - 18000, 0f, z);
		}

		private static uint NextNodeID()
		{
			float num = 100000f;
			uint result = 100000u;
			foreach (KeyValuePair<uint, float> child in children)
			{
				if (num > child.Value)
				{
					num = child.Value;
					result = child.Key;
				}
			}
			return result;
		}

		private static async UniTaskVoid DrawLine(Vector3 destination, NavType navType)
		{
			uint i = (uint)destID;
			int j = 0;
			if (!lineRenderer)
			{
				await EnsureInit();
			}
			if (!lineRenderer.gameObject.activeInHierarchy)
			{
				_destQuad.SetActive(value: true);
				lineRenderer.gameObject.SetActive(value: true);
			}
			lineRenderer.positionCount = 1;
			lineRenderer.SetPosition(j++, destination + V.GlobalOffset);
			while (i != depID && cameFrom.ContainsKey(i))
			{
				lineRenderer.positionCount = j + 1;
				NodePosition position = nodes[(int)cameFrom[i]].position;
				lineRenderer.SetPosition(j, new Vector3(position.x, position.y, position.z) + V.GlobalOffset);
				i = cameFrom[i];
				j++;
			}
			lineRenderer.positionCount = j + 1;
			if (navType == NavType.SERVICE)
			{
				lineRenderer.SetPosition(j, departure + V.GlobalOffset);
			}
			else
			{
				lineRenderer.SetPosition(j, player.transform.position);
			}
			if (_depQuad != null)
			{
				_depQuad.SetActive(value: false);
			}
			switch (navType)
			{
			case NavType.DEPARTURE:
				DrawQuad(ref _destQuad, destination, Color.green);
				break;
			case NavType.DESTINATION:
				DrawQuad(ref _destQuad, destination, Color.red);
				break;
			case NavType.SERVICE:
				_depQuad.SetActive(value: true);
				DrawQuad(ref _depQuad, departure, Color.green);
				DrawQuad(ref _destQuad, destination, Color.red);
				break;
			case NavType.MAP:
				DrawQuad(ref _destQuad, destination, Color.red);
				break;
			}
		}

		private static void DrawQuad(ref GameObject quad, Vector3 destination, Color color)
		{
			NodePosition nodePosition = new NodePosition(destination + V.GlobalOffset);
			quad.transform.position = new Vector3(nodePosition.x, nodePosition.y, nodePosition.z);
			quad.GetComponent<Renderer>().material.color = color;
		}

		private static void DrawCameFrom()
		{
			int num = 0;
			foreach (KeyValuePair<uint, uint> item in cameFrom)
			{
				_ = item;
				GameObject.CreatePrimitive(PrimitiveType.Cube).name = num.ToString();
				num++;
			}
			Debug.Break();
		}

		private static void Clear()
		{
			cameFrom.Clear();
			if (lineRenderer != null)
			{
				lineRenderer.gameObject.SetActive(value: false);
			}
			if (_destQuad != null)
			{
				_destQuad.SetActive(value: false);
			}
			if (_depQuad != null)
			{
				_depQuad.SetActive(value: false);
			}
			departure = Vector3.positiveInfinity;
			aStarLock = false;
			isDrawing = false;
		}

		public static void CancelGPSDraw()
		{
			if (GPSToken.Count > 0)
			{
				GPSToken.Dequeue()?.Cancel();
			}
			Clear();
		}

		public static async UniTaskVoid DrawPath(Vector3 startPosition, Vector3 destination, NavType navType = NavType.DEPARTURE)
		{
			if (GPSToken.Count > 0)
			{
				CancelGPSDraw();
			}
			if (!(V.DriverPerson != null) || aStarLock)
			{
				return;
			}
			aStarLock = true;
			isDrawing = true;
			await EnsureInit();
			depID = FindClosestPoint(startPosition);
			if (depID == -1)
			{
				Debug.Log("FAIL: No Dep close point found");
				return;
			}
			destID = FindClosestPoint(destination);
			if (destID == -1)
			{
				Debug.Log("FAIL: No DEST close point found");
				return;
			}
			await UniTask.Delay(TimeSpan.FromMilliseconds(8.0));
			Debug.Log($"DrawPATH: {depID} : {destID}");
			departure = startPosition;
			AStarSearch(destination, navType).Forget();
		}

		public static void ClearPath()
		{
		}

		public static async void RestartGPS(MapCamera mapCamera)
		{
			Debug.Log($"RESTART {cashedDest} : {cashedNavType}");
			Clear();
			await UniTask.Delay(TimeSpan.FromMilliseconds(15.0));
			if ((cashedNavType == NavType.MAP && cashedDest != Vector3.positiveInfinity) || Global.TaskManager.Task != null)
			{
				DrawGPS(cashedDest, mapCamera, cashedNavType).Forget();
			}
		}

		public static void ForgetCash()
		{
			cashedDest = Vector3.positiveInfinity;
			cashedNavType = NavType.MAP;
		}
	}
}

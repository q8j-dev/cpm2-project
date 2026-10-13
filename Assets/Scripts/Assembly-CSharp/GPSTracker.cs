using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using _Car_Parking;

public static class GPSTracker
{
	public enum GPS
	{
		DEPARTURE = 0,
		DESTINATION = 1,
		SERVICE = 2,
		MAP = 3
	}

	private const float LineDrawOffset = 5f;

	public static GameObject GPSButton;

	public static GameObject allroadsNavMesh;

	private static readonly Vector3 _teleportOffset = Vector3.up * 7f;

	private static NavMeshPath path;

	private static NavMeshPath pathDynamicLine;

	private static NavMeshPath pathAfterCheckpoint;

	private static NavMeshPath pathConnector;

	private static GameObject quadDep;

	private static Material depMat;

	private static GameObject quadDest;

	private static Material destMat;

	private static GameObject newLine;

	private static GameObject newLineFar;

	private static GameObject newLineAfter;

	private static GameObject newLineConnector;

	private static LineRenderer lineNear;

	private static LineRenderer lineFar;

	private static LineRenderer lineAfter;

	private static LineRenderer lineConnector;

	private static int ind = 0;

	private const int areaMask = 16;

	private const int midPointIndex = 8;

	private const int midResetPointIndex = 10;

	private const int startStaticLineIndex = 13;

	private static Vector3 prevDep;

	private static Vector3 prevDest;

	private static Vector3 mid;

	private static Vector3 midPoint;

	private static Vector3 midResetPoint;

	private static Vector3 startStaticLinePoint;

	private static Vector3 checkpointPosition;

	private static bool redrawLineFar;

	private static bool redrawLineAfter;

	private static bool isGlobalPointChanged;

	private static bool _isDrawing = false;

	private static Queue<CancellationTokenSource> GPSToken = new Queue<CancellationTokenSource>();

	public static bool IsDrawing
	{
		get
		{
			return _isDrawing;
		}
		set
		{
			_isDrawing = value;
			OnIsDrawingChangedEvent?.Invoke(value);
		}
	}

	public static event Action<bool> OnIsDrawingChangedEvent;

	public static Vector3 GetPrevDest()
	{
		return prevDest;
	}

	public static async UniTaskVoid DrawGPS(Transform destination, GPS gps, MapCamera mapCamera)
	{
		if (GPSToken.Count > 0)
		{
			CancellationTokenSource cancellationTokenSource = GPSToken.Dequeue();
			cancellationTokenSource.Cancel();
			cancellationTokenSource.Dispose();
		}
		ClearPath(forget: true);
		CancellationTokenSource token = new CancellationTokenSource();
		GPSToken.Enqueue(token);
		while (FlagsStorage.IsFreeDriveMode && !token.IsCancellationRequested)
		{
			IsDrawing = true;
			if (mapCamera.serviceMapIsOpen)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(1000.0));
				continue;
			}
			if (V.DriverPerson != null)
			{
				DrawPath(V.DriverPerson.transform.position - V.GlobalOffset, destination.localPosition, gps);
			}
			if (gps == GPS.MAP && Vector3.Distance(V.DriverPerson.transform.position - V.GlobalOffset, destination.localPosition) < 25f)
			{
				CancelGPSDraw();
			}
			await UniTask.Delay(TimeSpan.FromMilliseconds(250.0));
		}
	}

	public static void DrawGPS(Vector3 destination, GPS gps, MapCamera mapCamera)
	{
		DrawGPSInternal(destination, gps, mapCamera).Forget();
	}

	private static async UniTaskVoid DrawGPSInternal(Vector3 destination, GPS gps, MapCamera mapCamera)
	{
		if (GPSToken.Count > 0)
		{
			CancellationTokenSource cancellationTokenSource = GPSToken.Dequeue();
			cancellationTokenSource.Cancel();
			cancellationTokenSource.Dispose();
		}
		ClearPath(forget: true);
		CancellationTokenSource token = new CancellationTokenSource();
		GPSToken.Enqueue(token);
		while (!token.IsCancellationRequested && (bool)mapCamera)
		{
			IsDrawing = true;
			if (mapCamera.serviceMapIsOpen)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(1000.0));
				continue;
			}
			if ((bool)V.DriverPerson)
			{
				DrawPath(V.DriverPerson.transform.position - V.GlobalOffset, destination, gps);
				if (gps == GPS.MAP && Vector3.Distance(V.DriverPerson.transform.position - V.GlobalOffset, destination) < 25f)
				{
					CancelGPSDraw();
				}
			}
			await UniTask.Delay(TimeSpan.FromMilliseconds(250.0));
		}
	}

	public static void CancelGPSDraw()
	{
		if (GPSToken.Count > 0)
		{
			CancellationTokenSource cancellationTokenSource = GPSToken.Dequeue();
			cancellationTokenSource.Cancel();
			cancellationTokenSource.Dispose();
		}
		ClearPath(forget: true);
	}

	public static void DrawPath(Vector3 startPosition, Vector3 destination, GPS gps = GPS.DEPARTURE)
	{
		prevDest = destination + V.GlobalOffset;
		startPosition += V.GlobalOffset;
		destination += V.GlobalOffset;
		startPosition += new Vector3(0f, 1f, 0f);
		destination += new Vector3(0f, 1f, 0f);
		ClearPath();
		switch (gps)
		{
		case GPS.DEPARTURE:
			DrawQuad(ref quadDest, ref destMat, destination, Color.green);
			break;
		case GPS.DESTINATION:
			DrawQuad(ref quadDest, ref destMat, destination, Color.red);
			break;
		case GPS.SERVICE:
			DrawQuad(ref quadDep, ref depMat, startPosition, Color.green);
			DrawQuad(ref quadDest, ref destMat, destination, Color.red);
			break;
		case GPS.MAP:
			DrawQuad(ref quadDest, ref destMat, destination, Color.red);
			break;
		}
		checkpointPosition = GameObject.Find("Global point/GPS checkpoints/0").transform.position;
		if (allroadsNavMesh != null)
		{
			allroadsNavMesh.GetComponent<NavMeshMediator>().NavMeshSurface.enabled = true;
		}
		float magnitude = (startPosition - checkpointPosition).magnitude;
		if ((startPosition - destination).magnitude > magnitude)
		{
			redrawLineAfter = false;
			if ((startPosition.x < destination.x && startPosition.x < checkpointPosition.x) || (startPosition.x > destination.x && startPosition.x > checkpointPosition.x))
			{
				if (pathAfterCheckpoint == null)
				{
					pathAfterCheckpoint = new NavMeshPath();
					NavMesh.CalculatePath(checkpointPosition, destination, 16, pathAfterCheckpoint);
					redrawLineAfter = true;
					if (newLineFar != null)
					{
						newLineFar.SetActive(value: false);
					}
				}
			}
			else
			{
				pathAfterCheckpoint = null;
				newLineAfter?.SetActive(value: false);
			}
		}
		if (newLine == null)
		{
			InitLine();
		}
		if (pathDynamicLine == null)
		{
			pathDynamicLine = new NavMeshPath();
		}
		if (path == null || isGlobalPointChanged)
		{
			isGlobalPointChanged = false;
			path = new NavMeshPath();
			Vector3 targetPosition = ((pathAfterCheckpoint != null) ? checkpointPosition : destination);
			NavMesh.CalculatePath(startPosition, targetPosition, 16, path);
			if (path.corners.Length <= 13)
			{
				midPoint = targetPosition;
				midResetPoint = targetPosition;
				startStaticLinePoint = targetPosition;
				redrawLineFar = false;
			}
			else
			{
				midPoint = path.corners[8];
				midResetPoint = path.corners[10];
				startStaticLinePoint = path.corners[13];
				redrawLineFar = true;
			}
		}
		if ((double)Mathf.Abs((midPoint - destination).magnitude) < 0.001)
		{
			newLineFar.SetActive(value: false);
			redrawLineFar = false;
		}
		else
		{
			float magnitude2 = (startPosition - midPoint).magnitude;
			float magnitude3 = (startPosition - midResetPoint).magnitude;
			if ((double)Mathf.Abs(Mathf.Min(magnitude2, magnitude3) - magnitude3) < 0.001)
			{
				if (path.corners.Length <= 13 + ind)
				{
					midPoint = destination;
					midResetPoint = destination;
					startStaticLinePoint = destination;
					redrawLineFar = false;
				}
				else
				{
					midPoint = path.corners[ind + 8];
					midResetPoint = path.corners[ind + 10];
					startStaticLinePoint = path.corners[ind + 13];
					ind += 8;
					redrawLineFar = true;
				}
			}
			else if (path.corners != null)
			{
				redrawLineFar = true;
			}
		}
		if (NavMesh.CalculatePath(startPosition, (pathAfterCheckpoint == null || (pathAfterCheckpoint != null && path.corners?.Length > 13 + ind)) ? startStaticLinePoint : checkpointPosition, 16, pathDynamicLine))
		{
			prevDep = startPosition;
		}
		if (pathDynamicLine.corners != null && pathDynamicLine.corners.Length > 18)
		{
			path = new NavMeshPath();
			NavMesh.CalculatePath(prevDep, (pathAfterCheckpoint != null) ? checkpointPosition : destination, 16, path);
			ind = 5;
			if (path.corners == null || path.corners.Length <= 13)
			{
				midPoint = destination;
				midResetPoint = destination;
				startStaticLinePoint = destination;
				redrawLineFar = false;
			}
			else
			{
				midPoint = path.corners[8];
				midResetPoint = path.corners[10];
				startStaticLinePoint = path.corners[13];
				redrawLineFar = true;
			}
		}
		RedrawLines();
	}

	private static void RedrawLines()
	{
		NavMesh.CalculatePath(prevDep, (pathAfterCheckpoint == null || (pathAfterCheckpoint != null && path.corners?.Length > 13 + ind)) ? startStaticLinePoint : checkpointPosition, 16, pathDynamicLine);
		lineNear.positionCount = pathDynamicLine.corners.Length;
		for (int i = 0; i < lineNear.positionCount; i++)
		{
			lineNear.SetPosition(i, pathDynamicLine.corners[i] + Vector3.up * 5f);
		}
		newLine.SetActive(value: true);
		if (redrawLineAfter && pathAfterCheckpoint != null)
		{
			newLineAfter.SetActive(value: false);
			redrawLineAfter = false;
			if (pathAfterCheckpoint.corners.Length != 0)
			{
				lineAfter.positionCount = pathAfterCheckpoint.corners.Length;
			}
			if (pathAfterCheckpoint.corners.Length != 0)
			{
				for (int j = 0; j < lineAfter.positionCount; j++)
				{
					lineAfter.SetPosition(j, pathAfterCheckpoint.corners[j] + Vector3.up * 5f);
				}
			}
			newLineAfter.SetActive(value: true);
		}
		if (redrawLineFar)
		{
			if ((bool)newLineFar)
			{
				newLineFar.SetActive(value: false);
			}
			redrawLineFar = false;
			int num = 0;
			for (int k = 0; k < path.corners.Length; k++)
			{
				if ((double)Mathf.Abs((path.corners[k] - pathDynamicLine.corners[pathDynamicLine.corners.Length - 1]).magnitude) < 0.1)
				{
					num = k;
				}
			}
			lineFar.positionCount = path.corners.Length - num;
			for (int l = num; l < path.corners.Length; l++)
			{
				lineFar.SetPosition(l - num, path.corners[l] + Vector3.up * 5f);
			}
			newLineFar.SetActive(value: true);
		}
		newLineConnector.SetActive(value: false);
		bool flag = true;
		pathConnector = new NavMeshPath();
		if (pathAfterCheckpoint != null && newLineFar.activeSelf && (lineFar.GetPosition(lineFar.positionCount - 1) - lineAfter.GetPosition(0)).magnitude > 1f)
		{
			NavMesh.CalculatePath(lineFar.GetPosition(lineFar.positionCount - 1), lineAfter.GetPosition(0), 16, pathConnector);
		}
		else if (pathAfterCheckpoint != null && newLineAfter.activeSelf && (lineAfter.GetPosition(lineAfter.positionCount - 1) - prevDest).magnitude > 1f)
		{
			NavMesh.CalculatePath(lineAfter.GetPosition(lineAfter.positionCount - 1), prevDest, 16, pathConnector);
		}
		else if (pathAfterCheckpoint == null && path != null && newLineFar.activeSelf && (lineFar.GetPosition(lineFar.positionCount - 1) - prevDest).magnitude > 1f)
		{
			NavMesh.CalculatePath(lineFar.GetPosition(lineFar.positionCount - 1), prevDest, 16, pathConnector);
		}
		else
		{
			flag = false;
		}
		if (flag)
		{
			lineConnector.positionCount = pathConnector.corners.Length;
			for (int m = 0; m < lineConnector.positionCount; m++)
			{
				lineConnector.SetPosition(m, pathConnector.corners[m] + Vector3.up * 5f);
			}
			newLineConnector.SetActive(value: true);
		}
	}

	public static void ClearPath(bool forget = false)
	{
		if (forget)
		{
			if (newLineFar != null)
			{
				newLineFar.SetActive(value: false);
			}
			if (newLineAfter != null)
			{
				newLineAfter.SetActive(value: false);
			}
			path = null;
			pathAfterCheckpoint = null;
			if (newLineConnector != null)
			{
				newLineConnector.SetActive(value: false);
			}
			pathConnector = null;
			prevDest = default;
			IsDrawing = false;
			if (allroadsNavMesh != null)
			{
				allroadsNavMesh.GetComponent<NavMeshMediator>().NavMeshSurface.enabled = false;
			}
		}
		if (pathDynamicLine != null)
		{
			pathDynamicLine = null;
			if (newLine != null)
			{
				newLine.SetActive(value: false);
			}
			if (quadDep != null)
			{
				quadDep.SetActive(value: false);
			}
			if (quadDest != null)
			{
				quadDest.SetActive(value: false);
			}
		}
	}

	public static float DistanceBetween(Vector3 startPosition, Vector3 destination)
	{
		float num = 0f;
		checkpointPosition = GameObject.Find("Global point/GPS checkpoints/0").transform.position;
		if (allroadsNavMesh != null)
		{
			allroadsNavMesh.GetComponent<NavMeshMediator>().NavMeshSurface.enabled = true;
		}
		NavMeshPath navMeshPath = new NavMeshPath();
		float magnitude = (startPosition - checkpointPosition).magnitude;
		if ((startPosition - destination).magnitude > magnitude && ((startPosition.x < destination.x && startPosition.x < checkpointPosition.x) || (startPosition.x > destination.x && startPosition.x > checkpointPosition.x)))
		{
			NavMesh.CalculatePath(checkpointPosition + V.GlobalOffset, destination + V.GlobalOffset, 16, navMeshPath);
			for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
			{
				num += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
			}
		}
		NavMeshPath navMeshPath2 = new NavMeshPath();
		NavMesh.CalculatePath(startPosition + V.GlobalOffset, (navMeshPath.corners.Length != 0) ? checkpointPosition : (destination + V.GlobalOffset), 16, navMeshPath2);
		for (int j = 0; j < navMeshPath2.corners.Length - 1; j++)
		{
			num += Vector3.Distance(navMeshPath2.corners[j], navMeshPath2.corners[j + 1]);
		}
		if (allroadsNavMesh != null)
		{
			allroadsNavMesh.GetComponent<NavMeshMediator>().NavMeshSurface.enabled = false;
		}
		return num;
	}

	private static void DrawQuad(ref GameObject quad, ref Material quadMat, Vector3 quadPosition, Color quadColor)
	{
		if (quad == null)
		{
			quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			quad.layer = 13;
			quad.transform.rotation = Quaternion.Euler(90f, 45f, 0f);
			quad.transform.localScale = new Vector3(35f, 35f, 35f);
			quad.GetComponent<MeshCollider>().enabled = false;
			if (quadMat == null)
			{
				quadMat = new Material(Shader.Find("Unlit/Color"));
				quadMat.renderQueue = 3002;
			}
			quad.GetComponent<Renderer>().sharedMaterial = quadMat;
		}
		quadMat.color = quadColor;
		quad.transform.position = quadPosition;
		quad.SetActive(value: true);
	}

	private static void InitLine()
	{
		Color color = (Color.yellow + Color.red) / 2f;
		color.a = 0.9f;
		newLine = new GameObject("LineNear")
		{
			layer = 13
		};
		newLineFar = new GameObject("LineFar")
		{
			layer = 13
		};
		newLineAfter = new GameObject("LineAfter")
		{
			layer = 13
		};
		newLineConnector = new GameObject("LineConnector")
		{
			layer = 13
		};
		lineNear = newLine.AddComponent<LineRenderer>();
		LineRenderer lineRenderer = lineNear;
		float startWidth = (lineNear.endWidth = 20f);
		lineRenderer.startWidth = startWidth;
		lineFar = newLineFar.AddComponent<LineRenderer>();
		LineRenderer lineRenderer2 = lineFar;
		startWidth = (lineFar.endWidth = 20f);
		lineRenderer2.startWidth = startWidth;
		lineAfter = newLineAfter.AddComponent<LineRenderer>();
		LineRenderer lineRenderer3 = lineAfter;
		startWidth = (lineAfter.endWidth = 20f);
		lineRenderer3.startWidth = startWidth;
		lineConnector = newLineConnector.AddComponent<LineRenderer>();
		LineRenderer lineRenderer4 = lineConnector;
		startWidth = (lineConnector.endWidth = 20f);
		lineRenderer4.startWidth = startWidth;
		Material material = new Material(Shader.Find("Unlit/Color"));
		material.color = color;
		material.renderQueue = 3000;
		lineNear.GetComponent<Renderer>().sharedMaterial = material;
		lineFar.GetComponent<Renderer>().sharedMaterial = material;
		lineAfter.GetComponent<Renderer>().sharedMaterial = material;
		lineConnector.GetComponent<Renderer>().sharedMaterial = material;
	}

	public static void RecalcPath()
	{
		checkpointPosition = GameObject.Find("Global point/GPS checkpoints/0").transform.position;
		isGlobalPointChanged = true;
		pathAfterCheckpoint = null;
		if (lineAfter != null && lineAfter.positionCount > 0)
		{
			Vector3 vector = checkpointPosition - lineAfter.GetPosition(0);
			for (int i = 0; i < lineAfter.positionCount; i++)
			{
				lineAfter.SetPosition(i, lineAfter.GetPosition(i) + vector + Vector3.up * 5f);
			}
		}
	}

	public static void TeleportTo(GameObject target, Vector3 toPos)
	{
		Vector3 position = toPos + _teleportOffset;
		target.transform.position = position;
	}

	public static void ResetStatics()
	{
		OnIsDrawingChangedEvent = null;
		GPSButton = null;
		allroadsNavMesh = null;
		path = null;
		pathDynamicLine = null;
		pathAfterCheckpoint = null;
		pathConnector = null;
		quadDep = null;
		quadDest = null;
		depMat = null;
		destMat = null;
		newLine = null;
		newLineFar = null;
		newLineAfter = null;
		newLineConnector = null;
		lineNear = null;
		lineFar = null;
		lineAfter = null;
		lineConnector = null;
		_isDrawing = false;
		while (GPSToken.Count > 0)
		{
			CancellationTokenSource cancellationTokenSource = GPSToken.Dequeue();
			try
			{
				cancellationTokenSource.Cancel();
			}
			catch
			{
			}
			cancellationTokenSource.Dispose();
		}
	}
}

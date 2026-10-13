using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachinePathRecorder : MonoBehaviour
{
	[Header("Target")]
	public CinemachinePath TargetPath;

	public Transform RecordFrom;

	[Min(0.05f)]
	[Header("Settings")]
	public float RecordInterval = 0.3f;

	[Min(0.01f)]
	public float MinDistance = 0.5f;

	private readonly List<Vector3> _positions = new List<Vector3>();

	private Coroutine _recordingCoroutine;

	public bool IsRecording { get; private set; }

	[ContextMenu("Start Recording")]
	public void StartRecording()
	{
		if (!IsRecording)
		{
			if (!RecordFrom)
			{
				Debug.LogError("[Recorder] RecordFrom не задан");
				return;
			}
			_positions.Clear();
			IsRecording = true;
			_recordingCoroutine = StartCoroutine(RecordLoop());
			Debug.Log("[Recorder] Запись начата");
		}
	}

	[ContextMenu("Stop and Apply")]
	public void StopAndApply()
	{
		if (IsRecording)
		{
			IsRecording = false;
			if (_recordingCoroutine != null)
			{
				StopCoroutine(_recordingCoroutine);
				_recordingCoroutine = null;
			}
			ApplyToPath();
		}
	}

	private IEnumerator RecordLoop()
	{
		while (IsRecording)
		{
			Vector3 position = RecordFrom.position;
			if (_positions.Count != 0)
			{
				List<Vector3> positions = _positions;
				if (!(Vector3.Distance(positions[positions.Count - 1], position) >= MinDistance))
				{
					goto IL_0066;
				}
			}
			_positions.Add(position);
			goto IL_0066;
			IL_0066:
			yield return new WaitForSeconds(RecordInterval);
		}
	}

	private void ApplyToPath()
	{
		if (!TargetPath)
		{
			Debug.LogError("[Recorder] TargetPath не задан");
			return;
		}
		if (_positions.Count < 2)
		{
			Debug.LogWarning("[Recorder] Недостаточно точек (нужно минимум 2)");
			return;
		}
		CinemachinePath.Waypoint[] array = new CinemachinePath.Waypoint[_positions.Count];
		for (int i = 0; i < _positions.Count; i++)
		{
			Vector3 position = TargetPath.transform.InverseTransformPoint(_positions[i]);
			Vector3 tangent = Vector3.forward;
			if (i < _positions.Count - 1)
			{
				tangent = TargetPath.transform.InverseTransformDirection((_positions[i + 1] - _positions[i]).normalized);
			}
			else if (i > 0)
			{
				tangent = TargetPath.transform.InverseTransformDirection((_positions[i] - _positions[i - 1]).normalized);
			}
			array[i] = new CinemachinePath.Waypoint
			{
				position = position,
				tangent = tangent,
				roll = 0f
			};
		}
		TargetPath.m_Waypoints = array;
		Debug.Log($"[Recorder] Применено {array.Length} waypoints в {TargetPath.name}");
	}
}

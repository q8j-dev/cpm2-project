using System;
using System.Collections.Generic;
using System.Linq;
using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Rally.Data;
using _Car_Parking.Scripts.Scopes;

public class RallyTriggersControl : MonoBehaviour
{
	[SerializeField]
	private GameObject FinishElements;

	[SerializeField]
	private GameObject StartElements;

	[SerializeField]
	private GameObject StartLine;

	[SerializeField]
	private GameObject StartMarker;

	public RallyMarshalController StartMarshall;

	public RallyMarshalController FinishMarshall;

	public GameObject StopLine;

	[SerializeField]
	private RallySeqenceTriggerLine TriggerLinePrefab;

	[SerializeField]
	private GameObject GaragePropsParent;

	public GameObject StartMarkerPlace;

	public GameObject FinishMarker;

	public RallySequenceUIController RallySequenceUIControllerPrefab;

	private IPrefabFactory _prefabFactory;

	private List<RallySeqenceTriggerLine> _triggerList = new List<RallySeqenceTriggerLine>();

	private RallySeqenceTriggerLine _startTriggerLine;

	private int _cachedActiveIndex = -1;

	private int _passedCheckpoints;

	private GameObject _trajectoryGameObject;

	public RallySequenceData RallySequenceData;

	public Action<int> CheckpointCrossed;

	private int _triggersListCount;

	private int _realCheckpointsPassed;

	private OptionalDependency<FreeTimeAttackController> _freeTimeAttackControllerDep;

	private FreeTimeAttackController _freeTimeAttackController;

	public List<RallySeqenceTriggerLine> TriggerList => _triggerList;

	public int CachedActiveIndex => _cachedActiveIndex;

	public int PassedCheckpoints => _passedCheckpoints;

	public int TotalCheckpoints => _triggerList.Count;

	public int RealCheckpointsTotal => _triggersListCount;

	public int RealCheckpointsPassed => _realCheckpointsPassed;

	public List<Vector3> RoadCenterline => RallySequenceData?.RoadCenterline;

	public float RoadHalfWidth => RallySequenceData?.RoadHalfWidth ?? 20f;

	[UnityEngine.Scripting.Preserve]
	[Inject]
	public void Construct(IPrefabFactory prefabFactory, OptionalDependency<FreeTimeAttackController> freeTimeAttackControllerDep)
	{
		_prefabFactory = prefabFactory;
		_freeTimeAttackControllerDep = freeTimeAttackControllerDep;
	}

	private void Awake()
	{
		_freeTimeAttackController = _freeTimeAttackControllerDep?.Value;
		if (_freeTimeAttackController != null)
		{
			_freeTimeAttackController.RegisterRallyTriggersControlPassive(this);
		}
	}

	private void SetGarage()
	{
		GaragePropsParent.transform.SetPositionAndRotation(RallySequenceData.RaceGaragePoint.Position, RallySequenceData.RaceGaragePoint.Rotation);
	}

	public async UniTask Init(bool spawnTrajectory = true)
	{
		_passedCheckpoints = 0;
		_realCheckpointsPassed = 0;
		_cachedActiveIndex = -1;
		DestroyTrajectory();
		ResetMarshalState();
		await SpawnTriggers(RallySequenceData);
		if (spawnTrajectory)
		{
			await UniTask.Yield();
			SpawnTrajectoryPrefabs(RallySequenceData).Forget();
			await UniTask.Yield();
		}
		if (_freeTimeAttackController != null)
		{
			return;
		}
		RallySeqenceTriggerLine rallySeqenceTriggerLine = _triggerList.FirstOrDefault();
		if (!(rallySeqenceTriggerLine == null))
		{
			int num = _triggerList.Count - 2;
			if (num >= 0)
			{
				RallySeqenceTriggerLine rallySeqenceTriggerLine2 = _triggerList.LastOrDefault();
				RallySeqenceTriggerLine rallySeqenceTriggerLine3 = _triggerList[num];
				SetGarage();
				StartElements.transform.SetPositionAndRotation(rallySeqenceTriggerLine.transform.position, rallySeqenceTriggerLine.transform.rotation);
				FinishElements.transform.SetPositionAndRotation(rallySeqenceTriggerLine3.transform.position, rallySeqenceTriggerLine3.transform.rotation);
				StopLine.transform.SetPositionAndRotation(rallySeqenceTriggerLine2.transform.position, rallySeqenceTriggerLine2.transform.rotation);
				Quaternion quaternion = rallySeqenceTriggerLine.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
				StartMarshall.transform.SetPositionAndRotation(rallySeqenceTriggerLine.transform.position, quaternion);
				StartMarshall.SetRoadPosition(rallySeqenceTriggerLine.transform.position, quaternion);
				Quaternion quaternion2 = rallySeqenceTriggerLine2.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
				FinishMarshall.transform.SetPositionAndRotation(rallySeqenceTriggerLine2.transform.position, quaternion2);
				FinishMarshall.SetRoadPosition(rallySeqenceTriggerLine2.transform.position, quaternion2);
				StartMarshall.ResetToInitialPosition();
				FinishMarshall.ResetToInitialPosition();
			}
		}
	}

	private void ResetMarshalState()
	{
		if (_freeTimeAttackController == null)
		{
			StartLine.SetActive(value: true);
			StartMarker.SetActive(value: true);
			StartMarshall.SetActive(active: true);
			FinishMarshall.SetActive(active: true);
		}
	}

	private async UniTaskVoid SpawnTrajectoryPrefabs(RallySequenceData rallySequenceData)
	{
		if ((bool)rallySequenceData.TrajectoryPrefab)
		{
			_trajectoryGameObject = _prefabFactory.Instantiate(rallySequenceData.TrajectoryPrefab, base.transform);
		}
	}

	private async UniTask SpawnTriggers(RallySequenceData rallySequenceData)
	{
		if (!V.Car)
		{
			return;
		}
		bool applyGlobalOffset = _freeTimeAttackController != null;
		List<RallySeqenceTriggerLine> triggerList = new List<RallySeqenceTriggerLine>();
		for (int i = 0; i < rallySequenceData.RallySequenceItems.Count; i++)
		{
			Vector3 vector = (applyGlobalOffset ? V.GlobalPoint.position : Vector3.zero);
			RallySequenceItem rallySequenceItem = rallySequenceData.RallySequenceItems[i];
			RallySeqenceTriggerLine rallySeqenceTriggerLine = _prefabFactory.Instantiate(TriggerLinePrefab, rallySequenceItem.Position + vector, rallySequenceItem.Rotation, base.transform);
			rallySeqenceTriggerLine.Init(rallySequenceItem);
			rallySeqenceTriggerLine.RegisterCar(V.Car);
			triggerList.Add(rallySeqenceTriggerLine);
			rallySeqenceTriggerLine.gameObject.SetActive(value: false);
			await UniTask.Yield();
		}
		for (int j = 0; j < triggerList.Count - 1; j++)
		{
			triggerList[j].SetNextTrigger(triggerList[j + 1]);
		}
		for (int k = 0; k < triggerList.Count - 1; k++)
		{
			Transform prev = ((k > 0) ? triggerList[k - 1].transform : triggerList[k].transform);
			Transform start = triggerList[k].transform;
			Transform end = triggerList[k + 1].transform;
			Transform nextEnd = ((k < triggerList.Count - 2) ? triggerList[k + 2].transform : triggerList[k + 1].transform);
			triggerList[k].SetSegmentControlPoints(prev, start, end, nextEnd);
		}
		for (int l = 0; l < triggerList.Count; l++)
		{
			int idx = l;
			bool isRealCheckpoint = IsRealCheckpoint(rallySequenceData.RallySequenceItems[l].SequenceTurnType);
			triggerList[l].SetIndex(l);
			RallySeqenceTriggerLine rallySeqenceTriggerLine2 = triggerList[l];
			rallySeqenceTriggerLine2.OnActivated = (Action<bool>)Delegate.Combine(rallySeqenceTriggerLine2.OnActivated, (Action<bool>)((bool activated) =>
			{
				if (activated)
				{
					CheckpointCrossed?.Invoke(idx);
					_cachedActiveIndex = idx;
				}
				else
				{
					_passedCheckpoints++;
					if (isRealCheckpoint)
					{
						_realCheckpointsPassed++;
					}
				}
			}));
		}
		_triggerList = triggerList;
		_triggersListCount = GetTriggersCount();
	}

	public void ActivateTriggers()
	{
		foreach (RallySeqenceTriggerLine trigger in _triggerList)
		{
			trigger.gameObject.SetActive(value: true);
			trigger.Activate();
		}
		if ((bool)StartElements)
		{
			StartElements.SetActive(value: true);
		}
		if ((bool)StartMarshall)
		{
			StartMarshall.SetActive(active: true);
		}
		if ((bool)FinishElements)
		{
			FinishElements.SetActive(value: true);
		}
		if ((bool)FinishMarshall)
		{
			FinishMarshall.SetActive(active: true);
		}
	}

	public void ActivateFirstCheckpointTrigger()
	{
		if (_triggerList.Count > 0)
		{
			_triggerList[0].Activate();
		}
	}

	public async UniTaskVoid OnStageRace(RallyMarshalController marshalController)
	{
		marshalController.SetAnimation("prepareStart", state: true);
		await UniTask.Delay(100);
		marshalController.SetAnimation("start", state: true);
	}

	public void OnCarNeedToStop(bool isRaceStarted)
	{
		if (isRaceStarted)
		{
			StartMarshall.SetAnimation("stop", state: true);
		}
		else
		{
			FinishMarshall.SetAnimation("stop", state: true);
		}
	}

	public void OnStartRace()
	{
		StartLine.gameObject.SetActive(value: false);
		StartMarshall.GoAway().Forget();
		StartMarker.SetActive(value: false);
	}

	public void RestartRace()
	{
		_cachedActiveIndex = -1;
		_passedCheckpoints = 0;
		_realCheckpointsPassed = 0;
		foreach (RallySeqenceTriggerLine trigger in _triggerList)
		{
			trigger.ResetPassedState();
		}
		if (_freeTimeAttackController == null)
		{
			StartLine.SetActive(value: true);
			StartElements.SetActive(value: true);
			StartMarshall.ResetToInitialPosition();
			StartMarshall.SetActive(active: true);
			StartMarker.SetActive(value: true);
			FinishElements.SetActive(value: true);
			FinishMarshall.SetActive(active: true);
		}
	}

	public void DestroyTrajectory()
	{
		foreach (RallySeqenceTriggerLine trigger in _triggerList)
		{
			if (trigger != null)
			{
				UnityEngine.Object.Destroy(trigger.gameObject);
			}
		}
		_triggerList.Clear();
		_triggersListCount = 0;
		if (_trajectoryGameObject != null)
		{
			UnityEngine.Object.Destroy(_trajectoryGameObject);
			_trajectoryGameObject = null;
		}
	}

	private static bool IsRealCheckpoint(RallySequenceTurnType turnType)
	{
		if (turnType != RallySequenceTurnType.Start && turnType != RallySequenceTurnType.Finish)
		{
			return turnType != RallySequenceTurnType.Stop;
		}
		return false;
	}

	private int GetTriggersCount()
	{
		int num = 0;
		for (int i = 0; i < _triggerList.Count; i++)
		{
			if (IsRealCheckpoint(_triggerList[i].RallySequenceItem.SequenceTurnType))
			{
				num++;
			}
		}
		return num;
	}
}

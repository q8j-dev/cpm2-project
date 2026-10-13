using System.Collections.Generic;
using System.Linq;
using Arikan;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace Car_Parking.Scripts.Racing
{
	public class RaceTriggersValidator : MonoBehaviour
	{
		private const int MissCheckpointMultiplier = 15;

		[SerializeField]
		private RacePropsData[] RacePropsDatas;

		[SerializeField]
		private V.placeToSpawn PlaceToSpawn;

		private List<RaceStartTrigger> _lastHiddenStartTriggers = new List<RaceStartTrigger>();

		private FreeTimeAttackController _timeAttackController;

		private TimeBannersManager _timeBannersManager;

		public StartTrafficLightObject StartTrafficLightObject;

		public MiniMapBounds MapBounds;

		public RallyTriggersControl RallyTriggersControl;

		[UnityEngine.Scripting.Preserve]
		[Inject]
		public void Construct(FreeTimeAttackController timeAttackController, TimeBannersManager timeBannersManager)
		{
			_timeAttackController = timeAttackController;
			_timeBannersManager = timeBannersManager;
		}

		private void Start()
		{
			_timeAttackController.RegisterTriggerValidatorPassive(this);
			_timeBannersManager.UpdateTablos(PlaceToSpawn);
		}

		public bool HasRacePropsData(int trackIndex, int trajectoryIndex)
		{
			return GetRacePropsData(trackIndex, trajectoryIndex) != null;
		}

		public void TryShowLastHiddenStartTriggersSafe()
		{
			if (_lastHiddenStartTriggers == null || _lastHiddenStartTriggers.Count == 0)
			{
				return;
			}
			foreach (RaceStartTrigger lastHiddenStartTrigger in _lastHiddenStartTriggers)
			{
				if ((bool)lastHiddenStartTrigger)
				{
					lastHiddenStartTrigger.gameObject.SetActive(value: true);
				}
			}
			_lastHiddenStartTriggers.Clear();
		}

		public void ShowStartTriggers(int trackIndex, int trajectoryIndex, bool state)
		{
			RacePropsData racePropsData = GetRacePropsData(trackIndex, trajectoryIndex);
			ShowStartTriggers(racePropsData, state);
		}

		private void ShowStartTriggers(RacePropsData propsData, bool state)
		{
			if (propsData == null || propsData.RaceStartTriggers == null)
			{
				return;
			}
			RaceStartTrigger[] raceStartTriggers = propsData.RaceStartTriggers;
			foreach (RaceStartTrigger raceStartTrigger in raceStartTriggers)
			{
				if ((bool)raceStartTrigger)
				{
					raceStartTrigger.gameObject.SetActive(state);
				}
			}
		}

		public void TryHideStartTriggersInLocation(V.placeToSpawn location)
		{
			RacePropsData[] array = RacePropsDatas.Where((RacePropsData data) => data.Location == location).ToArray();
			if (array == null || array.Length < 1)
			{
				return;
			}
			for (int num = 0; num < array.Length; num++)
			{
				if (array[num].RaceStartTriggers == null)
				{
					continue;
				}
				RaceStartTrigger[] raceStartTriggers = array[num].RaceStartTriggers;
				foreach (RaceStartTrigger raceStartTrigger in raceStartTriggers)
				{
					if ((bool)raceStartTrigger)
					{
						raceStartTrigger.gameObject.SetActive(value: false);
						_lastHiddenStartTriggers.Add(raceStartTrigger);
					}
				}
			}
		}

		public void TryShowLastHiddenStartTriggers()
		{
			if (_lastHiddenStartTriggers == null)
			{
				return;
			}
			foreach (RaceStartTrigger lastHiddenStartTrigger in _lastHiddenStartTriggers)
			{
				lastHiddenStartTrigger.gameObject.SetActive(value: true);
			}
			_lastHiddenStartTriggers = null;
		}

		public void Init(int index, int trajectoryIndex)
		{
			ActivateProps(state: true);
			ActivateCheckers(index, trajectoryIndex, state: true);
		}

		public RacePropsData[] GetRacePropsDatas()
		{
			return RacePropsDatas;
		}

		public Texture GetMapTexture(int index, int trajectoryIndex)
		{
			Texture obj = GetRacePropsData(index, trajectoryIndex)?.MapTexture;
			_ = (bool)obj;
			return obj;
		}

		private RacePropsData GetRacePropsData(int index, int trajectoryIndex)
		{
			for (int i = 0; i < RacePropsDatas.Length; i++)
			{
				if (RacePropsDatas[i].TrackIndex == index && RacePropsDatas[i].TrajectoryIndex == trajectoryIndex)
				{
					return RacePropsDatas[i];
				}
			}
			return null;
		}

		public void DeactivateAll()
		{
			ActivateProps(state: false);
			for (int i = 0; i < RacePropsDatas.Length; i++)
			{
				ActivateCheckers(RacePropsDatas[i], state: false);
				ShowStartTriggers(RacePropsDatas[i], state: true);
				EnableStartTriggers(RacePropsDatas[i].TrackIndex, RacePropsDatas[i].TrajectoryIndex, state: false);
			}
		}

		public void ActivateProps(bool state)
		{
			GlobalEventSystem.ActivateInSceneProps(state);
		}

		public void ActivateCheckers(int index, int trajectoryIndex, bool state)
		{
			RacePropsData racePropsData = GetRacePropsData(index, trajectoryIndex);
			ActivateCheckers(racePropsData, state);
		}

		private void ActivateCheckers(RacePropsData propsData, bool state)
		{
			if (propsData == null)
			{
				return;
			}
			for (int i = 0; i < propsData.Triggers.Length; i++)
			{
				propsData.Triggers[i].gameObject.SetActive(state);
				if (state)
				{
					propsData.Triggers[i].CheckpointData.time = 0f;
					propsData.Triggers[i].CheckpointData.id = i;
					propsData.Triggers[i].CheckpointData.missed = true;
				}
			}
		}

		public void EnableStartTriggers(int index, int trajectoryIndex, bool state)
		{
			RacePropsData racePropsData = GetRacePropsData(index, trajectoryIndex);
			if (racePropsData != null)
			{
				for (int i = 0; i < racePropsData.ActivatableTriggers.Length; i++)
				{
					racePropsData.ActivatableTriggers[i].gameObject.SetActive(state);
				}
			}
		}

		public List<CheckpointData> GetAllCheckpointsData(int trackIndex, int tracjectoryIndex)
		{
			List<CheckpointData> list = new List<CheckpointData>();
			RacePropsData racePropsData = GetRacePropsData(trackIndex, tracjectoryIndex);
			for (int i = 0; i < racePropsData.Triggers.Length; i++)
			{
				list.Add(racePropsData.Triggers[i].CheckpointData);
			}
			return list;
		}

		public int GetAllCheckpointsCount(int trackIndex, int tracjectoryIndex)
		{
			RacePropsData racePropsData = GetRacePropsData(trackIndex, tracjectoryIndex);
			if ((bool)RallyTriggersControl)
			{
				return RallyTriggersControl.TotalCheckpoints;
			}
			if (racePropsData == null)
			{
				return -1;
			}
			return racePropsData.Triggers.Length;
		}

		public void GetMissingCheckpointsCount(int index, int trajectoryIndex, out int missCheckpointsCount, out int missCheckpointMultiplierSecs)
		{
			if ((bool)RallyTriggersControl)
			{
				missCheckpointsCount = RallyTriggersControl.TotalCheckpoints - RallyTriggersControl.PassedCheckpoints;
				missCheckpointMultiplierSecs = 15;
				return;
			}
			int num = 0;
			int num2 = 0;
			RacePropsData racePropsData = GetRacePropsData(index, trajectoryIndex);
			if (racePropsData != null)
			{
				RaceFinishLineTrigger[] triggers = racePropsData.Triggers;
				foreach (RaceFinishLineTrigger raceFinishLineTrigger in triggers)
				{
					if (!raceFinishLineTrigger.CheckpointData.missed)
					{
						raceFinishLineTrigger.SetActive(active: false);
					}
					if (raceFinishLineTrigger.gameObject.activeSelf)
					{
						num++;
					}
				}
				num2 = racePropsData.MissCheckpointMultiplierSecs;
			}
			missCheckpointsCount = num;
			missCheckpointMultiplierSecs = num2;
		}

		public Transform GetRandomStartPoint(int raceIndex, int trajectoryIndex)
		{
			RacePropsData racePropsData = GetRacePropsData(raceIndex, trajectoryIndex);
			if (racePropsData == null)
			{
				return null;
			}
			int num = Random.Range(0, racePropsData.RestartPoints.Length);
			return racePropsData.RestartPoints[num];
		}

		public Transform GetNearestToRoadPoint(Transform raceCarTransform, int raceIndex, int trajectoryIndex)
		{
			Transform result = null;
			float num = float.MaxValue;
			RacePropsData racePropsData = GetRacePropsData(raceIndex, trajectoryIndex);
			if (racePropsData == null)
			{
				return null;
			}
			Transform[] toRoadPoints = racePropsData.ToRoadPoints;
			foreach (Transform transform in toRoadPoints)
			{
				if ((bool)transform)
				{
					float num2 = Vector3.SqrMagnitude(raceCarTransform.position - transform.position);
					if (num2 < num)
					{
						num = num2;
						result = transform;
					}
				}
			}
			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public abstract class GraphicsDistanceController
	{
		public enum PriorityType
		{
			Default = 0,
			DragRacing = 1,
			Garage = 2,
			RacingCircuit = 3
		}

		public enum SetType
		{
			Force = 0,
			SetIfLarger = 1,
			SetIfSmaller = 2
		}

		protected class SettingsDistanceData
		{
			public PriorityType PriorityType;

			public float Distance;

			public SetType SetType;

			public SettingsDistanceData(PriorityType priorityType, float distance)
			{
				PriorityType = priorityType;
				Distance = distance;
			}
		}

		private List<SettingsDistanceData> _settingsDistanceDatas = new List<SettingsDistanceData>();

		protected float CurrentDistance => GetCurrentSettingsDistanceData()?.Distance ?? DefaultDistance;

		protected abstract float DefaultDistance { get; }

		public void ClearSettings()
		{
			foreach (PriorityType value in Enum.GetValues(typeof(PriorityType)))
			{
				if (value != PriorityType.Default)
				{
					RemoveSettings(value);
				}
			}
		}

		public void RemoveSettings(PriorityType priorityType)
		{
			float currentDistance = CurrentDistance;
			for (int i = 0; i < _settingsDistanceDatas.Count; i++)
			{
				if (_settingsDistanceDatas[i].PriorityType == priorityType)
				{
					_settingsDistanceDatas.RemoveAt(i);
					break;
				}
			}
			if (!Mathf.Approximately(currentDistance, CurrentDistance))
			{
				OnCurrentSettingsDistanceChanged();
			}
		}

		public void SetSettings(PriorityType priorityType, float distance, SetType setType = SetType.Force)
		{
			float currentDistance = CurrentDistance;
			SettingsDistanceData settingsDistanceData = GetSettingsDistanceData(priorityType);
			if (settingsDistanceData == null)
			{
				settingsDistanceData = new SettingsDistanceData(priorityType, DefaultDistance);
				_settingsDistanceDatas.Add(settingsDistanceData);
			}
			settingsDistanceData.Distance = distance;
			settingsDistanceData.SetType = setType;
			if (!Mathf.Approximately(currentDistance, CurrentDistance))
			{
				OnCurrentSettingsDistanceChanged();
			}
		}

		public bool HaveSettings(PriorityType priorityType)
		{
			return GetSettingsDistanceData(priorityType) != null;
		}

		public float GetDistanceSettings(PriorityType priorityType)
		{
			return GetSettingsDistanceData(priorityType)?.Distance ?? (-1f);
		}

		protected virtual void OnCurrentSettingsDistanceChanged()
		{
		}

		protected SettingsDistanceData GetCurrentSettingsDistanceData()
		{
			SettingsDistanceData settingsDistanceData = null;
			if (_settingsDistanceDatas.Count > 0)
			{
				settingsDistanceData = _settingsDistanceDatas[0];
				for (int i = 1; i < _settingsDistanceDatas.Count; i++)
				{
					if (_settingsDistanceDatas[i].PriorityType > settingsDistanceData.PriorityType)
					{
						if (_settingsDistanceDatas[i].SetType == SetType.Force)
						{
							settingsDistanceData = _settingsDistanceDatas[i];
						}
						else if (_settingsDistanceDatas[i].SetType == SetType.SetIfLarger && _settingsDistanceDatas[i].Distance > settingsDistanceData.Distance)
						{
							settingsDistanceData = _settingsDistanceDatas[i];
						}
						else if (_settingsDistanceDatas[i].SetType == SetType.SetIfSmaller && _settingsDistanceDatas[i].Distance < settingsDistanceData.Distance)
						{
							settingsDistanceData = _settingsDistanceDatas[i];
						}
					}
				}
			}
			return settingsDistanceData;
		}

		private SettingsDistanceData GetSettingsDistanceData(PriorityType priorityType)
		{
			for (int i = 0; i < _settingsDistanceDatas.Count; i++)
			{
				if (_settingsDistanceDatas[i].PriorityType == priorityType)
				{
					return _settingsDistanceDatas[i];
				}
			}
			return null;
		}
	}
}

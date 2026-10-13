using System;
using System.Linq;
using CPMEngine;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using _Car_Parking;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class KeepRowComposition
	{
		private const float CheckBufferValue = 5f;

		private const int SplineResolution = 8;

		private const int SplineIteration = 4;

		private const float TargetDistance = 6f;

		private const float SpeedToExtendDistance = 30f;

		private Vector3 _targetPosition;

		private float _currentOffset;

		private float _splineLength;

		private float _checkBuffer = 5f;

		private Vector3[] _rowToFollow;

		private IDisposable _updateRowDisposable;

		private Sort2 _sort2;

		private CarInputManager _carInputManager;

		private SplineContainer _splineContainer;

		private VehicleData _vehicle;

		private float _smallest;

		public Transform FollowOnRacing { get; set; }

		public Vector3[] RacingTargets { get; set; }

		public int CurrentTargetIndex { get; set; }

		public int RaceToAddForFollow { get; set; } = 1;

		private void SetSplineContainer()
		{
			if ((bool)V.GlobalPoint && !V.GlobalPoint.gameObject.GetComponent<SplineContainer>())
			{
				_splineContainer = V.GlobalPoint.gameObject.AddComponent<SplineContainer>();
			}
		}

		public void ReInit(CarInputManager carInputManager, VehicleData vehicle)
		{
			SetSplineContainer();
			_carInputManager = carInputManager;
			_vehicle = vehicle;
			if ((bool)V.SceneControl)
			{
				_sort2 = V.SceneControl.GetSort;
			}
			_updateRowDisposable?.Dispose();
			if ((bool)_vehicle)
			{
				StartUpdateCurrentRow();
			}
			else
			{
				_sort2 = null;
			}
		}

		private void StartUpdateCurrentRow()
		{
			_updateRowDisposable = Observable.EveryUpdate().Subscribe((long _) =>
			{
				UpdateTick();
			}).AddTo(_vehicle.gameObject);
		}

		public void Dispose()
		{
			_updateRowDisposable?.Dispose();
			_updateRowDisposable = null;
		}

		public bool SetClosestRow()
		{
			_rowToFollow = GetClosestRow();
			if (_rowToFollow == null)
			{
				return false;
			}
			UpdateFollowTargetPosition();
			return true;
		}

		private Vector3[] GetClosestRow()
		{
			if (!_sort2)
			{
				return null;
			}
			Vector3[] array = null;
			int rowNumber = PlayerDatas.GetRowNumber(V.CurrentIndexArrayPositionCar);
			Vector3[][] array2;
			if (FlagsStorage.CloserMountain)
			{
				array2 = new Vector3[4][] { _sort2.MountainRoadRoundBigV, _sort2.MountainRoadRoundBigV2, _sort2.MountainRoadRoundBigV3, _sort2.MountainRoadRoundBigV4 };
			}
			else if (FlagsStorage.CloserCity2)
			{
				array2 = new Vector3[6][] { _sort2.row1City2, _sort2.row2City2, _sort2.row3City2, _sort2.row4City2, _sort2.row5City2, _sort2.row6City2 };
			}
			else
			{
				array2 = new Vector3[8][];
				switch (rowNumber)
				{
				case 2:
					array2[0] = null;
					array2[1] = null;
					array2[2] = null;
					array2[3] = _sort2.rowTrafficArrays[3];
					array2[4] = _sort2.rowTrafficArrays[4];
					array2[5] = null;
					array2[6] = null;
					array2[7] = null;
					break;
				case 4:
					array2[0] = null;
					array2[1] = null;
					array2[2] = _sort2.rowTrafficArrays[2];
					array2[3] = _sort2.rowTrafficArrays[3];
					array2[4] = _sort2.rowTrafficArrays[4];
					array2[5] = _sort2.rowTrafficArrays[5];
					array2[6] = null;
					array2[7] = null;
					break;
				case 6:
					array2[0] = null;
					array2[1] = _sort2.rowTrafficArrays[1];
					array2[2] = _sort2.rowTrafficArrays[2];
					array2[3] = _sort2.rowTrafficArrays[3];
					array2[4] = _sort2.rowTrafficArrays[4];
					array2[5] = _sort2.rowTrafficArrays[5];
					array2[6] = _sort2.rowTrafficArrays[6];
					array2[7] = null;
					break;
				case 8:
					array2[0] = _sort2.rowTrafficArrays[0];
					array2[1] = _sort2.rowTrafficArrays[1];
					array2[2] = _sort2.rowTrafficArrays[2];
					array2[3] = _sort2.rowTrafficArrays[3];
					array2[4] = _sort2.rowTrafficArrays[4];
					array2[5] = _sort2.rowTrafficArrays[5];
					array2[6] = _sort2.rowTrafficArrays[6];
					array2[7] = _sort2.rowTrafficArrays[7];
					break;
				}
			}
			int num = 0;
			float[] array3 = (FlagsStorage.CloserMountain ? new float[4] : ((!FlagsStorage.CloserCity2) ? new float[8] : new float[6]));
			Vector3[][] array4 = array2;
			foreach (Vector3[] array5 in array4)
			{
				array3[num] = 100f;
				if (array5 != null)
				{
					float num2 = 100f;
					int num3 = PlayerDatas.getEqualTargetIndex(V.CurrentIndexArrayPositionCar + V.DirectionCar, num);
					if (FlagsStorage.CloserMountain || FlagsStorage.CloserCity2)
					{
						num3 = V.CurrentIndexArrayPositionCar + V.DirectionCar;
					}
					if (array5.Length > num3)
					{
						num2 = Vector3.Distance(array5[num3], _vehicle.transform.localPosition + _vehicle.transform.forward * 2f);
					}
					array3[num] = num2;
				}
				num++;
			}
			int num4 = 0;
			num = 0;
			float num5 = array3[0];
			float[] array6 = array3;
			for (int i = 0; i < array6.Length; i++)
			{
				_ = array6[i];
				if (array3[num] < num5)
				{
					num5 = array3[num];
					num4 = num;
				}
				num++;
			}
			if (array3[num4] < 45f)
			{
				array = array2[num4];
			}
			if (array == null || !_splineContainer)
			{
				return null;
			}
			CreateSpline(array);
			_splineLength = _splineContainer.CalculateLength(0);
			float4 float5 = new float4(0f, 0f, 0f, float.PositiveInfinity);
			using NativeSpline spline = new NativeSpline(_splineContainer.Spline, _splineContainer.transform.localToWorldMatrix);
			float nearestPoint = SplineUtility.GetNearestPoint(spline, _vehicle.transform.TransformPoint(_vehicle.CarFrontPosition), out var nearest, out _currentOffset, 8, 4);
			float num6 = ((_vehicle.GetPowertrain.SpeedMps > 30f) ? 2f : 0f);
			nearest = spline.GetPointAtLinearDistance(_currentOffset, 6f + num6, out var resultPointT);
			_currentOffset = resultPointT;
			if (nearestPoint < float5.w)
			{
				float5 = new float4(nearest, nearestPoint);
			}
			_targetPosition = float5.xyz;
			return array;
		}

		private void CreateSpline(Vector3[] wayPoints)
		{
			_splineContainer.RemoveSplineAt(0);
			if (V.DirectionCar < 0)
			{
				wayPoints = wayPoints.Reverse().ToArray();
			}
			BezierKnot[] array = new BezierKnot[wayPoints.Length];
			_splineContainer.AddSpline();
			for (int i = 0; i < wayPoints.Length; i++)
			{
				array[i].Position = wayPoints[i];
				_splineContainer.Spline.Add(array[i], TangentMode.AutoSmooth);
			}
			Vector3 a = wayPoints[0];
			Vector3[] array2 = wayPoints;
			if (Vector3.Distance(a, array2[array2.Length - 1]) < 100f)
			{
				_splineContainer.Spline.Closed = true;
			}
		}

		private void UpdateTick()
		{
			if (!_carInputManager || (!FlagsStorage.IsFreeDriveMode && _vehicle.LevelsProvider != null && (bool)_vehicle.LevelsProvider.LevelScript && _vehicle.LevelsProvider.LevelScript.LevelType != LevelType.DragRace))
			{
				return;
			}
			if (FlagsStorage.RacingGameMode)
			{
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.FreeDriveProvider.DragRacingControlMultiplayer && (int)V.CoreGameManager.FreeDriveProvider.DragRacingControlMultiplayer.CheckPointOverAll != 0)
				{
					return;
				}
				if (CurrentTargetIndex == 0)
				{
					CurrentTargetIndex = 1;
				}
				int num = 2;
				if (FlagsStorage.SingeDragRacingMode)
				{
					num = 1;
					if (_vehicle.racingState == RacingState.OnRacing)
					{
						float speedMps = _vehicle.GetPowertrain.SpeedMps;
						int num2 = ((speedMps > 40f) ? 8 : ((!(speedMps > 20f)) ? num : 4));
						num = num2;
					}
				}
				else
				{
					float speedMps = _vehicle.GetPowertrain.SpeedMps;
					int num2 = ((speedMps > 60f) ? 4 : ((!(speedMps > 30f)) ? num : 2));
					num = num2;
					if (FlagsStorage.CloserMountain)
					{
						num--;
					}
					if (V.PlaceToSpawn == V.placeToSpawn.Highway)
					{
						num += 2;
					}
				}
				if ((bool)FollowOnRacing && _vehicle.racingState == RacingState.OnRacing)
				{
					_carInputManager.SetFollowPoint(FollowOnRacing.position - V.GlobalOffset);
				}
				else if (RacingTargets != null && RacingTargets.Length >= 3)
				{
					if (CurrentTargetIndex < 1)
					{
						CurrentTargetIndex = 1;
					}
					if (CurrentTargetIndex > RacingTargets.Length - 2)
					{
						CurrentTargetIndex = RacingTargets.Length - 2;
					}
					Vector3 localPosition = _vehicle.transform.localPosition;
					float num3 = Vector2.Distance(new Vector2(localPosition.x, localPosition.z), new Vector2(RacingTargets[CurrentTargetIndex].x, RacingTargets[CurrentTargetIndex].z));
					float num4 = Vector2.Distance(new Vector2(localPosition.x, localPosition.z), new Vector2(RacingTargets[CurrentTargetIndex - 1].x, RacingTargets[CurrentTargetIndex - 1].z));
					float num5 = Vector2.Distance(new Vector2(localPosition.x, localPosition.z), new Vector2(RacingTargets[CurrentTargetIndex + 1].x, RacingTargets[CurrentTargetIndex + 1].z));
					_smallest = Mathf.Min(num3, num4, num5);
					if (Mathf.Abs(_smallest - num4) < 0.01f)
					{
						CurrentTargetIndex--;
					}
					else if (Mathf.Abs(_smallest - num5) < 0.01f && CurrentTargetIndex < RacingTargets.Length - 2)
					{
						CurrentTargetIndex++;
					}
					if (CurrentTargetIndex + RaceToAddForFollow * num < RacingTargets.Length)
					{
						_carInputManager.SetFollowPoint(RacingTargets[CurrentTargetIndex + RaceToAddForFollow * num]);
					}
				}
			}
			else if (FlagsStorage.KeepRow && _rowToFollow != null)
			{
				UpdateFollowTargetPosition();
			}
		}

		private void UpdateFollowTargetPosition()
		{
			float num = _vehicle.GetPowertrain.SpeedMps;
			Vector3 b = _vehicle.transform.TransformPoint(_vehicle.CarFrontPosition);
			float num2 = Vector3.Distance(_targetPosition, b);
			float num3 = ((num > 30f) ? 2f : 0f);
			float num4 = 3f + num3;
			float num5 = 9f + num3;
			if (num2 > num5 || num2 < num4)
			{
				num *= ((num2 > num5) ? 0.8f : 1.2f);
				if (_checkBuffer < 0f)
				{
					_rowToFollow = GetClosestRow();
					_checkBuffer = 5f;
				}
				else
				{
					_checkBuffer -= Time.deltaTime;
				}
			}
			else
			{
				_checkBuffer = 5f;
			}
			_currentOffset = (_currentOffset + num * Time.deltaTime / _splineLength) % 1f;
			if ((double)_currentOffset >= 0.999)
			{
				Vector3 vector = _splineContainer.EvaluatePosition(0, 0f);
				float magnitude = (_targetPosition - vector).magnitude;
				_currentOffset = 0f;
				if (magnitude > 100f)
				{
					_carInputManager.QuickCarSettings.DisableKeepLane(message: true);
					return;
				}
			}
			_targetPosition = _splineContainer.EvaluatePosition(0, _currentOffset);
			_carInputManager.SetFollowPoint(_targetPosition);
		}
	}
}

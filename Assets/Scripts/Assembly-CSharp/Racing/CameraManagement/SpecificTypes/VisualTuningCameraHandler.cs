using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace Racing.CameraManagement.SpecificTypes
{
	public class VisualTuningCameraHandler : FreeLookCameraHandler
	{
		[Serializable]
		private struct VisualTuningCamPreset
		{
			public VisualTuningCamMode CamMode;

			public float XAxisValue;

			public float XAxisMin;

			public float XAxisMax;

			public float YAxisValue;

			public float FOV;

			public float TopRigHeight;

			public float MiddleRigRadius;

			public float BottomRigHeight;

			public float MidScreenXOffset;

			public float MidScreenYOffset;
		}

		[SerializeField]
		private VisualTuningCamPreset[] CamPresets = new VisualTuningCamPreset[0];

		private Dictionary<VisualTuningCamMode, VisualTuningCamPreset> _camPresets;

		private VisualTuningCamMode _prevCamMode;

		private CinemachineComposer _composer;

		private Tweener _fovTween;

		private Tweener _xAxisTween;

		private Tweener _yAxisTween;

		private Sequence _mySequence;

		protected override void Awake()
		{
			base.Awake();
			_composer = _camera.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
			_camPresets = new Dictionary<VisualTuningCamMode, VisualTuningCamPreset>();
			VisualTuningCamPreset[] camPresets = CamPresets;
			for (int i = 0; i < camPresets.Length; i++)
			{
				VisualTuningCamPreset value = camPresets[i];
				_camPresets.Add(value.CamMode, value);
			}
		}

		[ContextMenu("Debug/Rotate Full Circle")]
		private void DebugRotateFullCircleMenu()
		{
			DebugRotateFullCircle();
		}

		[ContextMenu("Debug/Rotate Top To Bottom")]
		private void DebugRotateTopToBottomMenu()
		{
			DebugRotateTopToBottom();
		}

		public override void SetActive(bool isActive, Transform target, Action<CinemachineVirtualCameraBase> onLiveCallback = null)
		{
			base.SetActive(isActive, target, onLiveCallback);
			if (!isActive)
			{
				_prevCamMode = VisualTuningCamMode.None;
			}
		}

		public override void SetRecenter(bool isActive)
		{
		}

		public void SetCamMode(BodyKitType bodyKitType, bool ignoreTween = false, bool limiting = true)
		{
			SetCamMode(bodyKitType switch
			{
				BodyKitType.Spoiler => VisualTuningCamMode.Spoiler, 
				BodyKitType.FrontBumper => VisualTuningCamMode.FrontBumper, 
				BodyKitType.RearBumper => VisualTuningCamMode.RearBumper, 
				BodyKitType.Skirt => VisualTuningCamMode.Skirt, 
				BodyKitType.Hood => VisualTuningCamMode.Hood, 
				BodyKitType.AirIntakeHood => VisualTuningCamMode.AirIntakeHood, 
				BodyKitType.AirIntakeRoof => VisualTuningCamMode.AirIntakeRoof, 
				BodyKitType.Fender => VisualTuningCamMode.Fender, 
				_ => VisualTuningCamMode.None, 
			}, ignoreTween, limiting);
		}

		public void SetCamMode(VisualTuningCamMode camMode, bool ignoreTween = false, bool limiting = true)
		{
			if (!_camPresets.TryGetValue(camMode, out var preset))
			{
				return;
			}
			_camera.m_XAxis.m_MinValue = -180f;
			_camera.m_XAxis.m_MaxValue = 180f;
			_camera.m_XAxis.m_Wrap = true;
			_composer.m_ScreenX = preset.MidScreenXOffset;
			_composer.m_ScreenY = preset.MidScreenYOffset;
			_mySequence?.Kill();
			_mySequence = DOTween.Sequence();
			_mySequence.Insert(0f, DOTween.To(() => _camera.m_Lens.FieldOfView, (float x) =>
			{
				_camera.m_Lens.FieldOfView = x;
			}, preset.FOV, 0.5f)).Insert(0f, DOTween.To(() => _camera.m_Orbits[0].m_Height, (float x) =>
			{
				_camera.m_Orbits[0].m_Height = x;
			}, preset.TopRigHeight, 0.5f)).Insert(0f, DOTween.To(() => _camera.m_Orbits[1].m_Radius, (float x) =>
			{
				_camera.m_Orbits[1].m_Radius = x;
			}, preset.MiddleRigRadius, 0.5f))
				.Insert(0f, DOTween.To(() => _camera.m_Orbits[2].m_Height, (float x) =>
				{
					_camera.m_Orbits[2].m_Height = x;
				}, preset.BottomRigHeight, 0.5f));
			if (_prevCamMode != camMode)
			{
				if (ignoreTween)
				{
					base.XAxisValue = preset.XAxisValue;
					base.YAxisValue = preset.YAxisValue;
				}
				else
				{
					_mySequence.Insert(0f, DOTween.To(() => _camera.m_XAxis.Value, (float x) =>
					{
						_camera.m_XAxis.Value = x;
					}, preset.XAxisValue, 0.5f)).Insert(0f, DOTween.To(() => _camera.m_YAxis.Value, (float y) =>
					{
						_camera.m_YAxis.Value = y;
					}, preset.YAxisValue, 0.5f));
				}
			}
			_mySequence.OnComplete(() =>
			{
				if (limiting)
				{
					_camera.m_XAxis.m_MinValue = preset.XAxisMin;
					_camera.m_XAxis.m_MaxValue = preset.XAxisMax;
					_camera.m_XAxis.m_Wrap = Math.Abs(preset.XAxisMax - 180f) < 0.1f;
				}
			});
			_mySequence.Play();
			_prevCamMode = camMode;
		}
	}
}

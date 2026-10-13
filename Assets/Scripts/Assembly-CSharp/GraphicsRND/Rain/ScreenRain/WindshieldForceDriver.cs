using UnityEngine;
using UnityEngine.VFX;

namespace GraphicsRND.Rain.ScreenRain
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	public sealed class WindshieldForceDriver : MonoBehaviour
	{
		[Header("References")]
		[Tooltip("The VFX that owns the ForceByMove property.")]
		public VisualEffect screenRainVFX;

		[Tooltip("Transform whose movement is tracked (typically the main camera or car root).")]
		public Transform trackedObject;

		[Tooltip("Camera used to resolve world velocity into screen axes. Leave empty to use the tracked object's own axes.")]
		public Camera screenCamera;

		[Header("Force Mapping")]
		[Tooltip("Multiplier applied to the raw velocity before writing to ForceByMove.")]
		[Min(0f)]
		public float forceScale = 1f;

		[Min(0f)]
		[Tooltip("Clamps the final ForceByMove magnitude to this value.")]
		public float maxForce = 10f;

		[Min(0.01f)]
		[Tooltip("Smooth-damp time for the output force (seconds). Lower = snappier.")]
		public float smoothTime = 0.12f;

		[Tooltip("Velocity magnitude below which ForceByMove is zeroed out.")]
		[Min(0f)]
		public float deadZoneSpeed = 0.05f;

		[Header("Axis Flip")]
		public bool flipX;

		public bool flipY;

		private static readonly int _propForceByMove = Shader.PropertyToID("ForceByMove");

		private Vector3 _prevWorldPos;

		private Vector2 _currentForce;

		private Vector2 _forceVel;

		private bool _prevPosValid;

		private bool _vfxValid;

		private void OnEnable()
		{
			_prevPosValid = false;
			_currentForce = Vector2.zero;
			_forceVel = Vector2.zero;
			_vfxValid = screenRainVFX != null && screenRainVFX.HasVector2(_propForceByMove);
		}

		private void OnValidate()
		{
			_vfxValid = screenRainVFX != null && screenRainVFX.HasVector2(_propForceByMove);
		}

		private void LateUpdate()
		{
			if (!_vfxValid || trackedObject == null)
			{
				return;
			}
			float num = (Application.isPlaying ? Time.deltaTime : EditorDelta());
			if (num <= 0f)
			{
				return;
			}
			Vector3 position = trackedObject.position;
			if (!_prevPosValid)
			{
				_prevWorldPos = position;
				_prevPosValid = true;
				return;
			}
			Vector3 lhs = (position - _prevWorldPos) / num;
			_prevWorldPos = position;
			Transform transform = ((screenCamera != null) ? screenCamera.transform : trackedObject);
			float num2 = Vector3.Dot(lhs, transform.right);
			float y = Vector3.Dot(lhs, transform.forward);
			Vector2 vector = new Vector2(0f - num2, y) * forceScale;
			if (flipX)
			{
				vector.x = 0f - vector.x;
			}
			if (flipY)
			{
				vector.y = 0f - vector.y;
			}
			float num3 = deadZoneSpeed * forceScale;
			if (vector.sqrMagnitude < num3 * num3)
			{
				vector = Vector2.zero;
			}
			vector = Vector2.ClampMagnitude(vector, maxForce);
			_currentForce.x = Mathf.SmoothDamp(_currentForce.x, vector.x, ref _forceVel.x, smoothTime, float.PositiveInfinity, num);
			_currentForce.y = Mathf.SmoothDamp(_currentForce.y, vector.y, ref _forceVel.y, smoothTime, float.PositiveInfinity, num);
			screenRainVFX.SetVector2(_propForceByMove, _currentForce);
		}

		private float EditorDelta()
		{
			return 0f;
		}
	}
}

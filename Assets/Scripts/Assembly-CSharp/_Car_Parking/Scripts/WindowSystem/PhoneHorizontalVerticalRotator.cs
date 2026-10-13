using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhoneHorizontalVerticalRotator : MonoBehaviour
	{
		public enum Rotation
		{
			Vertical = 0,
			Right = 90,
			Upside = 180,
			Left = -90
		}

		private static readonly Vector3[] Corners = new Vector3[4];

		[SerializeField]
		private RectTransform Root;

		[SerializeField]
		private RectTransform Target;

		[SerializeField]
		[FormerlySerializedAs("HorizontalRotation")]
		private Rotation TargetRotation = Rotation.Left;

		[SerializeField]
		private float Duration = 0.35f;

		[SerializeField]
		private Ease EaseType = Ease.InOutSine;

		[SerializeField]
		[Tooltip("Faded to zero alpha along with the rotation.")]
		[Header("Elements")]
		private CanvasGroup[] FadeOutGroups;

		[Tooltip("Disabled as soon as the rotation starts.")]
		[SerializeField]
		private GameObject[] HideOnStart;

		[Tooltip("Enabled as soon as the rotation starts: input blockers and horizontal-only content.")]
		[SerializeField]
		private GameObject[] ShowOnStart;

		private RectTransform _parent;

		private AspectRatioFitter _aspectRatioFitter;

		private float[] _startAlphas;

		private float _currentAngle;

		private float _startAngle;

		private float _endAngle;

		private Vector2 _startCenter;

		private Vector2 _endCenter;

		private Vector2 _startSize;

		private Vector2 _endSize;

		private bool _isRotating;

		public bool IsRotating => _isRotating;

		private void Awake()
		{
			if ((bool)Root)
			{
				_currentAngle = Root.localEulerAngles.z;
			}
		}

		public UniTask RotateAsync(CancellationToken token)
		{
			return RotateAsync(TargetRotation, token);
		}

		public async UniTask RotateAsync(Rotation rotation, CancellationToken token)
		{
			if (!Root || !Target || _isRotating)
			{
				return;
			}
			_isRotating = true;
			try
			{
				Prepare((float)rotation);
				float time = 0f;
				while (time < Duration)
				{
					time += Time.unscaledDeltaTime;
					Apply(DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(time / Duration), EaseType));
					await UniTask.Yield(PlayerLoopTiming.Update, token);
				}
				Apply(1f);
			}
			finally
			{
				_isRotating = false;
			}
		}

		private void Prepare(float angle)
		{
			_parent = Root.parent as RectTransform;
			Canvas.ForceUpdateCanvases();
			if (!_aspectRatioFitter)
			{
				_aspectRatioFitter = Root.GetComponent<AspectRatioFitter>();
			}
			if ((bool)_aspectRatioFitter)
			{
				_aspectRatioFitter.enabled = false;
			}
			CacheStartAlphas();
			SetActive(HideOnStart, state: false);
			SetActive(ShowOnStart, state: true);
			_startAngle = _currentAngle;
			_startSize = Root.rect.size;
			_startCenter = ToAnchorSpace(GetWorldCenter(Root));
			CollapseAnchorsToCenter();
			Vector2 sizeInParentSpace = GetSizeInParentSpace(Target);
			_endAngle = _startAngle + Mathf.DeltaAngle(_startAngle, angle);
			_endCenter = ToAnchorSpace(GetWorldCenter(Target));
			_endSize = (IsQuarterTurn(angle) ? new Vector2(sizeInParentSpace.y, sizeInParentSpace.x) : sizeInParentSpace);
			_currentAngle = _endAngle;
		}

		private void Apply(float t)
		{
			float num = Mathf.Lerp(_startAngle, _endAngle, t);
			Vector2 vector = Vector2.Lerp(_startSize, _endSize, t);
			Vector2 vector2 = Vector2.Lerp(_startCenter, _endCenter, t);
			Root.localRotation = Quaternion.Euler(0f, 0f, num);
			Root.sizeDelta = vector;
			Root.anchoredPosition = vector2 - PivotToCenterOffset(num, vector);
			ApplyFade(t);
		}

		private void CacheStartAlphas()
		{
			if (FadeOutGroups != null)
			{
				if (_startAlphas == null || _startAlphas.Length != FadeOutGroups.Length)
				{
					_startAlphas = new float[FadeOutGroups.Length];
				}
				for (int i = 0; i < FadeOutGroups.Length; i++)
				{
					_startAlphas[i] = (FadeOutGroups[i] ? FadeOutGroups[i].alpha : 0f);
				}
			}
		}

		private void ApplyFade(float t)
		{
			if (FadeOutGroups == null)
			{
				return;
			}
			for (int i = 0; i < FadeOutGroups.Length; i++)
			{
				if ((bool)FadeOutGroups[i])
				{
					FadeOutGroups[i].alpha = Mathf.Lerp(_startAlphas[i], 0f, t);
				}
			}
		}

		private static void SetActive(GameObject[] objects, bool state)
		{
			if (objects == null)
			{
				return;
			}
			foreach (GameObject gameObject in objects)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(state);
				}
			}
		}

		private void CollapseAnchorsToCenter()
		{
			Vector2 size = Root.rect.size;
			Vector3 position = Root.position;
			Root.anchorMin = new Vector2(0.5f, 0.5f);
			Root.anchorMax = new Vector2(0.5f, 0.5f);
			Root.sizeDelta = size;
			Root.position = position;
		}

		private Vector2 PivotToCenterOffset(float angle, Vector2 size)
		{
			Vector3 vector = new Vector3((0.5f - Root.pivot.x) * size.x * Root.localScale.x, (0.5f - Root.pivot.y) * size.y * Root.localScale.y, 0f);
			Vector3 vector2 = Quaternion.Euler(0f, 0f, angle) * vector;
			return new Vector2(vector2.x, vector2.y);
		}

		private Vector2 ToAnchorSpace(Vector3 worldPoint)
		{
			Vector3 vector = _parent.InverseTransformPoint(worldPoint);
			return new Vector2(vector.x, vector.y) - _parent.rect.center;
		}

		private Vector2 GetSizeInParentSpace(RectTransform rectTransform)
		{
			rectTransform.GetWorldCorners(Corners);
			Vector3 vector = _parent.InverseTransformPoint(Corners[0]);
			Vector3 vector2 = _parent.InverseTransformPoint(Corners[2]);
			return new Vector2(Mathf.Abs(vector2.x - vector.x), Mathf.Abs(vector2.y - vector.y));
		}

		private static Vector3 GetWorldCenter(RectTransform rectTransform)
		{
			rectTransform.GetWorldCorners(Corners);
			return (Corners[0] + Corners[2]) * 0.5f;
		}

		private static bool IsQuarterTurn(float angle)
		{
			return Mathf.Abs(Mathf.RoundToInt(angle / 90f)) % 2 == 1;
		}
	}
}

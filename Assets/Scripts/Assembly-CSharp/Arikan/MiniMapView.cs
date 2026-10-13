using System;
using System.Collections.Generic;
using System.Linq;
using Car_Parking.Scripts.Racing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace Arikan
{
	public class MiniMapView : MonoBehaviour
	{
		[Header("RectTransform Roots")]
		public RectTransform centeredDotCanvas;

		public RectTransform otherDotCanvas;

		[Header("Defult Sprite")]
		public Sprite defaultSprite;

		[Header("Default Dot Prefab")]
		public Image uiDotPrefab;

		[Header("ZoomReferences")]
		[SerializeField]
		private RectMask2D MapMask;

		[SerializeField]
		private float UnZoomScale = 0.5f;

		[SerializeField]
		private GameObject MapParent;

		[SerializeField]
		private Transform ShowTransform;

		[SerializeField]
		private Transform HideTransform;

		private bool _hideBigMap = true;

		private Tween _tween;

		[Header("Speed Scale")]
		[SerializeField]
		private float scaleAtMinSpeed = 1f;

		[SerializeField]
		private float scaleAtMaxSpeed = 0.6f;

		[SerializeField]
		private float maxSpeedKmh = 200f;

		[SerializeField]
		private float scaleSmoothTime = 0.5f;

		private Rigidbody _trackedBody;

		private float _currentScale = 1f;

		private float _scaleVelocity;

		private Dictionary<Transform, RectTransform> redDotMap = new Dictionary<Transform, RectTransform>();

		private KeyValuePair<Transform, RectTransform> mainMap;

		private KeyValuePair<Transform, RectTransform> _cachedCenteredMap;

		private MiniMapBounds _miniMapBounds;

		private Bounds _worldBounds;

		private Vector3 _sizeDif;

		private FreeTimeAttackController _controller;

		[Inject]
		public void Construct(OptionalDependency<MiniMapBounds> miniMapBounds)
		{
			_miniMapBounds = miniMapBounds.Value;
		}

		public void SetMiniMapBounds(MiniMapBounds miniMapBounds)
		{
			_miniMapBounds = miniMapBounds;
		}

		public Image FollowCentered(Transform target, Sprite icon = null, bool needCacheTransform = false)
		{
			if (centeredDotCanvas == null)
			{
				throw new NullReferenceException("[MiniMapView] centeredDotCanvas is null");
			}
			if (uiDotPrefab == null)
			{
				throw new NullReferenceException("[MiniMapView] uiDotPrefab is null");
			}
			if (target.lossyScale.x != 1f)
			{
				Debug.LogWarning("[MiniMapView] target.lossyScale != 1, this causes wrong positions over minimap", target);
			}
			if (mainMap.Key != null)
			{
				UnfollowTarget(mainMap.Key);
			}
			Image image = UnityEngine.Object.Instantiate(uiDotPrefab, centeredDotCanvas);
			image.sprite = icon ?? defaultSprite;
			mainMap = new KeyValuePair<Transform, RectTransform>(target, image.transform as RectTransform);
			_trackedBody = target.GetComponentInParent<Rigidbody>();
			if (_trackedBody == null)
			{
				_trackedBody = target.GetComponentInChildren<Rigidbody>();
			}
			if (needCacheTransform)
			{
				_cachedCenteredMap = new KeyValuePair<Transform, RectTransform>(target, image.transform as RectTransform);
			}
			return image;
		}

		public void SetTrackedVehicle(Rigidbody rb)
		{
			_trackedBody = rb;
		}

		public Image Follow(Transform target, Sprite icon = null)
		{
			if (otherDotCanvas == null)
			{
				throw new NullReferenceException("[MiniMapView] otherDotCanvas is null");
			}
			if (uiDotPrefab == null)
			{
				throw new NullReferenceException("[MiniMapView] uiDotPrefab is null");
			}
			UnfollowTarget(target);
			Image image = UnityEngine.Object.Instantiate(uiDotPrefab, otherDotCanvas);
			image.sprite = icon ?? defaultSprite;
			redDotMap[target] = image.transform as RectTransform;
			return image;
		}

		public void UnfollowTarget(Transform target)
		{
			RectTransform value;
			if (mainMap.Key == target)
			{
				if (mainMap.Value != null)
				{
					UnityEngine.Object.Destroy(mainMap.Value.gameObject);
				}
				mainMap = default;
			}
			else if (redDotMap.TryGetValue(target, out value))
			{
				if (value != null)
				{
					UnityEngine.Object.Destroy(value.gameObject);
				}
				redDotMap.Remove(target);
			}
		}

		public void ClearTargets()
		{
			if (mainMap.Key != null)
			{
				UnfollowTarget(mainMap.Key);
			}
			foreach (KeyValuePair<Transform, RectTransform> item in redDotMap.ToList())
			{
				UnfollowTarget(item.Key);
			}
		}

		private void LateUpdate()
		{
			if (!_miniMapBounds)
			{
				return;
			}
			if (_trackedBody != null && _hideBigMap)
			{
				float t = Mathf.Clamp01(_trackedBody.velocity.magnitude * 3.6f / maxSpeedKmh);
				float target = Mathf.Lerp(scaleAtMinSpeed, scaleAtMaxSpeed, t);
				_currentScale = Mathf.SmoothDamp(_currentScale, target, ref _scaleVelocity, scaleSmoothTime);
				base.transform.localScale = Vector3.one * _currentScale;
			}
			_worldBounds = _miniMapBounds.GetWorldRect();
			_sizeDif = new Vector3(otherDotCanvas.sizeDelta.x / _worldBounds.size.x, 1f, otherDotCanvas.sizeDelta.y / _worldBounds.size.z);
			if (mainMap.Key != null)
			{
				Transform key = mainMap.Key;
				RectTransform value = mainMap.Value;
				TranslateReverse(key, value);
			}
			foreach (KeyValuePair<Transform, RectTransform> item in redDotMap)
			{
				Transform key2 = item.Key;
				RectTransform value2 = item.Value;
				if (key2 != null)
				{
					Translate(key2, value2);
				}
			}
		}

		public void RemoveNullTargets()
		{
			redDotMap = redDotMap.Where((KeyValuePair<Transform, RectTransform> kv) => kv.Key != null).ToDictionary((KeyValuePair<Transform, RectTransform> kv) => kv.Key, (KeyValuePair<Transform, RectTransform> kv) => kv.Value);
		}

		public void Translate(Transform worldObj, RectTransform dot)
		{
			Matrix4x4 m = Matrix4x4.TRS(-_worldBounds.center, Quaternion.identity, Vector3.one) * worldObj.localToWorldMatrix;
			dot.localPosition = Vector3.Scale(_sizeDif, m.GetPosition()).XZ();
			dot.localEulerAngles = new Vector3(0f, 0f, 0f - m.GetRotation().eulerAngles.y);
		}

		public void TranslateReverse(Transform worldObj, RectTransform dot)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(_worldBounds.center, Quaternion.identity, Vector3.one);
			Matrix4x4 m = worldObj.worldToLocalMatrix * matrix4x;
			otherDotCanvas.localPosition = Vector3.Scale(_sizeDif, m.GetPosition()).XZ();
			otherDotCanvas.localEulerAngles = new Vector3(0f, 0f, 0f - m.GetRotation().eulerAngles.y);
		}

		private void OnDrawGizmosSelected()
		{
			if (_miniMapBounds != null)
			{
				Bounds worldRect = _miniMapBounds.GetWorldRect();
				Gizmos.DrawWireCube(worldRect.center, worldRect.size);
			}
		}

		public void DestroyMapItem(GameObject item)
		{
			UnityEngine.Object.Destroy(item);
		}

		public void ZoomMap()
		{
			_tween?.Kill();
			_hideBigMap = !_hideBigMap;
			Sequence sequence = DOTween.Sequence().SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
			MapMask.enabled = _hideBigMap;
			sequence.Append(MapParent.transform.DOScale(_hideBigMap ? 1f : UnZoomScale, 0.2f));
			sequence.Append(MapParent.transform.DOMove(_hideBigMap ? HideTransform.position : ShowTransform.position, 0.2f));
			_tween = sequence;
			if (_hideBigMap)
			{
				redDotMap.Remove(_cachedCenteredMap.Key);
				mainMap = _cachedCenteredMap;
				mainMap.Value.transform.SetParent(centeredDotCanvas);
				mainMap.Value.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				base.transform.localScale = Vector3.one;
			}
			else
			{
				mainMap = default;
				redDotMap.Add(_cachedCenteredMap.Key, _cachedCenteredMap.Value);
				_cachedCenteredMap.Value.transform.SetParent(otherDotCanvas);
				base.transform.localScale = new Vector3(scaleAtMaxSpeed, scaleAtMaxSpeed, scaleAtMaxSpeed);
			}
		}
	}
}

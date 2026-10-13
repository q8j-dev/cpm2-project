using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.CommonUtility.ScriptableObjects;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	public class SteamUIConfigApplier : MonoBehaviour
	{
		[SerializeField]
		private GameObject _root = default;

		[SerializeField]
		private Transform[] _hideableObjects;

		[SerializeField]
		private UITarget[] _movableTargets;

		[SerializeField]
		private SteamUILayoutConfig _config;

		[Tooltip("don't change field names, the editor script relies on them")]
		[SerializeField]
		private string _defaultConfigName = "NewSteamUILayoutConfig";

		[SerializeField]
		private bool _isConfigApplied;

		private Dictionary<string, Transform> _childLookup;

		private Dictionary<Transform, string> _pathByTransform;

		private Dictionary<string, Transform> _cachedHideableParents;

		private Dictionary<string, UIItemValues> _cachedMovableDefaults;

		private InputDeviceDetector _detector;

		private void Awake()
		{
			BuildChildLookup();
		}

		private void OnEnable()
		{
			_detector = InputDeviceDetector.Instance;
			if (_detector != null)
			{
				_detector.OnDeviceTypeChanged += OnMouseGamepadSwitch;
			}
			ScreenResolutionEvents.OnResolutionApplied += OnResolutionApplied;
		}

		private void OnDisable()
		{
			if (_detector != null)
			{
				_detector.OnDeviceTypeChanged -= OnMouseGamepadSwitch;
			}
			ScreenResolutionEvents.OnResolutionApplied -= OnResolutionApplied;
		}

		private void OnResolutionApplied()
		{
			if (_isConfigApplied && _config != null && _childLookup != null)
			{
				ApplyMovables();
			}
		}

		private void OnMouseGamepadSwitch(InputDeviceType inputDeviceType)
		{
			if (inputDeviceType == InputDeviceType.KeyboardMouse)
			{
				RestoreDefaults();
			}
			else
			{
				ApplyCurrentConfig();
			}
		}

		private void BuildChildLookup()
		{
			if (!_root)
			{
				return;
			}
			Transform[] componentsInChildren = _root.GetComponentsInChildren<Transform>(includeInactive: true);
			_childLookup = new Dictionary<string, Transform>(componentsInChildren.Length);
			_pathByTransform = new Dictionary<Transform, string>(componentsInChildren.Length);
			Transform transform = _root.transform;
			Transform[] array = componentsInChildren;
			foreach (Transform transform2 in array)
			{
				if (!(transform2 == transform))
				{
					string pathFromRoot = GetPathFromRoot(transform, transform2);
					_childLookup[pathFromRoot] = transform2;
					_pathByTransform[transform2] = pathFromRoot;
				}
			}
		}

		public static string GetPathFromRoot(Transform root, Transform child)
		{
			if (!child || child == root)
			{
				return "";
			}
			List<string> list = new List<string>();
			Transform transform = child;
			while ((bool)transform && transform != root)
			{
				list.Add(transform.name);
				transform = transform.parent;
			}
			list.Reverse();
			return string.Join("/", list);
		}

		public void EnsureChildLookup()
		{
			if (_childLookup == null)
			{
				BuildChildLookup();
			}
		}

		public string GetOriginalPath(Transform obj)
		{
			EnsureChildLookup();
			if (_pathByTransform != null && _pathByTransform.TryGetValue(obj, out var value))
			{
				return value;
			}
			return null;
		}

		public void PopulateArraysFromConfig()
		{
			if (!_config || !_root)
			{
				return;
			}
			EnsureChildLookup();
			if (_childLookup == null)
			{
				return;
			}
			if (_config.hideables != null)
			{
				List<Transform> list = new List<Transform>();
				string[] hideables = _config.hideables;
				foreach (string key in hideables)
				{
					if (_childLookup.TryGetValue(key, out var value))
					{
						list.Add(value);
					}
				}
				_hideableObjects = list.ToArray();
			}
			if (_config.movables == null)
			{
				return;
			}
			List<UITarget> list2 = new List<UITarget>();
			UIItemValues[] movables = _config.movables;
			foreach (UIItemValues uIItemValues in movables)
			{
				if (_childLookup.TryGetValue(uIItemValues.name, out var value2) && value2 is RectTransform target)
				{
					list2.Add(new UITarget
					{
						name = uIItemValues.name,
						target = target
					});
				}
			}
			_movableTargets = list2.ToArray();
		}

		public void ApplyDefaultValues()
		{
			RestoreDefaults();
		}

		private void ApplyCurrentConfig()
		{
			if (!_isConfigApplied && (bool)_config && _childLookup != null)
			{
				CacheDefaults();
				ApplyHideables();
				ApplyMovables();
				_isConfigApplied = true;
			}
		}

		public void ApplyConfigChanges(SteamUILayoutConfig configToApply = null)
		{
			if ((bool)configToApply && _config != configToApply)
			{
				RestoreDefaults();
				_config = configToApply;
			}
			if (!_isConfigApplied)
			{
				EnsureChildLookup();
				if ((bool)_config && _childLookup != null && _detector.CurrentDeviceType != InputDeviceType.KeyboardMouse)
				{
					CacheDefaults();
					ApplyHideables();
					ApplyMovables();
					_isConfigApplied = true;
				}
			}
		}

		private void CacheDefaults()
		{
			_cachedHideableParents = new Dictionary<string, Transform>();
			_cachedMovableDefaults = new Dictionary<string, UIItemValues>();
			if (_config.hideables != null)
			{
				string[] hideables = _config.hideables;
				foreach (string key in hideables)
				{
					if (_childLookup.TryGetValue(key, out var value))
					{
						_cachedHideableParents[key] = value.parent;
					}
				}
			}
			if (_config.movables == null)
			{
				return;
			}
			UIItemValues[] movables = _config.movables;
			foreach (UIItemValues uIItemValues in movables)
			{
				if (_childLookup.TryGetValue(uIItemValues.name, out var value2))
				{
					RectTransform rectTransform = value2 as RectTransform;
					if ((bool)rectTransform)
					{
						_cachedMovableDefaults[uIItemValues.name] = new UIItemValues
						{
							name = uIItemValues.name,
							anchoredPosition = rectTransform.anchoredPosition,
							sizeDelta = rectTransform.sizeDelta,
							anchorMin = rectTransform.anchorMin,
							anchorMax = rectTransform.anchorMax,
							pivot = rectTransform.pivot,
							rotation = rectTransform.localRotation.eulerAngles,
							scale = rectTransform.localScale
						};
					}
				}
			}
		}

		private void ApplyHideables()
		{
			if (_config.hideables == null)
			{
				return;
			}
			string[] hideables = _config.hideables;
			foreach (string key in hideables)
			{
				if (_childLookup.TryGetValue(key, out var value))
				{
					value.SetParent(base.transform, worldPositionStays: false);
				}
			}
		}

		private void ApplyMovables()
		{
			if (_config.movables == null)
			{
				return;
			}
			bool flag = IsWideAspect(_config.wideAspectThreshold);
			UIItemValues[] movables = _config.movables;
			foreach (UIItemValues uIItemValues in movables)
			{
				if (!_childLookup.TryGetValue(uIItemValues.name, out var value))
				{
					continue;
				}
				RectTransform rectTransform = value as RectTransform;
				if (!rectTransform)
				{
					continue;
				}
				if (flag && uIItemValues.useWideAspectOverride)
				{
					rectTransform.anchorMin = uIItemValues.wideAnchorMin;
					rectTransform.anchorMax = uIItemValues.wideAnchorMax;
					rectTransform.pivot = uIItemValues.widePivot;
					if (!uIItemValues.ignorePosition)
					{
						rectTransform.anchoredPosition = uIItemValues.wideAnchoredPosition;
					}
					rectTransform.sizeDelta = uIItemValues.wideSizeDelta;
					if (!uIItemValues.ignoreScale)
					{
						rectTransform.localScale = uIItemValues.wideScale;
					}
					rectTransform.localRotation = Quaternion.Euler(uIItemValues.wideRotation);
				}
				else
				{
					rectTransform.anchorMin = uIItemValues.anchorMin;
					rectTransform.anchorMax = uIItemValues.anchorMax;
					rectTransform.pivot = uIItemValues.pivot;
					if (!uIItemValues.ignorePosition)
					{
						rectTransform.anchoredPosition = uIItemValues.anchoredPosition;
					}
					rectTransform.sizeDelta = uIItemValues.sizeDelta;
					if (!uIItemValues.ignoreScale)
					{
						rectTransform.localScale = uIItemValues.scale;
					}
					rectTransform.localRotation = Quaternion.Euler(uIItemValues.rotation);
				}
			}
		}

		private static bool IsWideAspect(float threshold)
		{
			if (threshold <= 0f)
			{
				return false;
			}
			if (Screen.height <= 0)
			{
				return false;
			}
			return (float)Screen.width / (float)Screen.height >= threshold;
		}

		public void RestoreDefaults()
		{
			if (!_isConfigApplied)
			{
				return;
			}
			if (_cachedHideableParents != null)
			{
				foreach (KeyValuePair<string, Transform> cachedHideableParent in _cachedHideableParents)
				{
					if (_childLookup.TryGetValue(cachedHideableParent.Key, out var value) && (bool)cachedHideableParent.Value)
					{
						value.SetParent(cachedHideableParent.Value, worldPositionStays: false);
					}
				}
			}
			if (_cachedMovableDefaults != null)
			{
				foreach (KeyValuePair<string, UIItemValues> cachedMovableDefault in _cachedMovableDefaults)
				{
					if (_childLookup.TryGetValue(cachedMovableDefault.Key, out var value2))
					{
						RectTransform rectTransform = value2 as RectTransform;
						if ((bool)rectTransform)
						{
							UIItemValues value3 = cachedMovableDefault.Value;
							rectTransform.anchorMin = value3.anchorMin;
							rectTransform.anchorMax = value3.anchorMax;
							rectTransform.pivot = value3.pivot;
							rectTransform.anchoredPosition = value3.anchoredPosition;
							rectTransform.sizeDelta = value3.sizeDelta;
							rectTransform.localScale = value3.scale;
							rectTransform.localRotation = Quaternion.Euler(value3.rotation);
						}
					}
				}
			}
			_cachedHideableParents = null;
			_cachedMovableDefaults = null;
			_isConfigApplied = false;
		}
	}
}

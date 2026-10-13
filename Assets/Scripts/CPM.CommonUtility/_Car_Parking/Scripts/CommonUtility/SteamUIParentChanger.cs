using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.CommonUtility.ScriptableObjects;

namespace _Car_Parking.Scripts.CommonUtility
{
	public class SteamUIParentChanger : MonoBehaviour
	{
		[SerializeField]
		private Transform[] MovableObjects;

		[SerializeField]
		private bool _move = true;

		[SerializeField]
		private UITarget[] _targets;

		[SerializeField]
		private SteamUISceneConfig _config;

		public static event Action OnChangesApplied;

		public void ApplyAllChanges()
		{
			if (_move)
			{
				Transform[] movableObjects = MovableObjects;
				foreach (Transform obj in movableObjects)
				{
					obj.SetParent(base.transform);
					DisableHudSlotFollowers(obj);
				}
			}
			if (_targets != null)
			{
				UITarget[] targets = _targets;
				for (int i = 0; i < targets.Length; i++)
				{
					DisableHudSlotFollowers(targets[i].target);
				}
			}
			Apply();
			OnChangesApplied?.Invoke();
		}

		private void Apply()
		{
			if (!_config || _targets == null)
			{
				return;
			}
			bool flag = IsWideAspect(_config.wideAspectThreshold);
			UIItemValues[] items = _config.items;
			foreach (UIItemValues uIItemValues in items)
			{
				RectTransform rectTransform = FindTarget(uIItemValues.name);
				if (!rectTransform)
				{
					continue;
				}
				if (uIItemValues.disableRaycastTargets)
				{
					Graphic[] componentsInChildren = rectTransform.GetComponentsInChildren<Graphic>(includeInactive: true);
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].raycastTarget = false;
					}
				}
				if (flag && uIItemValues.useWideAspectOverride)
				{
					rectTransform.anchorMin = uIItemValues.wideAnchorMin;
					rectTransform.anchorMax = (uIItemValues.forceCornerAnchors ? uIItemValues.wideAnchorMin : uIItemValues.wideAnchorMax);
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
					rectTransform.anchorMax = (uIItemValues.forceCornerAnchors ? uIItemValues.anchorMin : uIItemValues.anchorMax);
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

		private static void DisableHudSlotFollowers(Component root)
		{
			if (!root)
			{
				return;
			}
			MonoBehaviour[] componentsInChildren = root.GetComponentsInChildren<MonoBehaviour>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if ((bool)componentsInChildren[i] && componentsInChildren[i].GetType().Name == "HudSlotFollower")
				{
					componentsInChildren[i].enabled = false;
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

		private void OnEnable()
		{
			ScreenResolutionEvents.OnResolutionApplied += OnResolutionApplied;
		}

		private void OnDisable()
		{
			ScreenResolutionEvents.OnResolutionApplied -= OnResolutionApplied;
		}

		private void OnResolutionApplied()
		{
			ApplyAllChanges();
		}

		private RectTransform FindTarget(string name)
		{
			return (from uiTarget in _targets
				where uiTarget.name == name
				select uiTarget.target).FirstOrDefault();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Drag
{
	public class WindowScript : MonoBehaviour
	{
		protected class SelectableSnapshot
		{
			public Selectable Component;

			public bool WasNavigable;
		}

		private UnityAction _onCloseCallback;

		private UnityAction _onEnableCallback;

		private const float HideDuration = 0.3f;

		private const float FadeDuration = 0.3f;

		private Tween _tween;

		private int _prevState;

		protected ManagerUI _managerUI;

		protected CanvasGroup _canvasGroup;

		protected InputMapSetEvent InputMapSetEvent;

		protected List<SelectableSnapshot> _snapshots;

		private readonly List<string> _exceptions = new List<string>();

		private bool _isSuspended;

		private bool _isClosing;

		public string GUID { get; set; }

		protected virtual bool ChangeRootGameInputGroup => false;

		protected virtual InputGroupsEnum WindowInputGroup { get; set; }

		public bool DidPushInputGroup { get; private set; }

		protected bool IsCloseCanceled
		{
			get
			{
				if (!_isClosing)
				{
					return !base.gameObject;
				}
				return true;
			}
		}

		protected virtual bool ForceHideCursorOnClose => true;

		public InputGroupsEnum GetWindowInputGroup()
		{
			return WindowInputGroup;
		}

		[Inject]
		public void ConstructBase(ManagerUI managerUI, InputMapSetEvent inputMapSetEvent)
		{
			InputMapSetEvent = inputMapSetEvent;
			_managerUI = managerUI;
		}

		protected virtual void Awake()
		{
			_prevState = V.MenuIndex;
		}

		protected virtual void Start()
		{
			_onEnableCallback?.Invoke();
			UiInputSetup.DisableUnnecessaryNavigation(base.gameObject);
		}

		protected virtual void OnEnable()
		{
			if (WindowInputGroup != InputGroupsEnum.None)
			{
				DidPushInputGroup = InputMapSetEvent.TrySetGroup(WindowInputGroup, ChangeRootGameInputGroup);
			}
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void RefreshCache(GameObject target = null, GameObject exceptions = null)
		{
			if (!target)
			{
				target = base.gameObject;
			}
			_snapshots = new List<SelectableSnapshot>();
			UiInputSetup.DisableUnnecessaryNavigation(target);
			Selectable[] componentsInChildren = GetComponentsInChildren<Selectable>(includeInactive: true);
			foreach (Selectable selectable in componentsInChildren)
			{
				if ((bool)exceptions && selectable.gameObject.transform.IsChildOf(exceptions.transform))
				{
					break;
				}
				List<SelectableSnapshot> snapshots = _snapshots;
				SelectableSnapshot obj = new SelectableSnapshot
				{
					Component = selectable
				};
				Navigation.Mode mode = selectable.navigation.mode;
				obj.WasNavigable = mode == Navigation.Mode.Automatic || mode == Navigation.Mode.Explicit;
				snapshots.Add(obj);
			}
		}

		public void SuspendUiNavigation(List<string> exceptions = null)
		{
			if (_isSuspended)
			{
				return;
			}
			if (_snapshots == null)
			{
				RefreshCache();
			}
			if (exceptions == null)
			{
				exceptions = _exceptions;
			}
			if (_snapshots != null)
			{
				foreach (SelectableSnapshot item in _snapshots.Where((SelectableSnapshot snapshot) => snapshot.Component))
				{
					item.WasNavigable = item.Component.navigation.mode == Navigation.Mode.Explicit || item.Component.navigation.mode == Navigation.Mode.Automatic;
					if (exceptions == null || !exceptions.Contains(item.Component.gameObject.name))
					{
						Navigation navigation = item.Component.navigation;
						navigation.mode = Navigation.Mode.None;
						item.Component.navigation = navigation;
					}
				}
			}
			_ = (bool)base.gameObject;
			_isSuspended = true;
		}

		public void RestoreUiNavigation()
		{
			if (!base.gameObject || !_isSuspended || _snapshots == null)
			{
				return;
			}
			foreach (SelectableSnapshot item in _snapshots.Where((SelectableSnapshot snapshot) => snapshot.Component))
			{
				Navigation navigation = item.Component.navigation;
				navigation.mode = (item.WasNavigable ? Navigation.Mode.Automatic : Navigation.Mode.None);
				item.Component.navigation = navigation;
			}
			_isSuspended = false;
		}

		public virtual void UpdateLanguage()
		{
		}

		public virtual void SetAdditionalData(string data)
		{
		}

		public async UniTask ShowUIAsync(bool state)
		{
			ShowUI(state);
			if ((bool)_canvasGroup)
			{
				_canvasGroup.alpha = ((!state) ? 1 : 0);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(0.30000001192092896));
		}

		public virtual void ShowUI(bool state)
		{
			if (!_canvasGroup)
			{
				_canvasGroup = GetComponent<CanvasGroup>();
			}
			if ((bool)_canvasGroup)
			{
				_tween?.Kill();
				_tween = _canvasGroup.DOFade(state ? 1 : 0, 0.3f).SetUpdate(isIndependentUpdate: true);
			}
			NavigationModeSet(state);
			if (state && WindowInputGroup != InputGroupsEnum.None)
			{
				InputMapSetEvent.TrySetGroup(WindowInputGroup, ChangeRootGameInputGroup);
			}
		}

		public virtual void InitCallback(UnityAction callback)
		{
			if (callback != null)
			{
				_onEnableCallback = callback;
			}
		}

		public virtual void InitOnCloseCallback(UnityAction callback)
		{
			if (callback != null)
			{
				_onCloseCallback = callback;
			}
		}

		public virtual void Close()
		{
			if (!IsCloseCanceled)
			{
				if (ForceHideCursorOnClose)
				{
					UISelectionCursor.HideCursor(force: true);
				}
				_onCloseCallback?.Invoke();
				_tween?.Kill();
				_isClosing = true;
				V.MenuIndex = _prevState;
				UnityEngine.Object.Destroy(base.gameObject);
				if (!string.IsNullOrEmpty(GUID))
				{
					_managerUI.OnWindowClose(GUID, ChangeRootGameInputGroup, this);
				}
			}
		}

		public virtual void NavigationModeSet(bool isEnabled)
		{
			if (isEnabled)
			{
				if ((bool)this)
				{
					RestoreUiNavigation();
				}
				else
				{
					Debug.Log("Close called on already destroyed window: " + GUID);
				}
			}
			else
			{
				SuspendUiNavigation();
			}
		}

		public virtual void ReloadWindow()
		{
		}

		public virtual void RebuildWindow()
		{
		}

		public void AddFallbackFont(TMP_FontAsset fallbackFont)
		{
			TextMeshProUGUI componentInChildren = GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
			if ((bool)componentInChildren)
			{
				TMP_FontAsset font = componentInChildren.font;
				if ((bool)font && (bool)fallbackFont && !font.fallbackFontAssetTable.Contains(fallbackFont))
				{
					font.fallbackFontAssetTable.Add(fallbackFont);
				}
			}
		}
	}
}

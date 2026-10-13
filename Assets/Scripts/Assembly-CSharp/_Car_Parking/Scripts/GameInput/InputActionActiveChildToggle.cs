using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.GameInput
{
	public class InputActionActiveChildToggle : MonoBehaviour
	{
		[Tooltip("Object toggled. Defaults to the first child if left empty.")]
		[SerializeField]
		private GameObject _target;

		[SerializeField]
		[Tooltip("Invert: show the target when Back is UNavailable instead.")]
		private bool _invert;

		[SerializeField]
		[Tooltip("Window names that suppress the hint while active (e.g. WaitingWindow), even if Back is available.")]
		private string[] _suppressWhenWindowsActive = new string[4] { "WaitingWindow", "WaitingWindowNotTransparent", "WaitingWindowDark", "ProgressWindow" };

		[SerializeField]
		[Tooltip("Current-window names that never show the hint: root menu and gameplay HUD windows. FirstGameWindow is a root too: its ESC opens the exit-game confirm, not a step back.")]
		private string[] _windowsWithoutBack = new string[10] { "MainMenuWindow", "ArenaLobbyWindow", "ObstacleRaceGameplayWindow", "CircuitPlayWindow", "MultiplayerRacePlayWindow", "FreeWalkWindow", "PhotoModeWindow", "DroneControlWindow", "ObstacleRaceResultWindow", "FirstGameWindow" };

		private const int SpawnedOnTopCheckPeriod = 10;

		private ManagerUI _managerUI;

		private bool _backAvailable;

		private bool _spawnedOnTopSuppressed;

		private int _lastSpawnedOnTopCheckFrame = -10;

		private void Awake()
		{
			if (!_target && base.transform.childCount > 0)
			{
				_target = base.transform.GetChild(0).gameObject;
			}
		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			Rebind();
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
			Unbind();
		}

		private void Update()
		{
			Apply();
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Rebind();
		}

		private void OnSceneUnloaded(Scene scene)
		{
			Unbind();
			ForceHide();
		}

		private void Rebind()
		{
			Unbind();
			ManagerUI managerUI = ((V.CoreGameManager != null) ? V.CoreGameManager.ManagerUI : null);
			if (managerUI == null)
			{
				ForceHide();
				return;
			}
			_managerUI = managerUI;
			ManagerUI managerUI2 = _managerUI;
			managerUI2.OnBackButtonActivated = (Action<bool>)Delegate.Combine(managerUI2.OnBackButtonActivated, new Action<bool>(Refresh));
			Refresh(enable: false);
		}

		private void Unbind()
		{
			if (_managerUI != null)
			{
				ManagerUI managerUI = _managerUI;
				managerUI.OnBackButtonActivated = (Action<bool>)Delegate.Remove(managerUI.OnBackButtonActivated, new Action<bool>(Refresh));
			}
			_managerUI = null;
		}

		private void ForceHide()
		{
			if ((bool)_target && _target.activeSelf)
			{
				_target.SetActive(value: false);
			}
		}

		private void Refresh(bool enable)
		{
			_backAvailable = enable;
			Apply();
		}

		private void Apply()
		{
			if ((bool)_target)
			{
				bool flag = _managerUI != null && (BackAvailable() ^ _invert) && !AnySuppressWindowActive();
				if (_target.activeSelf != flag)
				{
					_target.SetActive(flag);
				}
			}
		}

		private bool BackAvailable()
		{
			string currentWindow = _managerUI.GetCurrentWindow();
			if (string.IsNullOrEmpty(currentWindow))
			{
				return _backAvailable;
			}
			if (_windowsWithoutBack != null)
			{
				for (int i = 0; i < _windowsWithoutBack.Length; i++)
				{
					if (currentWindow == _windowsWithoutBack[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool AnySuppressWindowActive()
		{
			if (_managerUI == null || _suppressWhenWindowsActive == null)
			{
				return false;
			}
			for (int i = 0; i < _suppressWhenWindowsActive.Length; i++)
			{
				string text = _suppressWhenWindowsActive[i];
				if (!string.IsNullOrEmpty(text) && _managerUI.IsWindowActiveOrOpening(text))
				{
					return true;
				}
			}
			return SpawnedOnTopSuppressActive();
		}

		private bool SpawnedOnTopSuppressActive()
		{
			if (Time.frameCount - _lastSpawnedOnTopCheckFrame < 10)
			{
				return _spawnedOnTopSuppressed;
			}
			_lastSpawnedOnTopCheckFrame = Time.frameCount;
			_spawnedOnTopSuppressed = false;
			for (int i = 0; i < _suppressWhenWindowsActive.Length; i++)
			{
				string text = _suppressWhenWindowsActive[i];
				if (!string.IsNullOrEmpty(text) && _managerUI.IsWindowSpawnedOnTop(text))
				{
					_spawnedOnTopSuppressed = true;
					break;
				}
			}
			return _spawnedOnTopSuppressed;
		}
	}
}

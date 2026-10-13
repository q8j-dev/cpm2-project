using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility.ScriptableObjects;
using _Car_Parking.Scripts.Steam.InputHints;

namespace CPM.Scripts.VinylDrawer
{
	public class SteamUIStateConfigSwitcher : MonoBehaviour
	{
		[Serializable]
		private struct StateConfigEntry
		{
			public VinylsEditorInputState state;

			public SteamUILayoutConfig config;
		}

		[SerializeField]
		private SteamUIConfigApplier _steamUIConfigApplier;

		[SerializeField]
		private VinylsEditor _vinylsEditor;

		[SerializeField]
		private StateConfigEntry[] _stateConfigs;

		private VinylsEditorInputState _lastState;

		private Dictionary<VinylsEditorInputState, SteamUILayoutConfig> _stateConfigLookup;

		private void Awake()
		{
			if (_stateConfigs != null)
			{
				_stateConfigLookup = new Dictionary<VinylsEditorInputState, SteamUILayoutConfig>(_stateConfigs.Length);
				StateConfigEntry[] stateConfigs = _stateConfigs;
				for (int i = 0; i < stateConfigs.Length; i++)
				{
					StateConfigEntry stateConfigEntry = stateConfigs[i];
					_stateConfigLookup[stateConfigEntry.state] = stateConfigEntry.config;
				}
			}
		}

		private void OnEnable()
		{
			if (_vinylsEditor != null)
			{
				_vinylsEditor.OnEditorStateChanged += OnStateChanged;
			}
		}

		private void OnDisable()
		{
			if (_vinylsEditor != null)
			{
				_vinylsEditor.OnEditorStateChanged -= OnStateChanged;
			}
		}

		private void OnStateChanged(VinylsEditorInputState state)
		{
			if (_stateConfigLookup != null && _lastState != state && ((bool)_steamUIConfigApplier || TryGetComponent<SteamUIConfigApplier>(out _steamUIConfigApplier)))
			{
				if (_stateConfigLookup.TryGetValue(state, out var value))
				{
					_steamUIConfigApplier.ApplyConfigChanges(value);
				}
				else
				{
					_steamUIConfigApplier.ApplyDefaultValues();
				}
				_lastState = state;
			}
		}
	}
}

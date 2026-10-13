using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using VContainer.Unity;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class BindingOverridesService : IStartable, IDisposable
	{
		private readonly GameInputManager _inputManager;

		private readonly BindingsPrefsStore _store;

		private readonly RebindableActionsConfig _config;

		private readonly EventRegistry _eventRegistry;

		private readonly InputDeviceDetector _deviceDetector;

		private readonly ActiveWheelService _activeWheels;

		private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

		private RegistryHandle<ScriptableEvent> _bindingsChangedHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.BindingsChanged);

		private HashSet<string> _templateIds = new HashSet<string>();

		private HashSet<string> _authoredIds;

		private string _activeWheelLayout;

		public bool IsLoaded { get; private set; }

		public WheelKind ActiveWheelKind { get; private set; }

		public BindingOverridesService(GameInputManager inputManager, BindingsPrefsStore store, RebindableActionsConfig config, EventRegistry eventRegistry, InputDeviceDetector deviceDetector, ActiveWheelService activeWheels)
		{
			_inputManager = inputManager;
			_store = store;
			_config = config;
			_eventRegistry = eventRegistry;
			_deviceDetector = deviceDetector;
			_activeWheels = activeWheels;
		}

		void IStartable.Start()
		{
			LoadWhenInputReadyAsync(_disposeCts.Token).Forget();
		}

		private async UniTaskVoid LoadWhenInputReadyAsync(CancellationToken ct)
		{
			try
			{
				await UniTask.WaitUntil(() => _inputManager.InputAsset != null, PlayerLoopTiming.Update, ct);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			_templateIds = WheelTemplate.TemplateBindingIds(_inputManager.InputAsset);
			if (_deviceDetector != null)
			{
				_deviceDetector.BindingSource = _inputManager.InputAsset;
			}
			LoadSaved();
			if (_activeWheels != null)
			{
				_activeWheels.ActiveChanged += OnActiveWheelChanged;
			}
		}

		public void ApplyChanges(IReadOnlyList<BindingChange> changes)
		{
			if (changes != null && changes.Count != 0)
			{
				BindingChangeApplier.Apply(changes);
				CommitChanges();
			}
		}

		public bool ResetOverridesWhere(Predicate<InputBinding> match)
		{
			if (match != null)
			{
				return ResetOverridesWhere((InputAction action, int index) => match(action.bindings[index]));
			}
			return false;
		}

		public bool ResetOverridesWhere(Func<InputAction, int, bool> match)
		{
			InputActionAsset inputAsset = _inputManager.InputAsset;
			if (inputAsset == null || match == null)
			{
				return false;
			}
			bool flag = false;
			foreach (InputAction item in inputAsset.actionMaps.SelectMany((InputActionMap map) => map.actions))
			{
				for (int num = 0; num < item.bindings.Count; num++)
				{
					if (item.bindings[num].hasOverrides && match(item, num))
					{
						item.RemoveBindingOverride(num);
						flag = true;
					}
				}
			}
			if (flag)
			{
				CommitChanges();
			}
			return flag;
		}

		public bool ResetActiveWheel()
		{
			if (_inputManager.InputAsset == null || _activeWheelLayout == null)
			{
				return false;
			}
			_store.SaveWheelSets(WheelSetsJson.Remove(_store.LoadWheelSets(), _activeWheelLayout));
			ApplyActiveWheelSet();
			CommitChanges();
			return true;
		}

		private void CommitChanges()
		{
			string json = _inputManager.InputAsset.SaveBindingOverridesAsJson();
			OverrideSetJson.Split(json, _templateIds.Contains, out var included, out var excluded);
			_store.Save(excluded);
			if (_activeWheelLayout != null)
			{
				_store.SaveWheelSets(WheelSetsJson.Set(_store.LoadWheelSets(), _activeWheelLayout, included));
			}
			MirrorToAuthoredAsset(json);
			NotifyChanged();
		}

		private void MirrorToAuthoredAsset(string json)
		{
			InputActionAsset inputActionAsset = (_config ? _config.AuthoredAsset : null);
			if (!inputActionAsset || inputActionAsset == _inputManager.InputAsset)
			{
				return;
			}
			try
			{
				inputActionAsset.RemoveAllBindingOverrides();
				if (_authoredIds == null)
				{
					_authoredIds = BindingIdsOf(inputActionAsset);
				}
				if (!string.IsNullOrEmpty(json) && OverrideSetJson.Split(json, _authoredIds.Contains, out var included, out var _))
				{
					inputActionAsset.LoadBindingOverridesFromJson(included);
				}
			}
			catch (Exception)
			{
			}
		}

		private void NotifyChanged()
		{
			_bindingsChangedHandle.Get(_eventRegistry)?.Raise();
			_deviceDetector?.NotifyBindingsChanged();
		}

		private void LoadSaved()
		{
			InputActionAsset inputAsset = _inputManager.InputAsset;
			string text = _store.Load();
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					inputAsset.LoadBindingOverridesFromJson(text);
				}
				catch (Exception)
				{
					inputAsset.RemoveAllBindingOverrides();
					_store.Clear();
				}
			}
			ApplyActiveWheelSet();
			IsLoaded = true;
			if (RemoveReservedOverrides(inputAsset))
			{
				CommitChanges();
				return;
			}
			MirrorToAuthoredAsset(inputAsset.SaveBindingOverridesAsJson());
			NotifyChanged();
		}

		private bool RemoveReservedOverrides(InputActionAsset asset)
		{
			IReadOnlyList<string> readOnlyList = (_config ? _config.ReservedPaths : null);
			if (readOnlyList == null || readOnlyList.Count == 0)
			{
				return false;
			}
			bool result = false;
			foreach (InputAction item in asset.actionMaps.SelectMany((InputActionMap map) => map.actions))
			{
				for (int num = 0; num < item.bindings.Count; num++)
				{
					string overridePath = item.bindings[num].overridePath;
					if (!string.IsNullOrEmpty(overridePath) && BindingPaths.OverlapsAny(overridePath, readOnlyList))
					{
						item.RemoveBindingOverride(num);
						result = true;
					}
				}
			}
			return result;
		}

		private void OnActiveWheelChanged()
		{
			InputActionAsset inputAsset = _inputManager.InputAsset;
			if (!(inputAsset == null))
			{
				ApplyActiveWheelSet();
				MirrorToAuthoredAsset(inputAsset.SaveBindingOverridesAsJson());
				NotifyChanged();
			}
		}

		private void ApplyActiveWheelSet()
		{
			InputActionAsset inputAsset = _inputManager.InputAsset;
			RemoveTemplateOverrides(inputAsset);
			InputDevice inputDevice = _activeWheels?.ActiveWheel;
			ActiveWheelKind = ((inputDevice != null) ? _activeWheels.ActiveKind : WheelKind.None);
			object activeWheelLayout;
			if (inputDevice != null)
			{
				WheelKind activeWheelKind = ActiveWheelKind;
				if (activeWheelKind == WheelKind.Moza || activeWheelKind == WheelKind.Generic)
				{
					activeWheelLayout = inputDevice.layout;
					goto IL_0058;
				}
			}
			activeWheelLayout = null;
			goto IL_0058;
			IL_0058:
			_activeWheelLayout = (string)activeWheelLayout;
			if (_activeWheelLayout == null)
			{
				return;
			}
			string text = WheelSetsJson.Get(_store.LoadWheelSets(), _activeWheelLayout);
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					inputAsset.LoadBindingOverridesFromJson(text, removeExisting: false);
					return;
				}
				catch (Exception)
				{
					RemoveTemplateOverrides(inputAsset);
				}
			}
			BindingChangeApplier.Apply(WheelTemplate.DefaultChanges(inputAsset, ActiveWheelKind));
		}

		private static HashSet<string> BindingIdsOf(InputActionAsset asset)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (InputActionMap actionMap in asset.actionMaps)
			{
				foreach (InputBinding binding in actionMap.bindings)
				{
					hashSet.Add(binding.id.ToString());
				}
			}
			return hashSet;
		}

		private static void RemoveTemplateOverrides(InputActionAsset asset)
		{
			foreach (InputAction item in asset.actionMaps.SelectMany((InputActionMap map) => map.actions))
			{
				for (int num = 0; num < item.bindings.Count; num++)
				{
					InputBinding binding = item.bindings[num];
					if (binding.hasOverrides && WheelTemplate.IsTemplateSlot(in binding))
					{
						item.RemoveBindingOverride(num);
					}
				}
			}
		}

		public void Dispose()
		{
			if (_activeWheels != null)
			{
				_activeWheels.ActiveChanged -= OnActiveWheelChanged;
			}
			_disposeCts.Cancel();
			_disposeCts.Dispose();
			InputActionAsset inputActionAsset = (_config ? _config.AuthoredAsset : null);
			if ((bool)inputActionAsset)
			{
				inputActionAsset.RemoveAllBindingOverrides();
			}
		}
	}
}

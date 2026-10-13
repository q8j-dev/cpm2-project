using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[CreateAssetMenu(fileName = "RebindableActionsConfig", menuName = "Configs/RebindableActionsConfig")]
	public sealed class RebindableActionsConfig : ScriptableObject
	{
		[Serializable]
		public struct LinkedActions
		{
			[Tooltip("Action the player rebinds.")]
			public InputActionReference Leader;

			[Tooltip("Action whose bindings on the leader's old key move to the leader's new key.")]
			public InputActionReference Follower;
		}

		[Serializable]
		public struct DeviceLockedAction
		{
			public InputActionReference Action;

			[Tooltip("Pages where the action can't be rebound and keeps its keys.")]
			public RebindDevices Devices;
		}

		[Tooltip("GameInputController.inputactions. Overrides are mirrored onto it for its other readers.")]
		[SerializeField]
		private InputActionAsset _authoredAsset = default;

		[Tooltip("Visible actions that can't be rebound; nothing that can fire together with them may take their keys.")]
		[SerializeField]
		private List<InputActionReference> _lockedActions = new List<InputActionReference>();

		[Tooltip("Like Locked Actions, on the listed device pages only (linked followers still move with their leader).")]
		[SerializeField]
		private List<DeviceLockedAction> _deviceLockedActions = new List<DeviceLockedAction>();

		[SerializeField]
		private List<LinkedActions> _linked = new List<LinkedActions>();

		[Tooltip("Keys that can never be assigned (also blocks rebinding rows currently on them).")]
		[SerializeField]
		private List<string> _reservedPaths = new List<string>();

		[Tooltip("Actions whose Press/Hold bindings the player may switch between Press and Hold. Only list actions handled on 'performed' — handlers on 'started' fire on key-down, so Hold changes nothing.")]
		[SerializeField]
		private List<InputActionReference> _pressHoldActions = new List<InputActionReference>();

		private HashSet<Guid> _lockedIds;

		private Dictionary<Guid, RebindDevices> _deviceLockedIds;

		private HashSet<Guid> _pressHoldIds;

		private Dictionary<Guid, List<Guid>> _followerIds;

		private Dictionary<Guid, List<Guid>> _leaderIds;

		private Dictionary<Guid, Guid> _linkParent;

		public InputActionAsset AuthoredAsset => _authoredAsset;

		public IReadOnlyList<string> ReservedPaths => _reservedPaths;

		public bool IsNonRebindable(InputAction action)
		{
			EnsureCaches();
			if (action != null)
			{
				return _lockedIds.Contains(action.id);
			}
			return false;
		}

		public bool IsLockedOn(InputAction action, RebindDeviceKind device)
		{
			EnsureCaches();
			if (action != null && _deviceLockedIds.TryGetValue(action.id, out var value))
			{
				return value.Includes(device);
			}
			return false;
		}

		public bool AllowsPressHold(InputAction action)
		{
			EnsureCaches();
			if (action != null)
			{
				return _pressHoldIds.Contains(action.id);
			}
			return false;
		}

		public IReadOnlyList<Guid> GetFollowerIds(InputAction leader)
		{
			EnsureCaches();
			if (leader == null || !_followerIds.TryGetValue(leader.id, out var value))
			{
				return Array.Empty<Guid>();
			}
			return value;
		}

		public IReadOnlyList<Guid> GetLeaderIds(InputAction follower)
		{
			EnsureCaches();
			if (follower == null || !_leaderIds.TryGetValue(follower.id, out var value))
			{
				return Array.Empty<Guid>();
			}
			return value;
		}

		public bool AreLinked(InputAction a, InputAction b)
		{
			EnsureCaches();
			if (a != null && b != null && _linkParent.ContainsKey(a.id) && _linkParent.ContainsKey(b.id))
			{
				return LinkRootOf(a.id) == LinkRootOf(b.id);
			}
			return false;
		}

		private void OnEnable()
		{
			ClearCaches();
		}

		private void OnValidate()
		{
			ClearCaches();
		}

		private void ClearCaches()
		{
			_lockedIds = null;
			_deviceLockedIds = null;
			_pressHoldIds = null;
			_followerIds = null;
			_leaderIds = null;
			_linkParent = null;
		}

		private void EnsureCaches()
		{
			if (_lockedIds != null)
			{
				return;
			}
			_lockedIds = IdsOf(_lockedActions, "_lockedActions");
			_pressHoldIds = IdsOf(_pressHoldActions, "_pressHoldActions");
			_deviceLockedIds = new Dictionary<Guid, RebindDevices>();
			foreach (DeviceLockedAction deviceLockedAction in _deviceLockedActions)
			{
				if (TryIdOf(deviceLockedAction.Action, "_deviceLockedActions", out var id))
				{
					_deviceLockedIds[id] = (_deviceLockedIds.TryGetValue(id, out var value) ? (value | deviceLockedAction.Devices) : deviceLockedAction.Devices);
				}
			}
			_followerIds = new Dictionary<Guid, List<Guid>>();
			_leaderIds = new Dictionary<Guid, List<Guid>>();
			_linkParent = new Dictionary<Guid, Guid>();
			foreach (LinkedActions item in _linked)
			{
				if (TryIdOf(item.Leader, "_linked", out var id2) && TryIdOf(item.Follower, "_linked", out var id3))
				{
					if (!_followerIds.TryGetValue(id2, out var value2))
					{
						value2 = (_followerIds[id2] = new List<Guid>());
					}
					value2.Add(id3);
					if (!_leaderIds.TryGetValue(id3, out var value3))
					{
						value3 = (_leaderIds[id3] = new List<Guid>());
					}
					value3.Add(id2);
					JoinLinkSets(id2, id3);
				}
			}
		}

		private void JoinLinkSets(Guid a, Guid b)
		{
			_linkParent.TryAdd(a, a);
			_linkParent.TryAdd(b, b);
			Guid guid = LinkRootOf(a);
			Guid guid2 = LinkRootOf(b);
			if (guid != guid2)
			{
				_linkParent[guid2] = guid;
			}
		}

		private Guid LinkRootOf(Guid id)
		{
			while (_linkParent[id] != id)
			{
				id = _linkParent[id];
			}
			return id;
		}

		private HashSet<Guid> IdsOf(List<InputActionReference> references, string listName)
		{
			HashSet<Guid> hashSet = new HashSet<Guid>();
			foreach (InputActionReference reference in references)
			{
				if (TryIdOf(reference, listName, out var id))
				{
					hashSet.Add(id);
				}
			}
			return hashSet;
		}

		private bool TryIdOf(InputActionReference reference, string listName, out Guid id)
		{
			InputAction inputAction = (reference ? reference.action : null);
			id = inputAction?.id ?? Guid.Empty;
			if (inputAction != null)
			{
				return true;
			}
			return false;
		}
	}
}

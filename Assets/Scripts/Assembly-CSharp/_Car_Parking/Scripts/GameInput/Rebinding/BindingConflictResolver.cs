using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class BindingConflictResolver
	{
		private sealed class RebindRules : IRebindRules
		{
			private readonly RebindableActionsConfig _config;

			private readonly InputActionAsset _asset;

			private readonly Func<InputAction, bool> _isDisplayed;

			private readonly InputHintSpriteMap _glyphs;

			private readonly InputDeviceType _glyphDevice;

			private readonly IReadOnlyList<string[]> _groups;

			private readonly Func<BindingSlot, int, bool> _slotIsUnusedIn;

			private readonly Dictionary<InputAction, IReadOnlyList<InputAction>> _followers = new Dictionary<InputAction, IReadOnlyList<InputAction>>();

			private readonly Dictionary<InputAction, IReadOnlyList<InputAction>> _leaders = new Dictionary<InputAction, IReadOnlyList<InputAction>>();

			public RebindRules(RebindableActionsConfig config, InputActionAsset asset, Func<InputAction, bool> isDisplayed, InputHintSpriteMap glyphs, InputDeviceType glyphDevice, IReadOnlyList<string[]> groups, Func<BindingSlot, int, bool> slotIsUnusedIn)
			{
				_config = config;
				_asset = asset;
				_isDisplayed = isDisplayed;
				_glyphs = glyphs;
				_glyphDevice = glyphDevice;
				_groups = groups;
				_slotIsUnusedIn = slotIsUnusedIn;
			}

			public bool CanFireTogether(BindingSlot a, BindingSlot b)
			{
				return RebindScope.CanFireTogether(a, b, _groups, _slotIsUnusedIn);
			}

			public bool IsReservedPath(string path)
			{
				if ((bool)_config)
				{
					return BindingPaths.OverlapsAny(path, _config.ReservedPaths);
				}
				return false;
			}

			public bool HasGlyph(string path)
			{
				if (_glyphDevice != InputDeviceType.Wheel && (bool)_glyphs)
				{
					return _glyphs.HasSprite(path, _glyphDevice);
				}
				return true;
			}

			public ConflictPolicy PolicyFor(InputAction action)
			{
				if ((bool)_config && _config.IsNonRebindable(action))
				{
					return ConflictPolicy.Lock;
				}
				if (_isDisplayed != null && !_isDisplayed(action))
				{
					return ConflictPolicy.Ignore;
				}
				return ConflictPolicy.Unbind;
			}

			public bool IsLockedOn(InputAction action, RebindDeviceKind device)
			{
				if ((bool)_config)
				{
					return _config.IsLockedOn(action, device);
				}
				return false;
			}

			public bool AllowsPressHold(InputAction action)
			{
				if ((bool)_config)
				{
					return _config.AllowsPressHold(action);
				}
				return false;
			}

			public bool AreLinked(InputAction a, InputAction b)
			{
				if ((bool)_config)
				{
					return _config.AreLinked(a, b);
				}
				return false;
			}

			public IReadOnlyList<InputAction> FollowersOf(InputAction leader)
			{
				return Resolve(leader, _followers, (InputAction action) => _config.GetFollowerIds(action), "follower");
			}

			public IReadOnlyList<InputAction> LeadersOf(InputAction follower)
			{
				return Resolve(follower, _leaders, (InputAction action) => _config.GetLeaderIds(action), "leader");
			}

			private IReadOnlyList<InputAction> Resolve(InputAction action, Dictionary<InputAction, IReadOnlyList<InputAction>> cache, Func<InputAction, IReadOnlyList<Guid>> ids, string role)
			{
				if (action == null)
				{
					return Array.Empty<InputAction>();
				}
				if (cache.TryGetValue(action, out var value))
				{
					return value;
				}
				List<InputAction> list = new List<InputAction>();
				if ((bool)_config && (bool)_asset)
				{
					foreach (Guid item in ids(action))
					{
						InputAction inputAction = _asset.FindAction(item);
						if (inputAction != null)
						{
							list.Add(inputAction);
						}
					}
				}
				cache[action] = list;
				return list;
			}
		}

		private readonly GameInputManager _inputManager;

		private readonly InputMapSetEvent _inputMapSetEvent;

		private readonly RebindableActionsConfig _config;

		private List<string[]> _groupMapSets;

		private List<InputGroupsEnum> _groupIds;

		public BindingConflictResolver(GameInputManager inputManager, InputMapSetEvent inputMapSetEvent, RebindableActionsConfig config)
		{
			_inputManager = inputManager;
			_inputMapSetEvent = inputMapSetEvent;
			_config = config;
		}

		public IRebindRules CreateRules(Func<InputAction, bool> isDisplayed, Func<InputAction, string, InputGroupsEnum, bool> isUnusedIn, InputHintSpriteMap glyphs, InputDeviceType glyphDevice)
		{
			List<string[]> groupMapSets = GetGroupMapSets();
			return new RebindRules(_config, _inputManager.InputAsset, isDisplayed, glyphs, glyphDevice, groupMapSets, (isUnusedIn == null) ? null : ((Func<BindingSlot, int, bool>)((BindingSlot slot, int group) => IsUnusedIn(slot, group, isUnusedIn))));
		}

		private bool IsUnusedIn(BindingSlot slot, int group, Func<InputAction, string, InputGroupsEnum, bool> isUnusedIn)
		{
			if (_groupIds == null || group < 0 || group >= _groupIds.Count)
			{
				return false;
			}
			InputBinding binding = slot.Binding;
			return isUnusedIn(slot.Action, binding.isPartOfComposite ? binding.name : string.Empty, _groupIds[group]);
		}

		public RebindPlan Plan(BindingSlot target, string capturedPath, RebindDeviceProfile profile, IRebindRules rules)
		{
			return Plan(new RebindRequest(target, capturedPath), profile, rules);
		}

		public RebindPlan Plan(RebindRequest request, RebindDeviceProfile profile, IRebindRules rules)
		{
			List<InputActionMap> scope = RebindScope.ScopeMaps(_inputManager.InputAsset, request.Target.Action, rules.FollowersOf(request.Target.Action), GetGroupMapSets());
			return RebindPlanner.Plan(request, scope, profile, rules);
		}

		private List<string[]> GetGroupMapSets()
		{
			if (_groupMapSets != null)
			{
				return _groupMapSets;
			}
			List<string[]> list = new List<string[]>();
			List<InputGroupsEnum> list2 = new List<InputGroupsEnum>();
			foreach (InputGroupsEnum value in Enum.GetValues(typeof(InputGroupsEnum)))
			{
				if (_inputMapSetEvent.TryGetGroupMaps(value, out var maps) && maps != null)
				{
					list.Add(maps.Select((InputMapEnum map) => map.ToString()).ToArray());
					list2.Add(value);
				}
			}
			if (list.Count > 0)
			{
				_groupMapSets = list;
				_groupIds = list2;
			}
			return list;
		}
	}
}

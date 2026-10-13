using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using _Car_Parking.Scripts.GameInput;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[CreateAssetMenu(fileName = "New InputMapSetEvent", menuName = "SOAP/InputMapsConfig")]
	public class InputMapSetEvent : ScriptableObject
	{
		[SerializeField]
		protected bool debugLog;

		private readonly List<InputGroupsEnum> _gameplayRootGroupChain = new List<InputGroupsEnum>();

		private readonly List<InputGroupsEnum> _inputGroupChain = new List<InputGroupsEnum>();

		private IEnumerable<InputMapEnum> _mapsToEnable = Enumerable.Empty<InputMapEnum>();

		private IEnumerable<InputMapEnum> _mapsToDisable = Enumerable.Empty<InputMapEnum>();

		private IEnumerable<InputMapEnum> _oldMaps = Enumerable.Empty<InputMapEnum>();

		private IEnumerable<InputMapEnum> _newMaps = Enumerable.Empty<InputMapEnum>();

		[SerializeField]
		[Tooltip("DOES NOT UPDATE IN RUNTIME")]
		private List<InputMapGroup> inputGroups = new List<InputMapGroup>();

		private readonly Dictionary<InputGroupsEnum, IEnumerable<InputMapEnum>> _inputMapGroups = new Dictionary<InputGroupsEnum, IEnumerable<InputMapEnum>>();

		private readonly HashSet<InputGroupsEnum> _rootGroups = new HashSet<InputGroupsEnum>();

		public InputGroupsEnum CurrentInputGroup
		{
			get
			{
				if (_inputGroupChain.Count <= 0)
				{
					return GameplayRootGroup;
				}
				List<InputGroupsEnum> inputGroupChain = _inputGroupChain;
				return inputGroupChain[inputGroupChain.Count - 1];
			}
		}

		private InputGroupsEnum PreviousInputGroup
		{
			get
			{
				if (_inputGroupChain.Count > 1)
				{
					List<InputGroupsEnum> inputGroupChain = _inputGroupChain;
					return inputGroupChain[inputGroupChain.Count - 2];
				}
				return GameplayRootGroup;
			}
		}

		public InputGroupsEnum GameplayRootGroup
		{
			get
			{
				return _gameplayRootGroupChain.LastOrDefault();
			}
			private set
			{
				int num = _gameplayRootGroupChain.IndexOf(value);
				if (num >= 0)
				{
					_gameplayRootGroupChain.RemoveRange(num + 1, _gameplayRootGroupChain.Count - num - 1);
				}
				else
				{
					_gameplayRootGroupChain.Add(value);
				}
				_inputGroupChain.Clear();
				_ = debugLog;
			}
		}

		private event Action<InputGroupsEnum> OnRaised;

		public void Subscribe(Action<InputGroupsEnum> listener)
		{
			OnRaised += listener;
		}

		public void Unsubscribe(Action<InputGroupsEnum> listener)
		{
			OnRaised -= listener;
		}

		private void Raise(InputGroupsEnum value)
		{
			if (OnRaised == null)
			{
				return;
			}
			Delegate[] invocationList = OnRaised.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				try
				{
					((Action<InputGroupsEnum>)obj)(value);
				}
				catch (Exception)
				{
				}
			}
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		private void LogRaise()
		{
			if (!debugLog || OnRaised == null)
			{
				return;
			}
			Delegate[] invocationList = OnRaised.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				if (obj.Target != null)
				{
					_ = obj.Target.GetType().FullName;
				}
				if (obj.Method != null)
				{
					_ = obj.Method.Name;
				}
			}
		}

		public void Reset()
		{
			_ = debugLog;
			_inputGroupChain.Clear();
			_gameplayRootGroupChain.Clear();
		}

		private void OnDisable()
		{
			Reset();
		}

		public void Initialize()
		{
			Reset();
			OnRaised = null;
			_inputMapGroups.Clear();
			_rootGroups.Clear();
			foreach (InputMapGroup inputGroup in inputGroups)
			{
				_inputMapGroups.Add(inputGroup.GroupName, inputGroup.GetAllMaps());
				if (inputGroup.isRootGameplayGroup)
				{
					_rootGroups.Add(inputGroup.GroupName);
				}
			}
		}

		public bool TrySetGroup(InputGroupsEnum newGroup, bool setRoot = false)
		{
			if (newGroup == CurrentInputGroup)
			{
				if (setRoot && _rootGroups.Contains(CurrentInputGroup))
				{
					GameplayRootGroup = CurrentInputGroup;
				}
				return false;
			}
			if (!_inputMapGroups.ContainsKey(newGroup))
			{
				_ = debugLog;
				return false;
			}
			if (newGroup != PreviousInputGroup)
			{
				_inputGroupChain.Add(newGroup);
				SetChangedMaps(CurrentInputGroup, PreviousInputGroup);
			}
			else
			{
				SetChangedMaps(PreviousInputGroup, CurrentInputGroup);
				if (_inputGroupChain.Count > 0)
				{
					_inputGroupChain.RemoveAt(_inputGroupChain.Count - 1);
				}
			}
			if (setRoot && _rootGroups.Contains(CurrentInputGroup))
			{
				GameplayRootGroup = CurrentInputGroup;
			}
			Raise(newGroup);
			return true;
		}

		public bool TrySetPreviousGroup()
		{
			if (PreviousInputGroup == InputGroupsEnum.NoInput && _inputGroupChain.Count > 1)
			{
				_inputGroupChain.RemoveAt(_inputGroupChain.Count - 2);
			}
			_ = debugLog;
			return TrySetGroup(PreviousInputGroup, setRoot: true);
		}

		public bool TrySetRootGroup()
		{
			if (CurrentInputGroup == GameplayRootGroup)
			{
				return false;
			}
			if (!_inputMapGroups.ContainsKey(GameplayRootGroup))
			{
				_ = debugLog;
				return false;
			}
			_ = debugLog;
			SetChangedMaps(GameplayRootGroup, CurrentInputGroup);
			_inputGroupChain.Clear();
			Raise(GameplayRootGroup);
			return true;
		}

		public bool SetPreviousRootGroup()
		{
			if (_gameplayRootGroupChain.Count < 1)
			{
				return false;
			}
			InputGroupsEnum num;
			if (_gameplayRootGroupChain.Count <= 1)
			{
				num = GameplayRootGroup;
			}
			else
			{
				List<InputGroupsEnum> gameplayRootGroupChain = _gameplayRootGroupChain;
				num = gameplayRootGroupChain[gameplayRootGroupChain.Count - 2];
			}
			InputGroupsEnum inputGroupsEnum = num;
			_ = debugLog;
			SetChangedMaps(inputGroupsEnum, CurrentInputGroup);
			GameplayRootGroup = inputGroupsEnum;
			Raise(GameplayRootGroup);
			return true;
		}

		private void SetChangedMaps(InputGroupsEnum newGroup, InputGroupsEnum oldGroup)
		{
			_mapsToEnable = ((_inputMapGroups.TryGetValue(newGroup, out var value) && _inputMapGroups.TryGetValue(oldGroup, out var value2)) ? value.Except(value2) : _inputMapGroups[newGroup]);
			_mapsToDisable = ((_inputMapGroups.TryGetValue(oldGroup, out value2) && _inputMapGroups.TryGetValue(newGroup, out value)) ? value2.Except(value) : Enumerable.Empty<InputMapEnum>());
			if (!_inputMapGroups.TryGetValue(newGroup, out _newMaps))
			{
				_newMaps = Enumerable.Empty<InputMapEnum>();
			}
			if (!_inputMapGroups.TryGetValue(oldGroup, out _oldMaps))
			{
				_oldMaps = Enumerable.Empty<InputMapEnum>();
			}
		}

		public List<InputMapEnum> GetMapsToDisable()
		{
			return _mapsToDisable.ToList();
		}

		public List<InputMapEnum> GetMapsToEnable()
		{
			return _mapsToEnable.ToList();
		}

		public List<InputMapEnum> GetOldMaps()
		{
			return _oldMaps.ToList();
		}

		public List<InputMapEnum> GetNewMaps()
		{
			return _newMaps.ToList();
		}

		public bool TryGetCurrentMaps(out IEnumerable<InputMapEnum> maps)
		{
			return _inputMapGroups.TryGetValue(CurrentInputGroup, out maps);
		}

		public bool TryGetPreviousMaps(out IEnumerable<InputMapEnum> maps)
		{
			return _inputMapGroups.TryGetValue(PreviousInputGroup, out maps);
		}

		public bool TryGetGroupMaps(InputGroupsEnum group, out IEnumerable<InputMapEnum> maps)
		{
			return _inputMapGroups.TryGetValue(group, out maps);
		}
	}
}

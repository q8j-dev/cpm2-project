using UnityEngine;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Person.Locomotion.Input
{
	public class PersonInputAdapter
	{
		private const float SprintTriggerThreshold = 0.5f;

		private const float DefaultButtonHoldThreshold = 0.15f;

		private const float DefaultShuffleLatchDuration = 0.3f;

		private const float DefaultRunDeflectionEnter = 0.9f;

		private const float DefaultRunDeflectionExit = 0.75f;

		private const float DefaultDeflectionRunEnter = 0.55f;

		private const float DefaultDeflectionRunExit = 0.45f;

		private const float DefaultSprintDeflectionEnter = 0.88f;

		private const float DefaultSprintDeflectionExit = 0.75f;

		private const float DefaultSprintLockMinHoldTime = 0.6f;

		private const float SprintLockReleaseGrace = 0.18f;

		private const float DefaultSprintLockZoneMinUp = 0.7f;

		private const float DoubleTapWindow = 0.3f;

		private const float AnalogOnsetEpsilon = 0.01f;

		private const float AnalogRiseEpsilon = 0.01f;

		private const bool DiagStartTier = false;

		private int _tierDiagFrames;

		private float _tierDiagElapsed;

		private readonly GameObject _owner;

		private readonly CarInputManager _carInputManager;

		private readonly TouchMovePerson _touchMovePerson;

		private readonly PersonLocomotionConfig _config;

		private LocomotionInput _current;

		private bool _jumpLatched;

		private float _lastAnalogMag;

		private float _idleInputTimer = float.MaxValue;

		private float _settleTimer;

		private float _settlePeakMag;

		private bool _settleEngaged;

		private bool _onsetCommitted;

		private bool _analogRunLatched;

		private bool _analogSprintLatched;

		private bool _sprintLocked;

		private Vector2 _lockDir = Vector2.up;

		private float _lockArmTimer;

		private bool _releaseArmed;

		private float _offButtonTimer;

		private bool _lockWasDragging;

		private Vector2 _lockPendingDir = Vector2.up;

		private bool _crouchToggleLatched;

		private bool _crouchToggleState;

		private bool _walkMode;

		private bool _autoRun;

		private const float AutoRunCancelBackward = 0.5f;

		private float _moveInputDuration;

		private bool _moveReachedHold;

		private float _shuffleLatchTimer;

		private Vector2 _lastMove;

		private bool _lastSprintHeld;

		private bool _lastSprintTierHeld;

		private float _lastRunPressTime = -10f;

		private bool _sprintTierLatched;

		private bool _triggerHeldLast;

		private float _coastTimer;

		private bool _coastConsumed;

		private bool _coastActive;

		private bool _scriptedMoveActive;

		private Vector3 _scriptedMoveWorldDir;

		private float _scriptedMoveMagnitude;

		private bool _frozen;

		private readonly PersonDigitalOnsetSettle _desktopOnset = new PersonDigitalOnsetSettle();

		public LocomotionInput Current => _current;

		private static bool IsInputBlocked => false;

		public bool ScriptedMoveActive => _scriptedMoveActive;

		public bool WalkMode => _walkMode;

		public bool AutoRun => _autoRun;

		public PersonInputAdapter(GameObject owner, TouchMovePerson touchMovePerson, CarInputManager carInputManager, PersonLocomotionConfig config)
		{
			_owner = owner;
			_touchMovePerson = touchMovePerson;
			_carInputManager = carInputManager;
			_config = config;
		}

		public bool ConsumeJump()
		{
			if (!_jumpLatched)
			{
				return false;
			}
			_jumpLatched = false;
			return true;
		}

		public bool ConsumeCrouchToggle()
		{
			if (!_crouchToggleLatched)
			{
				return false;
			}
			_crouchToggleLatched = false;
			return true;
		}

		public void OnJumpButton()
		{
			_jumpLatched = true;
		}

		public void ResetPendingJump()
		{
			_jumpLatched = false;
			InputStorage.JumpRequested = false;
		}

		public void SetCrouch(bool toggle)
		{
			if (toggle)
			{
				_crouchToggleLatched = true;
			}
		}

		public void SetSprint(bool held)
		{
			_current.SprintHeld = held;
		}

		public void ToggleWalkMode()
		{
			_walkMode = !_walkMode;
		}

		public void SetWalkMode(bool walk)
		{
			_walkMode = walk;
		}

		public void ToggleAutoRun()
		{
			_autoRun = !_autoRun;
		}

		public void SetAutoRun(bool on)
		{
			_autoRun = on;
		}

		public void SetScriptedMoveWorld(Vector3 worldDir, float magnitude)
		{
			_scriptedMoveActive = true;
			_scriptedMoveWorldDir = worldDir;
			_scriptedMoveMagnitude = magnitude;
		}

		public void ClearScriptedMove()
		{
			_scriptedMoveActive = false;
		}

		public void CancelSprintLock()
		{
			ClearSprintLatches();
			ClearPublishedSprintFlags();
			_current.SprintHeld = false;
			_current.SprintTierHeld = false;
		}

		public void SetFrozen(bool frozen)
		{
			_frozen = frozen;
			if (_frozen)
			{
				ClearInputState();
			}
		}

		public void ResetAll()
		{
			ClearInputState();
			ClearPublishedInputFlags();
		}

		private void ClearPublishedInputFlags()
		{
			InputStorage.JumpRequested = false;
			InputStorage.CrouchRequested = false;
			InputStorage.PoseExitRequested = false;
			InputStorage.SprintLockCancelRequested = false;
			ClearPublishedSprintFlags();
		}

		private void ClearPublishedSprintFlags()
		{
			InputStorage.SprintLocked = false;
			InputStorage.SprintLockArmed = false;
			InputStorage.SprintLockArmProgress = 0f;
			InputStorage.SprintTierActive = false;
		}

		private void ClearSprintLatches()
		{
			_sprintTierLatched = false;
			_sprintLocked = false;
			_analogRunLatched = false;
			_analogSprintLatched = false;
			_releaseArmed = false;
			_lockArmTimer = 0f;
			_offButtonTimer = 0f;
			_lockWasDragging = false;
			_lastSprintHeld = false;
			_lastSprintTierHeld = false;
		}

		private void ClearInputState()
		{
			_current = default;
			_jumpLatched = false;
			_crouchToggleLatched = false;
			_crouchToggleState = false;
			_walkMode = false;
			_autoRun = false;
			ClearSprintLatches();
			_moveInputDuration = 0f;
			_moveReachedHold = false;
			_shuffleLatchTimer = 0f;
			_coastTimer = 0f;
			_coastActive = false;
			_coastConsumed = false;
			_lastMove = Vector2.zero;
			_scriptedMoveActive = false;
			ResetStartTierSettle();
		}

		private void ResetStartTierSettle()
		{
			_idleInputTimer = float.MaxValue;
			_settleTimer = 0f;
			_settlePeakMag = 0f;
			_settleEngaged = false;
			_onsetCommitted = false;
			_desktopOnset.Reset();
		}

		private void ApplyDesktopOnsetSettle()
		{
			float settleTime = (((bool)_config && !_scriptedMoveActive && !_autoRun) ? _config.KeyboardChordSettleTime : 0f);
			float coldIdleTime = (_config ? _config.MoveCoastTime : 0f);
			if (_desktopOnset.Hold(_current.HasMove, _current.Move.x, _current.Move.y, Time.deltaTime, settleTime, coldIdleTime))
			{
				SuppressOnset();
			}
		}

		public void ResetMoveState()
		{
			_scriptedMoveActive = false;
			_current.Move = Vector2.zero;
			_current.HasMove = false;
			_current.MovementInputTapped = false;
			_current.MovementInputPressed = false;
			_current.MovementInputHeld = false;
			_current.ShuffleRequested = false;
			_lastMove = Vector2.zero;
			_coastTimer = 0f;
			_coastConsumed = false;
			_coastActive = false;
			_moveInputDuration = 0f;
			_moveReachedHold = false;
			_shuffleLatchTimer = 0f;
			_analogRunLatched = false;
			_analogSprintLatched = false;
			_sprintTierLatched = false;
			ResetStartTierSettle();
		}

		public void Tick()
		{
			if (_owner != V.DriverPerson)
			{
				return;
			}
			if (_frozen || !FlagsStorage.OutOfCar)
			{
				ClearInputState();
				ClearPublishedInputFlags();
				return;
			}
			GatherMove();
			ApplyMoveCoast();
			ApplyStartTierSettle();
			ApplyDesktopOnsetSettle();
			UpdateMovementInputState();
			GatherActions();
			if (_coastActive)
			{
				_current.SprintHeld = _lastSprintHeld;
				_current.SprintTierHeld = _lastSprintTierHeld;
			}
			else if (_current.HasMove)
			{
				_lastSprintHeld = _current.SprintHeld;
				_lastSprintTierHeld = _current.SprintTierHeld;
			}
			GatherLook();
			if (IsInputBlocked)
			{
				_current.Move = Vector2.zero;
				_current.HasMove = false;
				_current.Look = Vector2.zero;
				_current.MovementInputTapped = false;
				_current.MovementInputPressed = false;
				_current.MovementInputHeld = false;
				_current.ShuffleRequested = false;
				_current.SprintHeld = false;
				_current.SprintTierHeld = false;
			}
			_current.CameraYaw = (_touchMovePerson ? _touchMovePerson.CameraYaw : 0f);
		}

		private void DiagnoseStartTier()
		{
			if (_current.MovementInputTapped)
			{
				_tierDiagFrames = 30;
				_tierDiagElapsed = 0f;
				Debug.Log($"[StartTier] TAPPED analogMag={_lastAnalogMag:F2} enter={ResolveRunDeflectionEnter():F2} " + $"runLatch={_analogRunLatched} sprintHeld={_current.SprintHeld} tierHeld={_current.SprintTierHeld} " + $"settleT={_settleTimer:F3} peak={_settlePeakMag:F2}");
			}
			if (_tierDiagFrames > 0)
			{
				_tierDiagFrames--;
				_tierDiagElapsed += Time.deltaTime;
				Debug.Log($"[StartTier] +{_tierDiagElapsed * 1000f:F0}ms analogMag={_lastAnalogMag:F2} " + $"runLatch={_analogRunLatched} sprintHeld={_current.SprintHeld} hasMove={_current.HasMove}");
			}
		}

		private void ApplyStartTierSettle()
		{
			if (!_current.HasMove)
			{
				_idleInputTimer += Time.deltaTime;
				_settleEngaged = false;
				_onsetCommitted = false;
				_settleTimer = 0f;
				_settlePeakMag = 0f;
				return;
			}
			if (_onsetCommitted)
			{
				_idleInputTimer = 0f;
				return;
			}
			float num = (_config ? _config.StartTierSettleTime : 0f);
			float num2 = ResolveRunDeflectionEnter();
			if (!_settleEngaged)
			{
				bool flag = _lastAnalogMag > 0.01f && _lastAnalogMag < num2;
				bool flag2 = _idleInputTimer > (_config ? _config.MoveCoastTime : 0f);
				if (num <= 0f || !flag || !flag2)
				{
					CommitOnset();
					return;
				}
				_settleEngaged = true;
				_settleTimer = 0f;
				_settlePeakMag = _lastAnalogMag;
				SuppressOnset();
			}
			else
			{
				_settleTimer += Time.deltaTime;
				bool num3 = _lastAnalogMag >= num2;
				bool flag3 = _lastAnalogMag <= _settlePeakMag + 0.01f;
				_settlePeakMag = Mathf.Max(_settlePeakMag, _lastAnalogMag);
				if ((num3 | flag3) || _settleTimer >= num)
				{
					CommitOnset();
				}
				else
				{
					SuppressOnset();
				}
			}
		}

		private float ResolveRunDeflectionEnter()
		{
			if ((bool)_config && _config.SprintActivation == SprintActivationMode.Deflection)
			{
				if (!_config)
				{
					return 0.55f;
				}
				return _config.DeflectionRunEnter;
			}
			if (!_config)
			{
				return 0.9f;
			}
			return _config.RunDeflectionEnter;
		}

		private void CommitOnset()
		{
			_onsetCommitted = true;
			_settleEngaged = false;
			_idleInputTimer = 0f;
		}

		private void SuppressOnset()
		{
			_current.HasMove = false;
			_current.Move = Vector2.zero;
		}

		private void ApplyMoveCoast()
		{
			if (_current.HasMove)
			{
				_lastMove = _current.Move;
				_coastTimer = 0f;
				_coastConsumed = false;
				_coastActive = false;
				return;
			}
			_coastActive = false;
			float num = (((bool)_config && !_config.InstantStopEnabled) ? _config.MoveCoastTime : 0f);
			if (num <= 0f)
			{
				return;
			}
			if (_coastTimer <= 0f && !_coastConsumed && _current.MovementInputHeld)
			{
				_coastTimer = num;
			}
			if (_coastTimer > 0f)
			{
				_coastTimer -= Time.deltaTime;
				_current.Move = _lastMove;
				_current.HasMove = true;
				_coastActive = true;
				if (_coastTimer <= 0f)
				{
					_coastConsumed = true;
				}
			}
		}

		private void UpdateMovementInputState()
		{
			if (_current.HasMove)
			{
				float num = (_config ? _config.ButtonHoldThreshold : 0.15f);
				if (_moveInputDuration == 0f)
				{
					_current.MovementInputTapped = true;
					_current.MovementInputPressed = false;
					_current.MovementInputHeld = false;
				}
				else if (_moveInputDuration < num)
				{
					_current.MovementInputTapped = false;
					_current.MovementInputPressed = true;
					_current.MovementInputHeld = false;
				}
				else
				{
					_current.MovementInputTapped = false;
					_current.MovementInputPressed = false;
					_current.MovementInputHeld = true;
					_moveReachedHold = true;
				}
				_moveInputDuration += Time.deltaTime;
				_shuffleLatchTimer = 0f;
			}
			else
			{
				if (_moveInputDuration > 0f && !_moveReachedHold)
				{
					_shuffleLatchTimer = (_config ? _config.ShuffleLatchDuration : 0.3f);
				}
				_moveInputDuration = 0f;
				_moveReachedHold = false;
				_current.MovementInputTapped = false;
				_current.MovementInputPressed = false;
				_current.MovementInputHeld = false;
			}
			if (_shuffleLatchTimer > 0f)
			{
				_shuffleLatchTimer -= Time.deltaTime;
			}
			_current.ShuffleRequested = _shuffleLatchTimer > 0f;
		}

		private void GatherMove()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if ((bool)_touchMovePerson)
			{
				Vector2 soapMove = _touchMovePerson.SoapMove;
				num2 = soapMove.x;
				num = soapMove.y;
				if (_autoRun)
				{
					if (num < -0.5f)
					{
						_autoRun = false;
					}
					else
					{
						num = 1f;
					}
				}
			}
			if (!(Mathf.Abs(num) > 0.01f) && !(Mathf.Abs(num2) > 0.01f))
			{
				num = InputStorage.LeftJoyInputY;
				num2 = InputStorage.LeftJoyInputX;
			}
			if (Mathf.Abs(num) < 0.01f && Mathf.Abs(num2) < 0.01f && (bool)_carInputManager && !InputStorage.GamepadActive)
			{
				num = _carInputManager.GetPersonForward - _carInputManager.GetPersonBackward;
				num2 = _carInputManager.GetPersonLeftRight;
			}
			_current.Move = Vector2.ClampMagnitude(new Vector2(num2, num), 1f);
			_current.HasMove = _current.Move.sqrMagnitude > 0.0025f;
			bool flag = (bool)_config && _config.SprintActivation == SprintActivationMode.Deflection;
			float num4 = ((!flag) ? (_config ? _config.RunDeflectionEnter : 0.9f) : (_config ? _config.DeflectionRunEnter : 0.55f));
			float num5 = ((!flag) ? (_config ? _config.RunDeflectionExit : 0.75f) : (_config ? _config.DeflectionRunExit : 0.45f));
			_lastAnalogMag = num3;
			if (num3 > num4)
			{
				_analogRunLatched = true;
			}
			else if (num3 < num5)
			{
				_analogRunLatched = false;
			}
			float num6 = (_config ? _config.SprintDeflectionEnter : 0.88f);
			float num7 = (_config ? _config.SprintDeflectionExit : 0.75f);
			if (flag)
			{
				if (num3 > num6)
				{
					_analogSprintLatched = true;
				}
				else if (num3 < num7)
				{
					_analogSprintLatched = false;
				}
			}
			else
			{
				_analogSprintLatched = false;
			}
			UpdateSprintLock(flag, num3, num6, num5);
			if (_scriptedMoveActive)
			{
				float num8 = (_touchMovePerson ? _touchMovePerson.CameraYaw : 0f);
				Vector3 vector = Quaternion.Euler(0f, 0f - num8, 0f) * _scriptedMoveWorldDir.normalized;
				_current.Move = new Vector2(vector.x, vector.z) * _scriptedMoveMagnitude;
				_current.HasMove = _current.Move.sqrMagnitude > 0.0025f;
			}
		}

		private void UpdateSprintLock(bool deflectionMode, float analogMag, float sprintEnter, float runExit)
		{
			if (!deflectionMode || !_config || !_config.SprintLockEnabled)
			{
				_sprintLocked = false;
				_lockArmTimer = 0f;
				_releaseArmed = false;
				_offButtonTimer = 0f;
				_lockWasDragging = false;
				InputStorage.SprintLockCancelRequested = false;
				InputStorage.SprintLocked = false;
				InputStorage.SprintLockArmed = false;
				InputStorage.SprintLockArmProgress = 0f;
				return;
			}
			float num = (_config ? _config.SprintLockMinHoldTime : 0.6f);
			bool leftJoyIsDrag = InputStorage.LeftJoyIsDrag;
			bool sprintLockAiming = InputStorage.SprintLockAiming;
			if (sprintLockAiming)
			{
				_lockArmTimer += Time.deltaTime;
				_offButtonTimer = 0f;
			}
			else
			{
				_lockArmTimer = 0f;
				if (leftJoyIsDrag)
				{
					_offButtonTimer += Time.deltaTime;
				}
			}
			if (sprintLockAiming && _lockArmTimer >= num)
			{
				_releaseArmed = true;
			}
			if (_offButtonTimer > 0.18f)
			{
				_releaseArmed = false;
			}
			if (leftJoyIsDrag && _current.HasMove)
			{
				_lockPendingDir = _current.Move.normalized;
			}
			if (_lockWasDragging && !leftJoyIsDrag)
			{
				if (_releaseArmed)
				{
					_sprintLocked = true;
				}
				if (_sprintLocked && _lockPendingDir.sqrMagnitude > 0.0001f)
				{
					_lockDir = _lockPendingDir;
				}
				_releaseArmed = false;
				_offButtonTimer = 0f;
			}
			if ((_sprintLocked & leftJoyIsDrag) && analogMag < runExit)
			{
				_sprintLocked = false;
			}
			if (InputStorage.SprintLockCancelRequested)
			{
				_sprintLocked = false;
			}
			InputStorage.SprintLockCancelRequested = false;
			if (_sprintLocked && !leftJoyIsDrag)
			{
				_current.Move = _lockDir;
				_current.HasMove = true;
			}
			_lockWasDragging = leftJoyIsDrag;
			InputStorage.SprintLocked = _sprintLocked;
			InputStorage.SprintLockArmed = _releaseArmed;
			InputStorage.SprintLockArmProgress = Mathf.Clamp01(_lockArmTimer / Mathf.Max(num, 0.0001f));
		}

		private void GatherActions()
		{
			bool triggerHeldLast = false;
			if ((bool)_touchMovePerson)
			{
				if (_touchMovePerson.ConsumeJump())
				{
					_jumpLatched = true;
				}
				if (_touchMovePerson.ConsumeCrouch())
				{
					_crouchToggleLatched = true;
					_crouchToggleState = !_crouchToggleState;
				}
				if (_touchMovePerson.ConsumeWalkToggle())
				{
					_walkMode = !_walkMode;
				}
				triggerHeldLast = _touchMovePerson.RunHeld;
			}
			if (InputStorage.JumpRequested)
			{
				InputStorage.JumpRequested = false;
				_jumpLatched = true;
			}
			if (InputStorage.CrouchRequested)
			{
				InputStorage.CrouchRequested = false;
				_crouchToggleLatched = true;
				_crouchToggleState = !_crouchToggleState;
			}
			bool crouchToggleState = _crouchToggleState;
			bool flag = (_triggerHeldLast = triggerHeldLast);
			triggerHeldLast = (!_walkMode | flag) || _autoRun;
			_sprintTierLatched = flag || _autoRun;
			if (false)
			{
				if (Time.time - _lastRunPressTime <= 0.3f)
				{
					_sprintTierLatched = true;
				}
				_lastRunPressTime = Time.time;
			}
			if (!triggerHeldLast)
			{
				_sprintTierLatched = false;
			}
			bool flag2 = (bool)_config && _config.SprintActivation == SprintActivationMode.Deflection;
			_current.CrouchHeld = crouchToggleState;
			_current.SprintHeld = triggerHeldLast || _analogRunLatched;
			_current.SprintTierHeld = _sprintTierLatched || (!flag2 && InputStorage.SprintHeld) || (flag2 && _analogSprintLatched) || _sprintLocked;
			if (_scriptedMoveActive)
			{
				_current.SprintHeld = false;
				_current.SprintTierHeld = false;
			}
			InputStorage.SprintTierActive = _current.SprintTierHeld;
			InputStorage.DeflectionSprintMode = flag2;
		}

		private void GatherLook()
		{
			_current.Look = new Vector2(InputStorage.RightJoyInputX, InputStorage.RightJoyInputY);
		}
	}
}

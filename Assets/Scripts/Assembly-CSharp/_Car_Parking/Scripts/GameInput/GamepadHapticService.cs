using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;
using _Car_Parking.Scripts.GameInput.Settings;

namespace _Car_Parking.Scripts.GameInput
{
	public class GamepadHapticService : ITickable, IDisposable
	{
		public const float PulseStrength = 0.25f;

		public const float PulseSeconds = 0.05f;

		private const float PulseHighFreqRatio = 1f;

		private const float ApplyEpsilon = 0.02f;

		private readonly DesktopControlsSettingsSO _settings;

		private readonly HapticConfigSO _config;

		private readonly GamepadMirrorTracker _mirrors;

		private float _continuousLeft;

		private float _continuousRight;

		private float _continuousModHz;

		private float _pulseLeft;

		private float _pulseRight;

		private float _pulseSustainEndUnscaled;

		private float _pulseEndUnscaled;

		private float _pulseFadeSeconds;

		private float _pulseModHz;

		private float _lastLeft = -1f;

		private float _lastRight = -1f;

		private bool _focused = true;

		private CancellationTokenSource _engineCts;

		private Gamepad _rumbleTarget;

		private readonly PlayStationBluetoothRumble _playStationBluetooth = new PlayStationBluetoothRumble();

		private readonly SwitchProRumble _switchPro = new SwitchProRumble();

		private bool _loggedBluetoothSendFailure;

		private bool Enabled
		{
			get
			{
				if (_focused)
				{
					if (!(_settings == null))
					{
						return _settings.VibrationEnabled;
					}
					return true;
				}
				return false;
			}
		}

		public GamepadHapticService(DesktopControlsSettingsSO settings, HapticConfigSO config, GamepadMirrorTracker mirrors)
		{
			_settings = settings;
			_config = config;
			_mirrors = mirrors;
			Application.focusChanged += OnFocusChanged;
		}

		public void PulseUiNavigate()
		{
			PulseFrom(_config ? _config.UiNavigate : default(HapticConfigSO.Pulse));
		}

		public void PulseGearChange()
		{
			PulseFrom(_config ? _config.GearChange : default(HapticConfigSO.Pulse));
		}

		public void PulseCircularHover()
		{
			PulseFrom(_config ? _config.CircularHover : default(HapticConfigSO.Pulse));
		}

		private void PulseFrom(HapticConfigSO.Pulse p)
		{
			if (_config == null)
			{
				Pulse();
			}
			else
			{
				Pulse(p.Strength, p.Seconds, p.HighFreqRatio, p.FadeSeconds, p.ModulationHz);
			}
		}

		public void PulseCollision(float impact01)
		{
			if (_config == null)
			{
				Pulse();
				return;
			}
			HapticConfigSO.Pulse collision = _config.Collision;
			Pulse(collision.Strength * Mathf.Clamp01(impact01), collision.Seconds, collision.HighFreqRatio, collision.FadeSeconds, collision.ModulationHz);
		}

		public void PulseEngineStart(Func<bool> engineRunning = null)
		{
			if (_config == null)
			{
				Pulse();
				return;
			}
			_engineCts?.Cancel();
			_engineCts?.Dispose();
			_engineCts = new CancellationTokenSource();
			PlayEngineStartAsync(engineRunning, _engineCts.Token).Forget();
		}

		private async UniTaskVoid PlayEngineStartAsync(Func<bool> engineRunning, CancellationToken ct)
		{
			if (_config.EngineStartDelaySeconds > 0f)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(_config.EngineStartDelaySeconds), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, ct);
			}
			float crankDeadline = Time.unscaledTime + Mathf.Max(0.1f, _config.EngineCrankMaxSeconds);
			int chug = 0;
			while ((engineRunning != null) ? (!engineRunning()) : (chug < _config.EngineCrankPulses))
			{
				if (engineRunning != null && Time.unscaledTime > crankDeadline)
				{
					return;
				}
				Pulse(_config.EngineCrankStrength, _config.EngineCrankPulseSeconds, _config.EngineCrankHighFreqRatio, 0f, _config.EngineCrankModulationHz);
				chug++;
				await UniTask.Delay(TimeSpan.FromSeconds(_config.EngineCrankInterval), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, ct);
			}
			Pulse(_config.EngineFireStrength, _config.EngineFireSeconds, _config.EngineFireHighFreqRatio, _config.EngineFireFadeSeconds, _config.EngineFireModulationHz);
		}

		public void SetContinuous(float left, float right, float modulationHz = 0f)
		{
			_continuousLeft = left;
			_continuousRight = right;
			_continuousModHz = modulationHz;
		}

		public void Pulse(float strength = 0.25f, float seconds = 0.05f, float highFreqRatio = 1f, float fadeSeconds = 0f, float modulationHz = 0f)
		{
			if (Enabled)
			{
				_pulseLeft = strength;
				_pulseRight = strength * Mathf.Clamp01(highFreqRatio);
				float unscaledTime = Time.unscaledTime;
				_pulseSustainEndUnscaled = unscaledTime + seconds;
				_pulseFadeSeconds = Mathf.Max(0f, fadeSeconds);
				_pulseEndUnscaled = _pulseSustainEndUnscaled + _pulseFadeSeconds;
				_pulseModHz = modulationHz;
			}
		}

		public void Tick()
		{
			_switchPro.Update(Time.unscaledTime);
			Gamepad preferred = _mirrors.GetPreferred();
			if (!Enabled)
			{
				Apply(preferred, 0f, 0f);
				return;
			}
			float num = (_settings ? _settings.VibrationIntensity : 1f);
			float unscaledTime = Time.unscaledTime;
			float b = 0f;
			float b2 = 0f;
			if (unscaledTime < _pulseEndUnscaled)
			{
				float num2 = 1f;
				if (_pulseFadeSeconds > 0f && unscaledTime > _pulseSustainEndUnscaled)
				{
					num2 = Mathf.Clamp01((_pulseEndUnscaled - unscaledTime) / _pulseFadeSeconds);
				}
				if (GateOpen(_pulseModHz, unscaledTime))
				{
					b = _pulseLeft * num2;
					b2 = _pulseRight * num2;
				}
			}
			float a = _continuousLeft;
			float a2 = _continuousRight;
			if (!GateOpen(_continuousModHz, unscaledTime))
			{
				a = 0f;
				a2 = 0f;
			}
			float left = Mathf.Clamp01(Mathf.Max(a, b) * num);
			float right = Mathf.Clamp01(Mathf.Max(a2, b2) * num);
			Apply(preferred, left, right);
		}

		private static bool GateOpen(float hz, float now)
		{
			if (hz <= 0.01f)
			{
				return true;
			}
			float num = 0.5f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
			if (hz >= num)
			{
				return true;
			}
			return now * hz % 1f < 0.5f;
		}

		private void OnFocusChanged(bool focused)
		{
			_focused = focused;
			if (!focused)
			{
				Apply(_mirrors.GetPreferred(), 0f, 0f);
			}
		}

		private void Apply(Gamepad gamepad, float left, float right)
		{
			gamepad = ResolveRumbleOutput(gamepad);
			if (gamepad != null)
			{
				if (gamepad != _rumbleTarget)
				{
					SwitchRumbleTarget(gamepad);
				}
				if (left < 0.02f)
				{
					left = 0f;
				}
				if (right < 0.02f)
				{
					right = 0f;
				}
				if (!(Mathf.Abs(left - _lastLeft) < 0.02f) || !(Mathf.Abs(right - _lastRight) < 0.02f))
				{
					_lastLeft = left;
					_lastRight = right;
					SetMotors(gamepad, left, right);
				}
			}
		}

		private Gamepad ResolveRumbleOutput(Gamepad gamepad)
		{
			return _mirrors.GetMirrorOf(gamepad) ?? gamepad;
		}

		private void SwitchRumbleTarget(Gamepad gamepad)
		{
			if (_rumbleTarget != null && _rumbleTarget.added)
			{
				SetMotors(_rumbleTarget, 0f, 0f);
			}
			_rumbleTarget = gamepad;
			_lastLeft = -1f;
			_lastRight = -1f;
		}

		private void SetMotors(Gamepad gamepad, float left, float right)
		{
			if (_switchPro.Handles(gamepad))
			{
				_switchPro.Set(gamepad, left, right, Time.unscaledTime);
			}
			else if (_playStationBluetooth.Handles(gamepad))
			{
				if (!_playStationBluetooth.Send(gamepad, left, right) && !_loggedBluetoothSendFailure)
				{
					_loggedBluetoothSendFailure = true;
				}
			}
			else
			{
				gamepad.SetMotorSpeeds(left, right);
			}
		}

		public void Dispose()
		{
			_engineCts?.Cancel();
			_engineCts?.Dispose();
			Application.focusChanged -= OnFocusChanged;
			if (_rumbleTarget != null && _rumbleTarget.added)
			{
				SetMotors(_rumbleTarget, 0f, 0f);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class RebindController : IDisposable
	{
		private sealed class Session : IDisposable
		{
			private RebindController _owner;

			public Session(RebindController owner)
			{
				_owner = owner;
			}

			public void Dispose()
			{
				_owner?.ExitSession();
				_owner = null;
			}
		}

		private const float CaptureTimeoutSeconds = 10f;

		private const float SettleMinSeconds = 0.2f;

		private const float SettleMaxSeconds = 1.5f;

		private readonly InputMapSetEvent _inputMapSetEvent;

		private readonly EventRegistry _eventRegistry;

		private readonly CancellationTokenSource _lifetimeCts = new CancellationTokenSource();

		private RegistryHandle<RebindCaptureStateEvent> _captureStateHandle = new RegistryHandle<RebindCaptureStateEvent>(EventRegistryIDs.BindingCaptureState);

		private int _sessionDepth;

		private bool _sessionPushedNoInput;

		private readonly List<ButtonControl> _heldInSession = new List<ButtonControl>();

		private Action<InputEventPtr, InputDevice> _trackHeld;

		public bool IsCapturing { get; private set; }

		public RebindController(InputMapSetEvent inputMapSetEvent, EventRegistry eventRegistry)
		{
			_inputMapSetEvent = inputMapSetEvent;
			_eventRegistry = eventRegistry;
		}

		public IDisposable EnterSession()
		{
			if (_sessionDepth++ == 0)
			{
				_sessionPushedNoInput = PushNoInput();
				_heldInSession.Clear();
				if (_trackHeld == null)
				{
					_trackHeld = TrackHeld;
				}
				InputSystem.onEvent += _trackHeld;
			}
			return new Session(this);
		}

		private void ExitSession()
		{
			if (_sessionDepth != 0 && --_sessionDepth <= 0)
			{
				InputSystem.onEvent -= _trackHeld;
				_heldInSession.Clear();
				PopNoInput(_sessionPushedNoInput);
				_sessionPushedNoInput = false;
			}
		}

		public async UniTask SettleAsync(CancellationToken ct)
		{
			if (_sessionDepth == 0)
			{
				return;
			}
			double start = Time.realtimeSinceStartupAsDouble;
			while (!ct.IsCancellationRequested)
			{
				double num = Time.realtimeSinceStartupAsDouble - start;
				if (num >= 1.5 || (num >= 0.20000000298023224 && _heldInSession.Count == 0))
				{
					break;
				}
				await UniTask.Yield(PlayerLoopTiming.Update);
			}
		}

		private void TrackHeld(InputEventPtr eventPtr, InputDevice device)
		{
			if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
			{
				return;
			}
			for (int num = _heldInSession.Count - 1; num >= 0; num--)
			{
				ButtonControl buttonControl = _heldInSession[num];
				if (buttonControl.device == device && buttonControl.ReadValueFromEvent(eventPtr, out var value) && !buttonControl.IsValueConsideredPressed(value))
				{
					_heldInSession.RemoveAt(num);
				}
			}
			foreach (InputControl item in eventPtr.EnumerateChangedControls(device))
			{
				if (item is ButtonControl { synthetic: false } buttonControl2 && !_heldInSession.Contains(buttonControl2) && buttonControl2.ReadValueFromEvent(eventPtr, out var value2) && buttonControl2.IsValueConsideredPressed(value2))
				{
					_heldInSession.Add(buttonControl2);
				}
			}
		}

		public async UniTask<CaptureResult> CaptureKeysAsync(RebindDeviceProfile profile, Func<ChordCapture, bool> accept, Action<ChordCapture> onRejected, Action<IReadOnlyList<string>> onKeysChanged, CancellationToken ct)
		{
			if (IsCapturing)
			{
				return CaptureResult.Busy;
			}
			IsCapturing = true;
			using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _lifetimeCts.Token);
			IDisposable session = EnterSession();
			KeyCaptureListener listener = null;
			RaiseState(RebindCaptureStateEnum.Listening);
			try
			{
				listener = new KeyCaptureListener(profile);
				listener.Start();
				int seenVersion = listener.Tracker.Version;
				double deadline = Time.realtimeSinceStartupAsDouble + 10.0;
				while (true)
				{
					await UniTask.Yield(PlayerLoopTiming.Update);
					double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
					if (linked.IsCancellationRequested || listener.Cancelled)
					{
						return CaptureResult.Cancelled;
					}
					listener.Tick(realtimeSinceStartupAsDouble);
					ChordTracker tracker = listener.Tracker;
					if (tracker.Version != seenVersion)
					{
						seenVersion = tracker.Version;
						deadline = realtimeSinceStartupAsDouble + 10.0;
						onKeysChanged?.Invoke(tracker.Candidate);
					}
					if (tracker.IsComplete)
					{
						ChordCapture chordCapture = new ChordCapture(tracker.Candidate.ToArray());
						if (accept == null || accept(chordCapture))
						{
							return CaptureResult.Captured(chordCapture.Key, chordCapture.Modifier);
						}
						if (profile.IsCancelChord(chordCapture.Keys))
						{
							return CaptureResult.Cancelled;
						}
						onRejected?.Invoke(chordCapture);
						listener.Restart();
						deadline = realtimeSinceStartupAsDouble + 10.0;
					}
					else if (realtimeSinceStartupAsDouble > deadline)
					{
						break;
					}
				}
				return CaptureResult.Cancelled;
			}
			finally
			{
				listener?.Dispose();
				session.Dispose();
				IsCapturing = false;
				RaiseState(RebindCaptureStateEnum.Idle);
			}
		}

		public async UniTask<CaptureResult> CaptureAxisAsync(RebindDeviceProfile profile, CaptureKind kind, AxisSemantic semantic, Func<CaptureResult, bool> accept, Action<CaptureResult> onRejected, CancellationToken ct)
		{
			if (IsCapturing)
			{
				return CaptureResult.Busy;
			}
			IsCapturing = true;
			using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _lifetimeCts.Token);
			IDisposable session = EnterSession();
			RaiseState(RebindCaptureStateEnum.Listening);
			try
			{
				CaptureResult captureResult;
				while (true)
				{
					captureResult = await PollAxisOnceAsync(profile, kind, semantic, linked.Token);
					if (captureResult.Status != CaptureStatus.Captured || accept == null || accept(captureResult))
					{
						break;
					}
					onRejected?.Invoke(captureResult);
				}
				return captureResult;
			}
			finally
			{
				session.Dispose();
				IsCapturing = false;
				RaiseState(RebindCaptureStateEnum.Idle);
			}
		}

		public async UniTask<CaptureResult> CaptureAnalogComboAsync(RebindDeviceProfile profile, CaptureKind kind, AxisSemantic semantic, Func<CaptureResult, bool> accept, Action<CaptureResult> onRejected, Action<IReadOnlyList<string>> onKeysChanged, CancellationToken ct)
		{
			if (IsCapturing)
			{
				return CaptureResult.Busy;
			}
			IsCapturing = true;
			using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _lifetimeCts.Token);
			IDisposable session = EnterSession();
			KeyCaptureListener listener = null;
			RaiseState(RebindCaptureStateEnum.Listening);
			try
			{
				AxisControlPoller poller = new AxisControlPoller(profile, kind);
				listener = new KeyCaptureListener(profile, poller);
				if (poller.Count == 0)
				{
					return CaptureResult.Cancelled;
				}
				AxisCaptureAnalyzer analyzer = new AxisCaptureAnalyzer(semantic);
				analyzer.Begin(poller.Read(), Time.realtimeSinceStartupAsDouble);
				poller.UseEvents();
				listener.Start();
				AxisCaptureResult? settled = null;
				int seenVersion = listener.Tracker.Version;
				double deadline = Time.realtimeSinceStartupAsDouble + 10.0;
				double realtimeSinceStartupAsDouble;
				do
				{
					IL_012b:
					await UniTask.Yield(PlayerLoopTiming.Update);
					realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
					if (linked.IsCancellationRequested || listener.Cancelled)
					{
						return CaptureResult.Cancelled;
					}
					ChordTracker tracker = listener.Tracker;
					if (!settled.HasValue && tracker.IsComplete)
					{
						if (profile.IsCancelChord(tracker.Candidate))
						{
							return CaptureResult.Cancelled;
						}
						listener.Restart();
					}
					if (tracker.Version != seenVersion)
					{
						seenVersion = tracker.Version;
						deadline = realtimeSinceStartupAsDouble + 10.0;
						onKeysChanged?.Invoke(tracker.Candidate);
					}
					IReadOnlyList<float> values = poller.Read();
					if (!settled.HasValue && analyzer.Feed(values, realtimeSinceStartupAsDouble, out var result))
					{
						settled = result;
					}
					if (!settled.HasValue)
					{
						continue;
					}
					AxisCaptureResult valueOrDefault = settled.GetValueOrDefault();
					if (!tracker.IsHolding)
					{
						IReadOnlyList<string> candidate = tracker.Candidate;
						CaptureResult captureResult = CaptureResult.CapturedAxis(poller.PathOf(valueOrDefault.Index), valueOrDefault.Invert, valueOrDefault.NeedsRemap, (candidate.Count == 1) ? candidate[0] : null, candidate.Count);
						if (accept == null || accept(captureResult))
						{
							return captureResult;
						}
						onRejected?.Invoke(captureResult);
						listener.Restart();
						analyzer.Begin(poller.Read(), realtimeSinceStartupAsDouble);
						settled = null;
						deadline = realtimeSinceStartupAsDouble + 10.0;
						goto IL_012b;
					}
				}
				while (!(realtimeSinceStartupAsDouble > deadline));
				return CaptureResult.Cancelled;
			}
			finally
			{
				listener?.Dispose();
				session.Dispose();
				IsCapturing = false;
				RaiseState(RebindCaptureStateEnum.Idle);
			}
		}

		private static async UniTask<CaptureResult> PollAxisOnceAsync(RebindDeviceProfile profile, CaptureKind kind, AxisSemantic semantic, CancellationToken ct)
		{
			AxisControlPoller poller = new AxisControlPoller(profile, kind);
			if (poller.Count == 0)
			{
				return CaptureResult.Cancelled;
			}
			AxisCaptureAnalyzer analyzer = new AxisCaptureAnalyzer(semantic);
			analyzer.Begin(poller.Read(), Time.realtimeSinceStartupAsDouble);
			double deadline = Time.realtimeSinceStartupAsDouble + 10.0;
			InputControlList<InputControl> cancels = FindCancelControls(profile, 0);
			try
			{
				double realtimeSinceStartupAsDouble;
				AxisCaptureResult result;
				do
				{
					await UniTask.Yield(PlayerLoopTiming.Update);
					realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
					if (ct.IsCancellationRequested || realtimeSinceStartupAsDouble > deadline || AnyPressedThisFrame(cancels))
					{
						return CaptureResult.Cancelled;
					}
				}
				while (!analyzer.Feed(poller.Read(), realtimeSinceStartupAsDouble, out result));
				return CaptureResult.CapturedAxis(poller.PathOf(result.Index), result.Invert, result.NeedsRemap);
			}
			finally
			{
				cancels.Dispose();
			}
		}

		private static InputControlList<InputControl> FindCancelControls(RebindDeviceProfile profile, int firstIndex)
		{
			InputControlList<InputControl> result = new InputControlList<InputControl>(Allocator.Persistent);
			for (int i = firstIndex; i < profile.CancelPaths.Count; i++)
			{
				using InputControlList<InputControl> inputControlList = InputSystem.FindControls(profile.CancelPaths[i]);
				result.AddRange(inputControlList);
			}
			return result;
		}

		private static bool AnyPressedThisFrame(InputControlList<InputControl> controls)
		{
			foreach (InputControl item in controls)
			{
				if (item is ButtonControl { wasPressedThisFrame: not false })
				{
					return true;
				}
			}
			return false;
		}

		private bool PushNoInput()
		{
			if (_inputMapSetEvent.CurrentInputGroup == InputGroupsEnum.NoInput)
			{
				return false;
			}
			if (_inputMapSetEvent.TryGetPreviousMaps(out var maps) && maps != null && !maps.Any())
			{
				return false;
			}
			return _inputMapSetEvent.TrySetGroup(InputGroupsEnum.NoInput);
		}

		private void PopNoInput(bool pushed)
		{
			if (pushed && _inputMapSetEvent.CurrentInputGroup == InputGroupsEnum.NoInput)
			{
				_inputMapSetEvent.TrySetPreviousGroup();
			}
		}

		private void RaiseState(RebindCaptureStateEnum state)
		{
			_captureStateHandle.Get(_eventRegistry)?.Raise(state);
		}

		public void Dispose()
		{
			if (_sessionDepth > 0 && _trackHeld != null)
			{
				InputSystem.onEvent -= _trackHeld;
			}
			_lifetimeCts.Cancel();
			_lifetimeCts.Dispose();
		}
	}
}

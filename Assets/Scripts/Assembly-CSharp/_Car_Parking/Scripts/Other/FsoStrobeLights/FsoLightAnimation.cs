using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	public class FsoLightAnimation : MonoBehaviour
	{
		public const int ToggleModeMaxCount = 9;

		private const float TimeHigh = 1f;

		private const float TimeMid = 0.2f;

		private const float TimeLow = 0.1f;

		private const int AnimationCount = 3;

		private readonly int _emissionColorID = Shader.PropertyToID("_EmissionColor");

		public GameObject _child;

		public GameObject _glowObject;

		private FsoStrobeLights _fsoStrobeLights;

		private CancellationTokenSource _animationCts;

		private Func<CancellationToken, UniTask> _pendingAnimation;

		private int _toggleModeIndex;

		private int _strobeIndex;

		private Sequence _animationSequence;

		private int _strobeCount;

		private Material _material;

		private Material _glowMaterial;

		private Color _fsoColor;

		private FsoStrobeLightsConfig _config;

		public GameObject Child
		{
			get
			{
				if (_child == null)
				{
					_child = base.transform.GetChild(0).gameObject;
				}
				return _child;
			}
		}

		public Color FsoColor
		{
			get
			{
				return _fsoColor;
			}
			set
			{
				_fsoColor = value;
				SetAllFsoColor(_fsoColor);
			}
		}

		private void SetAllFsoColor(Color materialColor)
		{
			_material.color = materialColor;
			float fsoColorHdrIntencity = _config.FsoColorHdrIntencity;
			float num = Mathf.Pow(2f, fsoColorHdrIntencity);
			_material.SetColor(_emissionColorID, materialColor * num);
			materialColor.a = ((materialColor == Color.black) ? 0f : 1f);
			_glowMaterial.color = materialColor;
		}

		private Color GetCurrentFsoColor()
		{
			return _material.color;
		}

		public void Init(int strobeIndex, int strobeCount, FsoStrobeLightsConfig config, FsoStrobeLights fsoStrobeLights)
		{
			_config = config;
			_strobeIndex = strobeIndex;
			_strobeCount = strobeCount;
			_material = Child.GetComponent<Renderer>().material;
			_fsoStrobeLights = fsoStrobeLights;
			Child.gameObject.SetActive(value: true);
			if (_glowObject == null)
			{
				_glowObject = base.transform.GetChild(1).gameObject;
			}
			_glowObject.SetActive(value: true);
			_glowMaterial = _glowObject.GetComponent<Renderer>().material;
		}

		private void OnEnable()
		{
			if (_pendingAnimation != null)
			{
				RunPendingAnimation();
			}
		}

		private void OnDisable()
		{
			CancelAnimation();
		}

		private void OnDestroy()
		{
			CancelAnimation();
		}

		public void Toggle(int mode)
		{
			_toggleModeIndex = mode;
			StartAnimation();
		}

		private void CancelAnimation()
		{
			_animationCts?.Cancel();
			_animationCts?.Dispose();
			_animationCts = null;
		}

		private void RunPendingAnimation()
		{
			CancelAnimation();
			if (_pendingAnimation != null && base.gameObject.activeInHierarchy)
			{
				_animationCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
				RunAnimationAsync(_pendingAnimation, _animationCts.Token).Forget();
			}
		}

		private static async UniTask RunAnimationAsync(Func<CancellationToken, UniTask> anim, CancellationToken ct)
		{
			try
			{
				await anim(ct);
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void StartAnimation()
		{
			CancelAnimation();
			_pendingAnimation = null;
			if (_animationSequence != null)
			{
				_animationSequence.Kill();
				_animationSequence = null;
			}
			SetLedEnable(value: true);
			switch (_toggleModeIndex)
			{
			case 1:
				_pendingAnimation = Animation1Async;
				break;
			case 2:
				_pendingAnimation = Animation2Async;
				break;
			case 3:
				_pendingAnimation = Animation3Async;
				break;
			case 4:
				_pendingAnimation = Animation4Async;
				break;
			case 5:
				_pendingAnimation = Animation5Async;
				break;
			case 6:
				_pendingAnimation = (CancellationToken ct) => Animation6Async(_strobeIndex, ct);
				break;
			case 7:
				_pendingAnimation = (CancellationToken ct) => Animation7Async(_strobeIndex, _strobeCount, ct);
				break;
			case 8:
				_pendingAnimation = Animation8Async;
				break;
			case 9:
				_pendingAnimation = (CancellationToken ct) => Animation9Async(_strobeIndex, _strobeCount, 1f, ct);
				break;
			default:
				SetAllFsoColor(FsoColor);
				break;
			}
			if (_pendingAnimation != null && base.gameObject.activeInHierarchy)
			{
				RunPendingAnimation();
			}
		}

		private void SetLedEnable(bool value)
		{
			SetAllFsoColor(value ? FsoColor : Color.black);
		}

		private async UniTask Animation1Async(CancellationToken ct)
		{
			SetLedEnable(value: true);
			await UniTask.Yield(ct);
		}

		private async UniTask Animation2Async(CancellationToken ct)
		{
			SetLedEnable(value: false);
			await UniTask.Yield(ct);
		}

		private async UniTask Animation3Async(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				for (int i = 0; i < 3; i++)
				{
					SetLedEnable(value: true);
					await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					SetLedEnable(value: false);
					await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
				SetLedEnable(value: false);
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}

		private async UniTask Animation4Async(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				SetLedEnable(value: true);
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				SetLedEnable(value: false);
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}

		private async UniTask Animation5Async(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				SetLedEnable(value: true);
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				SetLedEnable(value: false);
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}

		private async UniTask Animation6Async(int index, CancellationToken ct)
		{
			if (index % 2 == 0)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
			while (!ct.IsCancellationRequested)
			{
				SetLedEnable(value: true);
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				SetLedEnable(value: false);
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}

		private async UniTask Animation7Async(int index, int strobeCount, CancellationToken ct)
		{
			SetLedEnable(value: false);
			float num = (float)strobeCount * 0.2f;
			float oneWait = num / (float)strobeCount;
			await UniTask.Delay(TimeSpan.FromSeconds(0.2f * (float)index), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			while (!ct.IsCancellationRequested)
			{
				SetLedEnable(value: true);
				await UniTask.Delay(TimeSpan.FromSeconds(oneWait), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				SetLedEnable(value: false);
				for (int i = 0; i < strobeCount - 1; i++)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(oneWait), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
			}
		}

		private async UniTask Animation8Async(CancellationToken ct)
		{
			SetAllFsoColor(Color.black);
			StartFadeAnimation(FsoColor, 1f);
			await UniTask.Yield(ct);
		}

		private async UniTask Animation9Async(int index, int count, float time, CancellationToken ct)
		{
			bool flag = false;
			if (count == 4)
			{
				flag = index == 0 || index == 2;
			}
			if (count == 2)
			{
				flag = index == 0;
			}
			if (flag)
			{
				SetAllFsoColor(Color.black);
				await UniTask.Delay(TimeSpan.FromSeconds(time), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
			else
			{
				SetAllFsoColor(Color.black);
			}
			StartFadeAnimation(FsoColor, time);
			await UniTask.Yield(ct);
		}

		private void StartFadeAnimation(Color startColor, float time)
		{
			_animationSequence = DOTween.Sequence();
			_animationSequence.Append(DOTween.To(() => GetCurrentFsoColor(), (Color x) =>
			{
				SetAllFsoColor(x);
			}, startColor, time));
			_animationSequence.Append(DOTween.To(() => GetCurrentFsoColor(), (Color x) =>
			{
				SetAllFsoColor(x);
			}, Color.black, time));
			_animationSequence.SetLoops(-1);
			_animationSequence.OnKill(() =>
			{
				SetAllFsoColor(startColor);
			});
			_animationSequence.Play();
		}
	}
}

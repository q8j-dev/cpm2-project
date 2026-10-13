using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;

public class SceneTransitionWaitingWindow : WaitingWindow
{
	[SerializeField]
	private GameObject Root;

	private const float CloseDeadlineSeconds = 30f;

	private const float MenuFallbackArmSeconds = 60f;

	private const float AvatarAppearTimeoutSeconds = 10f;

	private const float BodyKitsTimeoutSeconds = 8f;

	private const int QuietFramesRequired = 30;

	private const float SupportRayStartHeight = 5f;

	private const float SupportRayLength = 60f;

	private bool _closing;

	protected override void Awake()
	{
		base.Awake();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	protected override void Start()
	{
		base.Start();
		if (!ScenesHelper.IsMenuSceneActive())
		{
			ArmClose();
		}
		else
		{
			ArmAfterMenuFallbackAsync(base.destroyCancellationToken).Forget();
		}
	}

	private async UniTask ArmAfterMenuFallbackAsync(CancellationToken cancellationToken)
	{
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(60.0), DelayType.Realtime, PlayerLoopTiming.Update, cancellationToken);
		}
		catch (OperationCanceledException)
		{
			return;
		}
		ArmClose();
	}

	public override void Close()
	{
	}

	protected override void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		base.OnDestroy();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (mode == LoadSceneMode.Single)
		{
			ArmClose();
		}
	}

	private void ArmClose()
	{
		if (!_closing)
		{
			_closing = true;
			CloseWhenWorldReadyAsync(base.destroyCancellationToken).Forget();
		}
	}

	private async UniTask CloseWhenWorldReadyAsync(CancellationToken cancellationToken)
	{
		float start = Time.realtimeSinceStartup;
		int quietFrames = 0;
		try
		{
			while (Time.realtimeSinceStartup - start < 30f)
			{
				quietFrames = ((!AsyncSceneSwitcher.InProgress && !SceneLoadTracker.HasPendingLoads) ? (quietFrames + 1) : 0);
				if (quietFrames >= 30 && IsAvatarReady(Time.realtimeSinceStartup - start))
				{
					break;
				}
				await UniTask.Yield(cancellationToken);
			}
		}
		catch (OperationCanceledException)
		{
			return;
		}
		if (SteamWorldPreloadToggle.Enabled && RamDevice.AllowHeavyPreload())
		{
			float graceEnd = Time.realtimeSinceStartup + 3f;
			bool preloadSeen = false;
			while (Time.realtimeSinceStartup < graceEnd)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				if (V.SceneControl != null && V.SceneControl.IsSteamPreloadInProgress)
				{
					preloadSeen = true;
					break;
				}
				await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
			}
			while (preloadSeen && V.SceneControl != null && V.SceneControl.IsSteamPreloadInProgress)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
			}
		}
		while (V.SceneControl != null && V.SceneControl.MpInitialCarsBuilding)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
		}
		if ((bool)Root)
		{
			Root.SetActive(value: false);
		}
		if ((bool)this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private bool IsAvatarReady(float elapsed)
	{
		GameObject gameObject = (V.Car ? V.Car : V.DriverPerson);
		if (!gameObject)
		{
			if (!ScenesHelper.IsMenuSceneActive())
			{
				return elapsed >= 10f;
			}
			return true;
		}
		if ((bool)V.Car && elapsed < 8f && V.Car.TryGetComponent<BodyKits>(out var component) && component.AllBodyKitsWasLoaded != null && !component.AllBodyKitsWasLoaded.Value)
		{
			return false;
		}
		int mask = LayerMask.GetMask("Road", "Terrain", "Default");
		return Physics.Raycast(gameObject.transform.position + Vector3.up * 5f, Vector3.down, 60f, mask, QueryTriggerInteraction.Ignore);
	}
}

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Providers;

public class BannerManager : MonoBehaviourPunCallbacks
{
	private int _bannerNumber = 14;

	public List<BannerController> currentBanners = new List<BannerController>();

	public Dictionary<int, float[]> dict;

	private Action _AddSelfToDictHandler;

	private Action _CheckIfSceneLoadedHandler;

	private readonly List<(int PlayerId, float[] Banners)> _pendingConnectUpdates = new List<(int, float[])>();

	private FreeDriveProvider _freeDriveProvider;

	private CancellationTokenSource _cancellationTokenSource;

	private CancellationTokenSource _cancellationTokenSourceAddSelfToDict;

	private CancellationTokenSource _cancellationTokenSourceMain;

	private void Start()
	{
		_cancellationTokenSource = new CancellationTokenSource();
		_cancellationTokenSourceAddSelfToDict = new CancellationTokenSource();
		_cancellationTokenSourceMain = new CancellationTokenSource();
		dict = new Dictionary<int, float[]>();
		InitWhenCoreReadyAsync(_cancellationTokenSourceMain.Token).Forget();
	}

	private async UniTaskVoid InitWhenCoreReadyAsync(CancellationToken cancellationToken)
	{
		try
		{
			while (!V.CoreGameManager || V.CoreGameManager.FreeDriveProvider == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (cancellationToken.IsCancellationRequested || !this)
				{
					return;
				}
			}
		}
		catch (OperationCanceledException)
		{
			return;
		}
		if (!this)
		{
			return;
		}
		_freeDriveProvider = V.CoreGameManager.FreeDriveProvider;
		PhotonView photonView = PhotonView.Get(base.gameObject);
		if ((PhotonNetwork.IsConnected && (bool)photonView && photonView.IsMine && base.gameObject == V.DriverPerson) || !PhotonNetwork.IsConnected)
		{
			_freeDriveProvider.BannerManager = this;
		}
		if (!PhotonNetwork.IsConnected)
		{
			return;
		}
		V.OnPlayerConnected += OnPlayerConnect;
		V.OnPlayerDisconnected += OnPlayerDisconnect;
		if (SceneManager.GetActiveScene().buildIndex != 13)
		{
			if (_AddSelfToDictHandler != null)
			{
				V.OnSceneLoaded -= _AddSelfToDictHandler;
			}
			_AddSelfToDictHandler = () =>
			{
				AddSelfToDict(_cancellationTokenSourceAddSelfToDict.Token).Forget();
			};
			V.OnSceneLoaded += _AddSelfToDictHandler;
		}
		else
		{
			AddSelfToDict(_cancellationTokenSourceAddSelfToDict.Token).Forget();
		}
	}

	private async UniTaskVoid AddSelfToDict(CancellationToken cancellationToken)
	{
		while (Camera.main == null || Camera.main.GetComponent<CoreGameManager>() == null)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
		}
		while (_freeDriveProvider == null || !_freeDriveProvider.BannerManager)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
		}
		float[] array = new float[_bannerNumber];
		for (int i = 0; i < _bannerNumber; i++)
		{
			array[i] = 0f;
		}
		dict[0] = array;
		if (_freeDriveProvider.BannerManager.dict != null && !(V.DriverPerson == null) && TryGetOwnerActorNumber(V.DriverPerson, out var actorNumber) && !_freeDriveProvider.BannerManager.dict.ContainsKey(actorNumber))
		{
			_freeDriveProvider.BannerManager.dict[actorNumber] = array;
		}
	}

	private static bool TryGetOwnerActorNumber(GameObject target, out int actorNumber)
	{
		actorNumber = 0;
		if (!target)
		{
			return false;
		}
		PhotonView photonView = PhotonView.Get(target);
		if (!photonView || photonView.Owner == null)
		{
			return false;
		}
		actorNumber = photonView.Owner.ActorNumber;
		return true;
	}

	public void OnPlayerConnect(Player newPlayer)
	{
		OnPlayerConnectInternal(newPlayer, _cancellationTokenSourceMain.Token).Forget();
	}

	private async UniTaskVoid OnPlayerConnectInternal(Player newPlayer, CancellationToken cancellationToken)
	{
		while (Camera.main?.GetComponent<CoreGameManager>() == null)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
		}
		while (V.DriverPerson == null || _freeDriveProvider == null || _freeDriveProvider.BannerManager == null)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
		}
		float[] array = new float[_bannerNumber];
		for (int i = 0; i < _bannerNumber; i++)
		{
			array[i] = 0f;
		}
		_freeDriveProvider.BannerManager.dict[newPlayer.ActorNumber] = array;
		if (_freeDriveProvider.BannerManager != this)
		{
			return;
		}
		int actorNumber = 0;
		while (!TryGetOwnerActorNumber(V.DriverPerson, out actorNumber) || !_freeDriveProvider.BannerManager.dict.ContainsKey(actorNumber))
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(10.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
		}
		PhotonView photonView = PhotonView.Get(V.DriverPerson);
		if ((bool)photonView && (bool)base.photonView && base.photonView.Owner != null && _freeDriveProvider.BannerManager.dict.TryGetValue(base.photonView.Owner.ActorNumber, out var value))
		{
			photonView.RPC("UpdateAtConnectRPC", newPlayer, value);
		}
	}

	[PunRPC]
	public void UpdateAtConnectRPC(float[] newDict, PhotonMessageInfo info)
	{
		if (newDict == null || info.photonView == null || info.photonView.Owner == null)
		{
			return;
		}
		int actorNumber = info.photonView.Owner.ActorNumber;
		if (SceneManager.GetActiveScene().buildIndex != 13)
		{
			_pendingConnectUpdates.Add((actorNumber, newDict));
			if (_CheckIfSceneLoadedHandler == null)
			{
				_CheckIfSceneLoadedHandler = FlushPendingConnectUpdates;
				V.OnSceneLoaded += _CheckIfSceneLoadedHandler;
			}
		}
		else
		{
			CheckIfSceneLoaded(actorNumber, newDict).Forget();
		}
	}

	private void FlushPendingConnectUpdates()
	{
		if (_CheckIfSceneLoadedHandler != null)
		{
			V.OnSceneLoaded -= _CheckIfSceneLoadedHandler;
			_CheckIfSceneLoadedHandler = null;
		}
		foreach (var (plID, newDict) in _pendingConnectUpdates)
		{
			CheckIfSceneLoaded(plID, newDict).Forget();
		}
		_pendingConnectUpdates.Clear();
	}

	private async UniTaskVoid CheckIfSceneLoaded(int plID, float[] newDict)
	{
		if (newDict == null)
		{
			return;
		}
		CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();
		try
		{
			while (Camera.main?.GetComponent<CoreGameManager>() == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
			while (_freeDriveProvider == null || _freeDriveProvider.BannerManager == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
		}
		catch (OperationCanceledException)
		{
			return;
		}
		if (!this)
		{
			return;
		}
		BannerManager bannerManager = _freeDriveProvider.BannerManager;
		if (bannerManager.dict == null)
		{
			return;
		}
		bannerManager.dict[plID] = newDict;
		if (bannerManager.currentBanners == null)
		{
			return;
		}
		foreach (BannerController currentBanner in bannerManager.currentBanners)
		{
			if ((bool)currentBanner && currentBanner.id >= 0 && currentBanner.id < newDict.Length && newDict[currentBanner.id] > (float)currentBanner.currentSpeed)
			{
				currentBanner.SetBanner(newDict[currentBanner.id], plID, PhotonNetwork.IsConnected);
			}
		}
	}

	public void UpdateInGameInvoke(float newSpeed, int id)
	{
		if (TryGetOwnerActorNumber(V.DriverPerson, out var actorNumber))
		{
			PhotonView.Get(V.DriverPerson).RpcToDefaultGroup("UpdateInGame", RpcTarget.All, newSpeed, id, actorNumber);
		}
	}

	[PunRPC]
	public void UpdateInGame(float speed, int bannerId, int plID)
	{
		if (_freeDriveProvider == null || !_freeDriveProvider.BannerManager || _freeDriveProvider.BannerManager.currentBanners == null || _freeDriveProvider.BannerManager.dict == null)
		{
			return;
		}
		foreach (BannerController currentBanner in _freeDriveProvider.BannerManager.currentBanners)
		{
			if (!currentBanner || currentBanner.id != bannerId)
			{
				continue;
			}
			if (!_freeDriveProvider.BannerManager.dict.ContainsKey(plID))
			{
				float[] array = new float[_bannerNumber];
				for (int i = 0; i < _bannerNumber; i++)
				{
					array[i] = 0f;
				}
				_freeDriveProvider.BannerManager.dict[plID] = array;
			}
			if (speed > _freeDriveProvider.BannerManager.dict[plID][bannerId])
			{
				_freeDriveProvider.BannerManager.dict[plID][bannerId] = speed;
			}
			currentBanner.SetBanner(speed, plID);
		}
	}

	public void OnPlayerDisconnect(Player newPlayer)
	{
		OnPlayerDisconnectInternal(newPlayer, _cancellationTokenSource.Token).Forget();
	}

	private async UniTaskVoid OnPlayerDisconnectInternal(Player newPlayer, CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested && Camera.main?.GetComponent<CoreGameManager>() == null)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
		}
		if (cancellationToken.IsCancellationRequested)
		{
			return;
		}
		while (!cancellationToken.IsCancellationRequested && _freeDriveProvider != null && (V.DriverPerson == null || _freeDriveProvider.BannerManager == null))
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
		}
		if (cancellationToken.IsCancellationRequested)
		{
			return;
		}
		await UniTask.Delay(TimeSpan.FromMilliseconds(2000.0));
		if (cancellationToken.IsCancellationRequested || _freeDriveProvider == null)
		{
			return;
		}
		BannerManager bannerManager = _freeDriveProvider.BannerManager;
		if (!bannerManager || bannerManager.dict == null || bannerManager.currentBanners == null)
		{
			return;
		}
		bannerManager.dict.Remove(newPlayer.ActorNumber);
		foreach (BannerController currentBanner in bannerManager.currentBanners)
		{
			if ((bool)currentBanner)
			{
				UpdateBanner(currentBanner);
			}
		}
	}

	public void UpdateBanner(BannerController banner)
	{
		if (!banner || _freeDriveProvider == null || !_freeDriveProvider.BannerManager || _freeDriveProvider.BannerManager.dict == null)
		{
			return;
		}
		int plID = 0;
		float num = 0f;
		foreach (KeyValuePair<int, float[]> item in _freeDriveProvider.BannerManager.dict)
		{
			if (item.Value[banner.id] >= num)
			{
				num = item.Value[banner.id];
				plID = item.Key;
			}
		}
		banner.SetBanner(num, plID, PhotonNetwork.IsConnected);
	}

	public void GetLog(string str)
	{
		foreach (KeyValuePair<int, float[]> item in _freeDriveProvider.BannerManager.dict)
		{
			str += $"\n\t{item.Key}: {item.Value[0]}, {item.Value[1]}";
		}
	}

	public void UpdateAllBanners()
	{
		foreach (BannerController currentBanner in currentBanners)
		{
			if (currentBanner.isActiveAndEnabled)
			{
				currentBanner.UpdateBanner();
			}
		}
	}

	public void OnDestroy()
	{
		V.OnPlayerConnected -= OnPlayerConnect;
		V.OnPlayerDisconnected -= OnPlayerDisconnect;
		if (_AddSelfToDictHandler != null)
		{
			V.OnSceneLoaded -= _AddSelfToDictHandler;
		}
		if (_CheckIfSceneLoadedHandler != null)
		{
			V.OnSceneLoaded -= _CheckIfSceneLoadedHandler;
		}
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSource);
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceAddSelfToDict);
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceMain);
		_AddSelfToDictHandler = null;
		_CheckIfSceneLoadedHandler = null;
		_pendingConnectUpdates.Clear();
	}

	public Player GetPlayerByOwnerID(int OwnerId)
	{
		if (!PhotonNetwork.IsConnected)
		{
			return null;
		}
		return PhotonNetwork.CurrentRoom.GetPlayer(OwnerId);
	}
}

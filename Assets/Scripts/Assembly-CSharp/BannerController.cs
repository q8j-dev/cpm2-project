using System;
using System.Text;
using System.Threading;
using CPM.PlayerData;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using ElectronicRoadSign;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;

public class BannerController : MonoBehaviour
{
	public int id;

	public int currentSpeed;

	private int _playerID;

	private ElectronicRoadSignScript _electronicRoadSignScript;

	private CancellationTokenSource _tempBlinkCts;

	private bool _canSetSpeed = true;

	private PlayerDataStorage _playerDataStorage;

	private bool _isDestroyed;

	private FreeDriveProvider _freeDriveProvider;

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
		_playerDataStorage = playerDataStorage;
	}

	private void Start()
	{
		_isDestroyed = false;
		_electronicRoadSignScript = base.gameObject.GetComponent<ElectronicRoadSignScript>();
		CheckIfSceneLoaded().Forget();
	}

	private async UniTaskVoid CheckIfSceneLoaded()
	{
		while (!(Camera.main?.GetComponent<CoreGameManager>()))
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
		}
		if (FlagsStorage.IsFreeDriveMode)
		{
			while (!V.DriverPerson || !_freeDriveProvider.BannerManager)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
			}
			if ((bool)_freeDriveProvider.BannerManager)
			{
				_freeDriveProvider.BannerManager.currentBanners.Add(this);
				_freeDriveProvider.BannerManager.UpdateBanner(this);
			}
			if (!PhotonNetwork.IsConnected)
			{
				SetBanner(ObscuredPrefs.GetInt("BannerController_" + id));
			}
		}
	}

	public void SetSpeed(float newSpeed)
	{
		if (!_canSetSpeed)
		{
			return;
		}
		_canSetSpeed = false;
		float[] value;
		if (!PhotonNetwork.IsConnected)
		{
			ObscuredPrefs.SetInt("BannerController_" + id, (int)newSpeed);
			SetBanner(newSpeed);
		}
		else if (PhotonNetwork.IsConnected && _freeDriveProvider != null && (bool)_freeDriveProvider.BannerManager && _freeDriveProvider.BannerManager.dict != null && _freeDriveProvider.BannerManager.dict.TryGetValue(0, out value) && id >= 0 && id < value.Length)
		{
			if (newSpeed > value[id])
			{
				value[id] = newSpeed;
			}
			_freeDriveProvider.BannerManager.UpdateInGameInvoke(newSpeed, id);
		}
	}

	public void SetBanner(float newSpeed, int plID = 0, bool upd = false)
	{
		if (_isDestroyed)
		{
			return;
		}
		if ((newSpeed > (float)currentSpeed) | upd)
		{
			currentSpeed = (int)newSpeed;
			try
			{
				Player playerByOwnerID = _freeDriveProvider.BannerManager.GetPlayerByOwnerID(plID);
				string text = ((playerByOwnerID == null) ? _playerDataStorage.PlayerName : playerByOwnerID.NickName);
				string nick = ((plID == 0) ? (_playerDataStorage.PlayerName ?? "") : (text ?? ""));
				_playerID = plID;
				_electronicRoadSignScript.MessageText = Normalize(nick, currentSpeed);
			}
			catch (Exception)
			{
			}
		}
		if (newSpeed > 50f)
		{
			if (_tempBlinkCts != null)
			{
				_tempBlinkCts.Cancel();
				_tempBlinkCts.Dispose();
				_tempBlinkCts = null;
			}
			if ((bool)base.gameObject && base.gameObject.activeInHierarchy)
			{
				_tempBlinkCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
				TempBlinkAsync((int)newSpeed, plID, _tempBlinkCts.Token).Forget();
			}
		}
		Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			CanSetSpeed();
		}).AddTo(this);
	}

	public void UpdateBanner()
	{
		SetBanner(currentSpeed, 0, upd: true);
	}

	private void CanSetSpeed()
	{
		_canSetSpeed = true;
	}

	private async UniTask TempBlinkAsync(int tempSpeed, int plID, CancellationToken ct)
	{
		_ = 1;
		try
		{
			string bestRecord;
			Player playerByOwnerID;
			string nick;
			if (PhotonNetwork.IsConnected)
			{
				try
				{
					playerByOwnerID = _freeDriveProvider.BannerManager.GetPlayerByOwnerID(_playerID);
					nick = ((playerByOwnerID == null) ? _playerDataStorage.PlayerName : playerByOwnerID.NickName);
					bestRecord = Normalize(nick, (int)_freeDriveProvider.BannerManager.dict[_playerID][id]);
				}
				catch
				{
					bestRecord = _electronicRoadSignScript.MessageText;
				}
			}
			else
			{
				bestRecord = _electronicRoadSignScript.MessageText;
			}
			int blinkRate = 200;
			int duration = 5000 / blinkRate;
			playerByOwnerID = _freeDriveProvider.BannerManager.GetPlayerByOwnerID(plID);
			nick = ((playerByOwnerID == null) ? _playerDataStorage.PlayerName : playerByOwnerID.NickName);
			string nick2 = ((plID == 0) ? (_playerDataStorage.PlayerName ?? "") : (nick ?? ""));
			string tempRecord = Normalize(nick2, tempSpeed);
			_electronicRoadSignScript.MessageText = tempRecord;
			await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			for (int i = 0; i < duration; i++)
			{
				_electronicRoadSignScript.MessageText = (_electronicRoadSignScript.MessageText.Equals("") ? tempRecord : "");
				await UniTask.Delay(TimeSpan.FromSeconds((float)blinkRate / 1000f), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
			_electronicRoadSignScript.MessageText = bestRecord;
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
		if (_tempBlinkCts != null)
		{
			_tempBlinkCts.Cancel();
			_tempBlinkCts.Dispose();
			_tempBlinkCts = null;
		}
		if (_freeDriveProvider != null && (bool)_freeDriveProvider.BannerManager)
		{
			_freeDriveProvider.BannerManager.currentBanners.Remove(this);
		}
	}

	private string Normalize(string nick, int speed)
	{
		StringBuilder stringBuilder = new StringBuilder("");
		if (nick.Length > 16)
		{
			for (int i = 0; i < 16; i++)
			{
				stringBuilder.Append(nick[i]);
			}
		}
		else
		{
			stringBuilder.Append(nick);
			for (int j = 0; j < 16 - nick.Length; j++)
			{
				stringBuilder.Append(' ');
			}
		}
		if (speed < 1000)
		{
			speed = (int)Utils.GetConvertedMileage(speed);
			stringBuilder.Append(speed.ToString());
			for (int k = 0; k < 3 - speed.ToString().Length; k++)
			{
				stringBuilder.Append(' ');
			}
		}
		else
		{
			stringBuilder.Append("999");
		}
		return stringBuilder.Append(FlagsStorage.Kph ? "km/h" : "mph").ToString();
	}
}

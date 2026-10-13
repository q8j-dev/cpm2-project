using System;
using CodeStage.AntiCheat.Storage;
using UniRx;
using UnityEngine;
using VContainer;

public class RoomDataItem : MonoBehaviour
{
	public string FriendIDInRoom;

	public string RoomNameToJoin;

	public GameObject AdditianWindow;

	public GameObject ShowAdditinalWindowButton;

	public GameObject isOnlinePoint;

	public Color BackColorOnPressed;

	private Color backColorInit;

	public string strLabel;

	public Color labelInitColor;

	public bool ShowingOfflineMes;

	public int maxPlayerInRoom;

	public int currentPlayerInRoom;

	public int passWord;

	public Texture passwordTexture;

	public GameObject mainWindow;

	public GameObject passwordInputWindow;

	public string accountID;

	public bool FriendInRoom;

	private IDisposable _resetLabelTextDisposable;

	public GameObject confirmDeleteWindow;

	private PlayerDatas.FriendsUtility friendsUtility;

	private MultiPlayerMenu multiPlayerMenu;

	[Inject]
	public void Construct(PlayerDatas.FriendsUtility friendsUtility, MultiPlayerMenu multiPlayerMenu)
	{
		this.friendsUtility = friendsUtility;
		this.multiPlayerMenu = multiPlayerMenu;
	}

	private void Start()
	{
		ShowMainWindow();
	}

	public void ShowAdditinalWindow()
	{
		AdditianWindow.gameObject.SetActive(value: true);
		ShowAdditinalWindowButton.gameObject.SetActive(value: false);
	}

	public void HideAdditinalWindow()
	{
		AdditianWindow.gameObject.SetActive(value: false);
		ShowAdditinalWindowButton.gameObject.SetActive(value: true);
	}

	public void ShowConfirmDeleteWindow()
	{
		confirmDeleteWindow.SetActive(value: true);
		multiPlayerMenu.showingPasswordInput = true;
	}

	public void HideConfirmDeleteWindow()
	{
		confirmDeleteWindow.SetActive(value: false);
		multiPlayerMenu.showingPasswordInput = false;
	}

	public void DeleteFromFiend()
	{
		friendsUtility.RemoveFriendWithLocalID(FriendIDInRoom);
		HideConfirmDeleteWindow();
		multiPlayerMenu.UpdateListFriendUI();
	}

	public void JoinRoomToThisFriend()
	{
		if (!ShowingOfflineMes)
		{
			MonoBehaviour.print("JoinRoomToThisFriend");
			if (passWord == 0 || passWord == -1)
			{
				ProceedToFreeDrive();
				return;
			}
			multiPlayerMenu.showingPasswordInput = true;
			ShowPasswordWindow();
		}
	}

	private void ProceedToFreeDrive()
	{
		if (ObscuredPrefs.HasKey(RoomNameToJoin))
		{
			int num = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			int num2 = ObscuredPrefs.GetInt(RoomNameToJoin, num);
			if (num - num2 < 7200)
			{
				ShowingOfflineMes = true;
				ScheduleLabelReset();
				return;
			}
			ObscuredPrefs.DeleteKey(RoomNameToJoin);
		}
		if (!FriendInRoom)
		{
			ShowingOfflineMes = true;
			ScheduleLabelReset();
		}
		else if (!isOnlinePoint.activeSelf)
		{
			ShowingOfflineMes = true;
			ScheduleLabelReset();
		}
		else if (currentPlayerInRoom >= maxPlayerInRoom)
		{
			ShowingOfflineMes = true;
			ScheduleLabelReset();
		}
		else
		{
			multiPlayerMenu.TryToJoinRoom(this);
		}
	}

	public void ResetLabelText2()
	{
		ScheduleLabelReset();
	}

	public void ResetLabelText()
	{
		ShowingOfflineMes = false;
	}

	private void ScheduleLabelReset()
	{
		if (_resetLabelTextDisposable == null)
		{
			_resetLabelTextDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				ResetLabelText();
				_resetLabelTextDisposable = null;
			}).AddTo(this);
		}
	}

	private void ShowPasswordWindow()
	{
		passwordInputWindow.SetActive(value: true);
		mainWindow.SetActive(value: false);
	}

	public void ShowMainWindow()
	{
		passwordInputWindow.SetActive(value: false);
		mainWindow.SetActive(value: true);
	}
}

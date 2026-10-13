using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Connection;
using CPM.PlayerData;
using CarInteractionAnimationsScripts;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Photon.Pun;
using RootMotion.FinalIK;
using Service;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Multiplayer.DataPack;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.Multiplayer.RpcMethods;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.WorldInteraction;

public class MultiPersonControl : MonoBehaviourPun, IPunInstantiateMagicCallback, IPunObservable
{
	private struct PoseSnapshot
	{
		public double Time;

		public Vector3 Pos;

		public Quaternion Rot;
	}

	private byte _interestGroup;

	[Inject]
	private InterestGroupManager _interestGroupManager;

	public WorldInteractionController WorldInteraction;

	private const float MaxAnchorOffset = 30f;

	private Vector3 _lastStreamDelta;

	private bool _hasStreamDelta;

	private static readonly PackerBase<Vector3> _positionPacker = new RangedVectorPacker((Precision: 7, x: -9.99f, y: 9.99f), (Precision: 15, x: -200f, y: 180f), (Precision: 7, x: -9.99f, y: 9.99f));

	private static readonly PersonLocoStatePacker _statePacker = new PersonLocoStatePacker();

	private static readonly PackerPipeline<Vector3, PersonLocoState> PreciseDataPipeline = new PackerPipeline<Vector3, PersonLocoState>(_positionPacker, _statePacker);

	private PersonLocomotion _locomotion;

	private RemoteLocomotionDriver _remoteDriver;

	public static float InterpolationDelay = 0.4f;

	private const int SnapshotCapacity = 16;

	private const float SnapshotTeleportDistance = 5f;

	private readonly PoseSnapshot[] _snapshots = new PoseSnapshot[16];

	private int _snapshotCount;

	private double _incomingSentTime;

	public const string ValidateRankFunctionName = "ValidateRank24_1";

	private readonly Vector3 _offsetPosForPhotoMode = new Vector3(0f, 0.9f, -1f);

	public Vector3 correctPlayerPos;

	public bool IsPoseLocked;

	private const RigidbodyConstraints RemoteConstraints = (RigidbodyConstraints)116;

	private const RigidbodyConstraints RemoteConstraintsDuringCarAnimation = RigidbodyConstraints.FreezeRotation;

	private Rigidbody _remoteBody;

	private Vector3 moveBlend;

	private Quaternion correctPlayerRot;

	private bool isMoving;

	public GameObject currentCar;

	public int placeOnCar;

	public bool isPassanger;

	private MultiPlayer multiPlayer;

	public Vector3 localCarPos;

	public string ThisPlayerID;

	public string ThisPlayerAccountID;

	public string ThisPlayerName;

	private Action<bool> _onPersonSpeak;

	public bool isSpeaking;

	public bool isMuted;

	public bool IsBlocked;

	public DriverActionController driverActionController;

	public float DistanceToDriverPerson;

	public bool ownPlayer;

	private bool prevMeshRenderState;

	public long lastL;

	public long lastSecond;

	private NewMoveController newMoveController;

	public GameObject FirstAndOwnCar;

	public int OwnCarViewID;

	private MainPerson mainPerson;

	public int Gender;

	public int[] equipments;

	public MeshRenderer rendererCheck;

	public Vector3 lastSendPos;

	public float dif;

	private IDisposable _onPhotonInstantiate2Disposable;

	public ObscuredInt? rank;

	public int rank2;

	public GameObject topLabel;

	private Camera mainCamera;

	private PlayerDataStorage _playerDataStorage;

	public bool forceDisablePerson;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	public Transform IkHeadPoint;

	public IDisposable ResetSpeakerTimerDisposable;

	public bool isSpeak;

	private RpcCollection _rpcCollection;

	private bool _calcDistanceLoopStarted;

	private static readonly Dictionary<string, (int Rank, float CachedAt)> ValidatedRankCache = new Dictionary<string, (int, float)>();

	private const float RankCacheTtlSeconds = 600f;

	public byte InterestGroup
	{
		get
		{
			return _interestGroup;
		}
		set
		{
			_interestGroup = value;
			base.photonView.Group = value;
		}
	}

	public Vector3Int LastSentRawPosition { get; set; }

	public Vector3Int LastReceivedRawPosition { get; set; }

	private PersonLocomotion Locomotion
	{
		get
		{
			if (!_locomotion)
			{
				return _locomotion = GetComponent<PersonLocomotion>();
			}
			return _locomotion;
		}
	}

	private RemoteLocomotionDriver RemoteDriver
	{
		get
		{
			if (!_remoteDriver)
			{
				return _remoteDriver = GetComponent<RemoteLocomotionDriver>();
			}
			return _remoteDriver;
		}
	}

	public static bool UseSnapshotInterpolation => CPM.Connection.MultiplayerSettings.UsePersonSnapshotInterpolation;

	public bool Reported { get; set; }

	public PersonAnimationsController PersonAnimationsController { get; set; }

	public ObscuredBool moneySend { get; set; }

	public void RequestPersonData()
	{
		base.photonView.RpcToDefaultGroup("RequestPersonDataOnSubscribe", RpcTarget.Others);
	}

	[PunRPC]
	public void RequestPersonDataOnSubscribe(PhotonMessageInfo info)
	{
		if (info.Sender != null && base.photonView.IsMine)
		{
			ForceSendNextStream();
			WorldInteraction?.ResendStateTo(info.Sender);
		}
	}

	private void ForceSendNextStream()
	{
		lastL = long.MaxValue;
	}

	public void AssignOwnInterestGroup()
	{
		InterestGroup = InterestGroupManager.PersonInterestGroup;
	}

	public void ResyncAfterTeleport()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		LastSentRawPosition = Vector3Int.RoundToInt(base.transform.position - V.GlobalOffset);
		LastReceivedRawPosition = LastSentRawPosition;
		if (PhotonNetwork.InRoom)
		{
			ForceSendNextStream();
			if ((bool)multiPlayer)
			{
				multiPlayer.RaiseRawPositionToAll(false, forceSendValue: true);
			}
		}
	}

	public void SetLastReceivedPosition(Vector3Int pos)
	{
		LastReceivedRawPosition = pos;
		if (!_interestGroupManager.IsSubscribedToInterestGroup(InterestGroup))
		{
			correctPlayerPos.x = pos.x;
			correctPlayerPos.z = pos.z;
			ClearSnapshots();
			return;
		}
		float num = correctPlayerPos.x - (float)pos.x;
		float num2 = correctPlayerPos.z - (float)pos.z;
		if (_hasStreamDelta && num * num + num2 * num2 > 900f)
		{
			ApplyStreamDelta(_lastStreamDelta);
			ClearSnapshots();
		}
	}

	private void StreamSend(PhotonStream stream)
	{
		if (!currentCar)
		{
			if (PhotonNetwork.SerializationRate != 5)
			{
				PhotonNetwork.SerializationRate = 5;
			}
			Transform transform = base.transform;
			Vector3 vector = transform.position - V.GlobalOffset;
			Vector3 t = vector - LastSentRawPosition.RoundedToTens();
			t.y = HeightOffset.ApplyOnSend(vector, t.y);
			PreciseDataPipeline.TrySendIfChanged(ref lastL, stream, t, BuildLocoState(transform), Convert.ToInt64);
		}
	}

	private PersonLocoState BuildLocoState(Transform t)
	{
		bool flag = (bool)Locomotion && Locomotion.IsActive && !Locomotion.IsFrozen;
		LocomotionStateSnapshot locomotionStateSnapshot = (flag ? Locomotion.Snapshot : default(LocomotionStateSnapshot));
		PersonNetFlags personNetFlags = PersonNetFlags.None;
		if (flag)
		{
			if (locomotionStateSnapshot.IsGrounded)
			{
				personNetFlags |= PersonNetFlags.Grounded;
			}
			if (locomotionStateSnapshot.IsJumping)
			{
				personNetFlags |= PersonNetFlags.Jumping;
			}
			if (locomotionStateSnapshot.IsCrouching)
			{
				personNetFlags |= PersonNetFlags.Crouching;
			}
			if (locomotionStateSnapshot.IsSliding || Locomotion.IsSlideCrouch)
			{
				personNetFlags |= PersonNetFlags.Sliding;
			}
			if (locomotionStateSnapshot.IsStarting)
			{
				personNetFlags |= PersonNetFlags.Starting;
			}
			if (locomotionStateSnapshot.IsQuickTurning)
			{
				personNetFlags |= PersonNetFlags.QuickTurning;
			}
			if (locomotionStateSnapshot.QuickTurnLeft)
			{
				personNetFlags |= PersonNetFlags.QuickTurnLeft;
			}
		}
		else
		{
			personNetFlags |= PersonNetFlags.Grounded;
		}
		if ((bool)driverActionController && (bool)driverActionController.ragdollCpm && driverActionController.ragdollCpm.isRagdoll)
		{
			personNetFlags |= PersonNetFlags.Ragdoll;
		}
		int emotion = (newMoveController ? newMoveController.Input.Emotion : 0);
		return new PersonLocoState
		{
			Yaw = Mathf.Repeat(t.eulerAngles.y, 360f),
			MoveSpeed = (flag ? locomotionStateSnapshot.MoveSpeed : 0f),
			Flags = personNetFlags,
			Emotion = emotion
		};
	}

	private void StreamReceive(PhotonStream stream, PhotonMessageInfo info)
	{
		_incomingSentTime = info.SentServerTime;
		PreciseDataPipeline.TryReceive<long>(stream, Convert.ToInt64, ReceivePreciseLocoData);
		EnableDisableMeshRendererOutside();
	}

	private void ReceivePreciseLocoData(Vector3 position, PersonLocoState state)
	{
		_lastStreamDelta = position;
		_hasStreamDelta = true;
		ApplyStreamDelta(position);
		correctPlayerRot = Quaternion.Euler(0f, state.Yaw, 0f);
		PushSnapshot(_incomingSentTime, correctPlayerPos, correctPlayerRot);
		if ((bool)RemoteDriver)
		{
			RemoteDriver.ApplyState(in state);
		}
	}

	private void ApplyStreamDelta(Vector3 delta)
	{
		Vector3 vector = LastReceivedRawPosition.RoundedToTens();
		delta.y = HeightOffset.ApplyOnReceive(vector + delta, delta.y);
		correctPlayerPos = vector + delta;
	}

	public void ClearSnapshots()
	{
		_snapshotCount = 0;
	}

	private void PushSnapshot(double time, Vector3 pos, Quaternion rot)
	{
		if (_snapshotCount > 0)
		{
			PoseSnapshot poseSnapshot = _snapshots[_snapshotCount - 1];
			if (time <= poseSnapshot.Time)
			{
				if (poseSnapshot.Time - time < 60.0)
				{
					return;
				}
				_snapshotCount = 0;
			}
			else if ((pos - poseSnapshot.Pos).sqrMagnitude > 25f)
			{
				_snapshotCount = 0;
			}
		}
		if (_snapshotCount == 16)
		{
			Array.Copy(_snapshots, 1, _snapshots, 0, 15);
			_snapshotCount--;
		}
		_snapshots[_snapshotCount++] = new PoseSnapshot
		{
			Time = time,
			Pos = pos,
			Rot = rot
		};
	}

	private bool TrySampleSnapshots(out Vector3 pos, out Quaternion rot)
	{
		pos = default;
		rot = default;
		if (_snapshotCount == 0)
		{
			return false;
		}
		double num = PhotonNetwork.Time - (double)InterpolationDelay;
		while (_snapshotCount > 2 && _snapshots[1].Time <= num)
		{
			Array.Copy(_snapshots, 1, _snapshots, 0, _snapshotCount - 1);
			_snapshotCount--;
		}
		PoseSnapshot poseSnapshot = _snapshots[0];
		PoseSnapshot poseSnapshot2 = _snapshots[_snapshotCount - 1];
		if (num >= poseSnapshot2.Time)
		{
			pos = poseSnapshot2.Pos;
			rot = poseSnapshot2.Rot;
			return true;
		}
		if (num <= poseSnapshot.Time)
		{
			pos = poseSnapshot.Pos;
			rot = poseSnapshot.Rot;
			return true;
		}
		for (int i = 0; i < _snapshotCount - 1; i++)
		{
			PoseSnapshot poseSnapshot3 = _snapshots[i];
			PoseSnapshot poseSnapshot4 = _snapshots[i + 1];
			if (!(num < poseSnapshot3.Time) && !(num > poseSnapshot4.Time))
			{
				double num2 = poseSnapshot4.Time - poseSnapshot3.Time;
				float t = ((num2 > 0.0001) ? Mathf.Clamp01((float)((num - poseSnapshot3.Time) / num2)) : 1f);
				pos = Vector3.Lerp(poseSnapshot3.Pos, poseSnapshot4.Pos, t);
				rot = Quaternion.Slerp(poseSnapshot3.Rot, poseSnapshot4.Rot, t);
				return true;
			}
		}
		pos = poseSnapshot2.Pos;
		rot = poseSnapshot2.Rot;
		return true;
	}

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, OnlineCollectionsProvider onlineCollectionsProvider, RpcCollection rpcCollection)
	{
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_playerDataStorage = playerDataStorage;
		_rpcCollection = rpcCollection;
	}

	[PunRPC]
	private void SendValueStr(byte a, string b, string name)
	{
		if (!FlagsStorage.IsMainMenu)
		{
			_rpcCollection?.GetRpcObject(a)?.ExecuteOnPerson(V.CoreGameManager.FreeDriveProvider, b, name);
		}
	}

	private void Awake()
	{
		correctPlayerRot = Quaternion.identity;
		mainPerson = GetComponent<MainPerson>();
		GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Takeover;
		driverActionController = GetComponent<DriverActionController>();
		newMoveController = GetComponent<NewMoveController>();
		if (!TryGetComponent<RemoteLegIkSuppressor>(out var _))
		{
			base.gameObject.AddComponent<RemoteLegIkSuppressor>();
		}
		if ((bool)V.CoreGameManager.GetComponent<MultiPlayer>())
		{
			multiPlayer = V.CoreGameManager.GetComponent<MultiPlayer>();
		}
		mainCamera = V.CoreGameManager.GetComponent<Camera>();
		if (!_calcDistanceLoopStarted)
		{
			_calcDistanceLoopStarted = true;
			CalcDistanceLoop(base.destroyCancellationToken).Forget();
		}
	}

	private async UniTask CalcDistanceLoop(CancellationToken token)
	{
		_ = 1;
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0.1f, 9f)), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			while (!token.IsCancellationRequested)
			{
				calcDistance();
				await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void OnDestroy()
	{
		PersonMultiplayerDataSending.RemovePersonData(this);
		if (FlagsStorage.IsMultiFree)
		{
			_onlineCollectionsProvider.UnregisterPlayer(this);
		}
		if ((bool)multiPlayer && multiPlayer.VoiceChat != null && multiPlayer.VoiceChat.SpeakingPlayers != null && multiPlayer.VoiceChat.SpeakingPlayers.Contains(this))
		{
			AkSoundEngine.PostEvent(multiPlayer.VoiceChat.StopReceiveGme, base.gameObject);
			multiPlayer.VoiceChat.SpeakingPlayers.Remove(this);
		}
	}

	private void IkHead()
	{
		if ((bool)IkHeadPoint && (bool)currentCar && !(mainPerson == null) && !(mainPerson.ik == null))
		{
			Transform head = mainPerson.ik.references.head;
			head.rotation = Quaternion.Euler(IkHeadPoint.eulerAngles.x, head.eulerAngles.y, head.eulerAngles.z);
		}
	}

	private void calcDistance()
	{
		if (!this)
		{
			return;
		}
		if ((bool)V.DriverPerson && !ownPlayer && (bool)V.CoreGameManager)
		{
			if (forceDisablePerson && base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			DistanceToDriverPerson = Vector3.Distance(base.transform.position, V.CoreGameManager.transform.position);
			if (currentCar == null && (bool)topLabel)
			{
				topLabel.SetActive(DistanceToDriverPerson < 20f && !FlagsStorage.HideAllTopLabel);
			}
			if (DistanceToDriverPerson < 120f)
			{
				if (!rendererCheck || !rendererCheck.isVisible)
				{
					return;
				}
				if (mainPerson.skin == null && equipments.Length != 0)
				{
					mainPerson.LoadMultiPlayerPersonSkin();
					driverActionController.AddHeadMove();
					driverActionController.AddRagdoll();
					if ((bool)currentCar)
					{
						currentCar.GetComponent<DriverController>().PreSetPerson(base.gameObject, placeOnCar);
					}
				}
			}
		}
		if ((bool)currentCar && currentCar.gameObject.activeSelf && !base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	private void Start()
	{
		driverActionController = GetComponent<DriverActionController>();
	}

	public void SubscribeIkHead()
	{
		if (!(mainPerson == null) && !(mainPerson.ik == null))
		{
			IKSolverFullBodyBiped solver = mainPerson.ik.solver;
			solver.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(solver.OnPostUpdate, new IKSolver.UpdateDelegate(IkHead));
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			StreamSend(stream);
		}
		else
		{
			StreamReceive(stream, info);
		}
	}

	private static bool TryGetCachedRank(string accountId, out int cachedRank)
	{
		cachedRank = 0;
		if (!ValidatedRankCache.TryGetValue(accountId, out (int, float) value))
		{
			return false;
		}
		if (Time.realtimeSinceStartup - value.Item2 > 600f)
		{
			ValidatedRankCache.Remove(accountId);
			return false;
		}
		(cachedRank, _) = value;
		return true;
	}

	public async UniTaskVoid validateRank()
	{
		string accountId = ThisPlayerAccountID;
		if (TryGetCachedRank(accountId, out var cachedRank))
		{
			rank = cachedRank;
			return;
		}
		await UniTask.Delay(TimeSpan.FromMilliseconds(1000 * UnityEngine.Random.Range(2, 5)));
		if (TryGetCachedRank(accountId, out cachedRank))
		{
			rank = cachedRank;
			return;
		}
		FunctionCallResult<int> functionCallResult = await new FirebaseWrappedFunctionFactory().Create("ValidateRank24_1").CallAsyncResult<int>(accountId);
		if (functionCallResult.IsFail)
		{
			rank = 0;
			return;
		}
		int data = functionCallResult.Data;
		int num = ((data > 6 && data < 11) ? data : 0);
		rank = num;
		ValidatedRankCache[accountId] = (num, Time.realtimeSinceStartup);
	}

	private void OnPhotonInstantiate2()
	{
		if (FlagsStorage.IsMainMenu)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			_onPhotonInstantiate2Disposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				OnPhotonInstantiate2();
			}).AddTo(this);
			return;
		}
		Awake();
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(13);
		SceneManager.MoveGameObjectToScene(base.gameObject, sceneByBuildIndex);
		base.gameObject.transform.parent = V.CoreGameManager.globalPoint.transform;
		if (!base.photonView.IsMine)
		{
			_onlineCollectionsProvider.RegisterPlayer(this);
			PersonMultiplayerDataSending.ApplyPersonMultiplayerData(this);
			PlayerDatas.friendsUtility.UpdateFriendName(ThisPlayerID, ThisPlayerAccountID, ThisPlayerName);
			if (PlayerDatas.friendsUtility.HasFriendWithID(ThisPlayerID) && (bool)V.DriverPerson)
			{
				V.CoreGameManager.SetMessage(string.Concat(new LocalizedString("MapGlossary.Player"), " ", ThisPlayerName, " ", new LocalizedString("MultiPersonControl.HasJoined")), 2f);
			}
			V.CoreGameManager.MenuLevelParent.blinkPlus();
			V.CoreGameManager.FreeDriveProvider.MultiPlayer.OnNewPlayerCreated(base.photonView.CreatorActorNr, base.photonView.Owner.UserId);
			GetComponent<CapsuleCollider>().radius = 0.1f;
			GetComponent<NewMoveController>().Speed = 1f;
			Rigidbody rigidbody = (_remoteBody = GetComponent<Rigidbody>());
			rigidbody.useGravity = false;
			rigidbody.interpolation = RigidbodyInterpolation.None;
			rigidbody.constraints = (RigidbodyConstraints)116;
			V.RaiseNewPersonCreated(this);
		}
		else
		{
			ownPlayer = true;
			ThisPlayerID = _playerDataStorage.PlayerID;
			AssignOwnInterestGroup();
		}
		GetComponent<DriverActionController>().CreatePersonIcon();
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		OnPhotonInstantiate2();
	}

	public void ExitMultiPlayerCar(GameObject car)
	{
	}

	private void SendRpcEnterCar(bool isSendMulti, int idPerson, int idCar, string data, string targetPlayerOwnerUserId)
	{
		if (isSendMulti)
		{
			if (targetPlayerOwnerUserId.IsNullOrEmpty())
			{
				multiPlayer.SendValueStr<RpcSitIntoCar>($"{idPerson},{idCar}", data);
			}
			else
			{
				multiPlayer.SendValueStrToPlayer<RpcSitIntoCar>($"{idPerson},{idCar}", data, targetPlayerOwnerUserId);
			}
		}
	}

	public void SendCarEnterPassanger(GameObject Person, GameObject car, int placeToEnter, bool isSendMulti = true, string targetPlayerOwnerUserId = "", bool playDefaultDoorSound = true)
	{
		if (!FlagsStorage.InGarageFree)
		{
			int viewID = car.GetComponent<PhotonView>().ViewID;
			int viewID2 = Person.GetComponent<PhotonView>().ViewID;
			_ = viewID + "?" + placeToEnter;
			string text = (playDefaultDoorSound ? "" : ",0");
			string data = placeToEnter + text;
			SendRpcEnterCar(isSendMulti, viewID2, viewID, data, targetPlayerOwnerUserId);
			if (car.GetComponent<PhotonView>().IsMine)
			{
				PlayerDatas.SetOnlineCarBool(car, isOnline: false);
			}
			else
			{
				PlayerDatas.SetOnlineCarBool(car, isOnline: true);
			}
		}
	}

	public void SendCarEnter(GameObject Person, GameObject car, bool isSendMulti = true, string targetPlayerOwnerUserId = "", bool playDefaultDoorSound = true)
	{
		if (!FlagsStorage.InGarageFree)
		{
			int viewID = car.GetComponent<PhotonView>().ViewID;
			int viewID2 = Person.GetComponent<PhotonView>().ViewID;
			string text = (playDefaultDoorSound ? "" : ",0");
			string data = "0,0" + text;
			SendRpcEnterCar(isSendMulti, viewID2, viewID, data, targetPlayerOwnerUserId);
			PlayerDatas.SetOnlineCarBool(car, isOnline: false);
			car.GetComponent<PhotonView>().RequestOwnership();
			Global.TaskManager.DeliveryManager.RequestOwnershit(car);
			Global.TaskManager.CargoManager.RequestOwnershit(car);
		}
	}

	public void SendCarExit(GameObject Person, GameObject car, bool isSendMulti = true, string targetPlayerOwnerUserId = "", bool playDefaultDoorSound = true)
	{
		if (FlagsStorage.InGarageFree)
		{
			return;
		}
		int viewID = car.GetComponent<PhotonView>().ViewID;
		int viewID2 = Person.GetComponent<PhotonView>().ViewID;
		string text = (playDefaultDoorSound ? "" : ",0");
		string text2 = "-1,0" + text;
		if (isSendMulti)
		{
			if (targetPlayerOwnerUserId.IsNullOrEmpty())
			{
				multiPlayer.SendValueStr<RpcGetOutCar>($"{viewID2},{viewID}", text2);
			}
			else
			{
				multiPlayer.SendValueStrToPlayer<RpcGetOutCar>($"{viewID2},{viewID}", text2, targetPlayerOwnerUserId);
			}
		}
	}

	private void EnableSpeaking()
	{
		isSpeaking = true;
		if ((bool)driverActionController.speakingLabel)
		{
			driverActionController.speakingLabel.SetActive(value: true);
		}
		if ((bool)currentCar && !isPassanger && (bool)currentCar.GetComponent<MultiPlayerPosControl>().SpeakingIcon)
		{
			currentCar.GetComponent<MultiPlayerPosControl>().SpeakingIcon.SetActive(value: true);
		}
		V.CoreGameManager.MenuLevelParent.ChangeVolumesOnVoiceChat(lowerVolumes: true);
		_onPersonSpeak?.Invoke(obj: true);
	}

	private void DisableSpeaking()
	{
		isSpeaking = false;
		if ((bool)driverActionController.speakingLabel)
		{
			driverActionController.speakingLabel.SetActive(value: false);
		}
		if ((bool)currentCar && !isPassanger && (bool)currentCar.GetComponent<MultiPlayerPosControl>().SpeakingIcon)
		{
			currentCar.GetComponent<MultiPlayerPosControl>().SpeakingIcon.SetActive(value: false);
		}
		V.CoreGameManager.MenuLevelParent.ChangeVolumesOnVoiceChat(lowerVolumes: false);
		_onPersonSpeak?.Invoke(obj: false);
	}

	private void ApplyRemoteConstraints(bool inCarAnimation)
	{
		if (!base.photonView.IsMine && PhotonNetwork.IsConnected && !(base.gameObject == V.DriverPerson) && ((bool)_remoteBody || TryGetComponent<Rigidbody>(out _remoteBody)))
		{
			RigidbodyConstraints rigidbodyConstraints = (inCarAnimation ? RigidbodyConstraints.FreezeRotation : ((RigidbodyConstraints)116));
			if (_remoteBody.constraints != rigidbodyConstraints)
			{
				_remoteBody.constraints = rigidbodyConstraints;
			}
		}
	}

	private void clampPositions()
	{
		Vector3 vector = correctPlayerPos;
		vector.x = Mathf.Clamp(vector.x, -20000f, 20000f);
		vector.z = Mathf.Clamp(vector.z, -20000f, 20000f);
		vector.y = Mathf.Clamp(vector.y, -500f, 500f);
		correctPlayerPos = vector;
		vector = base.transform.position;
		vector.x = Mathf.Clamp(vector.x, -20000f, 20000f);
		vector.z = Mathf.Clamp(vector.z, -20000f, 20000f);
		vector.y = Mathf.Clamp(vector.y, -500f, 500f);
		base.transform.position = vector;
		if (base.transform.rotation == new Quaternion(0f, 0f, 0f, 0f))
		{
			base.transform.rotation = Quaternion.identity;
		}
		if (base.photonView.IsMine && base.transform.position.y < -300f)
		{
			Vector3 position = base.transform.position;
			position.y = 5f;
			base.transform.position = position;
		}
	}

	public void SnapForPose(Vector3 worldPos, Quaternion worldRot)
	{
		Transform transform = base.transform;
		transform.position = worldPos;
		transform.rotation = worldRot;
		correctPlayerPos = transform.localPosition;
		correctPlayerRot = transform.localRotation;
		ClearSnapshots();
	}

	private void Update()
	{
		clampPositions();
		if (mainPerson.racingPerson)
		{
			if (newMoveController.playingRandom)
			{
				return;
			}
			EnableDisableMeshRendererInside();
			EnableDisableMeshRendererOutside();
		}
		bool flag = (bool)PersonAnimationsController && PersonAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate;
		bool flag2 = (bool)PersonAnimationsController && PersonAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate;
		bool flag3 = (bool)PersonAnimationsController && PersonAnimationsController.SitGetOutCarSystem.SitGetOutData.IsLockSetPosGetOutCar;
		ApplyRemoteConstraints(flag | flag2);
		bool flag4 = false;
		bool flag5 = false;
		if ((bool)driverActionController.ragdollCpm && driverActionController.ragdollCpm.isRagdoll)
		{
			flag4 = driverActionController.ragdollCpm.isRagdoll;
			flag5 = driverActionController.ragdollCpm.followPositionOnline;
		}
		if (!base.photonView.IsMine && PhotonNetwork.IsConnected && base.gameObject != V.DriverPerson && !currentCar && !flag && !flag2 && (!flag4 | flag5) && !IsPoseLocked)
		{
			if (UseSnapshotInterpolation && TrySampleSnapshots(out var pos, out var rot))
			{
				base.transform.localPosition = pos;
				base.transform.localRotation = rot;
			}
			else
			{
				if (Vector3.Distance(base.transform.localPosition, correctPlayerPos) < 5f)
				{
					base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, correctPlayerPos, Time.deltaTime * 5f);
				}
				else
				{
					base.transform.localPosition = correctPlayerPos;
				}
				base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, correctPlayerRot, Time.deltaTime * 5f);
			}
		}
		if ((bool)currentCar && !flag4 && !IsPoseLocked && !flag3 && !flag && !flag2)
		{
			if (FlagsStorage.IsFreeDriveMode)
			{
				base.transform.localPosition = currentCar.transform.TransformPoint(localCarPos) - V.GlobalOffset;
			}
			else
			{
				base.transform.localPosition = currentCar.transform.TransformPoint(localCarPos);
			}
			LastReceivedRawPosition = Vector3Int.RoundToInt(base.transform.localPosition);
			base.transform.localEulerAngles = new Vector3(currentCar.transform.localEulerAngles.x, currentCar.transform.localEulerAngles.y, 0f);
			if (base.gameObject.activeSelf && !currentCar.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		if (base.photonView.IsMine)
		{
			return;
		}
		EnableDisableMeshRendererInside();
		if (isSpeak)
		{
			if (!IsBlocked && !isSpeaking)
			{
				EnableSpeaking();
			}
		}
		else if (isSpeaking)
		{
			DisableSpeaking();
		}
	}

	private void EnableDisableMeshRendererOutside()
	{
		if (!currentCar)
		{
			if ((bool)mainPerson.mainBodySkin)
			{
				mainPerson.mainBodySkin.enabled = DistanceToDriverPerson < 60f;
			}
			if ((bool)mainPerson.SkinnedMeshRender && (bool)mainCamera)
			{
				mainPerson.SkinnedMeshRender.enabled = DistanceToDriverPerson < mainCamera.farClipPlane - 10f;
			}
		}
	}

	private void EnableDisableMeshRendererInside()
	{
		if (base.gameObject == V.DriverPerson || !currentCar)
		{
			return;
		}
		if ((bool)mainPerson.mainBodySkin)
		{
			mainPerson.mainBodySkin.enabled = DistanceToDriverPerson < 12f;
		}
		if ((bool)mainPerson.SkinnedMeshRender && (bool)currentCar)
		{
			mainPerson.SkinnedMeshRender.enabled = DistanceToDriverPerson < (float)(30 + CPMGraphicsSettingsManager.Instance.Quality * 10);
			if (prevMeshRenderState != mainPerson.SkinnedMeshRender.enabled && mainPerson.SkinnedMeshRender.enabled)
			{
				driverActionController.UpdateIK();
			}
			prevMeshRenderState = mainPerson.SkinnedMeshRender.enabled;
		}
	}

	public GameObject FindFirstAndOwnCar()
	{
		MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			if ((bool)multiPlayerPosControl && multiPlayerPosControl.photonView.CreatorActorNr == base.photonView.CreatorActorNr)
			{
				FirstAndOwnCar = multiPlayerPosControl.gameObject;
				return FirstAndOwnCar;
			}
		}
		return null;
	}
}

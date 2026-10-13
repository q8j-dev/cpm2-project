using System;
using System.Collections.Generic;
using CarAnimationScripts;
using Photon.Pun;
using RootMotion.FinalIK;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other;

public class TaxiClientController : MonoBehaviourPunCallbacks
{
	private static readonly uint Play_door = AudioWrapper.GetId("Play_Car_Door_Close");

	private IDisposable _checkForLoadFreeDriveDisposable;

	private IDisposable _createParticleDisposable;

	public GameObject passengerCircle;

	public GameObject mapIcon;

	public int zoneNumber;

	public ParticleSystem bucksEffect;

	public GameObject person;

	[HideInInspector]
	public bool isPassenger;

	private Rigidbody carRigidbody;

	[HideInInspector]
	public int carID;

	[HideInInspector]
	public Transform _root;

	[HideInInspector]
	public bool inCar;

	public TaxiController parent;

	public Animator _animator;

	public Collider carCollider;

	public AudioClip _voice_womanThanks;

	public AudioClip _voice_manThanks;

	public MainPerson mainPerson;

	private void Start()
	{
		_root = base.gameObject.transform.parent;
		if (TaxiController._taxiController != null)
		{
			parent = TaxiController._taxiController;
		}
		if (_root != null && (bool)_root.GetComponent<Animator>())
		{
			_animator = _root.GetComponent<Animator>();
		}
		if (FlagsStorage.IsMainMenu)
		{
			_checkForLoadFreeDriveDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				CheckForLoadFreeDrive();
			}).AddTo(this);
		}
		else
		{
			_root.gameObject.transform.parent = V.CoreGameManager.globalPoint.transform;
		}
		Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			checkDistanceToCamera();
		}).AddTo(this);
	}

	private void checkDistanceToCamera()
	{
		bool flag = Vector3.Distance(base.transform.position, V.CoreGameManager.transform.position) < 60f;
		if ((bool)mainPerson.PersonAnimationsController)
		{
			mainPerson.PersonAnimationsController.Animator.enabled = flag;
		}
		if ((bool)mainPerson.ik)
		{
			mainPerson.ik.enabled = flag;
		}
	}

	private void CheckForLoadFreeDrive()
	{
		if (FlagsStorage.IsMainMenu)
		{
			_checkForLoadFreeDriveDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				CheckForLoadFreeDrive();
			}).AddTo(this);
		}
		else if (_root.parent == null)
		{
			Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(13);
			SceneManager.MoveGameObjectToScene(_root.gameObject, sceneByBuildIndex);
			_root.gameObject.transform.parent = V.CoreGameManager.globalPoint.transform;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (inCar || carCollider != null || (!other.CompareTag("Car") && !other.CompareTag("OnlineCar")))
		{
			return;
		}
		VehicleData component = other.transform.parent.GetComponent<VehicleData>();
		if ((bool)component && component.VehicleMembersController.HasPlaceForPassenger)
		{
			carCollider = other;
			_root = base.gameObject.transform.parent;
			if (PhotonNetwork.IsConnected)
			{
				_root.GetComponent<PhotonView>().RequestOwnership();
			}
			CheckCar();
		}
	}

	public void AnimWalk(bool walkCondition)
	{
		if (_animator != null)
		{
			_animator.SetBool("walk", walkCondition);
		}
	}

	private void CheckCar()
	{
		if ((bool)_animator)
		{
			_animator.enabled = true;
		}
		if (!carCollider)
		{
			return;
		}
		GameObject gameObject = carCollider.transform.parent.gameObject;
		if (!gameObject.GetComponent<VehicleData>() || !gameObject.GetComponent<VehicleData>().VehicleMembersController.HasPlaceForPassenger)
		{
			return;
		}
		if (Vector3.Distance(carCollider.transform.position, _root.transform.position) > 10f)
		{
			carCollider = null;
			AnimWalk(walkCondition: false);
			return;
		}
		carRigidbody = gameObject.GetComponent<Rigidbody>();
		if (!carRigidbody)
		{
			return;
		}
		if (carRigidbody.velocity.magnitude < 2f)
		{
			Vector3 forward = carCollider.transform.transform.position - _root.position;
			forward.y = 0f;
			_root.transform.rotation = Quaternion.Lerp(_root.transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 50f);
			AnimWalk(walkCondition: true);
			if (Vector3.Distance(carCollider.transform.position, _root.transform.position) < 3f && gameObject == V.Car)
			{
				InCarPassanger(_root.gameObject, gameObject, isRPC: true);
				_root.transform.SetParent(carCollider.transform);
				AnimWalk(walkCondition: false);
				CancelInvoke("CheckCar");
			}
			else if (V.Car == null)
			{
				AnimWalk(walkCondition: false);
			}
		}
		else
		{
			AnimWalk(walkCondition: false);
		}
	}

	public void InCarPassanger(GameObject Person, GameObject car, bool isRPC)
	{
		CancelInvoke("CheckCar");
		_root = base.gameObject.transform.parent;
		inCar = true;
		if (car.TryGetComponent<PhotonView>(out var component))
		{
			carID = component.ViewID;
		}
		if (_root.TryGetComponent<PhotonView>(out var component2) & isRPC)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("TaxiClientController.YouHaveAPassenger"), 5f);
			if ((bool)parent)
			{
				parent.CreateFinishZone(base.gameObject.transform, zoneNumber);
			}
			else
			{
				TaxiController._taxiController.CreateFinishZone(base.gameObject.transform, zoneNumber);
			}
			if (!component2.IsMine && PhotonNetwork.IsConnected)
			{
				component2.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
			}
		}
		isPassenger = true;
		if (Person.TryGetComponent<Animator>(out var component3))
		{
			component3.applyRootMotion = false;
			component3.SetBool("wait", value: false);
			component3.SetBool("walk", value: false);
			component3.SetBool("realwalk", value: false);
		}
		if (Person.TryGetComponent<CapsuleCollider>(out var component4))
		{
			component4.enabled = false;
		}
		if (TryGetComponent<TaxiGenderSound>(out var component5))
		{
			component5.waitParticle.SetActive(value: false);
			UnityEngine.Object.Destroy(component5);
		}
		if (base.gameObject.TryGetComponent<CapsuleCollider>(out var component6))
		{
			UnityEngine.Object.Destroy(component6);
		}
		if (mapIcon != null)
		{
			UnityEngine.Object.Destroy(mapIcon);
		}
		if (passengerCircle != null)
		{
			UnityEngine.Object.Destroy(passengerCircle);
		}
		if (Person.TryGetComponent<Rigidbody>(out var component7))
		{
			UnityEngine.Object.Destroy(component7);
		}
		VehicleData component8 = car.GetComponent<VehicleData>();
		DriverController component9 = car.GetComponent<DriverController>();
		DriverActionController component10 = Person.GetComponent<DriverActionController>();
		FullBodyBipedIK ik = component10.ik;
		ik.enabled = true;
		ik.solver.IKPositionWeight = 1f;
		component10.EnableDisableAllColliders(WillEnable: false);
		int placeForPassenger = V.CoreGameManager.CarOutInControl.GetPlaceForPassenger(car);
		component9.PreSetPerson(Person, placeForPassenger);
		component8.VehicleMembersController.SetMember(placeForPassenger, Person);
		Person.transform.SetParent(car.transform);
		if (!Person.TryGetComponent<MultiPersonControl>(out var _))
		{
			CarAnimationsController component12 = car.GetComponent<CarAnimationsController>();
			List<Transform> list = ((!(component12 != null) || !(component12.CarAnimationReferences != null)) ? null : component12.CarAnimationsPoints?.SeatPointsToSit);
			if (list != null && placeForPassenger >= 0 && placeForPassenger < list.Count && (bool)list[placeForPassenger])
			{
				Person.transform.rotation = list[placeForPassenger].rotation;
			}
			else
			{
				Person.transform.localRotation = Quaternion.identity;
			}
		}
		playCarCloseSound(car);
	}

	public void playCarCloseSound(GameObject car)
	{
		if ((bool)car)
		{
			AudioWrapper.PostEvent(Play_door, car);
		}
	}

	public void OutCar(GameObject person, GameObject car)
	{
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.pitch = UnityEngine.Random.Range(0.8f, 1f);
		if ((int)mainPerson.Gender == 1)
		{
			audioSource.PlayOneShot(_voice_manThanks);
		}
		else
		{
			audioSource.PlayOneShot(_voice_womanThanks);
		}
		if ((bool)person.GetComponent<Rigidbody>())
		{
			UnityEngine.Object.Destroy(person.GetComponent<Rigidbody>());
		}
		if (person.GetComponent<Rigidbody>() == null)
		{
			person.AddComponent<Rigidbody>();
		}
		Rigidbody component = person.GetComponent<Rigidbody>();
		VehicleData component2 = car.GetComponent<VehicleData>();
		component.constraints = RigidbodyConstraints.FreezeRotation;
		person.GetComponent<Rigidbody>().mass = 1f;
		person.transform.parent = car.transform;
		Vector3 localPosition = person.transform.localPosition;
		Vector3 vector = V.Car.transform.right * UnityEngine.Random.Range(2.6f, 3f);
		vector.y = 0f;
		if (component2.RightHanded)
		{
			localPosition -= vector;
		}
		else
		{
			localPosition += vector;
		}
		float num = PersonOutHeight(localPosition, car);
		FlagsStorage.KeepRow = false;
		if (component2.isTruck)
		{
			localPosition.y += 2f;
		}
		person.transform.localPosition = localPosition;
		localPosition = person.transform.position;
		person.transform.position = new Vector3(localPosition.x, num - 0.5f, localPosition.z);
		person.transform.parent = null;
		person.transform.eulerAngles = Vector3.zero;
		person.GetComponent<DriverActionController>().ik.solver.IKPositionWeight = 0f;
		person.GetComponent<DriverActionController>().EnableDisableAllColliders(WillEnable: true);
		if (FlagsStorage.IsMultiFree)
		{
			if ((bool)person.GetComponent<MultiPersonControl>())
			{
				person.GetComponent<MultiPersonControl>().SendCarExit(person, car);
			}
			if (!FlagsStorage.IsPassenger && (bool)person.GetComponent<MultiPersonControl>())
			{
				car.GetComponent<MultiPlayerPosControl>().updateLabel(person, Show: false);
			}
			component2.CarMirrorController.SetMirror(val: false);
		}
		playCarCloseSound(car);
	}

	private float PersonOutHeight(Vector3 v3, GameObject car)
	{
		Vector3 direction = car.transform.TransformDirection(Vector3.down);
		float y = car.transform.localPosition.y + 0.2f;
		v3.y = y;
		LayerMask layerMask = -1;
		float maxDistance = 10f;
		if (Physics.Raycast(v3, direction, out var hitInfo, maxDistance, layerMask.value, QueryTriggerInteraction.Ignore))
		{
			maxDistance = hitInfo.distance;
			v3.y = v3.y - hitInfo.distance + 0.2f;
			return v3.y;
		}
		return car.transform.localPosition.y;
	}

	public void ClientDestroyEffect()
	{
		_createParticleDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			CreateParticle();
		}).AddTo(this);
	}

	private void CreateParticle()
	{
		UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(bucksEffect, base.gameObject.transform.parent.transform.localPosition, Quaternion.identity).gameObject, 2f);
		if (FlagsStorage.IsMultiFree)
		{
			PhotonNetwork.Destroy(base.gameObject.transform.parent.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject.transform.parent.gameObject);
		}
	}
}

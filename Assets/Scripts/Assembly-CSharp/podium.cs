using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;

public class podium : MonoBehaviour
{
	[Serializable]
	public class WinnerPlace
	{
		public Transform personPlace;

		public Transform carPlace;
	}

	public int maxPerson;

	public List<GameObject> allPerson;

	public List<GameObject> carsPlaced;

	public DragRacingController controller;

	public CircuitRaceControlller circuitController;

	public Transform crowdPlane;

	public Transform LosePlayerPlace;

	public Transform LoseMyCarPlace;

	public AnimationDataSO AnimationDataSo;

	public WinnerPlace[] allPlaces;

	private AnimationDataSO.AnimationDataItem[] podiumAnimations;

	private AnimationDataSO.AnimationDataItem[] crowdAnimations;

	public List<LookAtIK> listLookAt;

	private bool _isPlayWiningCrowd;

	private const string PlayWiningCrowdAmbienceAkEventName = "Play_Wining_Crowd_Ambience";

	private const string PlayWiningCelebrationAkEventName = "Play_Wining_Celebration";

	private readonly Dictionary<MainPerson, List<Renderer>> _hiddenPersonRenderers = new Dictionary<MainPerson, List<Renderer>>();

	private void Start()
	{
		carsPlaced = new List<GameObject>();
		podiumAnimations = GetAnimationsByY(13, 2, 3, 12, 20, 25, 30, 32, 33, 35);
		crowdAnimations = GetAnimationsByY(3, 2, 12, 13, 30, 32, 30, 32, 33, 35);
	}

	private AnimationDataSO.AnimationDataItem[] GetAnimationsByY(params int[] ys)
	{
		List<AnimationDataSO.AnimationDataItem> list = new List<AnimationDataSO.AnimationDataItem>(ys.Length);
		foreach (int num in ys)
		{
			AnimationDataSO.AnimationDataItem byY = AnimationDataSo.GetByY(num);
			if (byY == null)
			{
				Debug.LogError($"podium: no animation with Y = {num}");
			}
			else
			{
				list.Add(byY);
			}
		}
		return list.ToArray();
	}

	private GameObject CreateOnePerson()
	{
		GameObject obj = Resources.Load<GameObject>("Other/crowdPerson");
		GameObject gameObject = V.CoreGameManager._prefabFactory.Instantiate(obj);
		if (FlagsStorage.SingeDragRacingMode && gameObject.TryGetComponent<MainPerson>(out var component))
		{
			component.IsDragRacingCrowd = true;
		}
		return gameObject;
	}

	public async UniTaskVoid PopulateCrowd(Transform square, Vector3 toLook, bool myPersonLose)
	{
		CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
		if (!FlagsStorage.IsMultiFree)
		{
			if (!_isPlayWiningCrowd)
			{
				AudioWrapper.PostEvent("Play_Wining_Crowd_Ambience", base.gameObject);
				_isPlayWiningCrowd = true;
			}
			AudioWrapper.PostEvent("Play_Wining_Celebration", base.gameObject);
		}
		if (myPersonLose && !FlagsStorage.IsMultiFree)
		{
			MainPerson component = V.DriverPerson.GetComponent<MainPerson>();
			SetPodiumPersonVisible(component, visible: false);
			if (!FlagsStorage.OutOfCar)
			{
				V.CoreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.LastUsedCar);
				SetPodiumPersonVisible(component, visible: false);
			}
			if (V.DriverPerson.TryGetComponent<DriverActionController>(out var component2) && (bool)component2.ik)
			{
				component2.ik.enabled = false;
			}
			Rigidbody component3 = V.DriverPerson.GetComponent<Rigidbody>();
			component3.isKinematic = true;
			component3.interpolation = RigidbodyInterpolation.None;
			component3.constraints = (RigidbodyConstraints)122;
			V.DriverPerson.transform.position = LosePlayerPlace.position;
			V.DriverPerson.transform.rotation = LosePlayerPlace.rotation;
			V.LastUsedCar.transform.SetPositionAndRotation(LoseMyCarPlace.position + controller.CarYOffset(V.LastUsedCar), LoseMyCarPlace.rotation);
			SetPodiumPersonVisible(component, visible: true);
		}
		List<Vector3> positions = new List<Vector3>();
		float num = 0.8f;
		Bounds bounds = square.GetComponent<Renderer>().bounds;
		float x = bounds.size.x;
		float z = bounds.size.z;
		int num2 = Mathf.FloorToInt(x / num);
		int num3 = Mathf.FloorToInt(z / num);
		Vector3 vector = bounds.center - new Vector3(x / 2f, 0f, z / 2f);
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num3; j++)
			{
				Vector3 item = vector + new Vector3((float)i * num, 0f, (float)j * num);
				positions.Add(item);
			}
		}
		for (int k = 0; k < positions.Count; k++)
		{
			Vector3 value = positions[k];
			int index = UnityEngine.Random.Range(k, positions.Count);
			positions[k] = positions[index];
			positions[index] = value;
		}
		int personCount = Mathf.Max(num2, num3);
		if (maxPerson != 0)
		{
			personCount = maxPerson;
		}
		for (int l = 0; l < personCount; l++)
		{
			GameObject person = CreateOnePerson();
			allPerson.Add(person);
			Vector3 vector2 = positions[UnityEngine.Random.Range(0, positions.Count)];
			positions.Remove(vector2);
			person.transform.position = vector2;
			if ((bool)V.GlobalPoint)
			{
				person.transform.SetParent(V.GlobalPoint);
			}
			await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, destroyToken);
			if (!person)
			{
				continue;
			}
			person.transform.LookAt(toLook);
			Vector3 eulerAngles = person.transform.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			person.transform.eulerAngles = eulerAngles;
			if (person.TryGetComponent<DriverActionController>(out var component4) && (bool)component4.ik)
			{
				component4.ik.enabled = false;
			}
			Animator animator = person.GetComponent<Animator>();
			for (int wait = 0; wait < 19; wait++)
			{
				if ((bool)animator)
				{
					break;
				}
				await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, destroyToken);
				if (!person)
				{
					break;
				}
				animator = person.GetComponent<Animator>();
			}
			if (!person)
			{
				continue;
			}
			if ((bool)animator)
			{
				animator.applyRootMotion = false;
				NewMoveController component5 = person.GetComponent<NewMoveController>();
				if ((bool)component5)
				{
					_ = component5.SetRandomAnimator(animator, crowdAnimations, 40);
				}
			}
			Rigidbody component6 = person.GetComponent<Rigidbody>();
			if ((bool)component6)
			{
				component6.constraints = (RigidbodyConstraints)122;
			}
		}
		AddLookAtIkAllPerson();
	}

	public void ClearPodium()
	{
		base.gameObject.SetActive(value: false);
		foreach (MainPerson item in _hiddenPersonRenderers.Keys.ToList())
		{
			SetPodiumPersonVisible(item, visible: true);
		}
		if ((bool)controller)
		{
			foreach (GameObject item2 in carsPlaced.Where((GameObject car) => car))
			{
				controller.DestroyRacingCar(item2);
			}
		}
		ClearCrowd();
	}

	public void ClearCrowd()
	{
		StopCrowdSound();
		foreach (GameObject item in allPerson.Where((GameObject person) => person))
		{
			UnityEngine.Object.Destroy(item);
		}
	}

	public void StopCrowdSound()
	{
		if (_isPlayWiningCrowd)
		{
			AudioWrapper.PostEvent("Stop_Wining_Crowd_Ambience", base.gameObject);
			_isPlayWiningCrowd = false;
		}
	}

	public async UniTaskVoid SetPersonForPodiums(GameObject car1, GameObject car2, GameObject car3)
	{
		ClearPodium();
		bool controllerExist = controller;
		if (controllerExist)
		{
			controller.SoundEventChannel.InvokeDragSpeakerEvent("Goodbyes");
		}
		List<GameObject> allCars = new List<GameObject> { car1, car2, car3 };
		base.gameObject.SetActive(value: true);
		bool isMyPersonLose = true;
		for (int i = 0; i <= 2; i++)
		{
			GameObject car4 = allCars[i];
			if (car4 == null)
			{
				continue;
			}
			carsPlaced.Add(car4);
			VehicleData component = car4.GetComponent<VehicleData>();
			component.racingState = RacingState.None;
			if (controllerExist)
			{
				controller.dragRacingTree.StopCar(car4);
			}
			GameObject person = null;
			if ((bool)component.CarValues)
			{
				person = component.CarValues.driver;
			}
			if (person == null)
			{
				if (!allCars[i].TryGetComponent<DriverController>(out var component2))
				{
					MonoBehaviour.print("person is null..");
					continue;
				}
				person = component2.DriverPerson;
				if (person == null)
				{
					MonoBehaviour.print("person is null..");
					continue;
				}
			}
			person.GetComponent<DriverActionController>().EnableDisableAllColliders(WillEnable: true);
			MainPerson mainPerson = person.GetComponent<MainPerson>();
			SetPodiumPersonVisible(mainPerson, visible: false);
			await UniTask.Delay(100);
			V.CoreGameManager.CarOutInControl.OutCar(person, car4);
			SetPodiumPersonVisible(mainPerson, visible: false);
			await UniTask.Delay(300);
			person.GetComponent<NewMoveController>().SetRandomAnimator(mainPerson.Animator, podiumAnimations).Forget();
			if ((bool)mainPerson && (bool)mainPerson.ik)
			{
				mainPerson.ik.enabled = false;
			}
			if (isMyPersonLose)
			{
				isMyPersonLose = person != V.DriverPerson;
			}
			person.GetComponent<Animator>().applyRootMotion = false;
			Rigidbody component3 = person.GetComponent<Rigidbody>();
			component3.velocity = Vector3.zero;
			component3.interpolation = RigidbodyInterpolation.None;
			person.transform.SetPositionAndRotation(allPlaces[i].personPlace.transform.position, allPlaces[i].personPlace.rotation);
			component3.constraints = (RigidbodyConstraints)122;
			SetPodiumPersonVisible(mainPerson, visible: true);
		}
		if (!isMyPersonLose)
		{
			allCars.Remove(V.LastUsedCar);
			allCars.Add(V.LastUsedCar);
		}
		for (int j = 0; j < allCars.Count; j++)
		{
			allCars[j].transform.SetPositionAndRotation(allPlaces[j].carPlace.position, allPlaces[j].carPlace.rotation);
		}
		PopulateCrowd(crowdPlane, allPlaces[0].personPlace.position, isMyPersonLose).Forget();
		if (controllerExist)
		{
			controller.dragRacingTree.BlinkLeftSideMats().Forget();
			controller.dragRacingTree.BlinkRightSideMats().Forget();
			controller.dragRacingTree.rightBlinkCount = 2000;
			controller.dragRacingTree.leftBlinkCount = 2000;
		}
	}

	public void AddLookAtIkAllPerson()
	{
	}

	public async void UpdateLookIK()
	{
		while (true)
		{
			foreach (LookAtIK lookAt in listLookAt)
			{
				await UniTask.Delay(100);
				lookAt.UpdateSolverExternal();
			}
		}
	}

	private void AddLookAtIK(GameObject person)
	{
		LookAtIK lookAtIK = person.AddComponent<LookAtIK>();
		listLookAt.Add(lookAtIK);
		FullBodyBipedIK ik = person.GetComponent<DriverActionController>().ik;
		lookAtIK.solver.head.transform = ik.references.head;
		lookAtIK.solver.headWeight = 1f;
		lookAtIK.enabled = false;
		lookAtIK.solver.target = V.CoreGameManager.gameObject.transform;
	}

	private void SetPodiumPersonVisible(MainPerson mainPerson, bool visible)
	{
		if (!mainPerson)
		{
			return;
		}
		if (!visible)
		{
			if (!_hiddenPersonRenderers.TryGetValue(mainPerson, out var value))
			{
				value = new List<Renderer>();
				_hiddenPersonRenderers[mainPerson] = value;
			}
			Renderer[] componentsInChildren = mainPerson.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.enabled)
				{
					renderer.enabled = false;
					value.Add(renderer);
				}
			}
		}
		else
		{
			if (!_hiddenPersonRenderers.TryGetValue(mainPerson, out var value2))
			{
				return;
			}
			_hiddenPersonRenderers.Remove(mainPerson);
			foreach (Renderer item in value2)
			{
				if ((bool)item)
				{
					item.enabled = true;
				}
			}
		}
	}
}

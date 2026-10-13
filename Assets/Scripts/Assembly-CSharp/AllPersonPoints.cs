using UnityEngine;

public class AllPersonPoints : MonoBehaviour
{
	public int RecordIndex;

	private int toSet;

	public TargetPositionRecorder mainTPR;

	public TargetPositionRecorder[] allTargets;

	[HideInInspector]
	public GameObject LHand;

	[HideInInspector]
	public GameObject RHand;

	[HideInInspector]
	public GameObject LUpperArm;

	[HideInInspector]
	public GameObject RUpperArm;

	[HideInInspector]
	public GameObject mainBody;

	public Vector3 mainBodyPosition;

	[HideInInspector]
	public GameObject LFoot;

	[HideInInspector]
	public GameObject RFoot;

	public GameObject LBendGoal;

	public GameObject RBendGoal;

	[HideInInspector]
	public GameObject LLegBendGoal;

	[HideInInspector]
	public GameObject RLegBendGoal;

	[HideInInspector]
	public GameObject Head;

	[HideInInspector]
	public GameObject Driver;

	[HideInInspector]
	public GameObject Shifter;

	public Transform[] allPlaces;

	public bool editMode;

	public bool onShifterNow;

	private Rigidbody rb;

	[field: SerializeField]
	public bool EnableIkHead { get; private set; }

	private void Awake()
	{
		if (editMode)
		{
			GameObject gameObject = base.transform.Find("L Thigh").gameObject;
			GameObject gameObject2 = base.transform.Find("R Thigh").gameObject;
			gameObject.transform.localPosition = (gameObject.transform.localPosition + gameObject2.transform.localPosition) / 2f;
			Object.Destroy(gameObject2);
			mainBody = createPrimitive(Color.yellow, gameObject);
			LUpperArm = base.transform.Find("L UpperArm").gameObject;
			if ((bool)LUpperArm)
			{
				LUpperArm = createPrimitive(Color.red, LUpperArm);
			}
			RUpperArm = base.transform.Find("R UpperArm").gameObject;
			if ((bool)RUpperArm)
			{
				RUpperArm = createPrimitive(Color.red, RUpperArm);
			}
			LBendGoal = base.transform.Find("L bend goal").gameObject;
			if ((bool)LBendGoal)
			{
				LBendGoal = createPrimitive(Color.yellow, LBendGoal);
			}
			RBendGoal = base.transform.Find("R bend goal").gameObject;
			if ((bool)RBendGoal)
			{
				RBendGoal = createPrimitive(Color.yellow, RBendGoal);
			}
			LHand = base.transform.Find("L Hand").gameObject;
			if ((bool)LHand)
			{
				LHand = createPrimitive(Color.yellow, LHand);
			}
			RHand = base.transform.Find("R Hand").gameObject;
			if ((bool)RHand)
			{
				RHand = createPrimitive(Color.yellow, RHand);
			}
			LLegBendGoal = base.transform.Find("L leg bend goal").gameObject;
			if ((bool)LLegBendGoal)
			{
				LLegBendGoal = createPrimitive(Color.blue, LLegBendGoal);
			}
			Transform transform = base.transform.Find("Head");
			Head = (transform ? transform.gameObject : null);
			if ((bool)Head)
			{
				Head = createPrimitive(Color.blue, Head);
			}
			RLegBendGoal = base.transform.Find("R leg bend goal").gameObject;
			if ((bool)RLegBendGoal)
			{
				RLegBendGoal = createPrimitive(Color.blue, RLegBendGoal);
			}
			LFoot = base.transform.Find("L Foot").gameObject;
			if ((bool)LFoot)
			{
				LFoot = createPrimitive(Color.blue, LFoot);
			}
			RFoot = base.transform.Find("R Foot").gameObject;
			if ((bool)RFoot)
			{
				RFoot = createPrimitive(Color.blue, RFoot);
			}
			Shifter = base.transform.Find("Shifter").gameObject;
			if ((bool)Shifter)
			{
				Shifter = createPrimitive(Color.blue, Shifter);
			}
			mainTPR.LHand = LHand;
			mainTPR.RHand = RHand;
			mainTPR.LUpperArm = LUpperArm;
			mainTPR.RUpperArm = RUpperArm;
			mainTPR.mainBody = mainBody;
			mainTPR.LFoot = LFoot;
			mainTPR.RFoot = RFoot;
			mainTPR.LBendGoal = LBendGoal;
			mainTPR.RBendGoal = RBendGoal;
			mainTPR.LLegBendGoal = LLegBendGoal;
			mainTPR.RLegBendGoal = RLegBendGoal;
			mainTPR.Shifter = Shifter;
			mainTPR.Head = Head;
		}
		else
		{
			base.transform.localPosition = Vector3.zero;
			foreach (Transform item in base.transform)
			{
				Object.Destroy(item.gameObject);
			}
		}
		allPlaces = new Transform[allTargets.Length];
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (editMode)
		{
			TargetPositionRecorder targetPositionRecorder = allTargets[RecordIndex];
			RecordIndex = Mathf.Clamp(RecordIndex, 0, allTargets.Length);
			if ((bool)LHand)
			{
				targetPositionRecorder.LHandPosition = LHand.transform.localPosition;
				targetPositionRecorder.LHandRot = LHand.transform.localEulerAngles;
			}
			if ((bool)RHand)
			{
				targetPositionRecorder.RHandPosition = RHand.transform.localPosition;
				targetPositionRecorder.RHandRot = RHand.transform.localEulerAngles;
			}
			if ((bool)LUpperArm)
			{
				targetPositionRecorder.LUpperArmPosition = LUpperArm.transform.localPosition;
			}
			if ((bool)RUpperArm)
			{
				targetPositionRecorder.RUpperArmPosition = RUpperArm.transform.localPosition;
			}
			if ((bool)mainBody)
			{
				targetPositionRecorder.mainBodyPosition = mainBody.transform.localPosition;
			}
			if ((bool)RFoot)
			{
				targetPositionRecorder.RFootPosition = RFoot.transform.localPosition;
				targetPositionRecorder.RFootRot = RFoot.transform.localEulerAngles;
			}
			if ((bool)LFoot)
			{
				targetPositionRecorder.LFootPosition = LFoot.transform.localPosition;
				targetPositionRecorder.LFootRot = LFoot.transform.localEulerAngles;
			}
			if ((bool)LBendGoal)
			{
				targetPositionRecorder.LBendGoalPosition = LBendGoal.transform.localPosition;
			}
			if ((bool)RBendGoal)
			{
				targetPositionRecorder.RBendGoalPosition = RBendGoal.transform.localPosition;
			}
			if ((bool)LLegBendGoal)
			{
				targetPositionRecorder.LLegBendGoalPosition = LLegBendGoal.transform.localPosition;
			}
			if ((bool)RLegBendGoal)
			{
				targetPositionRecorder.RLegBendGoalPosition = RLegBendGoal.transform.localPosition;
			}
			if ((bool)Head)
			{
				targetPositionRecorder.HeadPosition = Head.transform.localPosition;
				targetPositionRecorder.HeadRotationEuler = Head.transform.localEulerAngles;
			}
			if ((bool)Shifter)
			{
				targetPositionRecorder.ShifterPos = Shifter.transform.localPosition;
				targetPositionRecorder.ShifterRot = Shifter.transform.eulerAngles;
			}
			if (base.transform.localPosition != Vector3.zero)
			{
				targetPositionRecorder.ParentPosion = base.transform.localPosition;
			}
			if (base.transform.localEulerAngles != Vector3.zero)
			{
				targetPositionRecorder.ParentRot = base.transform.localEulerAngles;
			}
		}
	}

	public void CreatePersonTargets(int place)
	{
		if (!allPlaces[place])
		{
			TargetPositionRecorder targetPositionRecorder = allTargets[place];
			GameObject gameObject = new GameObject("Place" + place);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localEulerAngles = targetPositionRecorder.ParentRot;
			gameObject.transform.localPosition = targetPositionRecorder.ParentPosion;
			allPlaces[place] = gameObject.transform;
			GameObject gameObject2 = new GameObject("LHand");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.LHandPosition;
			gameObject2.transform.localEulerAngles = targetPositionRecorder.LHandRot;
			targetPositionRecorder.LHand = gameObject2;
			gameObject2 = new GameObject("RHand");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.RHandPosition;
			gameObject2.transform.localEulerAngles = targetPositionRecorder.RHandRot;
			targetPositionRecorder.RHand = gameObject2;
			gameObject2 = new GameObject("LUpperArm");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.LUpperArmPosition;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			targetPositionRecorder.LUpperArm = gameObject2;
			gameObject2 = new GameObject("RUpperArm");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.RUpperArmPosition;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			targetPositionRecorder.RUpperArm = gameObject2;
			gameObject2 = new GameObject("mainBody");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.mainBodyPosition;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			targetPositionRecorder.mainBody = gameObject2;
			gameObject2 = new GameObject("LFoot");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.LFootPosition;
			gameObject2.transform.localEulerAngles = targetPositionRecorder.LFootRot;
			targetPositionRecorder.LFoot = gameObject2;
			gameObject2 = new GameObject("RFoot");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.RFootPosition;
			gameObject2.transform.localEulerAngles = targetPositionRecorder.RFootRot;
			targetPositionRecorder.RFoot = gameObject2;
			gameObject2 = new GameObject("LBendGoal");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.LBendGoalPosition;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			targetPositionRecorder.LBendGoal = gameObject2;
			if (EnableIkHead)
			{
				gameObject2 = new GameObject("Head");
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localPosition = targetPositionRecorder.HeadPosition;
				gameObject2.transform.localEulerAngles = targetPositionRecorder.HeadRotationEuler;
				targetPositionRecorder.Head = gameObject2;
			}
			gameObject2 = new GameObject("RBendGoal");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.RBendGoalPosition;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			targetPositionRecorder.RBendGoal = gameObject2;
			gameObject2 = new GameObject("LLegBendGoal");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.LLegBendGoalPosition;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			targetPositionRecorder.LLegBendGoal = gameObject2;
			gameObject2 = new GameObject("RLegBendGoal");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.RLegBendGoalPosition;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			targetPositionRecorder.RLegBendGoal = gameObject2;
			gameObject2 = new GameObject("Shifter");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = targetPositionRecorder.ShifterPos;
			gameObject2.transform.localEulerAngles = targetPositionRecorder.ShifterRot;
			targetPositionRecorder.Shifter = gameObject2;
		}
	}

	private GameObject createPrimitive(Color color, GameObject copy)
	{
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		obj.transform.parent = base.transform;
		obj.transform.localPosition = copy.transform.localPosition;
		obj.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
		obj.transform.localRotation = copy.transform.localRotation;
		obj.GetComponent<MeshRenderer>().material.color = color;
		obj.name = copy.name;
		Object.Destroy(copy);
		return obj;
	}
}

using System;
using UnityEngine;

[Serializable]
public class TargetPositionRecorder
{
	[HideInInspector]
	public GameObject LHand;

	[HideInInspector]
	public GameObject RHand;

	public Vector3 LHandPosition;

	public Vector3 LHandRot;

	public Vector3 RHandPosition;

	public Vector3 RHandRot;

	public GameObject LUpperArm;

	public GameObject RUpperArm;

	public Vector3 LUpperArmPosition;

	public Vector3 RUpperArmPosition;

	[HideInInspector]
	public GameObject mainBody;

	public Vector3 mainBodyPosition;

	[HideInInspector]
	public GameObject LFoot;

	[HideInInspector]
	public GameObject RFoot;

	public Vector3 LFootPosition;

	public Vector3 LFootRot;

	public Vector3 RFootPosition;

	public Vector3 RFootRot;

	[HideInInspector]
	public GameObject LBendGoal;

	[HideInInspector]
	public GameObject RBendGoal;

	public Vector3 LBendGoalPosition;

	public Vector3 RBendGoalPosition;

	[HideInInspector]
	public GameObject LLegBendGoal;

	[HideInInspector]
	public GameObject RLegBendGoal;

	public Vector3 LLegBendGoalPosition;

	public Vector3 RLegBendGoalPosition;

	[HideInInspector]
	public GameObject Head;

	public Vector3 HeadPosition;

	public Vector3 HeadRotationEuler;

	public GameObject Shifter;

	public Vector3 ShifterPos;

	public Vector3 ShifterRot;

	public Vector3 ParentPosion;

	public Vector3 ParentRot;

	private void Start()
	{
	}

	private void Update()
	{
	}
}

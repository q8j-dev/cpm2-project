using UnityEngine;

public class LocationFlagsController : MonoBehaviour
{
	[SerializeField]
	private FlagTrigger[] Triggers;

	[SerializeField]
	private int CheckStepsAhead = 3;

	[SerializeField]
	private float CheckRadius = default;

	private FlagNode<IFlagTrigger> _firstNode;

	private void Start()
	{
		InitializeChain();
	}

	[ContextMenu("InitializeComponents")]
	public void Initialize()
	{
		Triggers = GetComponentsInChildren<FlagTrigger>();
	}

	private void InitializeChain()
	{
		if (Triggers == null || Triggers.Length == 0)
		{
			return;
		}
		FlagNode<IFlagTrigger> flagNode = null;
		FlagNode<IFlagTrigger> flagNode2 = null;
		for (int i = 0; i < Triggers.Length; i++)
		{
			FlagNode<IFlagTrigger> flagNode3 = new FlagNode<IFlagTrigger>(Triggers[i]);
			if (_firstNode == null)
			{
				_firstNode = flagNode3;
			}
			Triggers[i].Init(this, flagNode3);
			Triggers[i].SetRadius(CheckRadius);
			if (flagNode != null)
			{
				flagNode.Next = flagNode3;
				flagNode3.Previous = flagNode;
				flagNode2 = flagNode3;
			}
			flagNode = flagNode3;
		}
		flagNode2.Next = _firstNode;
		_firstNode.Previous = flagNode2;
	}

	public void OnFlagEntered(FlagNode<IFlagTrigger> enteredNode)
	{
		CheckNextSteps(enteredNode);
	}

	private void CheckNextSteps(FlagNode<IFlagTrigger> enteredNode)
	{
		FlagNode<IFlagTrigger> flagNode = enteredNode;
		if (flagNode != null)
		{
			for (int i = 0; i < CheckStepsAhead; i++)
			{
				flagNode.Value.CheckTrigger();
				flagNode = flagNode.Next;
			}
		}
	}
}

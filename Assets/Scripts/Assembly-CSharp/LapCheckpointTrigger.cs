using UnityEngine;

public class LapCheckpointTrigger : MonoBehaviour
{
	[SerializeField]
	private bool IsFinishLine;

	[SerializeField]
	private CircuitRaceControlller LapRaceController;

	public int CheckpointIndex;

	private MeshRenderer _meshRenderer;

	private void Awake()
	{
		_meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
	}

	public void Init(int index)
	{
		CheckpointIndex = index;
	}

	public void SetCheckpoint(bool state)
	{
	}

	public int GetCheckPointId()
	{
		return CheckpointIndex;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 31)
		{
			LapRaceController.RegisterCheckPoint(other.gameObject, CheckpointIndex, IsFinishLine);
		}
	}
}

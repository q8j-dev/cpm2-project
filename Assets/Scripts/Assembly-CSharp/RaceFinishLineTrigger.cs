using Car_Parking.Scripts.Racing;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts;

public class RaceFinishLineTrigger : MonoBehaviour
{
	[SerializeField]
	private float TriggerCoolDown = 3f;

	public CheckpointData CheckpointData;

	[SerializeField]
	private bool IsFinishLine;

	[SerializeField]
	private bool IsExitLine;

	[SerializeField]
	private TimeAttackStatus[] StopRaceStatuses;

	[SerializeField]
	private RaceTriggersValidator RaceTriggersValidator;

	[SerializeField]
	private bool DontDisable;

	private FreeTimeAttackController _controller;

	private float _lastPassTime;

	[Inject]
	[UnityEngine.Scripting.Preserve]
	public void Construct(FreeTimeAttackController controller)
	{
		_controller = controller;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != LayerViaName.CarLayer || !other.transform.parent.TryGetComponent<FreeRaceCar>(out var component))
		{
			return;
		}
		if (IsExitLine)
		{
			component.StopRace(StopRaceStatuses);
		}
		else
		{
			if (Time.time - _lastPassTime <= TriggerCoolDown)
			{
				return;
			}
			_lastPassTime = Time.time;
			CheckpointData.time = Time.time - _controller.StartTime;
			if (IsFinishLine)
			{
				component.OnCrossFinishLane();
				return;
			}
			CheckpointData.missed = false;
			if (DontDisable)
			{
				if (component.CurrentTriggerIndex >= CheckpointData.id)
				{
					component.UserWrongWay();
				}
				else
				{
					component.CurrentTriggerIndex = CheckpointData.id;
				}
			}
			else
			{
				component.CurrentTriggerIndex = CheckpointData.id;
				base.gameObject.SetActive(value: false);
			}
		}
	}
}

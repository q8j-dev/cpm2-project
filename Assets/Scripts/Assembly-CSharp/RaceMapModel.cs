using UnityEngine;

public class RaceMapModel : MonoBehaviour
{
	public int index;

	public GameObject StartArc;

	public Transform[] StartPositions;

	public Transform[] ReverseStartPositions;

	public Transform CheckPointsParent;

	public Transform StarLine;

	public Transform CheckerParent;

	public GameObject[] RedLights;

	public GameObject GreenLights;

	public RaceConfigSO RaceConfig;

	public GameObject[] ObstaclesForEnable;

	public float[] StartArcYAngles;

	public CircuitLapMapElement[] MapElements;

	public GameObject MapFinishObject;

	public float StartCarYAngle;

	public Texture MapTexture;

	public int CarsCount;

	public void SetCheckpointsDirection(int direction = 0)
	{
		if (direction == 0)
		{
			direction = RaceConfig.Direction;
		}
		SetDirection(direction);
	}

	public void SetDirectionForTest(int direction)
	{
		SetDirection(direction);
	}

	private void SetDirection(int direction)
	{
		Quaternion localRotation = StartArc.transform.rotation;
		localRotation = new Quaternion(localRotation.x, (direction > 0) ? StartArcYAngles[0] : StartArcYAngles[1], localRotation.z, localRotation.w);
		StartArc.transform.localRotation = localRotation;
	}

	public void SetUIMapElements(bool state)
	{
		for (int i = 0; i < MapElements.Length; i++)
		{
			MapElements[i].Select(state);
		}
	}
}

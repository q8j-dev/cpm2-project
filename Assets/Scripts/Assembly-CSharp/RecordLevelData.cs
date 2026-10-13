using UnityEngine;

public class RecordLevelData : MonoBehaviour
{
	public Vector3 lookAtPoint2 = Vector3.zero;

	public LevelScript LevelScript;

	public int indexEdit;

	public GameObject car;

	public GameObject parkingCubes;

	public Material[] mainMats;

	public void RecordInitialCam()
	{
		LevelScript.LevelsProvider.CurrentLevelData.InitialCamPos = base.transform.position;
		LevelScript.LevelsProvider.CurrentLevelData.InitialCamRot = base.transform.eulerAngles;
	}

	public void RecordFixedCam()
	{
		LevelScript.LevelsProvider.CurrentLevelData.FixedCamPos = base.transform.position;
	}

	public void RecordInitialCar()
	{
		LevelScript.LevelsProvider.CurrentLevelData.CarInitialPos = car.transform.position;
		LevelScript.LevelsProvider.CurrentLevelData.CarInitRot = car.transform.eulerAngles;
	}

	public void RecordFinalCar()
	{
		LevelScript.LevelsProvider.CurrentLevelData.CarTargetPos = car.transform.position;
		LevelScript.LevelsProvider.CurrentLevelData.CarTargetRot = car.transform.eulerAngles;
		if (!parkingCubes)
		{
			LevelScript.LevelsProvider.CurrentLevelData.ParkingTriggerParentPos = car.transform.position;
			LevelScript.LevelsProvider.CurrentLevelData.ParkingTriggerParentRot = car.transform.eulerAngles;
		}
	}

	public void RecordParkingCubes()
	{
		LevelScript.LevelsProvider.CurrentLevelData.ParkingTriggerParentPos = parkingCubes.transform.position;
		LevelScript.LevelsProvider.CurrentLevelData.ParkingTriggerParentRot = parkingCubes.transform.eulerAngles;
	}

	public void SetCarsMaterial()
	{
		foreach (Transform item in car.gameObject.transform)
		{
			Material materialByName = GetMaterialByName(item.gameObject.name);
			if ((bool)materialByName)
			{
				item.gameObject.GetComponent<MeshRenderer>().material = materialByName;
			}
		}
	}

	private Material GetMaterialByName(string matName)
	{
		if ("Default-Material" == matName.Trim())
		{
			return mainMats[0];
		}
		Material[] array = mainMats;
		foreach (Material material in array)
		{
			if (material.name.Trim() == matName.Trim())
			{
				MonoBehaviour.print(material.name);
				return material;
			}
		}
		return null;
	}
}

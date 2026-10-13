using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Levels/OneLevelScript", order = 21)]
public class LevelDataScript : ScriptableObject
{
	public LevelType Type;

	public int ID;

	public int SceneForLoad;

	public int IndexInScene;

	public string Title;

	public int CategoryIndex;

	public ObscuredFloat LevelTimeObscured;

	public ObscuredBool RewardByTime = true;

	[Header("Car Settings")]
	public int CarId;

	public MainCarValues CarConfig;

	[Header("Racing Level Settings")]
	public int RacingCarId;

	public Vector3 RacingCarInitialPos;

	public Vector3 RacingCarInitialRot;

	public ObscuredInt Reward;

	[Header("Camera Start Position")]
	public Vector3 InitialCamPos;

	public Vector3 InitialCamRot;

	[Header("Car Start Position")]
	public Vector3 CarInitialPos;

	public Vector3 CarInitRot;

	[Header("Car Finish Position")]
	public Vector3 CarTargetPos;

	public Vector3 CarTargetRot;

	public bool SetCarTargetPos;

	public bool SetCarTargetRot;

	[Header("Finish Position")]
	public Vector3 ParkingTriggerParentPos;

	public Vector3 ParkingTriggerParentRot;

	[Header("Trailer Options")]
	public bool WithTrailer;

	public bool LoadTrailer;

	public int TrailerIndex;

	public int HasLoad;

	[Header("Tow truck Options")]
	public bool TowCarLevel;

	public int TowCarId;

	public Vector3 TowCarPos;

	[Header("Camera Options")]
	public bool HasFixedCam;

	public bool DisableCamChange;

	public bool IsReverseParking;

	public Vector3 FixedCamPos;

	public bool StopCarCompletely;

	public bool InParkingScene;

	public bool OutSideStart;

	public bool DriverOutSideBegin;

	public Vector3 DriverStartPos;

	public Vector3 DriverStartRot;

	public bool ChangeCamToFixed;

	public int InitialCamIndex = -1;

	public CameraDatas[] CameraData;

	public float RelativeTrailerAngle;

	public int StartGear = 4;

	public int ParkingToUse = 1;

	public int ShowPartsWhenStartPressed;

	public bool EnableOtherSceneStart;

	public bool EnableParkingSceneStart;

	public bool CheckForStop = true;

	public float FieldOfView = 60f;

	public string[] Messages;

	public bool ignoreCollision;

	public bool HideTopLabel;

	public bool IsCarCameraSettingForLevel;

	public float LimitCarDamaged;

	[Header("Ads")]
	public bool DisableAdContinue;

	public LevelDataScript GetCopy()
	{
		return (LevelDataScript)MemberwiseClone();
	}
}

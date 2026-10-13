using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Other.Helpers;

public class AnalyticWheres : MonoBehaviour
{
	public float level;

	public ObscuredFloat multiFreeDrive;

	public ObscuredFloat singleFree;

	public ObscuredFloat DragRacing;

	public ObscuredFloat CircuitRacing;

	public ObscuredFloat last_time_up;

	public float last_send;

	public float last_save;

	private int up_skip;

	internal void Update()
	{
		if (++up_skip >= 200)
		{
			up_skip = 0;
			float num = Time.time * 1000f;
			float num2 = (num - (float)last_time_up) / 60000f;
			last_time_up = num;
			last_send += num2;
			last_save += num2;
			if (FlagsStorage.LevelStarted)
			{
				level += num2;
			}
			else if (FlagsStorage.IsMultiFree)
			{
				multiFreeDrive = (float)multiFreeDrive + num2;
			}
			else if (SceneManager.GetActiveScene().buildIndex == 13)
			{
				singleFree = (float)singleFree + num2;
			}
			else if (ScenesHelper.IsDragRacingSceneActive())
			{
				DragRacing = (float)DragRacing + num2;
			}
			else if (ScenesHelper.IsLapRacingSceneActive())
			{
				CircuitRacing = (float)CircuitRacing + num2;
			}
			if (last_send >= 5f)
			{
				last_send = 0f;
				return;
			}
			_ = last_save;
			_ = 1f;
		}
	}

	public void Save()
	{
		AllTimeGamePlayUtil.Set((float)AllTimeGamePlayUtil.Get() + level + (float)multiFreeDrive + (float)singleFree + (float)DragRacing + (float)CircuitRacing);
		float num = ObscuredPrefs.GetFloat("ana_levels");
		ObscuredPrefs.SetFloat("ana_levels", num + level);
		float num2 = ObscuredPrefs.GetFloat("ana_multiFree");
		ObscuredPrefs.SetFloat("ana_multiFree", (float)multiFreeDrive + num2);
		float num3 = ObscuredPrefs.GetFloat("ana_singleFree");
		ObscuredPrefs.SetFloat("ana_singleFree", (float)singleFree + num3);
		last_save = 0f;
		level = (multiFreeDrive = (singleFree = (DragRacing = (CircuitRacing = 0f))));
	}

	private void Start()
	{
		last_time_up = Time.time * 1000f;
		V.AnalyticWheres = this;
		level = 0f;
		multiFreeDrive = 0f;
		singleFree = 0f;
		DragRacing = 0f;
		CircuitRacing = 0f;
	}
}

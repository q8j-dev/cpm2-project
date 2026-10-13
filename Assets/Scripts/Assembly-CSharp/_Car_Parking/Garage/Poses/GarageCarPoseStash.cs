using System;
using System.Collections.Generic;
using Persistence.Systems;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Garage.Poses
{
	public class GarageCarPoseStash : IAllPrefsSaver
	{
		[Serializable]
		private struct Entry
		{
			public string CarId;

			public SavedPoseDto Pose;
		}

		[Serializable]
		private class Payload
		{
			public List<Entry> Entries = new List<Entry>();
		}

		private const string PrefsKey = "GarageCarPosesStash";

		private const int MaxEntries = 512;

		private Payload _snapshot;

		public void Save()
		{
			try
			{
				Payload payload = ReadStash();
				int num = 0;
				if (EasySaveDataHandler.TryGetFile("GarageCarPoses_v2.es3", out var file) && file != null)
				{
					string[] keys = file.GetKeys();
					foreach (string text in keys)
					{
						List<SavedPoseDto> list = file.Load<List<SavedPoseDto>>(text, null);
						if (list == null)
						{
							continue;
						}
						foreach (SavedPoseDto item in list)
						{
							Upsert(payload, text, item);
							num++;
						}
					}
				}
				Trim(payload);
				_snapshot = payload;
			}
			catch (Exception arg)
			{
				_snapshot = null;
				Debug.LogWarning($"[GarageCarPoseStash] Could not snapshot garage poses before wipe: {arg}");
			}
		}

		public void Load()
		{
			if (_snapshot != null)
			{
				WriteStash(_snapshot);
				_snapshot = null;
				PlayerPrefs.Save();
			}
		}

		public static void RestoreInto(string carId)
		{
			if (string.IsNullOrEmpty(carId) || !PlayerPrefs.HasKey("GarageCarPosesStash"))
			{
				return;
			}
			try
			{
				Payload stash = ReadStash();
				List<SavedPoseDto> list = Take(stash, carId);
				if (list.Count == 0)
				{
					return;
				}
				List<SavedPoseDto> list2 = null;
				if (EasySaveDataHandler.TryGetFile("GarageCarPoses_v2.es3", out var file) && file != null && file.KeyExists(carId))
				{
					list2 = file.Load(carId, new List<SavedPoseDto>());
				}
				if (list2 == null)
				{
					list2 = new List<SavedPoseDto>();
				}
				foreach (SavedPoseDto item in list)
				{
					int garageId = item.GarageId;
					if (!list2.Exists((SavedPoseDto p) => p.GarageId == garageId))
					{
						list2.Add(item);
					}
				}
				EasySaveDataHandler.SaveValues(new Dictionary<string, object> { { carId, list2 } }, "GarageCarPoses_v2.es3");
				WriteStash(stash);
			}
			catch (Exception arg)
			{
				Debug.LogWarning($"[GarageCarPoseStash] Could not restore stashed poses for {carId}: {arg}");
			}
		}

		public static void Forget(string carId)
		{
			if (!string.IsNullOrEmpty(carId) && PlayerPrefs.HasKey("GarageCarPosesStash"))
			{
				Payload stash = ReadStash();
				if (Take(stash, carId).Count > 0)
				{
					WriteStash(stash);
				}
			}
		}

		private static Payload ReadStash()
		{
			string text = PlayerPrefs.GetString("GarageCarPosesStash", string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				return new Payload();
			}
			try
			{
				Payload payload = JsonUtility.FromJson<Payload>(text);
				if (payload?.Entries != null)
				{
					return payload;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[GarageCarPoseStash] Corrupt stash dropped: " + ex.Message);
			}
			return new Payload();
		}

		private static void WriteStash(Payload stash)
		{
			if (stash.Entries.Count == 0)
			{
				PlayerPrefs.DeleteKey("GarageCarPosesStash");
			}
			else
			{
				PlayerPrefs.SetString("GarageCarPosesStash", JsonUtility.ToJson(stash));
			}
		}

		private static void Upsert(Payload stash, string carId, SavedPoseDto pose)
		{
			stash.Entries.RemoveAll((Entry e) => e.CarId == carId && e.Pose.GarageId == pose.GarageId);
			stash.Entries.Add(new Entry
			{
				CarId = carId,
				Pose = pose
			});
		}

		private static List<SavedPoseDto> Take(Payload stash, string carId)
		{
			List<SavedPoseDto> list = new List<SavedPoseDto>();
			foreach (Entry entry in stash.Entries)
			{
				if (entry.CarId == carId)
				{
					list.Add(entry.Pose);
				}
			}
			if (list.Count > 0)
			{
				stash.Entries.RemoveAll((Entry e) => e.CarId == carId);
			}
			return list;
		}

		private static void Trim(Payload stash)
		{
			int num = stash.Entries.Count - 512;
			if (num > 0)
			{
				stash.Entries.RemoveRange(0, num);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CPM.Ghost.Core;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace CPM.Ghost
{
	public sealed class GhostStorage
	{
		private const string IndexFile = "index.json";

		private const string GhostExt = ".ghost";

		public const string RootFolderName = "ghosts";

		private readonly IGhostLogger _log;

		private readonly string _root;

		private readonly Dictionary<string, List<GhostRecordingMeta>> _indexCache = new Dictionary<string, List<GhostRecordingMeta>>();

		public GhostStorage(IGhostLogger log = null)
			: this(Path.Combine(Application.persistentDataPath, "ghosts"), log)
		{
		}

		public GhostStorage(string rootDirectory, IGhostLogger log = null)
		{
			_root = rootDirectory;
			_log = log ?? NullGhostLogger.Instance;
		}

		private string RaceDir(string raceKey)
		{
			return Path.Combine(_root, raceKey);
		}

		private string IndexPath(string raceKey)
		{
			return Path.Combine(RaceDir(raceKey), "index.json");
		}

		private string GhostPath(string raceKey, string replayId)
		{
			return Path.Combine(RaceDir(raceKey), replayId + ".ghost");
		}

		public async UniTask SaveAsync(GhostRecording rec, int maxReplays, CancellationToken ct)
		{
			if (rec == null)
			{
				_log.Warn("[Ghost] SaveAsync: null recording");
				return;
			}
			try
			{
				byte[] bytes = await UniTask.RunOnThreadPool(() => GhostSerializer.Serialize(rec), configureAwait: true, ct);
				await UniTask.RunOnThreadPool(() =>
				{
					Directory.CreateDirectory(RaceDir(rec.RaceKey));
					File.WriteAllBytes(GhostPath(rec.RaceKey, rec.ReplayId), bytes);
				}, configureAwait: true, ct);
				UpsertMeta(rec);
				PruneToTop(rec.RaceKey, Mathf.Max(1, maxReplays));
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				_log.Error("[Ghost] SaveAsync failed: " + ex2.Message);
			}
		}

		public async UniTask<GhostRecording> LoadAsync(string raceKey, string replayId, CancellationToken ct)
		{
			try
			{
				string path = GhostPath(raceKey, replayId);
				if (!File.Exists(path))
				{
					return null;
				}
				return await UniTask.RunOnThreadPool(() => GhostSerializer.Deserialize(File.ReadAllBytes(path)), configureAwait: true, ct);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				_log.Error("[Ghost] LoadAsync failed: " + ex2.Message);
				return null;
			}
		}

		public async UniTask<GhostRecording> LoadBestRecordingAsync(string raceKey, CancellationToken ct)
		{
			GhostRecordingMeta meta;
			return (!TryGetBestMeta(raceKey, out meta)) ? null : (await LoadAsync(raceKey, meta.ReplayId, ct));
		}

		public bool TryGetBestMeta(string raceKey, out GhostRecordingMeta meta)
		{
			List<GhostRecordingMeta> list = LoadIndex(raceKey);
			meta = null;
			foreach (GhostRecordingMeta item in list)
			{
				if (meta == null || item.TimeMs < meta.TimeMs)
				{
					meta = item;
				}
			}
			return meta != null;
		}

		public void DeleteAllForRace(string raceKey)
		{
			try
			{
				if (Directory.Exists(RaceDir(raceKey)))
				{
					Directory.Delete(RaceDir(raceKey), recursive: true);
				}
				_indexCache.Remove(raceKey);
			}
			catch (Exception ex)
			{
				_log.Error("[Ghost] DeleteAllForRace failed: " + ex.Message);
			}
		}

		public void DeleteAll()
		{
			try
			{
				if (Directory.Exists(_root))
				{
					Directory.Delete(_root, recursive: true);
				}
				_indexCache.Clear();
			}
			catch (Exception ex)
			{
				_log.Error("[Ghost] DeleteAll failed: " + ex.Message);
			}
		}

		private List<GhostRecordingMeta> LoadIndex(string raceKey)
		{
			if (_indexCache.TryGetValue(raceKey, out var value))
			{
				return value;
			}
			List<GhostRecordingMeta> list = new List<GhostRecordingMeta>();
			try
			{
				string path = IndexPath(raceKey);
				if (File.Exists(path))
				{
					list = JsonConvert.DeserializeObject<List<GhostRecordingMeta>>(File.ReadAllText(path)) ?? new List<GhostRecordingMeta>();
				}
			}
			catch (Exception ex)
			{
				_log.Error("[Ghost] LoadIndex failed: " + ex.Message);
			}
			_indexCache[raceKey] = list;
			return list;
		}

		private void SaveIndex(string raceKey, List<GhostRecordingMeta> list)
		{
			try
			{
				Directory.CreateDirectory(RaceDir(raceKey));
				File.WriteAllText(IndexPath(raceKey), JsonConvert.SerializeObject(list));
				_indexCache[raceKey] = list;
			}
			catch (Exception ex)
			{
				_log.Error("[Ghost] SaveIndex failed: " + ex.Message);
			}
		}

		private void UpsertMeta(GhostRecording rec)
		{
			List<GhostRecordingMeta> list = LoadIndex(rec.RaceKey);
			list.RemoveAll((GhostRecordingMeta m) => m.ReplayId == rec.ReplayId);
			list.Add(new GhostRecordingMeta
			{
				ReplayId = rec.ReplayId,
				RaceKey = rec.RaceKey,
				CarId = rec.CarId,
				TimeMs = rec.TimeMs,
				RecordedAtUnixMs = rec.RecordedAtUnixMs
			});
			SaveIndex(rec.RaceKey, list);
		}

		private void PruneToTop(string raceKey, int max)
		{
			List<GhostRecordingMeta> list = LoadIndex(raceKey);
			if (list.Count <= max)
			{
				return;
			}
			List<GhostRecordingMeta> list2 = list.OrderBy((GhostRecordingMeta m) => m.TimeMs).Take(max).ToList();
			HashSet<string> keepIds = new HashSet<string>(list2.Select((GhostRecordingMeta m) => m.ReplayId));
			foreach (GhostRecordingMeta item in list.Where((GhostRecordingMeta m) => !keepIds.Contains(m.ReplayId)))
			{
				try
				{
					string path = GhostPath(raceKey, item.ReplayId);
					if (File.Exists(path))
					{
						File.Delete(path);
					}
				}
				catch (Exception ex)
				{
					_log.Error("[Ghost] Prune delete failed: " + ex.Message);
				}
			}
			SaveIndex(raceKey, list2);
		}
	}
}

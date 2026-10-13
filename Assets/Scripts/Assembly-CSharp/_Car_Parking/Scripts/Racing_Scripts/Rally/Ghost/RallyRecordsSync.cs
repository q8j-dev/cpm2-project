using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Ghost.Core;
using CPM.PlayerData;
using CPM.RacingRecords;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	[Preserve]
	public sealed class RallyRecordsSync
	{
		private const string Mode = "race";

		private readonly IRacingRecordsBackend _backend;

		private readonly PlayerDataStorage _playerData;

		private readonly Dictionary<string, GhostRecording> _opponentCache = new Dictionary<string, GhostRecording>();

		public event Action<string> ResultSubmitted;

		public RallyRecordsSync(IRacingRecordsBackend backend, PlayerDataStorage playerData)
		{
			_backend = backend;
			_playerData = playerData;
		}

		public void SubmitStart(string raceUid, string raceKey, int carId, int carClass, bool rain, CancellationToken ct)
		{
			_backend.StartRace("race", raceUid, raceKey, null, carId, carClass, rain, ct).Forget();
		}

		public void SubmitFinish(string raceUid, string raceKey, int carId, int carClass, long score, GhostRecording recording, CancellationToken ct)
		{
			SubmitFinishAsync(raceUid, raceKey, carId, carClass, score, recording, ct).Forget();
		}

		private async UniTaskVoid SubmitFinishAsync(string raceUid, string raceKey, int carId, int carClass, long score, GhostRecording recording, CancellationToken ct)
		{
			string nickname = ((_playerData != null) ? _playerData.PlayerName : string.Empty);
			await _backend.SubmitResult("race", raceUid, raceKey, nickname, carId, carClass, score, ct);
			ResultSubmitted?.Invoke(raceKey);
			await _backend.SubmitReplay("race", raceUid, raceKey, null, score, recording, ct);
		}

		public async UniTask<GhostRecording> DownloadOpponent(string userId, string raceKey, CancellationToken ct)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(raceKey))
			{
				return null;
			}
			string key = userId + "|" + raceKey;
			if (_opponentCache.TryGetValue(key, out var value))
			{
				return value;
			}
			Result<GhostRecording> result = await _backend.GetReplay("race", userId, raceKey, ct);
			if (result.IsFail)
			{
				return null;
			}
			_opponentCache[key] = result.Value;
			return result.Value;
		}

		public void ClearOpponentCache()
		{
			_opponentCache.Clear();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CPM.Ghost.Core;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace CPM.RacingRecords
{
	[Preserve]
	public class RacingRecordsBackend : IRacingRecordsBackend
	{
		private readonly BackendClient _backendClient;

		public RacingRecordsBackend(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<Result<IReadOnlyList<RacingRecord>>> GetRecords(string mode, string mapId, int count, int page, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(mapId) || count <= 0)
			{
				return Result<IReadOnlyList<RacingRecord>>.Fail();
			}
			HttpRequest request = await BackendFactory.CreateRequest($"tops-service/users/v1/{mode}/{mapId}/top?count={count}&page={page}", HttpVerb.Get, cancellationToken);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, cancellationToken);
			if (result.IsFail)
			{
				return Result<IReadOnlyList<RacingRecord>>.Fail();
			}
			string value = Encoding.UTF8.GetString(result.Value.Body);
			try
			{
				List<RacingRecord> list = JsonConvert.DeserializeObject<List<RacingRecord>>(value);
				if (list == null)
				{
					return Result<IReadOnlyList<RacingRecord>>.Fail();
				}
				return Result<IReadOnlyList<RacingRecord>>.Success(list);
			}
			catch (Exception)
			{
				return Result<IReadOnlyList<RacingRecord>>.Fail();
			}
		}

		public async UniTask<Result<bool>> StartRace(string mode, string raceUid, string raceKey, string roomId, int carId, int carClass, bool rain, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(raceUid) || string.IsNullOrEmpty(raceKey))
			{
				return Result<bool>.Fail();
			}
			StartRaceRequest payload = new StartRaceRequest
			{
				RaceUid = raceUid,
				RaceKey = raceKey,
				RoomId = roomId,
				CarId = carId,
				CarClass = carClass,
				Rain = rain
			};
			return await PostAsync("race-service/v1/" + mode + "/start", payload, cancellationToken);
		}

		public async UniTask<Result<bool>> SubmitResult(string mode, string raceUid, string raceKey, string nickname, int carId, int carClass, long score, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(raceUid) || string.IsNullOrEmpty(raceKey))
			{
				return Result<bool>.Fail();
			}
			RaceResultRequest payload = new RaceResultRequest
			{
				RaceUid = raceUid,
				RaceKey = raceKey,
				Nickname = (nickname ?? string.Empty),
				CarId = carId,
				CarClass = carClass,
				Score = score
			};
			return await PostAsync("race-service/v1/" + mode + "/result", payload, cancellationToken);
		}

		public async UniTask<Result<bool>> SubmitReplay(string mode, string raceUid, string raceKey, string roomId, long score, GhostRecording recording, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(raceUid) || string.IsNullOrEmpty(raceKey) || recording == null)
			{
				return Result<bool>.Fail();
			}
			RaceReplayRequest payload = RaceReplayRequest.FromRecording(raceUid, raceKey, roomId, score, recording);
			return await PostAsync("race-service/v1/" + mode + "/replay", payload, cancellationToken);
		}

		public async UniTask<Result<GhostRecording>> GetReplay(string mode, string userId, string raceKey, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(raceKey))
			{
				return Result<GhostRecording>.Fail();
			}
			HttpRequest request = await BackendFactory.CreateRequest("race-service/v1/" + mode + "/replay/" + userId + "/" + raceKey, HttpVerb.Get, cancellationToken);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, cancellationToken);
			if (result.IsFail)
			{
				return Result<GhostRecording>.Fail();
			}
			try
			{
				ReplayResponse replayResponse = JsonConvert.DeserializeObject<ReplayResponse>(Encoding.UTF8.GetString(result.Value.Body));
				if (replayResponse == null || !replayResponse.TryGetRecording(out var recording))
				{
					return Result<GhostRecording>.Fail();
				}
				return Result<GhostRecording>.Success(recording);
			}
			catch (Exception)
			{
				return Result<GhostRecording>.Fail();
			}
		}

		private async UniTask<Result<bool>> PostAsync<T>(string url, T payload, CancellationToken ct)
		{
			HttpRequest httpRequest = await BackendFactory.CreateRequest(url, HttpVerb.Post, ct);
			httpRequest.Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
			httpRequest.BodyType = HttpBodyType.Json;
			if ((await _backendClient.HttpAsync(httpRequest, ct)).IsFail)
			{
				return Result<bool>.Fail();
			}
			return Result<bool>.Success(result: true);
		}
	}
}

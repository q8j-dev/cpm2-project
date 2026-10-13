using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[UnityEngine.Scripting.Preserve]
	public class PlayerProfileBackend : IPlayerProfileBackend
	{
		private readonly BackendClient _backendClient;

		[Inject]
		public PlayerProfileBackend(BackendClient backendClient)
		{
			_backendClient = backendClient;
		}

		public async UniTask<Result<PlayerProfileData>> GetProfileAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!(await BackendFactory.WaitForAuthTokenAsync(cancellationToken)))
			{
				return Result<PlayerProfileData>.Fail();
			}
			HttpRequest request = await BackendFactory.CreateRequest("progress-service/v1/player/profile", HttpVerb.Get, cancellationToken);
			Result<HttpResponse, Exception> result = await _backendClient.HttpAsync(request, cancellationToken);
			if (result.IsFail)
			{
				return Result<PlayerProfileData>.Fail();
			}
			try
			{
				PlayerProfileData playerProfileData = JsonConvert.DeserializeObject<PlayerProfileData>(Encoding.UTF8.GetString(result.Value.Body));
				if (playerProfileData == null)
				{
					return Result<PlayerProfileData>.Fail();
				}
				return Result<PlayerProfileData>.Success(playerProfileData);
			}
			catch (Exception)
			{
				return Result<PlayerProfileData>.Fail();
			}
		}

		public async UniTask<Result<bool>> SaveFavouritesAsync(ProfileFavouriteItemsData favourites, CancellationToken cancellationToken = default(CancellationToken))
		{
			HttpRequest httpRequest = await BackendFactory.CreateRequest("progress-service/v1/player/favourites", HttpVerb.Post, cancellationToken);
			httpRequest.Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(favourites));
			httpRequest.BodyType = HttpBodyType.Json;
			if ((await _backendClient.HttpAsync(httpRequest, cancellationToken)).IsFail)
			{
				return Result<bool>.Fail();
			}
			return Result<bool>.Success(result: true);
		}
	}
}

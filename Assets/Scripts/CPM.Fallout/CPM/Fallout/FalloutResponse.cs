using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace CPM.Fallout
{
	[Preserve]
	public static class FalloutResponse
	{
		public static Result<bool> ParseOk(byte[] body)
		{
			BackendEnvelope<object> backendEnvelope = Deserialize<object>(body);
			if (backendEnvelope == null || backendEnvelope.Code != 1)
			{
				return Result<bool>.Fail();
			}
			return Result<bool>.Success(result: true);
		}

		public static Result<TData> Parse<TData>(byte[] body) where TData : class
		{
			BackendEnvelope<TData> backendEnvelope = Deserialize<TData>(body);
			if (backendEnvelope == null || backendEnvelope.Code != 1 || backendEnvelope.Data == null)
			{
				return Result<TData>.Fail();
			}
			return Result<TData>.Success(backendEnvelope.Data);
		}

		private static BackendEnvelope<T> Deserialize<T>(byte[] body)
		{
			if (body == null || body.Length == 0)
			{
				return null;
			}
			try
			{
				return JsonConvert.DeserializeObject<BackendEnvelope<T>>(Encoding.UTF8.GetString(body));
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}

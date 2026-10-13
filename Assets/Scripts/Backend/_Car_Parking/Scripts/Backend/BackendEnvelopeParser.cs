using System;
using System.IO;
using System.Text;
using Firebase.Functions;
using Newtonsoft.Json;

namespace _Car_Parking.Scripts.Backend
{
	public static class BackendEnvelopeParser
	{
		private static readonly JsonSerializer Serializer = JsonSerializer.CreateDefault();

		public static FunctionCallResult<T> Parse<T>(byte[] body) where T : class
		{
			if (body == null || body.Length == 0)
			{
				return FunctionCallResult<T>.Fail(FunctionStatus.ParseErrorServerFunctionResponse, "empty body");
			}
			try
			{
				BackendEnvelope<T> backendEnvelope;
				using (MemoryStream stream = new MemoryStream(body, writable: false))
				{
					using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
					using JsonTextReader reader2 = new JsonTextReader(reader);
					backendEnvelope = Serializer.Deserialize<BackendEnvelope<T>>(reader2);
				}
				if (backendEnvelope == null)
				{
					return FunctionCallResult<T>.Fail(FunctionStatus.ResultDataIsNull, "envelope null");
				}
				if (backendEnvelope.Code != 1)
				{
					return FunctionCallResult<T>.Fail((FunctionStatus)backendEnvelope.Code);
				}
				if (backendEnvelope.Data == null)
				{
					return FunctionCallResult<T>.Fail(FunctionStatus.ResultDataIsNull, "data null");
				}
				return FunctionCallResult<T>.Ok(backendEnvelope.Data);
			}
			catch (Exception ex)
			{
				return FunctionCallResult<T>.Fail(FunctionStatus.ParseErrorServerFunctionResponse, ex.Message);
			}
		}
	}
}

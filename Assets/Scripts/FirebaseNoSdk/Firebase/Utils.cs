using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Firebase.Errors;
using Newtonsoft.Json;
using Unions;
using UnityEngine;
using UnityEngine.Networking;

namespace Firebase
{
	public static class Utils
	{
		public static async UniTask<Union<TRes, NoInternetConnection, HttpError>> PostAsync<TArg, TRes>(string url, TArg body, int? timeout, Dictionary<string, string> headers = null)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return new NoInternetConnection();
			}
			using UnityWebRequest r = new UnityWebRequest(url, "POST");
			string s = JsonConvert.SerializeObject(body);
			r.timeout = timeout ?? 60;
			r.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(s));
			r.downloadHandler = new DownloadHandlerBuffer();
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					r.SetRequestHeader(header.Key, header.Value);
				}
			}
			_ = r.SendWebRequest();
			while (!r.isDone)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), DelayType.UnscaledDeltaTime);
			}
			UnityWebRequest.Result result = r.result;
			if ((uint)(result - 2) <= 2u)
			{
				return new HttpError(r.responseCode, r.downloadHandler.text);
			}
			return JsonConvert.DeserializeObject<TRes>(r.downloadHandler.text);
		}

		public static async UniTask<Union<string, NoInternetConnection, HttpError>> PostRawAsync<TArg>(string url, TArg body, int? timeout, Dictionary<string, string> headers = null)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return new NoInternetConnection();
			}
			using UnityWebRequest r = new UnityWebRequest(url, "POST");
			string s = JsonConvert.SerializeObject(body);
			r.timeout = timeout ?? 60;
			r.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(s));
			r.downloadHandler = new DownloadHandlerBuffer();
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					r.SetRequestHeader(header.Key, header.Value);
				}
			}
			_ = r.SendWebRequest();
			while (!r.isDone)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), DelayType.UnscaledDeltaTime);
			}
			string text = r.downloadHandler.text;
			UnityWebRequest.Result result = r.result;
			if ((uint)(result - 2) <= 2u)
			{
				return new HttpError(r.responseCode, text);
			}
			return text;
		}

		public static async UniTask<Union<TRes, NoInternetConnection, HttpError, GoogleError>> PostAsyncWithErrorHandling<TArg, TRes>(string url, TArg body, Dictionary<string, string> headers = null)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return new NoInternetConnection();
			}
			using UnityWebRequest r = new UnityWebRequest(url, "POST");
			string s = JsonConvert.SerializeObject(body);
			r.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(s));
			r.downloadHandler = new DownloadHandlerBuffer();
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					r.SetRequestHeader(header.Key, header.Value);
				}
			}
			_ = r.SendWebRequest();
			while (!r.isDone)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), DelayType.UnscaledDeltaTime);
			}
			string text = r.downloadHandler.text;
			UnityWebRequest.Result result = r.result;
			bool transportError = result == UnityWebRequest.Result.ConnectionError || result == UnityWebRequest.Result.DataProcessingError || result == UnityWebRequest.Result.ProtocolError;
			return ParseResponse<TRes>(text, r.responseCode, transportError);
		}

		public static Union<TRes, NoInternetConnection, HttpError, GoogleError> ParseResponse<TRes>(string res, long responseCode, bool transportError)
		{
			GoogleErrorHolder googleErrorHolder = null;
			try
			{
				googleErrorHolder = JsonConvert.DeserializeObject<GoogleErrorHolder>(res);
			}
			catch (JsonException)
			{
			}
			if (googleErrorHolder != null && googleErrorHolder.error != null)
			{
				return googleErrorHolder.error;
			}
			if (transportError)
			{
				return new HttpError(responseCode, res);
			}
			try
			{
				return JsonConvert.DeserializeObject<TRes>(res);
			}
			catch (JsonException)
			{
				return new HttpError(responseCode, res);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Networking;

namespace _Car_Parking.Scripts.Analytics.Root
{
	public static class AnalyticsTransport
	{
		private const string Protocol = "https";

		private const string Host = "192.168.0.138";

		private const int Port = 8080;

		private const string ApiVersion = "v1";

		private const string Project = "CPM_RACING";

		private const string ApiKey = "320b93f3-e7f4-410a-a52c-e24da363ad04";

		private static string _env;

		private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
		{
			Converters = { (JsonConverter)new StringEnumConverter() }
		};

		private static string BundleId => UnityEngine.Device.SystemInfo.deviceUniqueIdentifier;

		private static string BuildUrl()
		{
			return "https://stats.ogames.kz/v1/project/CPM_RACING/batch/push";
		}

		public static void SetEnv(string env)
		{
			_env = env;
		}

		public static async Task<bool> SendBatchAsync(List<Dictionary<string, object>> batch)
		{
			if (!_env.Equals("dev", StringComparison.Ordinal) && !_env.Equals("prod", StringComparison.Ordinal))
			{
				CPMAnalyticDebug.Log("Environment isn't valid. (env: " + _env + ")", LogType.Error);
				return false;
			}
			string text = JsonConvert.SerializeObject(batch, Formatting.Indented, JsonSettings);
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			using UnityWebRequest request = new UnityWebRequest(BuildUrl(), "POST");
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.SetRequestHeader("X-Bundle-Id", BundleId);
			request.SetRequestHeader("X-Client-Env", _env);
			request.SetRequestHeader("X-Api-Key", "320b93f3-e7f4-410a-a52c-e24da363ad04");
			CPMAnalyticDebug.Log("Batch send:" + text);
			UnityWebRequestAsyncOperation operation = request.SendWebRequest();
			while (!operation.isDone)
			{
				await Task.Yield();
			}
			if (request.result == UnityWebRequest.Result.Success)
			{
				CPMAnalyticDebug.Log("Batch sent successfully!");
				return true;
			}
			CPMAnalyticDebug.Log($"Batch failed: {request.responseCode} | {request.error}", LogType.Warning);
			return false;
		}

		public static string ToCurl(UnityWebRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("curl -X " + request.method + " \"" + request.url + "\" \\\n");
			foreach (KeyValuePair<string, string> responseHeader in request.GetResponseHeaders())
			{
				stringBuilder.Append("    -H \"" + responseHeader.Key + ": " + responseHeader.Value + "\" \\\n");
			}
			if (request.uploadHandler != null && request.uploadHandler.data != null && request.uploadHandler.data.Length != 0)
			{
				string text = Encoding.UTF8.GetString(request.uploadHandler.data);
				stringBuilder.Append("    -d '" + text.Replace("'", "\\'") + "'");
			}
			else if (stringBuilder[stringBuilder.Length - 2] == '\\')
			{
				stringBuilder.Length -= 2;
			}
			return stringBuilder.ToString();
		}
	}
}

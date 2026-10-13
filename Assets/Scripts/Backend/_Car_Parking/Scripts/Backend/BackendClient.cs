using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Backend
{
	public class BackendClient
	{
		public struct Config
		{
			public string HttpBaseUrl { get; set; }

			public int DefaultHttpTimeoutInSeconds { get; set; }
		}

		private readonly IHttpClient _httpClient;

		private readonly Config _config;

		[Inject]
		public BackendClient(IHttpClient httpClient, Config config)
		{
			_httpClient = httpClient;
			_config = config;
		}

		public async UniTask<Result<HttpResponse, Exception>> HttpAsync(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			string endPoint = request.EndPoint;
			string httpBaseUrl = _config.HttpBaseUrl;
			string url = httpBaseUrl + endPoint + BuildQueryString(endPoint, request.QueryParameters);
			Dictionary<string, string> dictionary = ((request.Headers == null) ? new Dictionary<string, string>() : new Dictionary<string, string>(request.Headers));
			dictionary["X-Client-Platform"] = GetPlatform();
			dictionary["X-Client-Version"] = Application.version;
			dictionary["X-Client-DeviceId"] = SystemInfo.deviceUniqueIdentifier;
			dictionary["Content-Type"] = GetHttpContentType(request.BodyType);
			string value = ((!PhotonNetwork.InRoom) ? null : PhotonNetwork.CurrentRoom?.Name);
			if (!string.IsNullOrEmpty(value))
			{
				dictionary["X-Photon-Room"] = value;
			}
			int timeoutInSeconds = ((request.TimeoutInSeconds == -95381381) ? _config.DefaultHttpTimeoutInSeconds : request.TimeoutInSeconds);
			try
			{
				InternalHttpRequest request2 = new InternalHttpRequest
				{
					Url = url,
					Method = request.Method,
					Body = request.Body,
					Headers = dictionary,
					TimeoutInSeconds = timeoutInSeconds
				};
				return Result<HttpResponse, Exception>.Success(await _httpClient.SendAsync(request2, cancellationToken));
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw;
			}
			catch (Exception ex2)
			{
				string arg = ((ex2 is UnityWebRequestException ex3) ? $"status: {ex3.ResponseCode}. body: {ex3.Text}" : ex2.ToString());
				if (!(ex2 is UnityWebRequestException ex4) || !request.IsExpectedStatus(ex4.ResponseCode))
				{
					UnityEngine.Debug.LogError($"[BackendClient] {request.Method} {url} failed: {arg}");
				}
				return Result<HttpResponse, Exception>.Fail(ex2);
			}
		}

		private static bool IsAbsoluteUrl(string endPoint)
		{
			if (endPoint != null)
			{
				if (!endPoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
				{
					return endPoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
				}
				return true;
			}
			return false;
		}

		private static string BuildQueryString(string endpoint, Dictionary<string, string> query)
		{
			if (query == null || query.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			char value = ((endpoint != null && endpoint.IndexOf('?') >= 0) ? '&' : '?');
			foreach (KeyValuePair<string, string> item in query)
			{
				if (!string.IsNullOrEmpty(item.Value))
				{
					stringBuilder.Append(value);
					stringBuilder.Append(Uri.EscapeDataString(item.Key));
					stringBuilder.Append('=');
					stringBuilder.Append(Uri.EscapeDataString(item.Value));
					value = '&';
				}
			}
			return stringBuilder.ToString();
		}

		private string GetPlatform()
		{
			return "STEAM";
		}

		private string GetHttpContentType(HttpBodyType bodyType)
		{
			return bodyType switch
			{
				HttpBodyType.Json => "application/json; charset=utf-8", 
				HttpBodyType.None => string.Empty, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		private static string FormatResponseBody(HttpResponse response)
		{
			if (response?.Body == null || response.Body.Length == 0)
			{
				return string.Empty;
			}
			if (!IsTextResponse(response))
			{
				return $"<{response.Body.Length} bytes>";
			}
			return Encoding.UTF8.GetString(response.Body);
		}

		private static bool IsTextResponse(HttpResponse response)
		{
			if (response.Headers == null)
			{
				return false;
			}
			foreach (KeyValuePair<string, string> header in response.Headers)
			{
				if (string.Equals(header.Key, "Content-Type", StringComparison.OrdinalIgnoreCase))
				{
					string text = header.Value ?? string.Empty;
					return text.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0 || text.StartsWith("text/", StringComparison.OrdinalIgnoreCase);
				}
			}
			return false;
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		private void Log(string message)
		{
			UnityEngine.Debug.Log("[BackendClient] " + message);
		}
	}
}

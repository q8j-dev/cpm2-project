using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Backend
{
	[Preserve]
	public class UnityHttpClient : IHttpClient
	{
		public async UniTask<HttpResponse> SendAsync(InternalHttpRequest request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			using UnityWebRequest webRequest = new UnityWebRequest(request.Url, GetStrMethod(request.Method));
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			webRequest.timeout = request.TimeoutInSeconds;
			byte[] body = request.Body;
			if (body != null && body.Length > 0)
			{
				webRequest.uploadHandler = new UploadHandlerRaw(request.Body);
			}
			foreach (KeyValuePair<string, string> header in request.Headers)
			{
				webRequest.SetRequestHeader(header.Key, header.Value);
			}
			UnityWebRequest unityWebRequest = await webRequest.SendWebRequest().WithCancellation(cancellationToken);
			return new HttpResponse
			{
				StatusCode = unityWebRequest.responseCode,
				Body = unityWebRequest.downloadHandler.data,
				Headers = unityWebRequest.GetResponseHeaders()
			};
		}

		private string GetStrMethod(HttpVerb httpVerb)
		{
			return httpVerb switch
			{
				HttpVerb.Get => "GET", 
				HttpVerb.Post => "POST", 
				HttpVerb.Head => "HEAD", 
				HttpVerb.Put => "PUT", 
				HttpVerb.Create => "CREATE", 
				HttpVerb.Delete => "DELETE", 
				_ => throw new ArgumentOutOfRangeException("httpVerb", httpVerb, null), 
			};
		}
	}
}

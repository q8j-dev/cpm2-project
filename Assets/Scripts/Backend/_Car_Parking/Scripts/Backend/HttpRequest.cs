using System.Collections.Generic;

namespace _Car_Parking.Scripts.Backend
{
	public class HttpRequest
	{
		public const int TIMEOUT_NOT_SET = -95381381;

		private long[] _expectedStatusCodes;

		public string EndPoint { get; set; }

		public HttpVerb Method { get; set; }

		public Dictionary<string, string> Headers { get; set; }

		public Dictionary<string, string> QueryParameters { get; set; }

		public byte[] Body { get; set; }

		public HttpBodyType BodyType { get; set; }

		public int TimeoutInSeconds { get; set; }

		public static HttpRequest Request(string endPoint, HttpVerb method = HttpVerb.Post)
		{
			return CreateHttpRequest(endPoint, null, HttpBodyType.None, method, -95381381);
		}

		public static HttpRequest Request(string endPoint, byte[] body, HttpBodyType bodyType, HttpVerb method = HttpVerb.Post, int timeoutInSeconds = -95381381)
		{
			return CreateHttpRequest(endPoint, body, bodyType, method, timeoutInSeconds);
		}

		public HttpRequest SetHeaders(Dictionary<string, string> headers)
		{
			Headers = headers;
			return this;
		}

		public HttpRequest Expect(params long[] statusCodes)
		{
			_expectedStatusCodes = statusCodes;
			return this;
		}

		public bool IsExpectedStatus(long statusCode)
		{
			if (_expectedStatusCodes == null)
			{
				return false;
			}
			for (int i = 0; i < _expectedStatusCodes.Length; i++)
			{
				if (_expectedStatusCodes[i] == statusCode)
				{
					return true;
				}
			}
			return false;
		}

		private static HttpRequest CreateHttpRequest(string endPoint, byte[] body, HttpBodyType bodyType, HttpVerb method, int timeoutInSeconds)
		{
			return new HttpRequest
			{
				EndPoint = endPoint,
				Method = method,
				Body = body,
				BodyType = bodyType,
				TimeoutInSeconds = timeoutInSeconds
			};
		}
	}
}

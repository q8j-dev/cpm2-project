using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Backend
{
	public interface IHttpClient
	{
		UniTask<HttpResponse> SendAsync(InternalHttpRequest request, CancellationToken cancellationToken);
	}
}

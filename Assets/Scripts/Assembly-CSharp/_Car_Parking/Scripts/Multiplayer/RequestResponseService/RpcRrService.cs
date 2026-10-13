using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	public class RpcRrService : MonoBehaviourPunCallbacks
	{
		private const string ExecutionFailedPayload = "\u0001RPCRR_EXEC_FAIL";

		private readonly Dictionary<int, UniTaskCompletionSource<object>> _pending = new Dictionary<int, UniTaskCompletionSource<object>>();

		private readonly Dictionary<RpcRrRequestObjectBase.CodeType, RpcRrRequestObjectBase> _rpcRrRequestObjects = new Dictionary<RpcRrRequestObjectBase.CodeType, RpcRrRequestObjectBase>();

		private int _requestIdCurrent;

		public bool HasActiveRequests => _pending.Count > 0;

		[Inject]
		public void Construct(IEnumerable<RpcRrRequestObjectBase> rpcObjectBase)
		{
			foreach (RpcRrRequestObjectBase item in rpcObjectBase)
			{
				_rpcRrRequestObjects.Add(item.Code, item);
			}
		}

		public async UniTask<T> Send<T>(Player target, RpcRrRequestObjectBase.CodeType codeType, string request, float timeoutSec = 30f, CancellationToken cancellationToken = default(CancellationToken)) where T : RpcRrResponseObjectBase, new()
		{
			_requestIdCurrent++;
			int requestId = _requestIdCurrent;
			UniTaskCompletionSource<object> uniTaskCompletionSource = new UniTaskCompletionSource<object>();
			_pending[requestId] = uniTaskCompletionSource;
			base.photonView.RPC("RPC_Request", target, requestId, (byte)codeType, request);
			try
			{
				return (T)(await uniTaskCompletionSource.Task.AttachExternalCancellation(cancellationToken).Timeout(TimeSpan.FromSeconds(timeoutSec)));
			}
			catch (Exception ex)
			{
				_pending.Remove(requestId);
				RpcRrResponseObjectBase.ErrorType error = RpcRrResponseObjectBase.ErrorType.Unknown;
				if (PhotonNetwork.CurrentRoom?.GetPlayer(target.ActorNumber) == null)
				{
					error = RpcRrResponseObjectBase.ErrorType.PlayerLeft;
				}
				else if (ex is TimeoutException)
				{
					error = RpcRrResponseObjectBase.ErrorType.Timeout;
				}
				return new T
				{
					Code = RpcRrResponseObjectBase.CodeType.Fail,
					Error = error
				};
			}
		}

		[PunRPC]
		private void RPC_Request(int requestId, byte codeType, string data, PhotonMessageInfo info)
		{
			RPCRequestAsync(requestId, codeType, data, info).Forget();
		}

		private async UniTask RPCRequestAsync(int requestId, byte codeType, string data, PhotonMessageInfo info)
		{
			RpcRrRequestObjectBase rpcRequestObject = GetRpcRequestObject((RpcRrRequestObjectBase.CodeType)codeType);
			if (rpcRequestObject != null)
			{
				string text;
				try
				{
					text = await rpcRequestObject.ExecuteRequestAsync(info.Sender, data);
				}
				catch (Exception)
				{
					text = "\u0001RPCRR_EXEC_FAIL";
				}
				base.photonView.RPC("RPC_Response", info.Sender, requestId, codeType, text);
			}
		}

		[PunRPC]
		private void RPC_Response(int requestId, byte codeType, string result)
		{
			if (!_pending.Remove(requestId, out var value))
			{
				return;
			}
			if (result == "\u0001RPCRR_EXEC_FAIL")
			{
				value.TrySetException(new Exception("Remote request handler failed"));
				return;
			}
			RpcRrRequestObjectBase rpcRequestObject = GetRpcRequestObject((RpcRrRequestObjectBase.CodeType)codeType);
			if (rpcRequestObject != null)
			{
				value.TrySetResult(rpcRequestObject.CreateResponse(result));
			}
		}

		private void CancelPending()
		{
			if (_pending.Count == 0)
			{
				return;
			}
			foreach (UniTaskCompletionSource<object> value in _pending.Values)
			{
				value.TrySetException(new Exception("RpcRr request cancelled: left room or service destroyed"));
			}
			_pending.Clear();
		}

		public override void OnLeftRoom()
		{
			CancelPending();
		}

		private void OnDestroy()
		{
			CancelPending();
		}

		private RpcRrRequestObjectBase GetRpcRequestObject(RpcRrRequestObjectBase.CodeType code)
		{
			if (_rpcRrRequestObjects.TryGetValue(code, out var value))
			{
				return value;
			}
			return null;
		}
	}
}

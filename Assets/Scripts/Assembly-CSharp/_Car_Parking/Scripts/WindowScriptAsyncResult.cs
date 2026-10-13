using System.Threading;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts
{
	public abstract class WindowScriptAsyncResult<TPayload> : WindowScript
	{
		public enum CodeType
		{
			Error = 0,
			Success = 1,
			Canceled = 2
		}

		public struct ResultData
		{
			public CodeType CodeType;

			public TPayload Payload;

			public ResultData(CodeType codeType, TPayload payload)
			{
				CodeType = codeType;
				Payload = payload;
			}
		}

		private UniTaskCompletionSource<ResultData> _completionSource;

		private CancellationTokenRegistration _getResultCtr;

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				base.Close();
				TrySetResult(new ResultData(CodeType.Canceled, default));
			}
		}

		public UniTask<ResultData> GetResultAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_completionSource != null)
			{
				return _completionSource.Task;
			}
			_completionSource = new UniTaskCompletionSource<ResultData>();
			if (cancellationToken.CanBeCanceled)
			{
				_getResultCtr = cancellationToken.Register(() =>
				{
					TrySetResult(new ResultData(CodeType.Error, default));
					Close();
				});
			}
			return _completionSource.Task;
		}

		protected void TrySetResult(CodeType codeType)
		{
			TrySetResult(new ResultData(codeType, default));
		}

		protected void TrySetResult(ResultData result)
		{
			if (_completionSource != null)
			{
				_completionSource.TrySetResult(result);
				_completionSource = null;
				_getResultCtr.Dispose();
			}
		}

		protected override void OnDestroy()
		{
			TrySetResult(new ResultData(CodeType.Error, default));
			base.OnDestroy();
		}
	}
}

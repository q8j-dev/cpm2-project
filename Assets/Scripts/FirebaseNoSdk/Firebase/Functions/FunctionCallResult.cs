namespace Firebase.Functions
{
	public readonly struct FunctionCallResult<T>
	{
		public FunctionStatus Status { get; }

		public T Data { get; }

		public string ErrorMessage { get; }

		public bool IsSuccess => Status == FunctionStatus.Success;

		public bool IsFail => !IsSuccess;

		public bool IsServerFail
		{
			get
			{
				if (Status <= FunctionStatus.Success)
				{
					return Status == FunctionStatus.UnknownError;
				}
				return true;
			}
		}

		public bool IsLocalFail => Status < FunctionStatus.UnknownError;

		private FunctionCallResult(FunctionStatus status, T data, string errorMessage)
		{
			Status = status;
			Data = data;
			ErrorMessage = errorMessage;
		}

		public static FunctionCallResult<T> Ok(T data)
		{
			return new FunctionCallResult<T>(FunctionStatus.Success, data, null);
		}

		public static FunctionCallResult<T> Fail(FunctionStatus status, string errorMessage = null)
		{
			return new FunctionCallResult<T>(status, default, errorMessage);
		}
	}
}

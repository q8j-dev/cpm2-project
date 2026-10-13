namespace _Car_Parking.Scripts.Backend
{
	public struct Result
	{
		public bool IsSuccess { get; }

		public bool IsFail => !IsSuccess;

		public Result(bool isSuccess)
		{
			IsSuccess = isSuccess;
		}

		public static Result Success()
		{
			return new Result(isSuccess: true);
		}

		public static Result Fail()
		{
			return new Result(isSuccess: false);
		}
	}
	public struct Result<T>
	{
		public bool IsSuccess { get; }

		public bool IsFail => !IsSuccess;

		public T Value { get; }

		public Result(T value)
		{
			IsSuccess = true;
			Value = value;
		}

		public static Result<T> Success(T result)
		{
			return new Result<T>(result);
		}

		public static Result<T> Fail()
		{
			return default;
		}
	}
	public struct Result<TResult, TError>
	{
		public bool IsSuccess { get; }

		public bool IsFail => !IsSuccess;

		public TResult Value { get; }

		public TError Error { get; }

		public Result(TResult value)
		{
			IsSuccess = true;
			Value = value;
			Error = default;
		}

		public Result(TError error)
		{
			IsSuccess = false;
			Error = error;
			Value = default;
		}

		public static Result<TResult, TError> Success(TResult result)
		{
			return new Result<TResult, TError>(result);
		}

		public static Result<TResult, TError> Fail(TError error)
		{
			return new Result<TResult, TError>(error);
		}
	}
}

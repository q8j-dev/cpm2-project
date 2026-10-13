using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Backend;

public class FirebaseWrappedFunction
{
	private readonly HttpsCallableReference _reference;

	public string FunctionName { get; }

	[Obsolete("FIX THAT IMMEDIATELY")]
	public bool WasTimeouted { get; }

	public static event Action<string, FunctionStatus> OnErrorCalled;

	public FirebaseWrappedFunction(string functionName, int? timeout)
	{
		_reference = FirebaseFunctions.DefaultInstance.GetHttpsCallable(functionName, timeout);
		FunctionName = functionName;
	}

	public async UniTask<FunctionCallResult<T>> CallAsyncResult<T>(object data = null, int networkAttempts = 1, Dictionary<string, string> extraHeaders = null)
	{
		FunctionCallResult<T> result = await _reference.CallAsyncResult<T>(data, networkAttempts, extraHeaders);
		if (result.IsFail)
		{
			ErrorCalled(result.Status);
		}
		return result;
	}

	public async UniTask<FunctionCallResult<T>> CallAsyncResult<T>(IDecoder decoder, object data = null, int networkAttempts = 1)
	{
		FunctionCallResult<string> functionCallResult = await _reference.CallAsyncResult<string>(data, networkAttempts);
		if (!functionCallResult.IsSuccess)
		{
			ErrorCalled(functionCallResult.Status);
			return FunctionCallResult<T>.Fail(functionCallResult.Status, functionCallResult.ErrorMessage);
		}
		try
		{
			Result<T, string> result = decoder.Decode<T>(functionCallResult.Data);
			if (result.IsSuccess)
			{
				if (result.Value != null)
				{
					try
					{
					}
					catch (Exception)
					{
					}
				}
				return FunctionCallResult<T>.Ok(result.Value);
			}
			ErrorCalled(FunctionStatus.DecodeError);
			return FunctionCallResult<T>.Fail(FunctionStatus.DecodeError, result.Error);
		}
		catch (Exception ex2)
		{
			ErrorCalled(FunctionStatus.DecodeError);
			return FunctionCallResult<T>.Fail(FunctionStatus.DecodeError, ex2.Message);
		}
	}

	private void ErrorCalled(FunctionStatus functionStatus)
	{
		OnErrorCalled?.Invoke(FunctionName, functionStatus);
	}
}

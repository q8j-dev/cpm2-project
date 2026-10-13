using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Wallet
{
	[Serializable]
	[Preserve]
	public class WalletOperationResult
	{
		public int Success;

		public int Value;

		public string Token;

		public string Data;

		public WalletOperationResult(int success, int value)
		{
			Success = success;
			Value = value;
		}

		public WalletOperationResult(int success, int value, string token)
		{
			Success = success;
			Value = value;
			Token = token;
		}

		public bool IsSuccess()
		{
			return Success == 1;
		}
	}
}

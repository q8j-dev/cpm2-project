using System;
using UnityEngine.Scripting;

namespace CPM.Account
{
	[Serializable]
	[Preserve]
	public class PasswordResetResult
	{
		public int result;

		public string message;
	}
}

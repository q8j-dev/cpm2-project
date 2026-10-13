using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace CPM.Balance
{
	public static class BalanceDocumentService
	{
		public static BalanceDocument Parse(byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0)
			{
				return new BalanceDocument();
			}
			try
			{
				return JsonConvert.DeserializeObject<BalanceDocument>(Encoding.UTF8.GetString(bytes), DocumentJsonSettings.Settings) ?? new BalanceDocument();
			}
			catch (Exception ex)
			{
				Debug.LogError("BalanceDocumentService.Parse: failed to parse balance document, using empty default. " + ex.Message);
				return new BalanceDocument();
			}
		}
	}
}

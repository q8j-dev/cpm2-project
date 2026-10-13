using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.BannerSystem.Rules
{
	public class DailyBannerRule : IBannerRule
	{
		private readonly string _saveKey;

		private string _lastDateStr;

		public DailyBannerRule(string saveKey)
		{
			_saveKey = saveKey;
		}

		public UniTask<bool> CanShow()
		{
			if (_lastDateStr.IsNullOrEmptyCollection())
			{
				_lastDateStr = PlayerPrefs.GetString(_saveKey, "");
			}
			if (DateTime.TryParseExact(_lastDateStr, "O", null, DateTimeStyles.RoundtripKind, out var result) && result.Date == DateTime.UtcNow.Date)
			{
				return new UniTask<bool>(result: false);
			}
			return new UniTask<bool>(result: true);
		}

		public void MarkShown()
		{
			PlayerPrefs.SetString(value: _lastDateStr = DateTime.UtcNow.ToString("O"), key: _saveKey);
			PlayerPrefs.Save();
		}
	}
}

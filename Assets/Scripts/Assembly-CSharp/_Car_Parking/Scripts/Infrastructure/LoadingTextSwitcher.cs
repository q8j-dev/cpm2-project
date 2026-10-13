using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Infrastructure
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LoadingTextSwitcher : MonoBehaviour
	{
		[SerializeField]
		private float Interval = 1.5f;

		[SerializeField]
		private string[] LoadingPhrasesKeys;

		private TextMeshProUGUI _text;

		private CancellationTokenSource _cts;

		private const int BlacklistSize = 2;

		private readonly Queue<int> _blacklist = new Queue<int>();

		private void Awake()
		{
			_text = GetComponent<TextMeshProUGUI>();
		}

		private void OnEnable()
		{
			if (LoadingPhrasesKeys != null && LoadingPhrasesKeys.Length != 0)
			{
				_blacklist.Clear();
				_cts = new CancellationTokenSource();
				SwitchTextLoop(_cts.Token).Forget();
			}
		}

		private void OnDisable()
		{
			_cts?.Cancel();
			_cts?.Dispose();
		}

		private async UniTaskVoid SwitchTextLoop(CancellationToken token)
		{
			System.Random rnd = new System.Random();
			while (!token.IsCancellationRequested)
			{
				int randomIndex = GetRandomIndex(rnd);
				_blacklist.Enqueue(randomIndex);
				if (_blacklist.Count > 2)
				{
					_blacklist.Dequeue();
				}
				_text.text = new LocalizedString(LoadingPhrasesKeys[randomIndex]);
				await UniTask.Delay(TimeSpan.FromSeconds(Interval), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
		}

		private int GetRandomIndex(System.Random rnd)
		{
			int count = LoadingPhrasesKeys.Length;
			List<int> list = (from i in Enumerable.Range(0, count)
				where !_blacklist.Contains(i)
				select i).ToList();
			if (list.Count == 0)
			{
				_blacklist.Clear();
				list = Enumerable.Range(0, count).ToList();
			}
			return list[rnd.Next(list.Count)];
		}
	}
}

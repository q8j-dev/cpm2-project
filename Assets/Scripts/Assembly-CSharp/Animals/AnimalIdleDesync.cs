using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animals
{
	[RequireComponent(typeof(Animator))]
	public class AnimalIdleDesync : MonoBehaviour
	{
		[SerializeField]
		private string[] CandidateStates = new string[12]
		{
			"IdleA", "IdleB", "LoopA", "LoopB", "LoopC", "GoToRest", "Resting", "RestGetUp", "RestBreak", "GoToSleep",
			"Sleeping", "SleepGetUp"
		};

		private void OnEnable()
		{
			Animator animator = GetComponent<Animator>();
			string text = PickState(CandidateStates, (string name) => animator.HasState(0, Animator.StringToHash(name)), (int max) => UnityEngine.Random.Range(0, max));
			if (text != null)
			{
				animator.Play(text, 0, UnityEngine.Random.value);
			}
		}

		public static string PickState(IReadOnlyList<string> candidates, Func<string, bool> hasState, Func<int, int> randomRange)
		{
			List<string> list = new List<string>();
			foreach (string candidate in candidates)
			{
				if (hasState(candidate))
				{
					list.Add(candidate);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			return list[randomRange(list.Count)];
		}
	}
}

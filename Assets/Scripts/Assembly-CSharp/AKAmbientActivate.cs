using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AKAmbientActivate : MonoBehaviour
{
	[SerializeField]
	private AkAmbient ambient;

	private void Awake()
	{
		MoveElements.GlobalPointMoved += OnGlobalPointMove;
	}

	private void OnEnable()
	{
		EnableComponent().Forget();
	}

	private void OnDestroy()
	{
		MoveElements.GlobalPointMoved -= OnGlobalPointMove;
	}

	private void OnGlobalPointMove()
	{
		EnableComponent().Forget();
	}

	private async UniTask EnableComponent()
	{
		if (!ambient)
		{
			return;
		}
		ambient.enabled = false;
		await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
		if ((bool)this && (bool)ambient)
		{
			ambient.enabled = true;
			if (ambient.multiPositionTypeLabel == MultiPositionTypeLabel.Large_Mode)
			{
				ApplyLargePosition(ambient);
			}
		}
	}

	private static void ApplyLargePosition(AkAmbient akAmbient)
	{
		if (akAmbient.LargeModePositions == null || akAmbient.LargeModePositions.Length == 0)
		{
			return;
		}
		AkGameObj[] components = akAmbient.gameObject.GetComponents<AkGameObj>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].enabled = false;
		}
		List<AkAmbientLargeModePositioner> list = new List<AkAmbientLargeModePositioner>();
		for (int j = 0; j < akAmbient.LargeModePositions.Length; j++)
		{
			if (akAmbient.LargeModePositions[j] != null && !list.Contains(akAmbient.LargeModePositions[j]))
			{
				list.Add(akAmbient.LargeModePositions[j]);
			}
		}
		AkPositionArray akPositionArray = new AkPositionArray((uint)list.Count);
		for (int k = 0; k < list.Count; k++)
		{
			akPositionArray.Add(list[k].Position, list[k].Forward, list[k].Up);
		}
		AkSoundEngine.SetMultiplePositions(akAmbient.gameObject, akPositionArray, (ushort)akPositionArray.Count, akAmbient.MultiPositionType);
	}
}

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GateController : MonoBehaviour
{
	public Transform Gate;

	public bool openNow;

	public float duration;

	[SerializeField]
	public Vector3 _openGate = new Vector3(0f, 0f, 90f);

	private void OnTriggerEnter(Collider other)
	{
		if (!openNow)
		{
			Gate.DOLocalRotate(_openGate, 3f, RotateMode.LocalAxisAdd);
			openNow = true;
			Close().Forget();
		}
	}

	public async UniTaskVoid Close()
	{
		if (openNow)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(6000.0));
			Gate.DOLocalRotate(-_openGate, 3f, RotateMode.LocalAxisAdd);
			await UniTask.Delay(TimeSpan.FromMilliseconds(3000.0));
			openNow = false;
		}
	}
}

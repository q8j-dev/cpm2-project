using System;
using DG.Tweening;

[Serializable]
public class CarLightValue
{
	public float LightValue;

	public int LightProperty;

	public float Duration = 0.74f;

	public float Delay;

	public bool Repeat;

	public bool isOn;

	public Tweener mainTweener;
}

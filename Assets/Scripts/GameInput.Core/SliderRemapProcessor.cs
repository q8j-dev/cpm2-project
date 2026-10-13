using UnityEngine;
using UnityEngine.InputSystem;

public class SliderRemapProcessor : InputProcessor<float>
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		InputSystem.RegisterProcessor<SliderRemapProcessor>("SliderRemap");
	}

	public override float Process(float value, InputControl control)
	{
		return Mathf.Clamp01((value + 1f) / 2f);
	}
}

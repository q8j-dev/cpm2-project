using UnityEngine;
using UnityEngine.InputSystem;

public class SliderPressInteraction : IInputInteraction<float>, IInputInteraction
{
	[Tooltip("Value in remapped [0..1] space at which the action performs. 0 = use defaultButtonPressPoint.")]
	public float pressPoint;

	[Tooltip("Full-release tolerance in remapped [0..1] space; action cancels at or below this value.")]
	public float releasePoint = 0.01f;

	[Tooltip("Seconds the control must sit at its default state (raw 0) before it counts as fully released. Distinguishes the at-rest snap from a mid-travel pass through center.")]
	public float restTime = 0.1f;

	private const float StartHysteresis = 0.02f;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		InputSystem.RegisterInteraction<SliderPressInteraction>("SliderPress");
	}

	public void Process(ref InputInteractionContext context)
	{
		if (context.timerHasExpired)
		{
			if (context.control.CheckStateIsAtDefault())
			{
				context.Canceled();
			}
			return;
		}
		float num = context.ReadValue<float>();
		float num2 = ((pressPoint > 0f) ? pressPoint : InputSystem.settings.defaultButtonPressPoint);
		switch (context.phase)
		{
		case InputActionPhase.Waiting:
			if (num > releasePoint + 0.02f)
			{
				context.Started();
				if (num >= num2)
				{
					context.PerformedAndStayPerformed();
				}
			}
			break;
		case InputActionPhase.Started:
			if (num >= num2)
			{
				context.PerformedAndStayPerformed();
			}
			else if (num <= releasePoint)
			{
				context.Canceled();
			}
			break;
		case InputActionPhase.Performed:
			if (num <= releasePoint)
			{
				context.Canceled();
			}
			break;
		}
		if ((context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed) && context.control.CheckStateIsAtDefault())
		{
			context.SetTimeout(restTime);
		}
	}

	public void Reset()
	{
	}
}

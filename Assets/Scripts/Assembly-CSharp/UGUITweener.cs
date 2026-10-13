using UnityEngine;
using UnityEngine.Events;

public class UGUITweener : MonoBehaviour
{
	public PlayStyle playStyle;

	public AnimationCurve animationCurve;

	public float duration;

	public float startDelay;

	public UnityEvent onFinish;
}

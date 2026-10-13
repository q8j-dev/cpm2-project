using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
	public Animator animator;

	public string onTriggerEnterParameterName;

	public string onTriggerExitParameterName;

	public AudioClip OpenSound;

	public AudioClip CloseSound;

	private void Start()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
			_ = animator == null;
		}
	}

	private void OnTriggerEnter()
	{
		if (onTriggerEnterParameterName != null)
		{
			base.gameObject.GetComponent<AudioSource>().PlayOneShot(OpenSound);
			animator.SetTrigger(onTriggerEnterParameterName);
		}
	}

	private void OnTriggerExit()
	{
		if (onTriggerExitParameterName != null)
		{
			base.gameObject.GetComponent<AudioSource>().PlayOneShot(CloseSound);
			animator.SetTrigger(onTriggerExitParameterName);
		}
	}
}

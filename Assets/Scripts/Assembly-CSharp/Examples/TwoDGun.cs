using TouchControlsKit;
using UnityEngine;

namespace Examples
{
	public class TwoDGun : MonoBehaviour
	{
		public Rigidbody2D rocket;

		public float speed = 20f;

		private TwoDPlayerControl playerCtrl;

		private Animator anim;

		private void Awake()
		{
			anim = base.transform.root.gameObject.GetComponent<Animator>();
			playerCtrl = base.transform.root.GetComponent<TwoDPlayerControl>();
		}

		private void Update()
		{
			if (TCKInput.GetButtonDown("shootButton"))
			{
				anim.SetTrigger("Shoot");
				if (playerCtrl.facingRight)
				{
					Object.Instantiate(rocket, base.transform.position, Quaternion.Euler(Vector3.zero)).velocity = new Vector2(speed, 0f);
				}
				else
				{
					Object.Instantiate(rocket, base.transform.position, Quaternion.Euler(new Vector3(0f, 0f, 180f))).velocity = new Vector2(0f - speed, 0f);
				}
			}
		}
	}
}

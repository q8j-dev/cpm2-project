using TouchControlsKit;
using UnityEngine;

namespace Examples
{
	public class TwoDPlayerControl : MonoBehaviour
	{
		public bool facingRight = true;

		public bool jump;

		public float moveForce = 365f;

		public float maxSpeed = 5f;

		public float jumpForce = 1000f;

		private Transform groundCheck;

		private bool grounded;

		private Animator anim;

		private void Awake()
		{
			groundCheck = base.transform.Find("groundCheck");
			anim = GetComponent<Animator>();
		}

		private void Update()
		{
			grounded = Physics2D.Linecast(base.transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Default"));
			if (TCKInput.GetButtonDown("jumpButton") && grounded)
			{
				jump = true;
			}
		}

		private void FixedUpdate()
		{
			float axis = TCKInput.GetAxis("DPad", "Horizontal");
			axis = Mathf.Clamp(axis, -1f, 1f);
			anim.SetFloat("Speed", Mathf.Abs(axis));
			if (axis * GetComponent<Rigidbody2D>().velocity.x < maxSpeed)
			{
				GetComponent<Rigidbody2D>().AddForce(Vector2.right * axis * moveForce);
			}
			if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > maxSpeed)
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
			}
			if (axis > 0f && !facingRight)
			{
				Flip();
			}
			else if (axis < 0f && facingRight)
			{
				Flip();
			}
			if (jump)
			{
				anim.SetTrigger("Jump");
				GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce * 1.5f));
				jump = false;
			}
		}

		private void Flip()
		{
			facingRight = !facingRight;
			Vector3 localScale = base.transform.localScale;
			localScale.x *= -1f;
			base.transform.localScale = localScale;
		}
	}
}

using UnityEngine;

namespace Examples
{
	public class TwoDCameraFollow : MonoBehaviour
	{
		public float xMargin = 1f;

		public float yMargin = 1f;

		public float xSmooth = 8f;

		public float ySmooth = 8f;

		public Vector2 maxXAndY;

		public Vector2 minXAndY;

		private Transform player;

		private void Awake()
		{
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}

		private bool CheckXMargin()
		{
			return Mathf.Abs(base.transform.position.x - player.position.x) > xMargin;
		}

		private bool CheckYMargin()
		{
			return Mathf.Abs(base.transform.position.y - player.position.y) > yMargin;
		}

		private void FixedUpdate()
		{
			TrackPlayer();
		}

		private void TrackPlayer()
		{
			float value = base.transform.position.x;
			float value2 = base.transform.position.y;
			if (CheckXMargin())
			{
				value = Mathf.Lerp(base.transform.position.x, player.position.x, xSmooth * Time.deltaTime);
			}
			if (CheckYMargin())
			{
				value2 = Mathf.Lerp(base.transform.position.y, player.position.y, ySmooth * Time.deltaTime);
			}
			value = Mathf.Clamp(value, minXAndY.x, maxXAndY.x);
			value2 = Mathf.Clamp(value2, minXAndY.y, maxXAndY.y);
			base.transform.position = new Vector3(value, value2, base.transform.position.z);
		}
	}
}

using UnityEngine;

public class BlobShadowController : MonoBehaviour
{
	private Transform mTranform;

	private void Start()
	{
		mTranform = base.transform;
	}

	private void Update()
	{
		mTranform.position = mTranform.parent.position + Vector3.up * 8.246965f;
		mTranform.rotation = Quaternion.LookRotation(-Vector3.up, mTranform.parent.forward);
	}
}

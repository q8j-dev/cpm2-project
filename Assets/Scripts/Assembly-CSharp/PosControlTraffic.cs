using Photon.Pun;
using UnityEngine;

public class PosControlTraffic : MonoBehaviourPun, IPunObservable
{
	private Vector3 correctPlayerPos;

	private Quaternion correctPlayerRot;

	public int indexBody = 1;

	public int indexBodyToSet = 3;

	public float koff = 3f;

	public float koff2 = 3f;

	private void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	private void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(indexBody);
		}
		else
		{
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			indexBodyToSet = (int)stream.ReceiveNext();
		}
	}
}

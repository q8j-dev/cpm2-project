using Photon.Pun;
using UnityEngine;

public class TaxiObservePositions : MonoBehaviour, IPunObservable
{
	public TaxiClientController _Client;

	private void Start()
	{
		if ((bool)GetComponent<TaxiClientController>())
		{
			_Client = GetComponent<TaxiClientController>();
		}
	}

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
			stream.SendNext(base.transform.localPosition);
		}
		else
		{
			base.transform.localPosition = (Vector3)stream.ReceiveNext();
		}
	}
}

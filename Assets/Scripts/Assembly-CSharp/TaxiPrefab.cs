using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.Localization;

public class TaxiPrefab : MonoBehaviourPun
{
	public GameObject parent;

	public int reward;

	private void Start()
	{
		if (parent == null && (bool)GameObject.Find("TaxiController"))
		{
			parent = GameObject.Find("TaxiController");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((other.CompareTag("Car") || other.CompareTag("OnlineCar")) && V.Car != null && V.Car == other.transform.parent.transform.gameObject)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("TaxiPrefab.PassengerPickedUp"), 5f);
			RemoveTaxiPrefab();
		}
	}

	public void RemoveTaxiPrefab()
	{
		if (base.photonView.IsMine)
		{
			base.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
			PhotonNetwork.Destroy(base.gameObject);
		}
	}
}

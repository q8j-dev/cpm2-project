using Photon.Pun;
using UnityEngine;

public class OpenMainMenu : MonoBehaviour
{
	private void Start()
	{
	}

	public void OpenMain()
	{
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.Disconnect();
		}
		Application.LoadLevel(0);
	}
}

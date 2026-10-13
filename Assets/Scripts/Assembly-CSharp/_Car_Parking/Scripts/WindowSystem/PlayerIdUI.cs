using CPM.PlayerData;
using TMPro;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PlayerIdUI : MonoBehaviour
	{
		private TMP_Text _playerIdText;

		private PlayerDataStorage _playerDataStorage;

		[Inject]
		public void Construct(PlayerDataStorage playerDataStorage)
		{
			_playerDataStorage = playerDataStorage;
		}

		private void Start()
		{
			_playerIdText = GetComponent<TMP_Text>();
			_playerIdText.text = $"Player Id:{_playerDataStorage.PlayerID}";
		}
	}
}

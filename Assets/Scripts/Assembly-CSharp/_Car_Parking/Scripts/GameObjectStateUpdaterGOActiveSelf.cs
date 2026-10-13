using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class GameObjectStateUpdaterGOActiveSelf : GameObjectStateUpdater
	{
		[SerializeField]
		private GameObject GO;

		[SerializeField]
		private bool Inverse;

		private GameObject GameObject
		{
			get
			{
				if (!GO)
				{
					return base.gameObject;
				}
				return GO;
			}
		}

		public override bool IsOn
		{
			get
			{
				if (!Inverse)
				{
					return GameObject.activeSelf;
				}
				return !GameObject.activeSelf;
			}
		}

		public override void UpdateState(bool isOn)
		{
			if (Inverse)
			{
				isOn = !isOn;
			}
			GameObject.SetActive(isOn);
		}
	}
}

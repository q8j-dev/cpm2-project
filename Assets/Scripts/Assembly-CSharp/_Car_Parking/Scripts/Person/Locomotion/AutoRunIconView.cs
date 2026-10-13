using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class AutoRunIconView : MonoBehaviour
	{
		[SerializeField]
		private GameObject _icon = default;

		private GameObject _person;

		private PersonLocomotion _locomotion;

		private bool _shown;

		private void Awake()
		{
			if ((bool)_icon)
			{
				_icon.SetActive(value: false);
			}
		}

		private void Update()
		{
			GameObject driverPerson = V.DriverPerson;
			if (driverPerson != _person)
			{
				_person = driverPerson;
				_locomotion = (driverPerson ? driverPerson.GetComponent<PersonLocomotion>() : null);
			}
			bool flag = (bool)_locomotion && _locomotion.AutoRun;
			if (flag != _shown)
			{
				_shown = flag;
				if ((bool)_icon)
				{
					_icon.SetActive(flag);
				}
			}
		}
	}
}

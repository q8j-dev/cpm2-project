using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI
{
	public class ArrowsController : MonoBehaviour
	{
		[SerializeField]
		private PointerEnterExitHandler LeftArrow;

		[SerializeField]
		private PointerEnterExitHandler RightArrow;

		private CarInputManager _carInputManger;

		[Inject]
		public void Construct(CarInputManager carInputManager)
		{
			_carInputManger = carInputManager;
		}

		private void Start()
		{
			LeftArrow.PointerEnter += () =>
			{
				_carInputManger.OnLeftSteerPressed();
			};
			LeftArrow.PointerExit += () =>
			{
				_carInputManger.OnLeftRightSteerReleased();
			};
			RightArrow.PointerEnter += () =>
			{
				_carInputManger.OnRightSteerPressed();
			};
			RightArrow.PointerExit += () =>
			{
				_carInputManger.OnLeftRightSteerReleased();
			};
		}
	}
}

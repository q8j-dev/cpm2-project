using System;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.BuyNewCar
{
	public class BuyCarUIBodyKitIndexUpdateButtons : MonoBehaviour
	{
		public const string PathToPrefab = "Assets/_Car Parking/Art/UI/Prefabs/UI/BodyKitsUIButtons.prefab";

		[SerializeField]
		private Button ButtonNext;

		[SerializeField]
		private Button ButtonPrev;

		[SerializeField]
		private ButtonPhysics ButtonPhysicsNext;

		[SerializeField]
		private ButtonPhysics ButtonPhysicsPrev;

		private BodyKitType _bodyKitType;

		private int _index;

		public event Action<BodyKitType, int> OnClicked;

		public void Init(BodyKitType bodyKitType, int index)
		{
			_bodyKitType = bodyKitType;
			_index = index;
			ButtonNext.gameObject.name = $"Button_{bodyKitType}_Next";
			ButtonPrev.gameObject.name = $"Button_{bodyKitType}_Prev";
			ButtonPhysicsNext.gameObject.name = $"ButtonPhysics_{bodyKitType}_Next";
			ButtonPhysicsPrev.gameObject.name = $"ButtonPhysics_{bodyKitType}_Prev";
		}

		private void OnEnable()
		{
			ButtonNext.onClick.AddListener(NextClickCallback);
			ButtonPrev.onClick.AddListener(PrevClickCallback);
			ButtonPhysicsNext.Clicked += ButtonPhysicsNextClickCallback;
			ButtonPhysicsPrev.Clicked += ButtonPhysicsPrevClickCallback;
		}

		private void OnDisable()
		{
			ButtonNext.onClick.RemoveListener(NextClickCallback);
			ButtonPrev.onClick.RemoveListener(PrevClickCallback);
			ButtonPhysicsNext.Clicked -= ButtonPhysicsNextClickCallback;
			ButtonPhysicsPrev.Clicked -= ButtonPhysicsPrevClickCallback;
		}

		private void ButtonPhysicsNextClickCallback(ButtonPhysics buttonPhysics)
		{
			ButtonNext.onClick.Invoke();
		}

		private void ButtonPhysicsPrevClickCallback(ButtonPhysics buttonPhysics)
		{
			ButtonPrev.onClick.Invoke();
		}

		private void NextClickCallback()
		{
			OnClicked?.Invoke(_bodyKitType, 1);
		}

		private void PrevClickCallback()
		{
			OnClicked?.Invoke(_bodyKitType, -1);
		}
	}
}

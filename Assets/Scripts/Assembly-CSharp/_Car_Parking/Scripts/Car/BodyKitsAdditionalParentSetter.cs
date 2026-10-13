using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Car
{
	public class BodyKitsAdditionalParentSetter : MonoBehaviour
	{
		private enum TypeDetail
		{
			FrontLeftDoor = 0,
			FrontRightDoor = 1,
			BackLeftDoor = 2,
			BackRightDoor = 3,
			TrunkRight = 4,
			TrunkLeft = 5,
			Trunk = 6
		}

		[SerializeField]
		private TypeDetail Type;

		private Transform _originalParent;

		private bodyKitValues _parentBodyKitValues;

		private void Start()
		{
			_originalParent = base.transform.parent;
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI)
			{
				V.CoreGameManager.ManagerUI.OnCurrentWindowUpdated += CurrentWindowUpdatedCallback;
			}
			bodyKitValues componentInParent = base.gameObject.GetComponentInParent<bodyKitValues>(includeInactive: true);
			if ((bool)componentInParent)
			{
				_parentBodyKitValues = componentInParent;
				_parentBodyKitValues.OnBodyKitDestroed += DestroyThis;
			}
			UpdateParent();
		}

		private void OnDestroy()
		{
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI)
			{
				V.CoreGameManager.ManagerUI.OnCurrentWindowUpdated -= CurrentWindowUpdatedCallback;
			}
			if ((bool)_parentBodyKitValues)
			{
				_parentBodyKitValues.OnBodyKitDestroed -= DestroyThis;
			}
		}

		private void DestroyThis()
		{
			Object.Destroy(base.gameObject);
		}

		private void CurrentWindowUpdatedCallback(WindowScript windowScript)
		{
			if (windowScript.GUID == WindowScriptType.VisualTuningWindow.ToString() && (bool)_originalParent)
			{
				base.transform.SetParent(_originalParent);
				UpdateParent();
			}
		}

		private void UpdateParent()
		{
			VehicleData componentInParent = GetComponentInParent<VehicleData>();
			if ((bool)componentInParent && (bool)componentInParent.CarBodyReferences)
			{
				Transform transform = Type switch
				{
					TypeDetail.FrontLeftDoor => componentInParent.CarBodyReferences.FrontLeftDoor, 
					TypeDetail.FrontRightDoor => componentInParent.CarBodyReferences.FrontRightDoor, 
					TypeDetail.BackLeftDoor => componentInParent.CarBodyReferences.BackLeftDoor, 
					TypeDetail.BackRightDoor => componentInParent.CarBodyReferences.BackRightDoor, 
					TypeDetail.TrunkRight => componentInParent.CarBodyReferences.FirstTrunkDoor, 
					TypeDetail.TrunkLeft => componentInParent.CarBodyReferences.SecondTrunkDoor, 
					TypeDetail.Trunk => componentInParent.CarBodyReferences.Trunk, 
					_ => null, 
				};
				if ((bool)transform)
				{
					base.transform.SetParent(transform);
					componentInParent.CarAnimationReferences?.UpdateWindows();
					componentInParent.CarAnimationsController?.UpdateOpenDoors();
				}
			}
		}
	}
}

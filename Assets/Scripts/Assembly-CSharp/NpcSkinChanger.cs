using System;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Characters;

[DefaultExecutionOrder(10)]
public class NpcSkinChanger : MonoBehaviour
{
	[Serializable]
	public struct EquipmentSlot
	{
		public int id;
	}

	public EquipmentSlot hair = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot face = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot beard = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot cap = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot mask = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot top = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot gloves = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot bag = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot pants = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot shoes = new EquipmentSlot
	{
		id = -1
	};

	public EquipmentSlot glasses = new EquipmentSlot
	{
		id = -1
	};

	private CharactersService _charactersService;

	private MainPerson _mainPerson;

	[Inject]
	public void Construct(CharactersService charactersService)
	{
		_charactersService = charactersService;
	}

	public void Init(MainPerson mainPerson)
	{
		_mainPerson = mainPerson;
		ClearKits();
		ApplyEquipment();
	}

	private void ClearKits()
	{
		for (int i = 0; i < _mainPerson.allKits.Length; i++)
		{
			if ((bool)_mainPerson.allKits[i])
			{
				UnityEngine.Object.Destroy(_mainPerson.allKits[i]);
				_mainPerson.allKits[i] = null;
			}
		}
	}

	public void ApplyEquipment()
	{
		TryApply(KitType.Hair, hair.id);
		TryApply(KitType.Face, face.id);
		TryApply(KitType.Beard, beard.id);
		TryApply(KitType.Caps, cap.id);
		TryApply(KitType.Mask, mask.id);
		TryApply(KitType.Top, top.id);
		TryApply(KitType.Gloves, gloves.id);
		TryApply(KitType.Bag, bag.id);
		TryApply(KitType.Pants, pants.id);
		TryApply(KitType.Shoes, shoes.id);
		TryApply(KitType.Glasses, glasses.id);
		_charactersService.LoadAllMask(_mainPerson.gameObject);
	}

	private void TryApply(KitType kitType, int id)
	{
		if (id < 0)
		{
			if (kitType == KitType.Face || kitType == KitType.Pants)
			{
				_charactersService.LoadPersonEquipment(_mainPerson.gameObject, (int)kitType, 0);
			}
		}
		else
		{
			_charactersService.LoadPersonEquipment(_mainPerson.gameObject, (int)kitType, id);
		}
	}
}

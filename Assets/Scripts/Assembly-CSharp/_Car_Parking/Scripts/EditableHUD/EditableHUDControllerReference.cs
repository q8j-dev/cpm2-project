using System.Collections.Generic;

namespace _Car_Parking.Scripts.EditableHUD
{
	public static class EditableHUDControllerReference
	{
		private static List<EditableHUDController> _hudControllers = new List<EditableHUDController>();

		public static List<EditableHUDController> GetCurrentHUDControllers()
		{
			List<EditableHUDController> list = new List<EditableHUDController>();
			EditableHUDController currentHUDController = GetCurrentHUDController();
			if (!currentHUDController)
			{
				return list;
			}
			for (int i = 0; i < _hudControllers.Count; i++)
			{
				if ((bool)_hudControllers[i] && _hudControllers[i].Priority == currentHUDController.Priority)
				{
					list.Add(_hudControllers[i]);
				}
			}
			return list;
		}

		public static EditableHUDController GetCurrentHUDController()
		{
			EditableHUDController editableHUDController = null;
			for (int i = 0; i < _hudControllers.Count; i++)
			{
				if (!editableHUDController || _hudControllers[i].Priority > editableHUDController.Priority)
				{
					editableHUDController = _hudControllers[i];
				}
			}
			return editableHUDController;
		}

		public static void AddHudController(EditableHUDController editableHUDController)
		{
			if (!_hudControllers.Contains(editableHUDController))
			{
				_hudControllers.Add(editableHUDController);
			}
		}

		public static void RemoveHudController(EditableHUDController editableHUDController)
		{
			_hudControllers.Remove(editableHUDController);
		}
	}
}

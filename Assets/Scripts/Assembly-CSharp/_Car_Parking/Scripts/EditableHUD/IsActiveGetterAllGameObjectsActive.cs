namespace _Car_Parking.Scripts.EditableHUD
{
	public class IsActiveGetterAllGameObjectsActive : IsActiveGetterGameObjectsBase
	{
		public override bool IsActive
		{
			get
			{
				for (int i = 0; i < GameObjectActiveSelfDatas.Length; i++)
				{
					if ((bool)GameObjectActiveSelfDatas[i].GameObject && !GameObjectActiveSelfDatas[i].GameObject.activeSelf)
					{
						return false;
					}
				}
				return true;
			}
		}
	}
}

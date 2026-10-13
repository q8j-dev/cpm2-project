using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag
{
	public abstract class UIElement : WindowScript
	{
		[Header("Main Image")]
		[SerializeField]
		internal Image detailImage = default;

		public virtual void Init(PartData part, IClickable clickable)
		{
		}

		public virtual void SetInstalledState(bool state)
		{
		}

		public virtual void OnClickItem()
		{
		}

		public virtual void SetImage(int partId)
		{
			detailImage.sprite = _managerUI.GetEngineIcon(partId);
		}
	}
}

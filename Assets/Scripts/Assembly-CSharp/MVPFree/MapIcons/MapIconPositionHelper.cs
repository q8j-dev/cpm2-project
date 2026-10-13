using MVPFree.MapIcons.Enum;
using UnityEngine;

namespace MVPFree.MapIcons
{
	public class MapIconPositionHelper : MonoBehaviour
	{
		public IconType IconType;

		public int Id = -1;

		private void Start()
		{
			Object.Destroy(this);
		}
	}
}

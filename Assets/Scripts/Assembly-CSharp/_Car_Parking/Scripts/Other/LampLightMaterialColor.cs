using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class LampLightMaterialColor : LampLight
	{
		[SerializeField]
		private MeshRenderer MeshRenderer;

		[SerializeField]
		private string ColorName;

		[SerializeField]
		private Color ColorStart;

		[SerializeField]
		private Color ColorFinish;

		private Material _material;

		private Material Material
		{
			get
			{
				if (!_material && (bool)MeshRenderer)
				{
					_material = MeshRenderer.material;
				}
				return _material;
			}
		}

		public override float Intensity
		{
			get
			{
				if ((bool)Material)
				{
					Color color = Material.GetColor(ColorName);
					return (Mathf.InverseLerp(ColorStart.r, ColorFinish.r, color.r) + Mathf.InverseLerp(ColorStart.g, ColorFinish.g, color.r) + Mathf.InverseLerp(ColorStart.b, ColorFinish.b, color.r)) / 3f;
				}
				return 0f;
			}
			protected set
			{
				if ((bool)Material)
				{
					Material.SetColor(ColorName, Color.Lerp(ColorStart, ColorFinish, value));
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ((bool)Material)
			{
				Object.Destroy(Material);
			}
			_material = null;
		}
	}
}

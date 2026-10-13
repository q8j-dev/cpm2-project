using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class LampLightMaterialColorAlpha : LampLight
	{
		[SerializeField]
		private MeshRenderer MeshRenderer;

		[SerializeField]
		private string ColorName;

		[SerializeField]
		private float AlphaMax = 1f;

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
				if (!Material)
				{
					return 0f;
				}
				return Material.GetColor(ColorName).a / AlphaMax;
			}
			protected set
			{
				if ((bool)Material)
				{
					Color color = Material.GetColor(ColorName);
					color.a = value * AlphaMax;
					Material.SetColor(ColorName, color);
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

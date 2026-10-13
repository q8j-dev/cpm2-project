using CPMEngine;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Car.TachometerCustom
{
	public class TachometerHolderGradient : MonoBehaviour
	{
		private const string GradientShaderName = "UI/GradientMask";

		[SerializeField]
		private TachometerPointsConfig PointsConfig;

		[SerializeField]
		private Material Material;

		private Image _image;

		private Powertrain _driveTrain;

		private TachometerPoint[] Points => PointsConfig.Points;

		public void TryInit(CarInteriorReferences interior, Powertrain powertrain)
		{
			if ((bool)interior && (bool)Material)
			{
				RebindShaderIfUnsupported();
				if ((bool)interior.Tachometer && interior.Tachometer.TryGetComponent<Image>(out _image))
				{
					_image.material = Material;
					EnsureSimpleImageType(_image);
				}
				_driveTrain = powertrain;
			}
		}

		private void RebindShaderIfUnsupported()
		{
			Shader shader = Material.shader;
			if (!shader || !shader.isSupported)
			{
				Shader shader2 = Shader.Find(shader ? shader.name : "UI/GradientMask");
				if ((bool)shader2 && shader2.isSupported)
				{
					Material.shader = shader2;
				}
			}
		}

		private static void EnsureSimpleImageType(Image image)
		{
			if (image.type != Image.Type.Simple)
			{
				image.type = Image.Type.Simple;
			}
		}

		private void LateUpdate()
		{
			if ((bool)_driveTrain && (bool)_image)
			{
				Color color = _image.color;
				color.a = TachometerInterpolator.Interpolate(Points, _driveTrain.CurrentRpm);
				_image.color = color;
			}
		}
	}
}

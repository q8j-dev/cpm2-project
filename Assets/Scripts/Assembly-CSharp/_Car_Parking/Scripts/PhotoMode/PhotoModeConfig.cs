using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PhotoMode
{
	[Preserve]
	[CreateAssetMenu(fileName = "PhotoModeConfig", menuName = "Configs/PhotoMode")]
	public class PhotoModeConfig : ScriptableObject
	{
		[Header("Default values")]
		public int DefaultZoom;

		public int DefaultCameraTilt;

		public int DefaultAperture;

		public int DefaultShutterSpeed;

		public int DefaultDistortion;

		public int DefaultColorCorrection;

		public int DefaultTemperature;

		public int DefaultExposure;

		public int DefaultPanningMode;

		public int DefaultFilter;

		[Header("Panning mode tutorial textures")]
		public Sprite[] PanningTutorSprites;

		[Header("Included enum collections")]
		public Aperture[] Apertures;

		public ShutterSpeed[] ShutterSpeeds;

		[Header("Info")]
		public CategoryInfo[] Categories;

		[Preserve]
		public PhotoModeConfig()
		{
		}
	}
}

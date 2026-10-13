using System;
using UnityEngine;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class CameraSettingsBlock
	{
		public CameraControls Car = new CameraControls
		{
			Look = new ControlSettingVector2
			{
				Default = new Vector2(0.18f, 0.153f)
			}
		};

		public CameraControls CarFpv = new CameraControls();

		public PersonCameraControls Person = new PersonCameraControls();

		public CameraControls Garage = new CameraControls();

		public DroneCameraControls Drone = new DroneCameraControls();

		public CameraControls Map = new CameraControls();

		public CameraControls Camera(CameraSettingsContext ctx)
		{
			return ctx switch
			{
				CameraSettingsContext.Car => Car, 
				CameraSettingsContext.CarFpv => CarFpv, 
				CameraSettingsContext.Garage => Garage, 
				CameraSettingsContext.Drone => Drone, 
				CameraSettingsContext.Map => Map, 
				_ => null, 
			};
		}
	}
}

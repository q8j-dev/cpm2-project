using System;
using Car_Parking.Scripts.Drag.CarCompositions;
using UnityEngine;
using _Car_Parking.Scripts.Car.Configs;

public interface IBodyKitValuesParent
{
	Material CarMaterial { get; }

	LightsComposition LightsComposition { get; }

	Transform transform { get; }

	DefaultMaterialsConfig DefaultMaterialsConfig { get; }

	event Action<Color> CarColorChanged;
}

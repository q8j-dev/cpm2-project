using System;
using System.Collections.Generic;

[Serializable]
public class CarLightGroup
{
	public List<CarLightValue> Values = new List<CarLightValue>();

	public bool isOn;
}

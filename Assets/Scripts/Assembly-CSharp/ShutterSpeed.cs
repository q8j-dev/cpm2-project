using UnityEngine;

public enum ShutterSpeed
{
	[InspectorName("2\"")]
	Sec2 = -2,
	[InspectorName("1\"")]
	Sec1 = -1,
	[InspectorName("1\\2")]
	_1_2 = 2,
	[InspectorName("1\\4")]
	_1_4 = 4,
	[InspectorName("1\\8")]
	_1_8 = 8,
	[InspectorName("1\\15")]
	_1_15 = 15,
	[InspectorName("1\\30")]
	_1_30 = 30,
	[InspectorName("1\\60")]
	_1_60 = 60,
	[InspectorName("1\\125")]
	_1_125 = 125,
	[InspectorName("1\\250")]
	_1_250 = 250,
	[InspectorName("1\\500")]
	_1_500 = 500,
	[InspectorName("1\\1000")]
	_1_1000 = 1000
}

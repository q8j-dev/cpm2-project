namespace CPM.Scripts.VinylDrawer
{
	public enum VinylsEditorInputState : byte
	{
		Default = 0,
		Selection = 1,
		MultiSelection = 2,
		MultiSelected = 3,
		OneSelected = 4,
		ColorChange = 5,
		SizeChange = 6,
		TextChange = 7,
		Plate = 8,
		PlateTypes = 9,
		Windows = 10,
		CameraOnly = 11
	}
}

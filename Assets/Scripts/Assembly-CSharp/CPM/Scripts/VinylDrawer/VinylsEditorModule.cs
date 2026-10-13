namespace CPM.Scripts.VinylDrawer
{
	public abstract class VinylsEditorModule
	{
		protected VinylsEditor vinylsEditor;

		public VinylsEditorModule(VinylsEditor vinylsEditor)
		{
			this.vinylsEditor = vinylsEditor;
		}

		public virtual void PrepareBodyStarted(VehicleData vehicleData)
		{
		}

		public virtual void PrepareBodyCompleted(VehicleData vehicleData)
		{
		}

		public virtual void CloseVinylsWindowStarted()
		{
		}

		public virtual void CloseVinylsWindowCompleted()
		{
		}
	}
}

using CPM.Transmission;

namespace _Car_Parking.Scripts.Drag
{
	public static class GearBoxTypeFilterUtility
	{
		public static GearBoxTypeFilters FromKind(GearboxKind kind)
		{
			return kind switch
			{
				GearboxKind.MT => GearBoxTypeFilters.MT, 
				GearboxKind.AMT => GearBoxTypeFilters.AMT, 
				GearboxKind.DCT => GearBoxTypeFilters.DCT, 
				GearboxKind.CVT => GearBoxTypeFilters.CVT, 
				GearboxKind.Sequential => GearBoxTypeFilters.Sequential, 
				_ => GearBoxTypeFilters.AT, 
			};
		}

		public static string LocalizationKey(GearBoxTypeFilters filter)
		{
			return filter switch
			{
				GearBoxTypeFilters.AT => "Gearboxfilterwindow.At", 
				GearBoxTypeFilters.MT => "Gearboxfilterwindow.Mt", 
				GearBoxTypeFilters.AMT => "Gearboxfilterwindow.Amt", 
				GearBoxTypeFilters.DCT => "Gearboxfilterwindow.Dct", 
				GearBoxTypeFilters.CVT => "Gearboxfilterwindow.Cvt", 
				GearBoxTypeFilters.Sequential => "Gearboxfilterwindow.Seq", 
				_ => "Gearboxfilterwindow.All", 
			};
		}

		public static string LocalizationKey(GearboxKind kind)
		{
			return LocalizationKey(FromKind(kind));
		}
	}
}

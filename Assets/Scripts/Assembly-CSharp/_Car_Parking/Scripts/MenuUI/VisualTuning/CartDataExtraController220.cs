using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class CartDataExtraController220 : CartDataExtraController
	{
		private readonly int[] TrimID = new int[3] { 3, 4, 5 };

		private BodyKitsTrimConfigs _trimConfigs;

		public CartDataExtraController220(BodyKitsTrimConfigs trimConfigs)
		{
			_trimConfigs = trimConfigs;
		}

		public override void AddSmart(BodyKitsCartData cartData, BodyKitType bodyKitType, int index)
		{
			if (bodyKitType != BodyKitType.FrontBumper)
			{
				return;
			}
			for (int i = 0; i < cartData.Trims.Count; i++)
			{
				if (_trimConfigs.GetTrimType(cartData.Trims[i]) == BodyKitsTrimConfigs.TrimType.Grill)
				{
					cartData.Trims.RemoveAt(i);
					i--;
				}
			}
			cartData.Trims.Add(TrimID[index % TrimID.Length]);
		}
	}
}

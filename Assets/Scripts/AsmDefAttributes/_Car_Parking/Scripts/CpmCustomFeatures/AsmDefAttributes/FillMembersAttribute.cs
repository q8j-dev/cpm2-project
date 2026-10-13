using System;
using System.Reflection;

namespace _Car_Parking.Scripts.CpmCustomFeatures.AsmDefAttributes
{
	public class FillMembersAttribute : Attribute
	{
		public FillMembersAttribute(MemberTypes memberTypes, int min, int max)
		{
		}
	}
}

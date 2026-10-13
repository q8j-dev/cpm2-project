using System;
using System.Reflection;

namespace _Car_Parking.Scripts.CpmCustomFeatures.AsmDefAttributes
{
	public class ShuffleMembersAttribute : Attribute
	{
		private MemberTypes _memberTypes;

		public ShuffleMembersAttribute(MemberTypes memberTypes = MemberTypes.Method)
		{
			_memberTypes = memberTypes;
		}
	}
}

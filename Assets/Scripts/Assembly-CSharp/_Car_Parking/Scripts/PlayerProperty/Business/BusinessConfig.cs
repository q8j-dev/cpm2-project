using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Sirenix.OdinInspector;
using _Car_Parking.Scripts.Free_drive;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	[Serializable]
	public class BusinessConfig
	{
		public int BusinessId;

		public string MainIconAddressablePath;

		public float ProfitPerSecond = 6f;

		public NameType Name;

		public GlobalCost Price = new GlobalCost(200000);

		public SceneType Location;

		private IEnumerable<ValueDropdownItem<NameType>> GetNameTypeValues()
		{
			return from NameType e in Enum.GetValues(typeof(NameType))
				select new ValueDropdownItem<NameType>(GetEnumMemberValue(e), e);
		}

		private string GetEnumMemberValue(NameType value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());
			if (field == null)
			{
				return value.ToString();
			}
			return field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? value.ToString();
		}

		private void OnNameChanged()
		{
		}
	}
}

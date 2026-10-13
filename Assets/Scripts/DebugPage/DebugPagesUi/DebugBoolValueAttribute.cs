using System;
using JetBrains.Annotations;

namespace DebugPagesUi
{
	[AttributeUsage(AttributeTargets.Field)]
	[MeansImplicitUse]
	public class DebugBoolValueAttribute : Attribute
	{
		public string Path { get; }

		public DebugBoolValueAttribute(string path)
		{
			Path = path;
		}
	}
}

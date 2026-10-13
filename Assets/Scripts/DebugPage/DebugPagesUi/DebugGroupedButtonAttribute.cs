using System;
using JetBrains.Annotations;

namespace DebugPagesUi
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[MeansImplicitUse]
	public class DebugGroupedButtonAttribute : Attribute
	{
		public string Path { get; }

		public string Name { get; }

		public DebugGroupedButtonAttribute(string path, string name = null)
		{
			Path = path;
			Name = name;
		}
	}
}

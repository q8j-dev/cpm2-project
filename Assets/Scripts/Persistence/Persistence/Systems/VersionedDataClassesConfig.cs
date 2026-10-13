using System;
using UnityEngine;

namespace Persistence.Systems
{
	[CreateAssetMenu(fileName = "VersionedDataClassesConfig", menuName = "Scriptable/Config/Persistence/VersionedDataClasses")]
	public class VersionedDataClassesConfig : ScriptableObject
	{
		public DataVersionHistory[] entryHistories;

		public string GetPath(Type type)
		{
			string assemblyQualifiedName = type.AssemblyQualifiedName;
			DataVersionHistory[] array = entryHistories;
			foreach (DataVersionHistory dataVersionHistory in array)
			{
				if (dataVersionHistory.ContainsType(assemblyQualifiedName))
				{
					return dataVersionHistory.path;
				}
			}
			throw new Exception($"Cant find path for type {type}");
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Persistence.Systems
{
	[CreateAssetMenu(menuName = "Configs/DataClasses", fileName = "DataClassesConfig")]
	public class DataClassesConfig : ScriptableObject, IDropDownFiller
	{
		[Serializable]
		public class DataPair
		{
			[StringDropDown(path = "Configs/DataClassesConfig")]
			public string classType;

			public string saveFileName;

			public bool createSampleFile;
		}

		private const string folderPath1 = "Assets/";

		private const string folderPath2 = "Resources";

		public string assembly;

		public string namespaceName = "Persistence";

		public List<DataPair> dataPairs = new List<DataPair>();

		public string saveFilesExtension = ".sav";

		private const string sampleFilesExtension = ".asset";

		public string sampleFilesFolder = "Samples";

		public string cipherStrategy;

		public string pathCipherStrategy;
	}
}

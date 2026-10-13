using System;
using System.Collections.Generic;

namespace Persistence.Systems
{
	public class DataHandlerFiller
	{
		private DataHandlerInfo info;

		private ICipherStrategy pathCipherStrategy;

		public DataHandlerFiller(DataClassesConfig config, ICipherStrategy pathCipherStrategy)
		{
			this.pathCipherStrategy = pathCipherStrategy;
			info = new DataHandlerInfo();
			FillDataPathsDict(config);
			FillResourcesPathsDict(config);
		}

		public DataHandlerInfo GetDataHandlerInfo()
		{
			return info;
		}

		private void FillDataPathsDict(DataClassesConfig config)
		{
			info.dataPaths = new Dictionary<Type, string>();
			foreach (DataClassesConfig.DataPair dataPair in config.dataPairs)
			{
				info.dataPaths.TryAdd(Type.GetType(dataPair.classType), pathCipherStrategy.Encode(dataPair.saveFileName + config.saveFilesExtension) ?? "");
			}
		}

		private void FillResourcesPathsDict(DataClassesConfig config)
		{
			info.jsonResourcesPaths = new Dictionary<Type, string>();
			foreach (DataClassesConfig.DataPair dataPair in config.dataPairs)
			{
				info.jsonResourcesPaths.TryAdd(Type.GetType(dataPair.classType), config.sampleFilesFolder + "/" + dataPair.saveFileName);
			}
		}
	}
}

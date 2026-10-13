using System;
using System.Buffers;
using System.Collections.Generic;
using DataEntities.Vinyl;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using Persistence.Systems;
using Sirenix.Utilities;
using UnityEngine.Scripting;
using _Car_Parking.Scripts;

namespace CPM.Database
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	[UnityEngine.Scripting.Preserve]
	public class PlatesDataForDatabase : IMemoryPackable<PlatesDataForDatabase>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class PlatesDataForDatabaseFormatter : MemoryPackFormatter<PlatesDataForDatabase>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlatesDataForDatabase value)
			{
				PlatesDataForDatabase.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PlatesDataForDatabase value)
			{
				PlatesDataForDatabase.Deserialize(ref reader, ref value);
			}
		}

		public Dictionary<int, PlateData> PlateData = new Dictionary<int, PlateData>();

		public List<VinylData> Plate1 = new List<VinylData>();

		public List<VinylData> Plate2 = new List<VinylData>();

		public List<VinylData> Plate3 = new List<VinylData>();

		public List<VinylData> Plate4 = new List<VinylData>();

		public List<VinylData> Plate5 = new List<VinylData>();

		public List<VinylData> Plate6 = new List<VinylData>();

		public static PlatesDataForDatabase GetPlatesDataFromAccount()
		{
			PlatesDataForDatabase platesDataForDatabase = new PlatesDataForDatabase();
			if (!EasySaveDataHandler.TryGetFile("plates/allData", out var file))
			{
				return platesDataForDatabase;
			}
			for (int i = 1; i < 7; i++)
			{
				PlateData value = file.Load<PlateData>(i.ToString());
				platesDataForDatabase.PlateData.Add(i, value);
				string plateVinylsPath = DirectoryHelper.GetPlateVinylsPath(i.ToString());
				List<VinylData> list = new List<VinylData>();
				if (EasySaveDataHandler.TryGetFile(plateVinylsPath, out var file2))
				{
					string[] keys = file2.GetKeys();
					keys.Sort();
					string[] array = keys;
					foreach (string key in array)
					{
						list.Add(file2.Load<VinylData>(key));
					}
				}
				switch (i)
				{
				case 1:
					platesDataForDatabase.Plate1 = list;
					break;
				case 2:
					platesDataForDatabase.Plate2 = list;
					break;
				case 3:
					platesDataForDatabase.Plate3 = list;
					break;
				case 4:
					platesDataForDatabase.Plate4 = list;
					break;
				case 5:
					platesDataForDatabase.Plate5 = list;
					break;
				case 6:
					platesDataForDatabase.Plate6 = list;
					break;
				}
			}
			return platesDataForDatabase;
		}

		static PlatesDataForDatabase()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PlatesDataForDatabase>())
			{
				MemoryPackFormatterProvider.Register(new PlatesDataForDatabaseFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PlatesDataForDatabase[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PlatesDataForDatabase>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, PlateData>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, PlateData>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<VinylData>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<VinylData>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlatesDataForDatabase? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(7);
			writer.WriteValue(in value.PlateData);
			ListFormatter.SerializePackable(ref writer, value.Plate1);
			ListFormatter.SerializePackable(ref writer, value.Plate2);
			ListFormatter.SerializePackable(ref writer, value.Plate3);
			ListFormatter.SerializePackable(ref writer, value.Plate4);
			ListFormatter.SerializePackable(ref writer, value.Plate5);
			ListFormatter.SerializePackable(ref writer, value.Plate6);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PlatesDataForDatabase? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			Dictionary<int, PlateData> value2;
			List<VinylData> value3;
			List<VinylData> value4;
			List<VinylData> value5;
			List<VinylData> value6;
			List<VinylData> value7;
			List<VinylData> value8;
			if (memberCount == 7)
			{
				if (value != null)
				{
					value2 = value.PlateData;
					value3 = value.Plate1;
					value4 = value.Plate2;
					value5 = value.Plate3;
					value6 = value.Plate4;
					value7 = value.Plate5;
					value8 = value.Plate6;
					reader.ReadValue(ref value2);
					ListFormatter.DeserializePackable(ref reader, ref value3);
					ListFormatter.DeserializePackable(ref reader, ref value4);
					ListFormatter.DeserializePackable(ref reader, ref value5);
					ListFormatter.DeserializePackable(ref reader, ref value6);
					ListFormatter.DeserializePackable(ref reader, ref value7);
					ListFormatter.DeserializePackable(ref reader, ref value8);
					goto IL_0191;
				}
				value2 = reader.ReadValue<Dictionary<int, PlateData>>();
				value3 = ListFormatter.DeserializePackable<VinylData>(ref reader);
				value4 = ListFormatter.DeserializePackable<VinylData>(ref reader);
				value5 = ListFormatter.DeserializePackable<VinylData>(ref reader);
				value6 = ListFormatter.DeserializePackable<VinylData>(ref reader);
				value7 = ListFormatter.DeserializePackable<VinylData>(ref reader);
				value8 = ListFormatter.DeserializePackable<VinylData>(ref reader);
			}
			else
			{
				if (memberCount > 7)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PlatesDataForDatabase), 7, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = null;
					value3 = null;
					value4 = null;
					value5 = null;
					value6 = null;
					value7 = null;
					value8 = null;
				}
				else
				{
					value2 = value.PlateData;
					value3 = value.Plate1;
					value4 = value.Plate2;
					value5 = value.Plate3;
					value6 = value.Plate4;
					value7 = value.Plate5;
					value8 = value.Plate6;
				}
				if (memberCount != 0)
				{
					reader.ReadValue(ref value2);
					if (memberCount != 1)
					{
						ListFormatter.DeserializePackable(ref reader, ref value3);
						if (memberCount != 2)
						{
							ListFormatter.DeserializePackable(ref reader, ref value4);
							if (memberCount != 3)
							{
								ListFormatter.DeserializePackable(ref reader, ref value5);
								if (memberCount != 4)
								{
									ListFormatter.DeserializePackable(ref reader, ref value6);
									if (memberCount != 5)
									{
										ListFormatter.DeserializePackable(ref reader, ref value7);
										if (memberCount != 6)
										{
											ListFormatter.DeserializePackable(ref reader, ref value8);
											_ = 7;
										}
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0191;
				}
			}
			value = new PlatesDataForDatabase
			{
				PlateData = value2,
				Plate1 = value3,
				Plate2 = value4,
				Plate3 = value5,
				Plate4 = value6,
				Plate5 = value7,
				Plate6 = value8
			};
			return;
			IL_0191:
			value.PlateData = value2;
			value.Plate1 = value3;
			value.Plate2 = value4;
			value.Plate3 = value5;
			value.Plate4 = value6;
			value.Plate5 = value7;
			value.Plate6 = value8;
		}
	}
}

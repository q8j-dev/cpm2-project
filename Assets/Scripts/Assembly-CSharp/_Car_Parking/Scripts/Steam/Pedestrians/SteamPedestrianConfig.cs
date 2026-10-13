using System;
using UnityEngine;
using _Car_Parking.Scripts.Characters;

namespace _Car_Parking.Scripts.Steam.Pedestrians
{
	[CreateAssetMenu(fileName = "SteamPedestrianConfig", menuName = "Scriptables/Steam Pedestrian Config")]
	public class SteamPedestrianConfig : ScriptableObject
	{
		[Serializable]
		public class RarityWeights
		{
			public float Common = 80f;

			public float Uncommon = 15f;

			public float Rare = 5f;

			public float Get(PedestrianRarity rarity)
			{
				return rarity switch
				{
					PedestrianRarity.Uncommon => Uncommon, 
					PedestrianRarity.Rare => Rare, 
					_ => Common, 
				};
			}
		}

		[Serializable]
		public class SlotChances
		{
			[Range(0f, 1f)]
			public float MaleHair = 0.85f;

			[Range(0f, 1f)]
			public float Beard = 0.4f;

			[Range(0f, 1f)]
			public float Cap = 0.25f;

			[Range(0f, 1f)]
			public float Glasses = 0.2f;
		}

		[Serializable]
		public class ExcludedKit
		{
			public PedestrianGender Gender;

			public KitType Slot;

			public int Id;
		}

		[Serializable]
		public class ArchetypeSlot
		{
			public KitType Slot;

			public string[] NameKeywords = Array.Empty<string>();

			[Range(0f, 1f)]
			public float Chance = 1f;

			[Tooltip("Заполняет Refresh Kit Pools")]
			public int[] MaleIds = Array.Empty<int>();

			[Tooltip("Заполняет Refresh Kit Pools")]
			public int[] FemaleIds = Array.Empty<int>();
		}

		[Serializable]
		public class StyleSlot
		{
			public KitType Slot;

			public string[] NameKeywords = Array.Empty<string>();

			[Tooltip("Заполняет Refresh Kit Pools")]
			public int[] MaleIds = Array.Empty<int>();

			[Tooltip("Заполняет Refresh Kit Pools")]
			public int[] FemaleIds = Array.Empty<int>();
		}

		[Serializable]
		public class Style
		{
			public string Name;

			[Min(0f)]
			public float Weight = 1f;

			public StyleSlot[] Slots = Array.Empty<StyleSlot>();
		}

		[Serializable]
		public class Archetype
		{
			public string Name;

			public PedestrianRarity Rarity = PedestrianRarity.Uncommon;

			public PedestrianGender Gender;

			[Min(0f)]
			public int Count = 2;

			[Tooltip("Имя образа для незанятых слотов; пусто — случайный")]
			public string Style = "Casual";

			public ArchetypeSlot[] Slots = Array.Empty<ArchetypeSlot>();
		}

		[Serializable]
		public class KitPool
		{
			public PedestrianGender Gender;

			public KitType Slot;

			public int[] Ids = Array.Empty<int>();
		}

		public const string ResourcesPath = "Configs/SteamPedestrianConfig";

		[Header("Частота классов (вес)")]
		public RarityWeights Weights = new RarityWeights();

		[Header("Обычные граждане")]
		[Min(0f)]
		public int CommonCount = 40;

		[Range(0f, 1f)]
		public float MaleShare = 0.5f;

		public SlotChances Chances = new SlotChances();

		public bool ExcludeRewardKits = true;

		public bool ExcludeEventKits = true;

		[Tooltip("Вещь исключается, если её название содержит любой из кусков (без учёта регистра)")]
		public string[] ExcludedNameKeywords = new string[54]
		{
			"Legioner", "Gladiator", "CitizenRome", "CitizensRome", "Astronaut", "Deadpool", "AOT", "Diver", "Matador", "Fury",
			"Tech_body", "hazmat", "Prisoner", "Donut", "Target", "Kimono", "Priest", "Robe", "Racer", "Traffic_Cop",
			"SWAT", "FBI", "Police", "Sheriff", "Marshall", "Firefighter", "Cowboy", "Indiana", "Walter", "DocBrown",
			"Macfly", "Vaas", "Wednesday", "Bank_Mask", "Paranja", "Elf", "christmas", "Pijama", "CROWN", "Gas_mask",
			"Sombrero", "helmet", "Hockey", "Mafia", "GodFather", "Italy", "Male_doc", "Mechanic", "Burger", "Cashier",
			"Waiter", "Furajka", "Gurta", "Kilt"
		};

		public ExcludedKit[] ExcludedKits = Array.Empty<ExcludedKit>();

		public const string StyleSummer = "Summer";

		public const string StyleCasual = "Casual";

		public const string StyleWarm = "Warm";

		public const string StyleFormal = "Formal";

		public const string StyleSport = "Sport";

		[Header("Образы: верх, низ и обувь одного набора")]
		public Style[] Styles = new Style[5]
		{
			new Style
			{
				Name = "Summer",
				Weight = 20f,
				Slots = new StyleSlot[4]
				{
					new StyleSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[4] { "Sleeveless", "T_Shirt_B1", "Tropic_Shirt", "Tropical_dress" }
					},
					new StyleSlot
					{
						Slot = KitType.Pants,
						NameKeywords = new string[3] { "Tropic_Short", "Male_pants", "Urban_pants" }
					},
					new StyleSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[8] { "Flipflops", "Hat_Sandals", "Tropical_shoes", "Female_tapki", "Converse", "Sneaker", "Nike", "Nyjah" }
					},
					new StyleSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[3] { "Male_Cup", "Arafat", "Hat_Sandals" }
					}
				}
			},
			new Style
			{
				Name = "Casual",
				Weight = 35f,
				Slots = new StyleSlot[4]
				{
					new StyleSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[15]
						{
							"189_Shirt", "190_Shirt", "191_Shirt", "192_Shirt", "Shirt_black", "Shirt_white", "Shirt_blue", "Shirt_red", "Arabic_shirt", "Urban_Body",
							"T_Shirt_B1", "Longsleeve", "female_everyday", "Everyday9", "Sweater"
						}
					},
					new StyleSlot
					{
						Slot = KitType.Pants,
						NameKeywords = new string[7] { "Male_pants", "Urban_pants", "Urban_Pants_2", "Male_Pants_Araben", "Female_pants", "Female_cargo_pants", "Korean_pants" }
					},
					new StyleSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[9] { "Sneaker", "Nike", "Nyjah", "Converse", "Urban_shoe", "Urban_2_boots", "Boots_D", "Korean_boots", "Female_baseball" }
					},
					new StyleSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[5] { "Male_Cup", "Arafat", "Peaky_blinders", "Urban_hat", "Hairs" }
					}
				}
			},
			new Style
			{
				Name = "Warm",
				Weight = 20f,
				Slots = new StyleSlot[4]
				{
					new StyleSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[7] { "Bombers", "Urban_hoodie", "Hoodie", "Female_leather_jacket", "Female_Nomad_jacket", "jacket_jeans", "Sweater" }
					},
					new StyleSlot
					{
						Slot = KitType.Pants,
						NameKeywords = new string[6] { "Male_pants", "Urban_pants", "Urban_Pants_2", "Female_pants", "Female_cargo_pants", "Korean_pants" }
					},
					new StyleSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[8] { "Boots_D", "Army_boots", "Urban_2_boots", "Urban_shoe", "Sneaker", "Nike", "Korean_boots", "Converse" }
					},
					new StyleSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[3] { "Urban_shapka", "Shapki_", "Peaky_blinders" }
					}
				}
			},
			new Style
			{
				Name = "Formal",
				Weight = 15f,
				Slots = new StyleSlot[4]
				{
					new StyleSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[3] { "Male_Office_Set_Jacket", "Suit_troyka", "Italian_" }
					},
					new StyleSlot
					{
						Slot = KitType.Pants,
						NameKeywords = new string[2] { "Male_Office_Set_Pants", "Female_pants" }
					},
					new StyleSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[3] { "Suit_male_troyka_shoes", "Italian_Shoes", "Boots_D" }
					},
					new StyleSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[2] { "Peaky_blinders", "Urban_hat" }
					}
				}
			},
			new Style
			{
				Name = "Sport",
				Weight = 10f,
				Slots = new StyleSlot[4]
				{
					new StyleSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[5] { "Male_Football", "Male_basketball", "Man_baseball", "Woman_sporty", "Female_baseball" }
					},
					new StyleSlot
					{
						Slot = KitType.Pants,
						NameKeywords = new string[3] { "Male_Football", "Man_baseball", "Female_cargo_pants" }
					},
					new StyleSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[7] { "Male_Football", "Male_basketball", "Man_baseball", "Female_baseball", "Sneaker", "Nike", "Converse" }
					},
					new StyleSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[1] { "Male_Cup" }
					}
				}
			}
		};

		[Header("Особые образы")]
		public Archetype[] Archetypes = new Archetype[7]
		{
			new Archetype
			{
				Name = "Mechanic",
				Rarity = PedestrianRarity.Uncommon,
				Count = 3,
				Slots = new ArchetypeSlot[2]
				{
					new ArchetypeSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[2] { "MechanicNPC", "Mechanic_body" }
					},
					new ArchetypeSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[1] { "Mechanic_hat" },
						Chance = 0.5f
					}
				}
			},
			new Archetype
			{
				Name = "Fast food",
				Rarity = PedestrianRarity.Uncommon,
				Count = 2,
				Slots = new ArchetypeSlot[3]
				{
					new ArchetypeSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[1] { "Burger_worker" }
					},
					new ArchetypeSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[1] { "Burger_worker_hat" }
					},
					new ArchetypeSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[2] { "Suit_male_troyka_shoes", "Boots_D" }
					}
				}
			},
			new Archetype
			{
				Name = "Cashier",
				Rarity = PedestrianRarity.Uncommon,
				Count = 2,
				Slots = new ArchetypeSlot[3]
				{
					new ArchetypeSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[1] { "Cashier" }
					},
					new ArchetypeSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[1] { "Cashier_hat" },
						Chance = 0.7f
					},
					new ArchetypeSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[2] { "Suit_male_troyka_shoes", "Boots_D" }
					}
				}
			},
			new Archetype
			{
				Name = "Waiter",
				Rarity = PedestrianRarity.Uncommon,
				Count = 2,
				Slots = new ArchetypeSlot[3]
				{
					new ArchetypeSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[1] { "Waiter" }
					},
					new ArchetypeSlot
					{
						Slot = KitType.Caps,
						Chance = 0f
					},
					new ArchetypeSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[2] { "Suit_male_troyka_shoes", "Boots_D" }
					}
				}
			},
			new Archetype
			{
				Name = "Doctor",
				Rarity = PedestrianRarity.Uncommon,
				Count = 2,
				Slots = new ArchetypeSlot[3]
				{
					new ArchetypeSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[1] { "Male_doc" }
					},
					new ArchetypeSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[1] { "Male_doc" },
						Chance = 0.5f
					},
					new ArchetypeSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[1] { "Male_doc" }
					}
				}
			},
			new Archetype
			{
				Name = "Police",
				Rarity = PedestrianRarity.Rare,
				Count = 4,
				Slots = new ArchetypeSlot[3]
				{
					new ArchetypeSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[4] { "Policeman_Blue", "Policeman_Brown", "Policeman_Green", "Policeman_Grey" }
					},
					new ArchetypeSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[1] { "Policeman_Sheriff" },
						Chance = 0.6f
					},
					new ArchetypeSlot
					{
						Slot = KitType.Shoes,
						NameKeywords = new string[2] { "38_Policeman", "Police_man_Japan_Botts" }
					}
				}
			},
			new Archetype
			{
				Name = "Firefighter",
				Rarity = PedestrianRarity.Rare,
				Count = 2,
				Slots = new ArchetypeSlot[2]
				{
					new ArchetypeSlot
					{
						Slot = KitType.Top,
						NameKeywords = new string[1] { "Firefighter" }
					},
					new ArchetypeSlot
					{
						Slot = KitType.Caps,
						NameKeywords = new string[1] { "Firefighter" }
					}
				}
			}
		};

		[Header("Заполняет Refresh Kit Pools")]
		public KitPool[] CommonPools = Array.Empty<KitPool>();

		public Style FindStyle(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			Style[] styles = Styles;
			foreach (Style style in styles)
			{
				if (style != null && string.Equals(style.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return style;
				}
			}
			return null;
		}

		public static int[] GetStylePool(Style style, int gender, KitType slot)
		{
			if (style == null)
			{
				return Array.Empty<int>();
			}
			StyleSlot[] slots = style.Slots;
			foreach (StyleSlot styleSlot in slots)
			{
				if (styleSlot.Slot == slot)
				{
					if (gender != 1)
					{
						return styleSlot.FemaleIds;
					}
					return styleSlot.MaleIds;
				}
			}
			return Array.Empty<int>();
		}

		public int[] GetCommonPool(int gender, KitType slot)
		{
			PedestrianGender pedestrianGender = ((gender == 1) ? PedestrianGender.Male : PedestrianGender.Female);
			KitPool[] commonPools = CommonPools;
			foreach (KitPool kitPool in commonPools)
			{
				if (kitPool.Gender == pedestrianGender && kitPool.Slot == slot)
				{
					return kitPool.Ids;
				}
			}
			return Array.Empty<int>();
		}
	}
}

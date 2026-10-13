using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Traffic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.Steam.Perf;

namespace _Car_Parking.Scripts.Steam.Pedestrians
{
	public sealed class SteamPedestrianFactory
	{
		private sealed class Outfit
		{
			public string Name;

			public int Gender;

			public int[] Ids;

			public PedestrianRarity Rarity;
		}

		private sealed class MeshBuilder
		{
			public readonly Vector3[] Vertices;

			public readonly Vector3[] Normals;

			public readonly Vector4[] Tangents;

			public readonly Vector2[] Uv0;

			public BoneWeight[] Weights;

			public readonly List<Material> Materials = new List<Material>();

			public readonly List<List<int>> Triangles = new List<List<int>>();

			public int Count;

			public bool MissingTangents;

			public MeshBuilder(int vertexCount)
			{
				Vertices = new Vector3[vertexCount];
				Normals = new Vector3[vertexCount];
				Tangents = new Vector4[vertexCount];
				Uv0 = new Vector2[vertexCount];
				Weights = new BoneWeight[vertexCount];
			}
		}

		private const int KitCount = 11;

		private const string MaleTemplatePath = "PeopleTrafficResources/Man_11";

		private const string FemaleTemplatePath = "PeopleTrafficResources/Girl_12";

		private const string MaleControllerPath = "PeopleTrafficResources/SteamPedestrianMale";

		private const string FemaleControllerPath = "PeopleTrafficResources/SteamPedestrianFemale";

		private const string MaleFacePrefix = "Body_Male_";

		private const string FemaleFacePrefix = "Body_Female_";

		private const string AlbedoProperty = "_Albedo";

		private const string NormalProperty = "_Normals";

		private const long BuildBudgetMs = 4L;

		private const string MaleKitsConfigPath = "Configs/PersonKitConfig/PersonKitConfigMale";

		private const string FemaleKitsConfigPath = "Configs/PersonKitConfig/PersonKitConfigFemale";

		private const string MaleMaskPath = "MenuPerson/MaleKit/Alpha_Mask/";

		private const string FemaleMaskPath = "MenuPerson/FemaleKit/Alpha_Mask/";

		private readonly GameObject[] _skeletons = new GameObject[2];

		private readonly string[] _bodyPaths = new string[2];

		private readonly Stopwatch _phase = new Stopwatch();

		private double _skinMs;

		private double _kitsMs;

		private double _combineMs;

		private double _templateMs;

		private double _cleanupMs;

		private int _identityParts;

		private int _uniformParts;

		private int _blendParts;

		private int _recalculatedTangents;

		private readonly Stopwatch _sub = new Stopwatch();

		private double _readMs;

		private double _loopMs;

		private double _meshMs;

		private readonly CharactersService _characters;

		private readonly SteamPedestrianAssets _assets;

		private readonly Transform _templates;

		public SteamPedestrianFactory(CharactersService characters, Transform parent)
		{
			_characters = characters;
			GameObject gameObject = new GameObject("[SteamPedestrians]");
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			_assets = gameObject.AddComponent<SteamPedestrianAssets>();
			GameObject gameObject2 = new GameObject("Templates");
			gameObject2.SetActive(value: false);
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
			_templates = gameObject2.transform;
		}

		public void Dispose()
		{
			if ((bool)_assets)
			{
				UnityEngine.Object.Destroy(_assets.gameObject);
			}
		}

		public async UniTask<List<SteamPedestrianTemplate>> BuildAsync(SteamPedestrianConfig config, int seed, CancellationToken ct)
		{
			List<SteamPedestrianTemplate> result = new List<SteamPedestrianTemplate>();
			List<Outfit> outfits = GenerateOutfits(config, new System.Random(seed));
			Stopwatch total = Stopwatch.StartNew();
			Dictionary<string, Texture> faces = new Dictionary<string, Texture>();
			await UniTask.WhenAll(_characters.PrefetchPersonAssetsAsync(1, driverOutfit: false, ct), _characters.PrefetchPersonAssetsAsync(0, driverOutfit: false, ct), PrefetchKitsAsync(outfits, ct), LoadFacesAsync(outfits, faces, ct));
			_ = total.ElapsedMilliseconds;
			Stopwatch frame = Stopwatch.StartNew();
			long buildMs = 0L;
			foreach (Outfit item in outfits)
			{
				if (!_assets)
				{
					break;
				}
				long elapsedMilliseconds = frame.ElapsedMilliseconds;
				try
				{
					GameObject gameObject = BuildOne(item, faces);
					if ((bool)gameObject)
					{
						result.Add(new SteamPedestrianTemplate
						{
							Prefab = gameObject,
							Rarity = item.Rarity
						});
					}
				}
				catch (Exception)
				{
				}
				buildMs += frame.ElapsedMilliseconds - elapsedMilliseconds;
				if (frame.ElapsedMilliseconds >= 4)
				{
					await UniTask.Yield(PlayerLoopTiming.Update, ct);
					frame.Restart();
				}
			}
			return result;
		}

		private static List<Outfit> GenerateOutfits(SteamPedestrianConfig config, System.Random rnd)
		{
			List<Outfit> list = new List<Outfit>();
			for (int i = 0; i < config.CommonCount; i++)
			{
				int gender = ((rnd.NextDouble() < (double)config.MaleShare) ? 1 : 0);
				int[] array = CommonOutfit(config, gender, rnd, null, out var usedStyle);
				if (array != null)
				{
					list.Add(new Outfit
					{
						Name = $"Ped_{usedStyle}_{i}_{GenderTag(gender)}",
						Gender = gender,
						Ids = array,
						Rarity = PedestrianRarity.Common
					});
				}
			}
			SteamPedestrianConfig.Archetype[] archetypes = config.Archetypes;
			foreach (SteamPedestrianConfig.Archetype archetype in archetypes)
			{
				for (int k = 0; k < archetype.Count; k++)
				{
					int num = PickArchetypeGender(archetype, config.MaleShare, rnd);
					int[] array2 = ((num < 0) ? null : CommonOutfit(config, num, rnd, archetype.Style, out var _));
					if (array2 == null)
					{
						break;
					}
					SteamPedestrianConfig.ArchetypeSlot[] slots = archetype.Slots;
					foreach (SteamPedestrianConfig.ArchetypeSlot archetypeSlot in slots)
					{
						int[] array3 = ((num == 1) ? archetypeSlot.MaleIds : archetypeSlot.FemaleIds);
						array2[(int)archetypeSlot.Slot] = ((array3.Length != 0 && rnd.NextDouble() < (double)archetypeSlot.Chance) ? array3[rnd.Next(array3.Length)] : (-1));
					}
					Normalize(array2, num);
					list.Add(new Outfit
					{
						Name = $"Ped_{archetype.Name}_{k}_{GenderTag(num)}",
						Gender = num,
						Ids = array2,
						Rarity = archetype.Rarity
					});
				}
			}
			return list;
		}

		private static int[] CommonOutfit(SteamPedestrianConfig config, int gender, System.Random rnd, string styleName, out string usedStyle)
		{
			int[] array = new int[11];
			for (int i = 0; i < 11; i++)
			{
				array[i] = -1;
			}
			SteamPedestrianConfig.Style style = PickStyle(config, gender, styleName, rnd);
			usedStyle = ((style != null) ? style.Name : "Common");
			SteamPedestrianConfig.SlotChances chances = config.Chances;
			array[1] = Pick(config, gender, KitType.Face, rnd);
			array[5] = PickStyled(config, style, gender, KitType.Top, rnd);
			array[8] = PickStyled(config, style, gender, KitType.Pants, rnd);
			array[9] = PickStyled(config, style, gender, KitType.Shoes, rnd);
			if (gender == 0 || rnd.NextDouble() < (double)chances.MaleHair)
			{
				array[0] = Pick(config, gender, KitType.Hair, rnd);
			}
			if (gender == 1 && rnd.NextDouble() < (double)chances.Beard)
			{
				array[2] = Pick(config, gender, KitType.Beard, rnd);
			}
			if (rnd.NextDouble() < (double)chances.Cap)
			{
				array[3] = PickStyledCap(config, style, gender, rnd);
			}
			if (rnd.NextDouble() < (double)chances.Glasses)
			{
				array[10] = Pick(config, gender, KitType.Glasses, rnd);
			}
			if (array[1] < 0 || array[5] < 0)
			{
				return null;
			}
			Normalize(array, gender);
			return array;
		}

		private static int Pick(SteamPedestrianConfig config, int gender, KitType slot, System.Random rnd)
		{
			int[] commonPool = config.GetCommonPool(gender, slot);
			if (commonPool.Length != 0)
			{
				return commonPool[rnd.Next(commonPool.Length)];
			}
			return -1;
		}

		private static int PickStyled(SteamPedestrianConfig config, SteamPedestrianConfig.Style style, int gender, KitType slot, System.Random rnd)
		{
			int[] stylePool = SteamPedestrianConfig.GetStylePool(style, gender, slot);
			if (stylePool.Length == 0)
			{
				return Pick(config, gender, slot, rnd);
			}
			return stylePool[rnd.Next(stylePool.Length)];
		}

		private static int PickStyledCap(SteamPedestrianConfig config, SteamPedestrianConfig.Style style, int gender, System.Random rnd)
		{
			if (style == null)
			{
				return Pick(config, gender, KitType.Caps, rnd);
			}
			int[] stylePool = SteamPedestrianConfig.GetStylePool(style, gender, KitType.Caps);
			if (stylePool.Length == 0)
			{
				return -1;
			}
			return stylePool[rnd.Next(stylePool.Length)];
		}

		private static SteamPedestrianConfig.Style PickStyle(SteamPedestrianConfig config, int gender, string styleName, System.Random rnd)
		{
			if (!string.IsNullOrEmpty(styleName))
			{
				SteamPedestrianConfig.Style style = config.FindStyle(styleName);
				if (HasTop(style, gender))
				{
					return style;
				}
			}
			float num = 0f;
			SteamPedestrianConfig.Style[] styles = config.Styles;
			foreach (SteamPedestrianConfig.Style style2 in styles)
			{
				if (HasTop(style2, gender))
				{
					num += Mathf.Max(0f, style2.Weight);
				}
			}
			if (num <= 0f)
			{
				return null;
			}
			float num2 = (float)rnd.NextDouble() * num;
			styles = config.Styles;
			foreach (SteamPedestrianConfig.Style style3 in styles)
			{
				if (HasTop(style3, gender))
				{
					num2 -= Mathf.Max(0f, style3.Weight);
					if (num2 <= 0f)
					{
						return style3;
					}
				}
			}
			return null;
		}

		private static bool HasTop(SteamPedestrianConfig.Style style, int gender)
		{
			if (style != null)
			{
				return SteamPedestrianConfig.GetStylePool(style, gender, KitType.Top).Length != 0;
			}
			return false;
		}

		private static int PickArchetypeGender(SteamPedestrianConfig.Archetype archetype, float maleShare, System.Random rnd)
		{
			bool flag = archetype.Gender != PedestrianGender.Female && HasRequiredSlots(archetype, 1);
			bool flag2 = archetype.Gender != PedestrianGender.Male && HasRequiredSlots(archetype, 0);
			if (flag & flag2)
			{
				if (!(rnd.NextDouble() < (double)maleShare))
				{
					return 0;
				}
				return 1;
			}
			if (!flag)
			{
				if (!flag2)
				{
					return -1;
				}
				return 0;
			}
			return 1;
		}

		private static bool HasRequiredSlots(SteamPedestrianConfig.Archetype archetype, int gender)
		{
			SteamPedestrianConfig.ArchetypeSlot[] slots = archetype.Slots;
			foreach (SteamPedestrianConfig.ArchetypeSlot archetypeSlot in slots)
			{
				if (archetypeSlot.Chance >= 1f && ((gender == 1) ? archetypeSlot.MaleIds : archetypeSlot.FemaleIds).Length == 0)
				{
					return false;
				}
			}
			return true;
		}

		private static void Normalize(int[] ids, int gender)
		{
			if (ids[1] < 0)
			{
				ids[1] = 0;
			}
			if (ids[8] < 0)
			{
				ids[8] = 0;
			}
			if (ids[9] < 0)
			{
				ids[9] = 0;
			}
			if (gender == 0 && ids[0] < 0)
			{
				ids[0] = 0;
			}
		}

		private static string GenderTag(int gender)
		{
			if (gender != 1)
			{
				return "F";
			}
			return "M";
		}

		private static async UniTask PrefetchKitsAsync(List<Outfit> outfits, CancellationToken ct)
		{
			PersonKitConfig personKitConfig = Resources.Load<PersonKitConfig>("Configs/PersonKitConfig/PersonKitConfigMale");
			PersonKitConfig personKitConfig2 = Resources.Load<PersonKitConfig>("Configs/PersonKitConfig/PersonKitConfigFemale");
			HashSet<string> hashSet = new HashSet<string>();
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (Outfit outfit in outfits)
			{
				PersonKitConfig config = ((outfit.Gender == 1) ? personKitConfig : personKitConfig2);
				for (int i = 0; i < 11; i++)
				{
					int num = outfit.Ids[i];
					if (num >= 0)
					{
						hashSet.Add($"{CharactersService.GetPathForKit(outfit.Gender, i)}skins/{num}_skin");
						string text = MaskPath(config, outfit.Gender, i, num);
						if (text != null)
						{
							hashSet2.Add(text);
						}
					}
				}
			}
			List<ResourceRequest> list = new List<ResourceRequest>(hashSet.Count + hashSet2.Count);
			foreach (string item in hashSet)
			{
				list.Add(Resources.LoadAsync<GameObject>(item));
			}
			foreach (string item2 in hashSet2)
			{
				list.Add(Resources.LoadAsync<Texture>(item2));
			}
			foreach (ResourceRequest item3 in list)
			{
				await item3.ToUniTask(null, PlayerLoopTiming.Update, ct);
			}
		}

		private static string MaskPath(PersonKitConfig config, int gender, int typeId, int id)
		{
			string text = typeId switch
			{
				5 => "Top/", 
				8 => "Pants/", 
				9 => "Shoes/", 
				_ => null, 
			};
			if (text == null || !config)
			{
				return null;
			}
			PersonElements.PersonKitData[] array = config.allKits[typeId];
			if (array == null || id >= array.Length || array[id].MainBodyMask == 0)
			{
				return null;
			}
			return ((gender == 1) ? "MenuPerson/MaleKit/Alpha_Mask/" : "MenuPerson/FemaleKit/Alpha_Mask/") + text + array[id].MainBodyMask;
		}

		private async UniTask LoadFacesAsync(List<Outfit> outfits, Dictionary<string, Texture> faces, CancellationToken ct)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Outfit outfit in outfits)
			{
				string text = FacePrefix(outfit.Gender);
				hashSet.Add(text + Mathf.Max(0, outfit.Ids[1]));
				hashSet.Add(text + "N");
			}
			List<UniTask<Texture>> list = new List<UniTask<Texture>>(hashSet.Count);
			List<string> order = new List<string>(hashSet);
			foreach (string item in order)
			{
				list.Add(AddressableLoader.LoadAssetAsync<Texture>(item, ct, _assets.gameObject));
			}
			Texture[] array = await UniTask.WhenAll(list);
			for (int i = 0; i < order.Count; i++)
			{
				faces[order[i]] = array[i];
			}
		}

		private static string FacePrefix(int gender)
		{
			if (gender != 1)
			{
				return "Body_Female_";
			}
			return "Body_Male_";
		}

		private GameObject BuildOne(Outfit outfit, Dictionary<string, Texture> faces)
		{
			CharactersService.SteamSkipSkinTexture = true;
			try
			{
				return BuildOneInternal(outfit, faces);
			}
			finally
			{
				CharactersService.SteamSkipSkinTexture = false;
			}
		}

		private GameObject BuildOneInternal(Outfit outfit, Dictionary<string, Texture> faces)
		{
			string text = FacePrefix(outfit.Gender);
			faces.TryGetValue(text + Mathf.Max(0, outfit.Ids[1]), out var value);
			faces.TryGetValue(text + "N", out var value2);
			GameObject gameObject = new GameObject("[SteamPedestrianBuild]");
			gameObject.SetActive(value: false);
			MainPerson mainPerson = gameObject.AddComponent<MainPerson>();
			mainPerson.Gender = outfit.Gender;
			mainPerson.allKits = new GameObject[11];
			GameObject gameObject2 = null;
			try
			{
				_phase.Restart();
				gameObject2 = _characters.LoadPersonSkin(gameObject, outfit.Gender, randomSkin: true);
				SkinnedMeshRenderer skinnedMeshRender = mainPerson.SkinnedMeshRender;
				Animator component = gameObject.GetComponent<Animator>();
				_skinMs += _phase.Elapsed.TotalMilliseconds;
				if (!gameObject2 || !skinnedMeshRender || !component || !component.avatar)
				{
					return null;
				}
				_phase.Restart();
				for (int i = 0; i < 11; i++)
				{
					if (outfit.Ids[i] >= 0)
					{
						_characters.LoadPersonEquipment(gameObject, i, outfit.Ids[i]);
					}
				}
				_characters.LoadAllMask(gameObject);
				_kitsMs += _phase.Elapsed.TotalMilliseconds;
				_phase.Restart();
				List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer> { skinnedMeshRender };
				GameObject[] allKits = mainPerson.allKits;
				foreach (GameObject gameObject3 in allKits)
				{
					if ((bool)gameObject3 && gameObject3.activeSelf && gameObject3.TryGetComponent<SkinnedMeshRenderer>(out var component2) && component2.enabled && (bool)component2.sharedMesh)
					{
						list.Add(component2);
					}
				}
				Material material = new Material(mainPerson.mainMat)
				{
					name = outfit.Name + "_Body"
				};
				if ((bool)value)
				{
					material.SetTexture("_Albedo", value);
				}
				if ((bool)value2)
				{
					material.SetTexture("_Normals", value2);
				}
				_assets.Own(material);
				Mesh mesh = Combine(list, skinnedMeshRender, mainPerson.mainMat, material, outfit.Name, out var materials);
				_assets.Own(mesh);
				_combineMs += _phase.Elapsed.TotalMilliseconds;
				_phase.Restart();
				GameObject result = CreateTemplate(outfit, gameObject2, skinnedMeshRender, mesh, materials, component.avatar);
				_templateMs += _phase.Elapsed.TotalMilliseconds;
				return result;
			}
			finally
			{
				_phase.Restart();
				mainPerson.ik = null;
				if ((bool)gameObject2)
				{
					UnityEngine.Object.DestroyImmediate(gameObject2);
				}
				gameObject.SetActive(value: true);
				UnityEngine.Object.Destroy(gameObject);
				_cleanupMs += _phase.Elapsed.TotalMilliseconds;
			}
		}

		private GameObject CreateTemplate(Outfit outfit, GameObject skin, SkinnedMeshRenderer body, Mesh mesh, Material[] materials, Avatar avatar)
		{
			GameObject gameObject = Resources.Load<GameObject>((outfit.Gender == 1) ? "PeopleTrafficResources/Man_11" : "PeopleTrafficResources/Girl_12");
			SkinnedMeshRenderer componentInChildren = gameObject.GetComponentInChildren<SkinnedMeshRenderer>(includeInactive: true);
			GameObject gameObject2 = new GameObject(outfit.Name)
			{
				layer = gameObject.layer
			};
			gameObject2.transform.SetParent(_templates, worldPositionStays: false);
			CopyAnimator(gameObject.GetComponent<Animator>(), gameObject2.AddComponent<Animator>(), avatar, outfit.Gender);
			NavMeshAgent navMeshAgent = gameObject2.AddComponent<NavMeshAgent>();
			CopyAgent(gameObject.GetComponent<NavMeshAgent>(), navMeshAgent);
			navMeshAgent.enabled = false;
			JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(gameObject.GetComponent<Behaviours>()), gameObject2.AddComponent<Behaviours>());
			GameObject gameObject3 = UnityEngine.Object.Instantiate(CleanSkeleton(outfit.Gender, skin, body, out var bodyPath), gameObject2.transform);
			gameObject3.name = "Skin";
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			SkinnedMeshRenderer component = gameObject3.transform.Find(bodyPath).GetComponent<SkinnedMeshRenderer>();
			Bounds localBounds = component.localBounds;
			component.sharedMesh = mesh;
			component.sharedMaterials = materials;
			component.localBounds = new Bounds(localBounds.center, localBounds.size + Vector3.one * 0.4f);
			component.updateWhenOffscreen = false;
			component.shadowCastingMode = componentInChildren.shadowCastingMode;
			component.receiveShadows = componentInChildren.receiveShadows;
			component.lightProbeUsage = componentInChildren.lightProbeUsage;
			component.reflectionProbeUsage = componentInChildren.reflectionProbeUsage;
			component.skinnedMotionVectors = componentInChildren.skinnedMotionVectors;
			component.gameObject.layer = componentInChildren.gameObject.layer;
			component.gameObject.AddComponent<DeactivatePerson>();
			return gameObject2;
		}

		private GameObject CleanSkeleton(int gender, GameObject skin, SkinnedMeshRenderer body, out string bodyPath)
		{
			if (!_skeletons[gender])
			{
				_bodyPaths[gender] = RelativePath(skin.transform, body.transform);
				GameObject gameObject = UnityEngine.Object.Instantiate(skin, _templates);
				gameObject.name = "Skeleton_" + GenderTag(gender);
				StripComponents(gameObject, gameObject.transform.Find(_bodyPaths[gender]).GetComponent<SkinnedMeshRenderer>());
				_skeletons[gender] = gameObject;
			}
			bodyPath = _bodyPaths[gender];
			return _skeletons[gender];
		}

		private static string RelativePath(Transform root, Transform target)
		{
			List<string> list = new List<string>();
			Transform transform = target;
			while ((bool)transform && transform != root)
			{
				list.Insert(0, transform.name);
				transform = transform.parent;
			}
			return string.Join("/", list);
		}

		private static void CopyAnimator(Animator source, Animator target, Avatar avatar, int gender)
		{
			RuntimeAnimatorController runtimeAnimatorController = (SteamPerfFlags.IsOn("pedAnim") ? Resources.Load<RuntimeAnimatorController>((gender == 1) ? "PeopleTrafficResources/SteamPedestrianMale" : "PeopleTrafficResources/SteamPedestrianFemale") : null);
			target.runtimeAnimatorController = (runtimeAnimatorController ? runtimeAnimatorController : source.runtimeAnimatorController);
			target.avatar = avatar;
			target.applyRootMotion = source.applyRootMotion;
			target.cullingMode = source.cullingMode;
			target.updateMode = source.updateMode;
		}

		private static void CopyAgent(NavMeshAgent source, NavMeshAgent target)
		{
			target.agentTypeID = source.agentTypeID;
			target.baseOffset = source.baseOffset;
			target.speed = source.speed;
			target.angularSpeed = source.angularSpeed;
			target.acceleration = source.acceleration;
			target.stoppingDistance = source.stoppingDistance;
			target.autoBraking = source.autoBraking;
			target.radius = source.radius;
			target.height = source.height;
			target.obstacleAvoidanceType = source.obstacleAvoidanceType;
			target.avoidancePriority = source.avoidancePriority;
			target.autoTraverseOffMeshLink = source.autoTraverseOffMeshLink;
			target.autoRepath = source.autoRepath;
			target.areaMask = source.areaMask;
			target.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
			target.autoBraking = false;
		}

		private static void StripComponents(GameObject skin, SkinnedMeshRenderer keep)
		{
			MonoBehaviour[] componentsInChildren = skin.GetComponentsInChildren<MonoBehaviour>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i]);
			}
			Joint[] componentsInChildren2 = skin.GetComponentsInChildren<Joint>(includeInactive: true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren2[i]);
			}
			Component[] componentsInChildren3 = skin.GetComponentsInChildren<Component>(includeInactive: true);
			foreach (Component component in componentsInChildren3)
			{
				if ((bool)component && !(component is Transform) && component != keep)
				{
					UnityEngine.Object.DestroyImmediate(component);
				}
			}
		}

		private Mesh Combine(List<SkinnedMeshRenderer> parts, SkinnedMeshRenderer target, Material bodySource, Material bodyReplacement, string name, out Material[] materials)
		{
			Transform[] bones = target.bones;
			Dictionary<string, int> dictionary = new Dictionary<string, int>(bones.Length);
			for (int i = 0; i < bones.Length; i++)
			{
				if ((bool)bones[i] && !dictionary.ContainsKey(bones[i].name))
				{
					dictionary[bones[i].name] = i;
				}
			}
			int rootIndex = (((bool)target.rootBone && dictionary.TryGetValue(target.rootBone.name, out var value)) ? value : 0);
			Matrix4x4[] bindposes = target.sharedMesh.bindposes;
			int num = 0;
			foreach (SkinnedMeshRenderer part in parts)
			{
				num += part.sharedMesh.vertexCount;
			}
			MeshBuilder meshBuilder = new MeshBuilder(num);
			foreach (SkinnedMeshRenderer part2 in parts)
			{
				AppendPart(meshBuilder, part2, dictionary, rootIndex, bindposes);
			}
			_sub.Restart();
			Mesh mesh = new Mesh
			{
				name = name
			};
			int count = meshBuilder.Count;
			if (count > 65535)
			{
				mesh.indexFormat = IndexFormat.UInt32;
			}
			mesh.SetVertices(meshBuilder.Vertices, 0, count);
			mesh.SetNormals(meshBuilder.Normals, 0, count);
			mesh.SetTangents(meshBuilder.Tangents, 0, count);
			mesh.SetUVs(0, meshBuilder.Uv0, 0, count);
			if (meshBuilder.Weights.Length != count)
			{
				Array.Resize(ref meshBuilder.Weights, count);
			}
			mesh.boneWeights = meshBuilder.Weights;
			mesh.bindposes = bindposes;
			mesh.subMeshCount = meshBuilder.Triangles.Count;
			for (int j = 0; j < meshBuilder.Triangles.Count; j++)
			{
				mesh.SetTriangles(meshBuilder.Triangles[j], j);
			}
			mesh.RecalculateBounds();
			if (meshBuilder.MissingTangents)
			{
				mesh.RecalculateTangents();
				_recalculatedTangents++;
			}
			mesh.UploadMeshData(markNoLongerReadable: true);
			_meshMs += _sub.Elapsed.TotalMilliseconds;
			materials = meshBuilder.Materials.ToArray();
			for (int k = 0; k < materials.Length; k++)
			{
				if (materials[k] == bodySource)
				{
					materials[k] = bodyReplacement;
				}
			}
			return mesh;
		}

		private void AppendPart(MeshBuilder builder, SkinnedMeshRenderer part, Dictionary<string, int> boneIndex, int rootIndex, Matrix4x4[] targetBindposes)
		{
			_sub.Restart();
			Mesh sharedMesh = part.sharedMesh;
			BoneWeight[] boneWeights = sharedMesh.boneWeights;
			if (boneWeights.Length != sharedMesh.vertexCount)
			{
				return;
			}
			Matrix4x4[] bindposes = sharedMesh.bindposes;
			Transform[] bones = part.bones;
			int[] array = new int[bindposes.Length];
			Matrix4x4[] array2 = new Matrix4x4[bindposes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = ((i < bones.Length) ? bones[i] : null);
				while ((bool)transform && !boneIndex.ContainsKey(transform.name))
				{
					transform = transform.parent;
				}
				array[i] = (transform ? boneIndex[transform.name] : rootIndex);
				array2[i] = targetBindposes[array[i]].inverse * bindposes[i];
			}
			bool[] used = UsedBones(boneWeights, array2.Length);
			bool flag = IsUniform(array2, used, out var shared);
			bool flag2 = flag && Approximately(shared, Matrix4x4.identity);
			bool flag3 = IsInjective(array, used, targetBindposes.Length);
			if (flag2)
			{
				_identityParts++;
			}
			else if (flag)
			{
				_uniformParts++;
			}
			else
			{
				_blendParts++;
			}
			Vector3[] vertices = sharedMesh.vertices;
			Vector3[] normals = sharedMesh.normals;
			Vector4[] tangents = sharedMesh.tangents;
			BakeBlendShapes(part, sharedMesh, vertices, normals, tangents);
			Vector2[] uv = sharedMesh.uv;
			bool flag4 = normals.Length == vertices.Length;
			bool flag5 = tangents.Length == vertices.Length;
			bool flag6 = uv.Length == vertices.Length;
			builder.MissingTangents |= !flag5;
			_readMs += _sub.Elapsed.TotalMilliseconds;
			_sub.Restart();
			int count = builder.Count;
			Vector3[] vertices2 = builder.Vertices;
			Vector3[] normals2 = builder.Normals;
			Vector4[] tangents2 = builder.Tangents;
			Vector2[] uv2 = builder.Uv0;
			BoneWeight[] weights = builder.Weights;
			Vector4 vector = new Vector4(1f, 0f, 0f, 1f);
			for (int j = 0; j < vertices.Length; j++)
			{
				int num = count + j;
				BoneWeight w = boneWeights[j];
				float num2 = w.weight0 + w.weight1 + w.weight2 + w.weight3;
				if (num2 <= 0f)
				{
					w.boneIndex0 = 0;
					num2 = (w.weight0 = 1f);
				}
				Vector3 vector2 = (flag4 ? normals[j] : Vector3.up);
				if (flag2)
				{
					vertices2[num] = vertices[j];
					normals2[num] = vector2;
					tangents2[num] = (flag5 ? tangents[j] : vector);
				}
				else
				{
					Vector3 vector3 = (flag5 ? ((Vector3)tangents[j]) : Vector3.right);
					Vector3 position;
					Vector3 resultNormal;
					Vector3 resultTangent;
					if (flag)
					{
						position = shared.MultiplyPoint3x4(vertices[j]);
						resultNormal = shared.MultiplyVector(vector2);
						resultTangent = shared.MultiplyVector(vector3);
					}
					else
					{
						position = (resultNormal = (resultTangent = Vector3.zero));
						Accumulate(array2, w.boneIndex0, w.weight0 / num2, vertices[j], vector2, vector3, ref position, ref resultNormal, ref resultTangent);
						Accumulate(array2, w.boneIndex1, w.weight1 / num2, vertices[j], vector2, vector3, ref position, ref resultNormal, ref resultTangent);
						Accumulate(array2, w.boneIndex2, w.weight2 / num2, vertices[j], vector2, vector3, ref position, ref resultNormal, ref resultTangent);
						Accumulate(array2, w.boneIndex3, w.weight3 / num2, vertices[j], vector2, vector3, ref position, ref resultNormal, ref resultTangent);
					}
					vertices2[num] = position;
					normals2[num] = resultNormal.normalized;
					resultTangent = resultTangent.normalized;
					tangents2[num] = (flag5 ? new Vector4(resultTangent.x, resultTangent.y, resultTangent.z, tangents[j].w) : vector);
				}
				uv2[num] = (flag6 ? uv[j] : Vector2.zero);
				if (flag3)
				{
					float num4 = 1f / num2;
					weights[num] = new BoneWeight
					{
						boneIndex0 = array[w.boneIndex0],
						weight0 = w.weight0 * num4,
						boneIndex1 = array[w.boneIndex1],
						weight1 = w.weight1 * num4,
						boneIndex2 = array[w.boneIndex2],
						weight2 = w.weight2 * num4,
						boneIndex3 = array[w.boneIndex3],
						weight3 = w.weight3 * num4
					};
				}
				else
				{
					weights[num] = RemapWeight(w, array);
				}
			}
			builder.Count += vertices.Length;
			Material[] sharedMaterials = part.sharedMaterials;
			for (int k = 0; k < sharedMesh.subMeshCount; k++)
			{
				Material item = ((sharedMaterials.Length == 0) ? null : sharedMaterials[Mathf.Min(k, sharedMaterials.Length - 1)]);
				int num5 = builder.Materials.IndexOf(item);
				if (num5 < 0)
				{
					num5 = builder.Materials.Count;
					builder.Materials.Add(item);
					builder.Triangles.Add(new List<int>());
				}
				int[] triangles = sharedMesh.GetTriangles(k);
				for (int l = 0; l < triangles.Length; l++)
				{
					triangles[l] += count;
				}
				builder.Triangles[num5].AddRange(triangles);
			}
			_loopMs += _sub.Elapsed.TotalMilliseconds;
		}

		private static bool[] UsedBones(BoneWeight[] weights, int boneCount)
		{
			bool[] array = new bool[boneCount];
			for (int i = 0; i < weights.Length; i++)
			{
				BoneWeight boneWeight = weights[i];
				if (boneWeight.weight0 > 0f && boneWeight.boneIndex0 < boneCount)
				{
					array[boneWeight.boneIndex0] = true;
				}
				if (boneWeight.weight1 > 0f && boneWeight.boneIndex1 < boneCount)
				{
					array[boneWeight.boneIndex1] = true;
				}
				if (boneWeight.weight2 > 0f && boneWeight.boneIndex2 < boneCount)
				{
					array[boneWeight.boneIndex2] = true;
				}
				if (boneWeight.weight3 > 0f && boneWeight.boneIndex3 < boneCount)
				{
					array[boneWeight.boneIndex3] = true;
				}
			}
			return array;
		}

		private static bool IsUniform(Matrix4x4[] toTarget, bool[] used, out Matrix4x4 shared)
		{
			shared = Matrix4x4.identity;
			bool flag = false;
			for (int i = 0; i < toTarget.Length; i++)
			{
				if (used[i])
				{
					if (!flag)
					{
						shared = toTarget[i];
						flag = true;
					}
					else if (!Approximately(toTarget[i], shared))
					{
						return false;
					}
				}
			}
			return flag;
		}

		private static bool IsInjective(int[] map, bool[] used, int targetCount)
		{
			bool[] array = new bool[targetCount];
			for (int i = 0; i < map.Length; i++)
			{
				if (used[i])
				{
					if (map[i] < 0 || map[i] >= targetCount || array[map[i]])
					{
						return false;
					}
					array[map[i]] = true;
				}
			}
			return true;
		}

		private static bool Approximately(Matrix4x4 a, Matrix4x4 b)
		{
			for (int i = 0; i < 16; i++)
			{
				if (Mathf.Abs(a[i] - b[i]) > 0.0001f * (1f + Mathf.Abs(a[i])))
				{
					return false;
				}
			}
			return true;
		}

		private static void Accumulate(Matrix4x4[] toTarget, int bone, float weight, Vector3 vertex, Vector3 normal, Vector3 tangent, ref Vector3 position, ref Vector3 resultNormal, ref Vector3 resultTangent)
		{
			if (!(weight <= 0f) && bone >= 0 && bone < toTarget.Length)
			{
				Matrix4x4 matrix4x = toTarget[bone];
				position += matrix4x.MultiplyPoint3x4(vertex) * weight;
				resultNormal += matrix4x.MultiplyVector(normal) * weight;
				resultTangent += matrix4x.MultiplyVector(tangent) * weight;
			}
		}

		private static BoneWeight RemapWeight(BoneWeight w, int[] map)
		{
			int ia = Map(w.boneIndex0, map);
			int ib = Map(w.boneIndex1, map);
			int ia2 = Map(w.boneIndex2, map);
			int ib2 = Map(w.boneIndex3, map);
			float target = w.weight0;
			float source = w.weight1;
			float source2 = w.weight2;
			float source3 = w.weight3;
			Merge(ref target, ia, ref source, ib);
			Merge(ref target, ia, ref source2, ia2);
			Merge(ref source, ib, ref source2, ia2);
			Merge(ref target, ia, ref source3, ib2);
			Merge(ref source, ib, ref source3, ib2);
			Merge(ref source2, ia2, ref source3, ib2);
			Order(ref target, ref ia, ref source, ref ib);
			Order(ref source2, ref ia2, ref source3, ref ib2);
			Order(ref target, ref ia, ref source2, ref ia2);
			Order(ref source, ref ib, ref source3, ref ib2);
			Order(ref source, ref ib, ref source2, ref ia2);
			float num = target + source + source2 + source3;
			if (num <= 0f)
			{
				return new BoneWeight
				{
					boneIndex0 = ia,
					weight0 = 1f
				};
			}
			return new BoneWeight
			{
				boneIndex0 = ia,
				weight0 = target / num,
				boneIndex1 = ib,
				weight1 = source / num,
				boneIndex2 = ia2,
				weight2 = source2 / num,
				boneIndex3 = ib2,
				weight3 = source3 / num
			};
		}

		private static void Merge(ref float target, int targetIndex, ref float source, int sourceIndex)
		{
			if (!(source <= 0f) && sourceIndex == targetIndex)
			{
				target += source;
				source = 0f;
			}
		}

		private static void Order(ref float wa, ref int ia, ref float wb, ref int ib)
		{
			if (!(wb <= wa))
			{
				float num = wb;
				float num2 = wa;
				wa = num;
				wb = num2;
				int num3 = ib;
				int num4 = ia;
				ia = num3;
				ib = num4;
			}
		}

		private static int Map(int bone, int[] map)
		{
			if (bone < 0 || bone >= map.Length)
			{
				return 0;
			}
			return map[bone];
		}

		private static void BakeBlendShapes(SkinnedMeshRenderer part, Mesh source, Vector3[] vertices, Vector3[] normals, Vector4[] tangents)
		{
			if (source.blendShapeCount == 0)
			{
				return;
			}
			Vector3[] array = new Vector3[vertices.Length];
			Vector3[] array2 = new Vector3[vertices.Length];
			Vector3[] array3 = new Vector3[vertices.Length];
			for (int i = 0; i < source.blendShapeCount; i++)
			{
				float blendShapeWeight = part.GetBlendShapeWeight(i);
				if (blendShapeWeight <= 0f)
				{
					continue;
				}
				int frameIndex = source.GetBlendShapeFrameCount(i) - 1;
				float blendShapeFrameWeight = source.GetBlendShapeFrameWeight(i, frameIndex);
				float num = ((blendShapeFrameWeight > 0f) ? (blendShapeWeight / blendShapeFrameWeight) : 0f);
				source.GetBlendShapeFrameVertices(i, frameIndex, array, array2, array3);
				for (int j = 0; j < vertices.Length; j++)
				{
					vertices[j] += array[j] * num;
					if (normals.Length == vertices.Length)
					{
						normals[j] += array2[j] * num;
					}
					if (tangents.Length == vertices.Length)
					{
						tangents[j] += (Vector4)(array3[j] * num);
					}
				}
			}
		}
	}
}

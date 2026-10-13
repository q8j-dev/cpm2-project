using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Database;
using CPM.PlayerData;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Rendering;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Car.CarSetter
{
	public class CharactersService
	{
		private const string PersonRoot = "MenuPerson/";

		private const string MaleAlphaMaskPath = "MaleKit/Alpha_Mask/";

		private const string FemaleAlphaMaskPath = "FemaleKit/Alpha_Mask/";

		private const string MaleSkinPath = "MenuPerson/MaleKit/body/Body_Male";

		private const string FemaleSkinPath = "MenuPerson/FemaleKit/body/Body_Female";

		private const string SubPathShoesAlpha = "Shoes/";

		private const string SubPathTopAlpha = "Top/";

		private const string SubPathPantsAlpha = "Pants/";

		private const string MaleSkinTexturePath = "Body_Male_";

		private const string FemaleSkinTexturePath = "Body_Female_";

		private ICharacterEquipmentDataService _characterEquipmentDataService;

		private PersonKitConfig _malePersonKitConfig;

		private PersonKitConfig _femalePersonKitConfig;

		private IPrefabFactory _prefabFactory;

		private IPersonLocomotionConfigService _personLocomotionConfigService;

		private IForcedOutfitProvider _forcedOutfit;

		private static List<int> AsianHeadCapsNotBald = new List<int>
		{
			0, 1, 2, 11, 12, 13, 14, 16, 17, 24,
			26, 28, 29, 30, 31, 32
		};

		private const string BaseMap = "_Albedo";

		private const string NormalMap = "_Normals";

		public static bool SteamSkipSkinTexture;

		public CharactersService(ICharacterEquipmentDataService characterEquipmentDataService, PersonKitConfigs personKitConfigs, IPrefabFactory prefabFactory, IPersonLocomotionConfigService personLocomotionConfigService, IForcedOutfitProvider forcedOutfit)
		{
			_characterEquipmentDataService = characterEquipmentDataService;
			_malePersonKitConfig = personKitConfigs.Male;
			_femalePersonKitConfig = personKitConfigs.Female;
			_prefabFactory = prefabFactory;
			_personLocomotionConfigService = personLocomotionConfigService;
			_forcedOutfit = forcedOutfit;
		}

		public GameObject LoadPersonSkin(GameObject person, int gender, bool randomSkin = false)
		{
			MainPerson component = person.GetComponent<MainPerson>();
			MultiPersonControl component2 = person.GetComponent<MultiPersonControl>();
			DriverActionController component3 = person.GetComponent<DriverActionController>();
			NewMoveController component4 = person.GetComponent<NewMoveController>();
			PersonLocomotionConfig personLocomotionConfig = _personLocomotionConfigService?.Locomotion;
			bool flag = !randomSkin && person == V.DriverPerson && personLocomotionConfig != null && personLocomotionConfig.UseMCUnityCharacter && personLocomotionConfig.MCUnityCharacterPrefab != null;
			if ((bool)component)
			{
				component.IsMCUnitySkin = flag;
			}
			GameObject gameObject = (flag ? personLocomotionConfig.MCUnityCharacterPrefab : Resources.Load<GameObject>((gender == 1) ? "MenuPerson/MaleKit/body/Body_Male" : "MenuPerson/FemaleKit/body/Body_Female"));
			if (!gameObject)
			{
				return null;
			}
			GameObject gameObject2 = _prefabFactory.Instantiate(gameObject, person.transform);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			component.SkinnedMeshRender = gameObject2.GetComponentInChildren<SkinnedMeshRenderer>();
			component.SkinnedMeshRender.lightProbeUsage = LightProbeUsage.BlendProbes;
			component.mainMat = createMainSkinMat(component.SkinnedMeshRender);
			component.DestroyMat = true;
			Animator component5 = gameObject2.GetComponent<Animator>();
			Animator animator = person.GetComponent<Animator>();
			SwitchHeadController component6 = gameObject2.GetComponent<SwitchHeadController>();
			if (!randomSkin)
			{
				PersonAnimationsController personAnimationsController = (component ? component.PersonAnimationsController : null);
				if (!animator)
				{
					animator = person.AddComponent<Animator>();
					if (!personAnimationsController)
					{
						personAnimationsController = (UnityEngine.Object.Instantiate(Resources.Load("PersonAnimationsController/PersonAnimationsController"), person.transform) as GameObject).GetComponent<PersonAnimationsController>();
						personAnimationsController.Init(component4);
						if (person == V.DriverPerson && (bool)V.Car && V.Car.TryGetComponent<CarAnimationsController>(out var component7))
						{
							personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentCar = component7;
						}
					}
					personAnimationsController.Animator = animator;
					if ((bool)component5.runtimeAnimatorController)
					{
						personAnimationsController.Animator.runtimeAnimatorController = component5.runtimeAnimatorController;
					}
				}
				component.ik = gameObject2.GetComponent<FullBodyBipedIK>();
				if ((bool)component2)
				{
					component2.SubscribeIkHead();
				}
				component3.PersonAnimationsController = personAnimationsController;
				personAnimationsController.Animator.avatar = component5.avatar;
				personAnimationsController.Animator.cullingMode = ((!(person == V.DriverPerson)) ? AnimatorCullingMode.CullCompletely : AnimatorCullingMode.AlwaysAnimate);
				personAnimationsController.Animator.applyRootMotion = true;
				component.PersonAnimationsController = personAnimationsController;
				if ((bool)component2)
				{
					component2.PersonAnimationsController = personAnimationsController;
				}
				component.SwitchHeadController = component6;
			}
			else
			{
				if (!animator)
				{
					animator = person.AddComponent<Animator>();
					if ((bool)component5.runtimeAnimatorController)
					{
						animator.runtimeAnimatorController = component5.runtimeAnimatorController;
					}
				}
				component.ik = gameObject2.GetComponent<FullBodyBipedIK>();
				if ((bool)component2)
				{
					component2.SubscribeIkHead();
				}
				animator.avatar = component5.avatar;
				animator.cullingMode = ((!(person == V.DriverPerson)) ? AnimatorCullingMode.CullCompletely : AnimatorCullingMode.AlwaysAnimate);
				animator.applyRootMotion = true;
				component.Animator = animator;
				if ((bool)component3)
				{
					component3.Animator = animator;
				}
			}
			if (FlagsStorage.IsLevelScene)
			{
				((!component.PersonAnimationsController) ? component.Animator : component.PersonAnimationsController.Animator).cullingMode = AnimatorCullingMode.AlwaysAnimate;
			}
			UnityEngine.Object.Destroy(component5);
			if ((bool)component3)
			{
				component3.PersonAnimationsController = component.PersonAnimationsController;
				component3.ik = component.ik;
			}
			if ((bool)component4)
			{
				component4.PersonAnimationsController = component.PersonAnimationsController;
			}
			gameObject2.layer = 17;
			if ((bool)component2 && (bool)component2.rendererCheck)
			{
				UnityEngine.Object.Destroy(component2.rendererCheck);
				component2.rendererCheck = null;
			}
			return gameObject2;
		}

		public async UniTask PrefetchPersonAssetsAsync(int gender, bool driverOutfit, CancellationToken ct)
		{
			PersonLocomotionConfig personLocomotionConfig = _personLocomotionConfigService?.Locomotion;
			if (driverOutfit && personLocomotionConfig != null && personLocomotionConfig.UseMCUnityCharacter && personLocomotionConfig.MCUnityCharacterPrefab != null)
			{
				return;
			}
			List<ResourceRequest> list = new List<ResourceRequest> { Resources.LoadAsync<GameObject>((gender == 1) ? "MenuPerson/MaleKit/body/Body_Male" : "MenuPerson/FemaleKit/body/Body_Female") };
			if (driverOutfit)
			{
				list.Add(Resources.LoadAsync("PersonAnimationsController/PersonAnimationsController"));
				EquipmentsData equipmentsData = GetEquipmentsData(gender);
				PersonKitConfig personKitConfig = ((gender == 1) ? _malePersonKitConfig : _femalePersonKitConfig);
				string arg = "MenuPerson/" + ((gender == 1) ? "MaleKit/Alpha_Mask/" : "FemaleKit/Alpha_Mask/");
				for (int i = 0; i < equipmentsData.SelectedEquipments.Count; i++)
				{
					int num = ((_forcedOutfit != null && _forcedOutfit.TryGetForcedKit(gender, i, out var kitId)) ? kitId : equipmentsData.SelectedEquipments[i]);
					if (num < 0)
					{
						continue;
					}
					list.Add(Resources.LoadAsync<GameObject>($"{GetPathForKit(gender, i)}skins/{num}_skin"));
					if ((i == 5 || i == 8 || i == 9) && i < personKitConfig.allKits.Length && personKitConfig.allKits[i] != null && num < personKitConfig.allKits[i].Length)
					{
						int mainBodyMask = personKitConfig.allKits[i][num].MainBodyMask;
						if (mainBodyMask != 0)
						{
							list.Add(Resources.LoadAsync<Texture>(string.Format("{0}{1}{2}", arg, i switch
							{
								5 => "Top/", 
								8 => "Pants/", 
								_ => "Shoes/", 
							}, mainBodyMask)));
						}
					}
				}
			}
			foreach (ResourceRequest item in list)
			{
				await item.ToUniTask(null, PlayerLoopTiming.Update, ct);
			}
		}

		public static Material createMainSkinMat(SkinnedMeshRenderer skinnedMeshRenderer)
		{
			Material material = new Material(skinnedMeshRenderer.material);
			material.SetFloat("_Smoothness", 0f);
			skinnedMeshRenderer.material = material;
			material.SetTexture("_Mask_Tors", null);
			material.SetTexture("_Mask_Pens", null);
			material.SetTexture("_Mask_Shoes", null);
			return material;
		}

		private static void TryInitBlendShapesByFace(SkinnedMeshRenderer skinnedMeshRenderer, int groupId)
		{
			for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
			{
				skinnedMeshRenderer.SetBlendShapeWeight(i, (i == groupId) ? 100 : 0);
			}
		}

		private static void UpdateBlendShapesByFace(MainPerson mainPerson, int faceID)
		{
			GameObject[] allKits = mainPerson.allKits;
			int blendShapeIndexByFace = GetBlendShapeIndexByFace(mainPerson.Gender, faceID);
			GameObject[] array = allKits;
			foreach (GameObject gameObject in array)
			{
				if ((bool)gameObject && gameObject.TryGetComponent<SkinnedMeshRenderer>(out var component))
				{
					TryInitBlendShapesByFace(component, blendShapeIndexByFace);
				}
			}
		}

		private static int GetBlendShapeIndexByFace(int gender, int faceID)
		{
			int result;
			if (gender == 1)
			{
				switch (faceID)
				{
				case 1:
				case 4:
				case 5:
					result = 10;
					break;
				case 3:
				case 6:
					result = 0;
					break;
				case 0:
				case 2:
					result = 1;
					break;
				default:
					result = 10;
					break;
				}
				return result;
			}
			switch (faceID)
			{
			case 0:
			case 1:
			case 2:
			case 4:
				result = 10;
				break;
			case 3:
			case 5:
				result = 0;
				break;
			case 6:
				result = 1;
				break;
			default:
				result = 10;
				break;
			}
			return result;
		}

		public GameObject LoadPersonEquipment(GameObject person, int typeID, int kitID, bool removeClosingCapOnHair = false)
		{
			MainPerson component = person.GetComponent<MainPerson>();
			if ((bool)component && component.IsMCUnitySkin)
			{
				return null;
			}
			PersonKitConfig personKitConfig = (((int)component.Gender == 0) ? _femalePersonKitConfig : _malePersonKitConfig);
			EquipmentsData equipmentsData = GetEquipmentsData(component.Gender);
			if (!component.SkinnedMeshRender)
			{
				return null;
			}
			if ((bool)component.allKits[typeID])
			{
				UnityEngine.Object.Destroy(component.allKits[typeID]);
				component.allKits[typeID] = null;
			}
			switch (typeID)
			{
			case 6:
			case 8:
			case 9:
				if (!component.allKits[5])
				{
					break;
				}
				switch (component.allKits[5].GetComponent<Equipment>().pack)
				{
				case PersonElements.Pack.noPants:
					if (typeID != 8)
					{
						break;
					}
					goto case PersonElements.Pack.police;
				case PersonElements.Pack.noPantsShoes:
					if (typeID != 8 && typeID != 9)
					{
						break;
					}
					goto case PersonElements.Pack.police;
				case PersonElements.Pack.noPantsGloves:
					if (typeID != 8 && typeID != 6)
					{
						break;
					}
					goto case PersonElements.Pack.police;
				case PersonElements.Pack.police:
					return null;
				}
				break;
			case 0:
			{
				if ((bool)component.allKits[5])
				{
					int hairToSet = component.allKits[5].GetComponent<Equipment>().hairToSet;
					switch (hairToSet)
					{
					case -2:
						return null;
					default:
						kitID = hairToSet;
						break;
					case -1:
						break;
					}
				}
				if (!component.allKits[3])
				{
					break;
				}
				int hairToSet2 = component.allKits[3].GetComponent<Equipment>().hairToSet;
				if (removeClosingCapOnHair)
				{
					if (hairToSet2 != kitID)
					{
						UnityEngine.Object.Destroy(component.allKits[3]);
						component.allKits[3] = null;
						RestoreHairHiddenByHat(component);
						equipmentsData.SetSelectedItem(3, -1);
						Save();
					}
				}
				else if (hairToSet2 == -2)
				{
					return null;
				}
				break;
			}
			case 3:
				if ((bool)component.allKits[5] && component.allKits[5].GetComponent<Equipment>().hairToSet != -1)
				{
					return null;
				}
				break;
			}
			GameObject gameObject = Resources.Load<GameObject>($"{GetPathForKit(component.Gender, typeID)}skins/{kitID}_skin");
			if (!gameObject)
			{
				_ = -2;
				return null;
			}
			if ((bool)personKitConfig && !HasKitConfigEntry(personKitConfig, typeID, kitID))
			{
				return null;
			}
			SkinnedMeshRenderer componentInChildren = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
			if ((bool)componentInChildren)
			{
				componentInChildren.probeAnchor = component.ik.references.pelvis;
				SkinnedMeshRenderer skinnedMeshRenderer = UnityEngine.Object.Instantiate(componentInChildren, person.transform);
				if (typeID == 1)
				{
					UpdateBlendShapesByFace(component, kitID);
				}
				else if ((bool)component.allKits[1])
				{
					int id = component.allKits[1].GetComponent<Equipment>().id;
					int blendShapeIndexByFace = GetBlendShapeIndexByFace(component.Gender, id);
					TryInitBlendShapesByFace(skinnedMeshRenderer, blendShapeIndexByFace);
				}
				Equipment equipment = skinnedMeshRenderer.gameObject.AddComponent<Equipment>();
				component.allKits[typeID] = equipment.gameObject;
				skinnedMeshRenderer.gameObject.layer = 17;
				equipment.skinnedMeshRendererPerson = component.SkinnedMeshRender;
				equipment.typeID = typeID;
				equipment.id = kitID;
				skinnedMeshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
				if ((bool)personKitConfig)
				{
					equipment.HidesBodyCompletely = personKitConfig.allKits[typeID][kitID].HidesBodyCompletely;
					equipment.pack = personKitConfig.allKits[typeID][kitID].pack;
					equipment.role = personKitConfig.allKits[typeID][kitID].role;
					equipment.hairToSet = personKitConfig.allKits[typeID][kitID].hairToSet;
					equipment.HideBeard = personKitConfig.allKits[typeID][kitID].HideBeard;
					skinnedMeshRenderer.gameObject.name = getKitName(typeID);
					if ((int)component.Gender == 1 && typeID == 0 && (bool)component.allKits[0])
					{
						int id2 = component.allKits[0].GetComponent<Equipment>().id;
						if (component.HairsToIgnore.Contains(id2))
						{
							SkinnedMeshRenderer component2 = skinnedMeshRenderer.GetComponent<SkinnedMeshRenderer>();
							Transform transform = gameObject.transform.Find("Male_Hair_07");
							if ((bool)transform)
							{
								SkinnedMeshRenderer skinnedMeshRenderer2 = null;
								for (int i = 0; i < 2; i++)
								{
									skinnedMeshRenderer2 = transform.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
									if ((bool)skinnedMeshRenderer2)
									{
										break;
									}
								}
								equipment.originalMesh = component2.sharedMesh;
								equipment.defaultMesh = skinnedMeshRenderer2.sharedMesh;
								equipment.originalMats = component2.sharedMaterials;
								equipment.defaultMats = skinnedMeshRenderer2.sharedMaterials;
							}
						}
					}
					if ((int)component.Gender == 1 && typeID == 1 && (bool)component.allKits[1] && component.allKits[1].GetComponent<Equipment>().id == 2)
					{
						SkinnedMeshRenderer component3 = skinnedMeshRenderer.GetComponent<SkinnedMeshRenderer>();
						Transform transform2 = gameObject.transform.Find("Asian_head_default");
						if ((bool)transform2)
						{
							SkinnedMeshRenderer component4 = transform2.transform.GetComponent<SkinnedMeshRenderer>();
							equipment.originalMats = component3.sharedMaterials;
							equipment.defaultMats = component4.sharedMaterials;
						}
					}
				}
				switch (typeID)
				{
				case 5:
				case 8:
				case 9:
					if (typeID != 5)
					{
						break;
					}
					component.equipmentRole = equipment.role;
					if (equipment.pack == PersonElements.Pack.police)
					{
						if ((bool)component.allKits[8])
						{
							UnityEngine.Object.Destroy(component.allKits[8]);
							component.allKits[8] = null;
						}
						if ((bool)component.allKits[9])
						{
							UnityEngine.Object.Destroy(component.allKits[9]);
							component.allKits[9] = null;
						}
						if ((bool)component.allKits[6])
						{
							UnityEngine.Object.Destroy(component.allKits[6]);
							component.allKits[6] = null;
						}
					}
					else if (equipment.pack == PersonElements.Pack.noPants)
					{
						if ((bool)component.allKits[8])
						{
							UnityEngine.Object.Destroy(component.allKits[8]);
							component.allKits[8] = null;
						}
					}
					else if (equipment.pack == PersonElements.Pack.noPantsShoes)
					{
						if ((bool)component.allKits[8])
						{
							UnityEngine.Object.Destroy(component.allKits[8]);
							component.allKits[8] = null;
						}
						if ((bool)component.allKits[9])
						{
							UnityEngine.Object.Destroy(component.allKits[9]);
							component.allKits[9] = null;
						}
					}
					else if (equipment.pack == PersonElements.Pack.noPantsGloves)
					{
						if ((bool)component.allKits[8])
						{
							UnityEngine.Object.Destroy(component.allKits[8]);
							component.allKits[8] = null;
						}
						if ((bool)component.allKits[6])
						{
							UnityEngine.Object.Destroy(component.allKits[6]);
							component.allKits[6] = null;
						}
					}
					break;
				case 1:
					if (kitID < 0)
					{
						kitID = 0;
					}
					if (!SteamSkipSkinTexture)
					{
						LoadSkinTextureAsync(component, kitID).Forget();
					}
					if ((bool)component.allKits[1] && component.allKits[1].GetComponent<Equipment>().id == 2)
					{
						if ((bool)component.allKits[3])
						{
							int id3 = component.allKits[3].GetComponent<Equipment>().id;
							SetAsianHead(component, !AsianHeadCapsNotBald.Contains(id3));
						}
						else
						{
							SetAsianHead(component, makeBald: true);
						}
					}
					break;
				}
				switch (typeID)
				{
				case 5:
				{
					int hairToSet4 = personKitConfig.allKits[5][kitID].hairToSet;
					if ((bool)component.allKits[3] && (hairToSet4 == -2 || hairToSet4 != -1) && (bool)component.allKits[3])
					{
						UnityEngine.Object.Destroy(component.allKits[3]);
						component.allKits[3] = null;
						RestoreHairHiddenByHat(component);
					}
					switch (hairToSet4)
					{
					case -2:
						UnityEngine.Object.Destroy(component.allKits[0]);
						component.allKits[0] = null;
						break;
					default:
						if ((bool)component.allKits[0])
						{
							if (component.allKits[0].GetComponent<Equipment>().id != hairToSet4)
							{
								LoadPersonEquipment(person, 0, hairToSet4);
							}
						}
						else
						{
							LoadPersonEquipment(person, 0, hairToSet4);
						}
						break;
					case -1:
						if (hairToSet4 == -1 && FlagsStorage.IsMainMenu)
						{
							int num2 = equipmentsData.SelectedEquipments[0];
							if (!component.allKits[0] && num2 != -1)
							{
								LoadPersonEquipment(person, 0, num2);
							}
						}
						break;
					}
					break;
				}
				case 3:
				{
					GameObject gameObject2 = component.allKits[0];
					if ((bool)personKitConfig && (bool)gameObject2)
					{
						int hairToSet3 = personKitConfig.allKits[3][kitID].hairToSet;
						int id4 = gameObject2.GetComponent<Equipment>().id;
						if (PersonElements.CapNeedHideInCar(component.Gender, kitID))
						{
							gameObject2.SetActive(value: false);
							component.HairHiddenByHat = true;
						}
						else
						{
							RestoreHairHiddenByHat(component);
							switch (hairToSet3)
							{
							case -2:
								UnityEngine.Object.Destroy(gameObject2);
								component.allKits[0] = null;
								break;
							default:
								if (id4 != hairToSet3)
								{
									LoadPersonEquipment(person, 0, hairToSet3);
								}
								break;
							case -1:
								if (component == V.DriverPerson)
								{
									int num = equipmentsData.SelectedEquipments[0];
									if (id4 != num)
									{
										LoadPersonEquipment(person, 0, num);
									}
								}
								break;
							}
						}
					}
					int id5 = component.allKits[3].GetComponent<Equipment>().id;
					SetAsianHead(component, !AsianHeadCapsNotBald.Contains(id5));
					if ((bool)component.allKits[2] && (bool)personKitConfig)
					{
						bool hideBeard = personKitConfig.allKits[3][kitID].HideBeard;
						component.allKits[2].SetActive(!hideBeard);
					}
					break;
				}
				case 4:
					if ((bool)component.allKits[2] && (bool)personKitConfig)
					{
						if (personKitConfig.allKits[4][kitID].HideBeard)
						{
							component.allKits[2].SetActive(value: false);
						}
						else
						{
							component.allKits[2].SetActive(value: true);
						}
					}
					break;
				}
				return component.allKits[typeID];
			}
			Debug.Log($"renderer of kit didn't find: {typeID},{kitID}");
			return null;
		}

		public void SetAsianHead(MainPerson mainPerson, bool makeBald)
		{
			if ((int)mainPerson.Gender != 1 || !mainPerson.allKits[1])
			{
				return;
			}
			Equipment component = mainPerson.allKits[1].GetComponent<Equipment>();
			if (component.id != 2)
			{
				return;
			}
			try
			{
				Material[] array = (makeBald ? component.originalMats : component.defaultMats);
				if (array != null)
				{
					mainPerson.allKits[1].GetComponent<SkinnedMeshRenderer>().materials = array;
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
		}

		public static void RestoreHairHiddenByHat(MainPerson mainPerson)
		{
			if ((bool)mainPerson && mainPerson.HairHiddenByHat)
			{
				mainPerson.HairHiddenByHat = false;
				if (mainPerson.allKits != null && mainPerson.allKits.Length != 0 && (bool)mainPerson.allKits[0])
				{
					mainPerson.allKits[0].SetActive(value: true);
				}
			}
		}

		public async UniTask<GameObject> LoadPersonEquipmentAsync(GameObject person, int typeID, int kitID)
		{
			MainPerson mainPerson = person.GetComponent<MainPerson>();
			PersonKitConfig personElements = (((int)mainPerson.Gender == 0) ? _femalePersonKitConfig : _malePersonKitConfig);
			EquipmentsData currentEquipment = GetEquipmentsData(mainPerson.Gender);
			if (!mainPerson.SkinnedMeshRender)
			{
				return null;
			}
			if ((bool)mainPerson.allKits[typeID])
			{
				UnityEngine.Object.Destroy(mainPerson.allKits[typeID]);
				mainPerson.allKits[typeID] = null;
			}
			if ((typeID == 8 || typeID == 9 || typeID == 6) && (bool)mainPerson.allKits[5])
			{
				Equipment component = mainPerson.allKits[5].GetComponent<Equipment>();
				if (component.pack == PersonElements.Pack.police)
				{
					return null;
				}
				if (component.pack == PersonElements.Pack.noPants && typeID == 8)
				{
					return null;
				}
				if ((component.pack == PersonElements.Pack.noPantsShoes && typeID == 8) || (component.pack == PersonElements.Pack.noPantsShoes && typeID == 9))
				{
					return null;
				}
				if ((component.pack == PersonElements.Pack.noPantsGloves && typeID == 8) || (component.pack == PersonElements.Pack.noPantsGloves && typeID == 6))
				{
					return null;
				}
			}
			if (typeID == 0)
			{
				if ((bool)mainPerson.allKits[5])
				{
					int hairToSet = mainPerson.allKits[5].GetComponent<Equipment>().hairToSet;
					switch (hairToSet)
					{
					case -2:
						return null;
					default:
						kitID = hairToSet;
						break;
					case -1:
						break;
					}
				}
				if ((bool)mainPerson.allKits[3] && mainPerson.allKits[3].GetComponent<Equipment>().hairToSet != kitID)
				{
					UnityEngine.Object.Destroy(mainPerson.allKits[3]);
					mainPerson.allKits[3] = null;
					RestoreHairHiddenByHat(mainPerson);
					currentEquipment.SetSelectedItem(3, -1);
					Save();
				}
			}
			if (typeID == 3 && (bool)mainPerson.allKits[5] && mainPerson.allKits[5].GetComponent<Equipment>().hairToSet != -1)
			{
				return null;
			}
			GameObject gameObject = (await Resources.LoadAsync<GameObject>(GetPathForKit(mainPerson.Gender, typeID) + "skins/" + kitID + "_skin")) as GameObject;
			if (!gameObject)
			{
				Debug.Log($"kit didn't find: {typeID},{kitID}");
				return null;
			}
			if ((bool)personElements && !HasKitConfigEntry(personElements, typeID, kitID))
			{
				return null;
			}
			SkinnedMeshRenderer componentInChildren = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
			if (!componentInChildren)
			{
				Debug.Log($"renderer of kit didn't find: {typeID},{kitID}");
				return null;
			}
			SkinnedMeshRenderer skinnedMeshRenderer = UnityEngine.Object.Instantiate(componentInChildren, person.transform);
			Equipment equipment = skinnedMeshRenderer.gameObject.AddComponent<Equipment>();
			mainPerson.allKits[typeID] = equipment.gameObject;
			skinnedMeshRenderer.gameObject.layer = 17;
			equipment.skinnedMeshRendererPerson = mainPerson.SkinnedMeshRender;
			equipment.typeID = typeID;
			equipment.id = kitID;
			skinnedMeshRenderer.lightProbeUsage = LightProbeUsage.Off;
			if ((bool)personElements)
			{
				equipment.HidesBodyCompletely = personElements.allKits[typeID][kitID].HidesBodyCompletely;
				equipment.pack = personElements.allKits[typeID][kitID].pack;
				equipment.role = personElements.allKits[typeID][kitID].role;
				equipment.hairToSet = personElements.allKits[typeID][kitID].hairToSet;
				equipment.HideBeard = personElements.allKits[typeID][kitID].HideBeard;
				skinnedMeshRenderer.gameObject.name = getKitName(typeID);
			}
			if ((typeID == 5 || typeID == 8 || typeID == 9) && typeID == 5)
			{
				mainPerson.equipmentRole = equipment.role;
				if (equipment.pack == PersonElements.Pack.police)
				{
					if ((bool)mainPerson.allKits[8])
					{
						UnityEngine.Object.Destroy(mainPerson.allKits[8]);
						mainPerson.allKits[8] = null;
					}
					if ((bool)mainPerson.allKits[9])
					{
						UnityEngine.Object.Destroy(mainPerson.allKits[9]);
						mainPerson.allKits[9] = null;
					}
					if ((bool)mainPerson.allKits[6])
					{
						UnityEngine.Object.Destroy(mainPerson.allKits[6]);
						mainPerson.allKits[6] = null;
					}
				}
				else if (equipment.pack == PersonElements.Pack.noPants)
				{
					if ((bool)mainPerson.allKits[8])
					{
						UnityEngine.Object.Destroy(mainPerson.allKits[8]);
						mainPerson.allKits[8] = null;
					}
				}
				else if (equipment.pack == PersonElements.Pack.noPantsShoes)
				{
					if ((bool)mainPerson.allKits[8])
					{
						UnityEngine.Object.Destroy(mainPerson.allKits[8]);
						mainPerson.allKits[8] = null;
					}
					if ((bool)mainPerson.allKits[9])
					{
						UnityEngine.Object.Destroy(mainPerson.allKits[9]);
						mainPerson.allKits[9] = null;
					}
				}
				else if (equipment.pack == PersonElements.Pack.noPantsGloves)
				{
					if ((bool)mainPerson.allKits[8])
					{
						UnityEngine.Object.Destroy(mainPerson.allKits[8]);
						mainPerson.allKits[8] = null;
					}
					if ((bool)mainPerson.allKits[6])
					{
						UnityEngine.Object.Destroy(mainPerson.allKits[6]);
						mainPerson.allKits[6] = null;
					}
				}
			}
			if (typeID == 1)
			{
				if (kitID < 0)
				{
					kitID = 0;
				}
				await LoadSkinTextureAsync(mainPerson, kitID);
			}
			if (typeID == 5)
			{
				int hairToSet2 = personElements.allKits[5][kitID].hairToSet;
				if ((bool)mainPerson.allKits[3] && (hairToSet2 == -2 || hairToSet2 != -1) && (bool)mainPerson.allKits[3])
				{
					UnityEngine.Object.Destroy(mainPerson.allKits[3]);
					mainPerson.allKits[3] = null;
					RestoreHairHiddenByHat(mainPerson);
				}
				switch (hairToSet2)
				{
				case -2:
					UnityEngine.Object.Destroy(mainPerson.allKits[0]);
					mainPerson.allKits[0] = null;
					break;
				default:
					if ((bool)mainPerson.allKits[0])
					{
						if (mainPerson.allKits[0].GetComponent<Equipment>().id != hairToSet2)
						{
							await LoadPersonEquipmentAsync(person, 0, hairToSet2);
						}
					}
					else
					{
						await LoadPersonEquipmentAsync(person, 0, hairToSet2);
					}
					break;
				case -1:
				{
					if (!FlagsStorage.IsMainMenu)
					{
						break;
					}
					int num = currentEquipment.SelectedEquipments[0];
					if ((bool)mainPerson.allKits[0])
					{
						if (mainPerson.allKits[0].GetComponent<Equipment>().id != num && num != -1)
						{
							await LoadPersonEquipmentAsync(person, 0, num);
						}
					}
					else if (num != -1)
					{
						await LoadPersonEquipmentAsync(person, 0, num);
					}
					break;
				}
				}
			}
			if (typeID == 3)
			{
				GameObject gameObject2 = mainPerson.allKits[0];
				if ((bool)personElements && (bool)gameObject2)
				{
					int hairToSet3 = personElements.allKits[3][kitID].hairToSet;
					int id = gameObject2.GetComponent<Equipment>().id;
					switch (hairToSet3)
					{
					case -2:
						UnityEngine.Object.Destroy(gameObject2);
						mainPerson.allKits[0] = null;
						break;
					default:
						if (id != hairToSet3)
						{
							await LoadPersonEquipmentAsync(person, 0, hairToSet3);
						}
						break;
					case -1:
						if (mainPerson == V.DriverPerson)
						{
							int num2 = currentEquipment.SelectedEquipments[0];
							if (id != num2)
							{
								await LoadPersonEquipmentAsync(person, 0, num2);
							}
						}
						break;
					}
				}
			}
			if (typeID != 4)
			{
				return mainPerson.allKits[typeID];
			}
			if (!mainPerson.allKits[2] || personElements == null)
			{
				return mainPerson.allKits[typeID];
			}
			bool hideBeard = personElements.allKits[4][kitID].HideBeard;
			mainPerson.allKits[2].SetActive(!hideBeard);
			return mainPerson.allKits[typeID];
		}

		public void LoadBodyMask(GameObject person, int typeID, int kitID)
		{
			MainPerson component = person.GetComponent<MainPerson>();
			Material mainMat = component.mainMat;
			string text = "MenuPerson/";
			int num = -1;
			if ((int)component.Gender == 1)
			{
				text += "MaleKit/Alpha_Mask/";
				num = _malePersonKitConfig.allKits[typeID][kitID].MainBodyMask;
			}
			else
			{
				text += "FemaleKit/Alpha_Mask/";
				num = _femalePersonKitConfig.allKits[typeID][kitID].MainBodyMask;
			}
			if (num != 0)
			{
				Texture value = Resources.Load<Texture>($"{text}{num}");
				switch (typeID)
				{
				case 5:
					mainMat.SetTexture("_Mask_Tors", value);
					break;
				case 8:
					mainMat.SetTexture("_Mask_Pens", value);
					break;
				case 9:
					mainMat.SetTexture("_Mask_Shoes", value);
					break;
				case 6:
				case 7:
					break;
				}
			}
		}

		public void LoadAllMask(GameObject person)
		{
			MainPerson component = person.GetComponent<MainPerson>();
			Material mainMat = component.mainMat;
			PersonKitConfig personKitConfig = _malePersonKitConfig;
			string text = "MenuPerson/";
			int num = -1;
			if ((int)component.Gender == 0)
			{
				text += "FemaleKit/Alpha_Mask/";
				personKitConfig = _femalePersonKitConfig;
			}
			else
			{
				text += "MaleKit/Alpha_Mask/";
			}
			Texture value = null;
			Texture value2 = null;
			Texture value3 = null;
			bool flag = false;
			bool flag2 = false;
			GameObject[] allKits = component.allKits;
			foreach (GameObject gameObject in allKits)
			{
				if (!gameObject)
				{
					continue;
				}
				Equipment component2 = gameObject.GetComponent<Equipment>();
				num = personKitConfig.allKits[component2.typeID][component2.id].MainBodyMask;
				if (num == 0)
				{
					continue;
				}
				if (component2.typeID == 5)
				{
					value = Resources.Load<Texture>(string.Format("{0}{1}{2}", text, "Top/", num));
					flag = component2.pack != PersonElements.Pack.no;
					flag2 = component2.pack == PersonElements.Pack.noPantsShoes || component2.pack == PersonElements.Pack.police;
				}
				else if (component2.typeID == 8)
				{
					if (!flag)
					{
						value2 = Resources.Load<Texture>(string.Format("{0}{1}{2}", text, "Pants/", num));
					}
				}
				else if (component2.typeID == 9 && !flag2)
				{
					value3 = Resources.Load<Texture>(string.Format("{0}{1}{2}", text, "Shoes/", num));
				}
			}
			mainMat.SetTexture("_Mask_Tors", value);
			mainMat.SetTexture("_Mask_Pens", value2);
			mainMat.SetTexture("_Mask_Shoes", value3);
		}

		public async UniTask LoadAllMaskAsync(GameObject person)
		{
			MainPerson component = person.GetComponent<MainPerson>();
			Material mainMat = component.mainMat;
			string path = "MenuPerson/";
			PersonKitConfig personKitConfig;
			if ((int)component.Gender == 0)
			{
				path += "FemaleKit/Alpha_Mask/";
				personKitConfig = _femalePersonKitConfig;
			}
			else
			{
				path += "MaleKit/Alpha_Mask/";
				personKitConfig = _malePersonKitConfig;
			}
			Texture _Mask_Tors = null;
			Texture _Mask_Pens = null;
			Texture _Mask_Shoes = null;
			bool canWearPants = true;
			bool canWearShoes = true;
			GameObject[] allKits = component.allKits;
			foreach (GameObject gameObject in allKits)
			{
				if (!gameObject)
				{
					continue;
				}
				Equipment component2 = gameObject.GetComponent<Equipment>();
				int mainBodyMask = personKitConfig.allKits[component2.typeID][component2.id].MainBodyMask;
				if (mainBodyMask == 0)
				{
					continue;
				}
				if (component2.typeID == 5)
				{
					_Mask_Tors = Resources.Load(path + "Top/" + mainBodyMask, typeof(Texture)) as Texture;
					if (component2.pack == PersonElements.Pack.police || component2.pack == PersonElements.Pack.noPantsShoes)
					{
						canWearPants = false;
						canWearShoes = false;
					}
					else if (component2.pack == PersonElements.Pack.noPants || component2.pack == PersonElements.Pack.noPantsGloves)
					{
						canWearPants = false;
					}
				}
				else if ((component2.typeID == 8) & canWearPants)
				{
					_Mask_Pens = (await Resources.LoadAsync<Texture>(path + "Pants/" + mainBodyMask)) as Texture;
				}
				else if ((component2.typeID == 9) & canWearShoes)
				{
					_Mask_Shoes = (await Resources.LoadAsync<Texture>(path + "Shoes/" + mainBodyMask)) as Texture;
				}
			}
			mainMat.SetTexture("_Mask_Tors", _Mask_Tors);
			mainMat.SetTexture("_Mask_Pens", _Mask_Pens);
			mainMat.SetTexture("_Mask_Shoes", _Mask_Shoes);
		}

		public static void UnLoadBodyMask(GameObject person, int typeID)
		{
			Material mainMat = person.GetComponent<MainPerson>().mainMat;
			MonoBehaviour.print("UnLoadBodyMask: " + typeID);
			switch (typeID)
			{
			case 5:
				mainMat.SetTexture("_Mask_Tors", null);
				break;
			case 8:
				mainMat.SetTexture("_Mask_Pens", null);
				break;
			case 9:
				mainMat.SetTexture("_Mask_Shoes", null);
				break;
			}
		}

		private static async UniTask LoadSkinTextureAsync(MainPerson mainPerson, int headIndex)
		{
			if (!mainPerson.mainMat)
			{
				return;
			}
			string path = (((int)mainPerson.Gender == 1) ? "Body_Male_" : "Body_Female_");
			GameObject target = mainPerson.gameObject;
			CancellationToken token = mainPerson.GetCancellationTokenOnDestroy();
			Texture maskTexture;
			Texture texture;
			try
			{
				maskTexture = await AddressableLoader.LoadAssetAsync<Texture>(path + headIndex, token, target);
				texture = await AddressableLoader.LoadAssetAsync<Texture>(path + "N", token, target);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			if ((bool)mainPerson)
			{
				if ((bool)maskTexture)
				{
					mainPerson.mainMat.SetTexture("_Albedo", maskTexture);
				}
				if ((bool)texture)
				{
					mainPerson.mainMat.SetTexture("_Normals", texture);
				}
			}
		}

		private static int GetSkinTextureIdxByHeadIdx(int headIndex, int gender)
		{
			if (gender == 1)
			{
				if ((uint)headIndex <= 6u)
				{
					return headIndex;
				}
			}
			else if ((uint)headIndex <= 2u)
			{
				return headIndex;
			}
			return 0;
		}

		public string GenerateInstantiationPlayerData()
		{
			string text = "";
			int gender = _characterEquipmentDataService.Gender;
			EquipmentsData equipmentsData = GetEquipmentsData(gender);
			text = gender.ToString() ?? "";
			for (int i = 0; i < equipmentsData.SelectedEquipments.Count; i++)
			{
				text = text + "?" + ((_forcedOutfit != null && _forcedOutfit.TryGetForcedKit(gender, i, out var kitId)) ? kitId : equipmentsData.SelectedEquipments[i]);
			}
			return text;
		}

		public GameObject CreateLocalPerson(int gender)
		{
			GameObject obj = Resources.Load<GameObject>("person" + gender);
			return _prefabFactory.Instantiate(obj);
		}

		public GameObject CreateNetworkedPerson(int ownCarViewId, PlayerDataStorage playerDataStorage, Vector3 position)
		{
			int gender = _characterEquipmentDataService.Gender;
			object[] data = PersonMultiplayerDataSending.SetDataForSend(ownCarViewId, GenerateInstantiationPlayerData(), playerDataStorage);
			return PhotonNetwork.Instantiate("person" + gender, position, Quaternion.identity, 0, data);
		}

		public EquipmentsData GetEquipmentsData(int genderInput)
		{
			EquipmentsData equipmentsData = ((genderInput == 0) ? _characterEquipmentDataService.Female : _characterEquipmentDataService.Male);
			equipmentsData = SaveEquipmentDataFromDatabase(equipmentsData, genderInput);
			if (equipmentsData.SelectedEquipments == null || equipmentsData.SelectedEquipments.Count == 0)
			{
				equipmentsData.SelectedEquipments = Enumerable.Repeat(-2, 11).ToList();
				Save();
			}
			return equipmentsData;
		}

		public void Save()
		{
			_characterEquipmentDataService.Save();
		}

		public EquipmentsData SaveEquipmentDataFromDatabase(EquipmentsData equipmentsData, int inputGender)
		{
			if (equipmentsData == null)
			{
				equipmentsData = new EquipmentsData();
			}
			if (equipmentsData.SelectedEquipments == null)
			{
				equipmentsData = new EquipmentsData();
			}
			if (inputGender == 0)
			{
				_characterEquipmentDataService.Female = equipmentsData;
			}
			else
			{
				_characterEquipmentDataService.Male = equipmentsData;
			}
			return equipmentsData;
		}

		private static string getKitName(int type)
		{
			return type switch
			{
				0 => type + " hair", 
				1 => type + " face", 
				2 => type + " beard", 
				3 => type + " cap", 
				4 => type + " mask", 
				5 => type + " top", 
				6 => type + " gloves", 
				7 => type + " bag", 
				8 => type + " pants", 
				9 => type + " shoes", 
				10 => type + " glasses", 
				_ => type + " kit", 
			};
		}

		public static string GetPathForKit(int gender, int TypeID)
		{
			string text = "MenuPerson/";
			text = ((gender != 1) ? (text + "FemaleKit/") : (text + "MaleKit/"));
			switch (TypeID)
			{
			case 0:
				text += "hair/";
				break;
			case 1:
				text += "head/";
				break;
			case 2:
				text += "beard/";
				break;
			case 3:
				text += "cap/";
				break;
			case 4:
				text += "mask/";
				break;
			case 5:
				text += "top/";
				break;
			case 6:
				text += "gloves/";
				break;
			case 7:
				text += "bag/";
				break;
			case 8:
				text += "pants/";
				break;
			case 9:
				text += "shoes/";
				break;
			case 10:
				text += "glasses/";
				break;
			}
			return text;
		}

		public static string GetIconPathForKit(int gender, int typeID, int id)
		{
			return $"{GetPathForKit(gender, typeID)}icons/{id}_icon";
		}

		private static bool HasKitConfigEntry(PersonKitConfig config, int typeID, int kitID)
		{
			PersonElements.PersonKitData[] array = config.allKits[typeID];
			if (array != null && kitID >= 0)
			{
				return kitID < array.Length;
			}
			return false;
		}
	}
}

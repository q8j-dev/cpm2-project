using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Scripts.Plugins.Car.BodyKits;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

namespace Kit
{
	public class KitController : MonoBehaviour
	{
		private Dictionary<string, Value> dict;

		private List<Button> buttons;

		private List<int> spoilerIndexes;

		private GameObject carModel;

		private CancellationTokenSource? _fadeCts;

		private int cash;

		private int coin;

		private bool _isDestroyed;

		private void OnDestroy()
		{
			_isDestroyed = true;
			_fadeCts?.Cancel();
			_fadeCts?.Dispose();
			_fadeCts = null;
		}

		public void Set(GameObject carModel, List<string> keys, Dictionary<string, Pos> positions, Dictionary<string, List<ObscuredInt>> prices, string parent)
		{
			spoilerIndexes = new List<int>();
			this.carModel = carModel;
			coin = (cash = 0);
			dict = new Dictionary<string, Value>();
			buttons = new List<Button>();
			Match match = Regex.Match(carModel.name, "\\d+$");
			string[] array = new string[8] { "spoiler", "fbumper", "rbumper", "skirt", "hood", "hair", "rair", "fender" };
			for (int i = 0; i < 8; i++)
			{
				string text = array[i].Substring(0, 4);
				if (!keys.Contains(text))
				{
					continue;
				}
				if (text == "spoi")
				{
					Transform[] spoilers = carModel.GetComponent<BodyKits>().spoilers;
					int num = spoilers.Length;
					for (int j = 0; j < num; j++)
					{
						if (spoilers[j] != null)
						{
							spoilerIndexes.Add(j);
						}
					}
					dict[text] = new Value(0, i, int.Parse(match.Value), spoilerIndexes.ToArray());
				}
				else
				{
					dict[text] = new Value(0, i, int.Parse(match.Value));
				}
			}
			parent = parent.Split('(')[0];
			GetAllFilesInDirectory("bodyKits/kit" + parent, positions, prices, int.Parse(parent)).Forget();
			GetAllFilesInDirectory("bodyKits", positions, prices).Forget();
		}

		public void AddButton(Button _transform)
		{
			buttons.Add(_transform);
		}

		public int getCoin()
		{
			return coin;
		}

		public int getCash()
		{
			return cash;
		}

		private async UniTaskVoid GetAllFilesInDirectory(string path, Dictionary<string, Pos> positions, Dictionary<string, List<ObscuredInt>> prices, int carId = -1)
		{
			string configPath = path + "/";
			configPath = ((carId != -1) ? (configPath + "bodyKitsConfig" + carId + ".asset") : (configPath + "bodyKitsConfig.asset"));
			IList<IResourceLocation> list = await Addressables.LoadResourceLocationsAsync(configPath, typeof(BodyKitsOneCarConfig)).Task;
			if (list != null && list.Count > 0)
			{
				Debug.Log("Start Loading asset for: " + configPath);
				AsyncOperationHandle<BodyKitsOneCarConfig> asyncOperationHandle = Addressables.LoadAssetAsync<BodyKitsOneCarConfig>(configPath).BindTo(base.gameObject);
				await asyncOperationHandle.Task;
				if (_isDestroyed)
				{
					return;
				}
				List<string> bodyKitsList = asyncOperationHandle.Result.bodyKitsList;
				if (bodyKitsList == null)
				{
					return;
				}
				{
					foreach (string item in bodyKitsList)
					{
						string[] source = item.Split('/');
						try
						{
							string text = source.Last().Split('.')[0];
							string text2 = string.Empty;
							int index = 0;
							for (int i = 0; i < text.Length; i++)
							{
								if (char.IsDigit(text[i]))
								{
									text2 += text[i];
								}
							}
							if (text2.Length > 0)
							{
								index = int.Parse(text2);
							}
							if ((text.Contains("spoi") || text.Contains("air")) && !positions.ContainsKey(text) && !positions.ContainsKey("h" + text) && !positions.ContainsKey("r" + text))
							{
								continue;
							}
							text2 = (prices.ContainsKey(text.Substring(0, 4)) ? prices[text.Substring(0, 4)][index].ToString() : "0");
							int num = int.Parse(text2);
							Transform value = null;
							if (positions.ContainsKey(text))
							{
								value = positions[text].transform;
							}
							if (positions.ContainsKey("h" + text))
							{
								text = "h" + text;
								value = positions[text].transform;
								if (dict.ContainsKey(text.Substring(0, 4)))
								{
									dict[text.Substring(0, 4)].Add(new KeyValuePair<string, Transform>(item, value), num);
								}
							}
							if (text.Contains("hair"))
							{
								text = text.Substring(1, text.Length - 1);
							}
							if (positions.ContainsKey("r" + text))
							{
								text = "r" + text;
								value = positions[text].transform;
								if (dict.ContainsKey(text.Substring(0, 4)))
								{
									dict[text.Substring(0, 4)].Add(new KeyValuePair<string, Transform>(item, value), num);
								}
							}
							if (dict.ContainsKey(text.Substring(0, 4)))
							{
								dict[text.Substring(0, 4)].Add(new KeyValuePair<string, Transform>(item, value), num);
							}
						}
						catch (Exception message)
						{
							Debug.LogWarning(message);
						}
					}
					return;
				}
			}
			Debug.LogWarning("No asset for: " + configPath);
		}

		public void Next(Transform car, string key)
		{
			(string, Transform) tuple = dict[key].Next(ref cash, ref coin);
			_fadeCts?.Cancel();
			_fadeCts?.Dispose();
			_fadeCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			FadeAsync(car, (key: key, path: tuple.Item1, trans: tuple.Item2), _fadeCts.Token).Forget();
		}

		public void Prev(Transform car, string key)
		{
			(string, Transform) tuple = dict[key].Prev(ref cash, ref coin);
			_fadeCts?.Cancel();
			_fadeCts?.Dispose();
			_fadeCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			FadeAsync(car, (key: key, path: tuple.Item1, trans: tuple.Item2), _fadeCts.Token).Forget();
		}

		public int[] GetSelectedKits()
		{
			string[] array = new string[8] { "spoiler", "fbumper", "rbumper", "skirt", "hood", "hair", "rair", "fender" };
			int[] array2 = Enumerable.Repeat(-1, 8).ToArray();
			for (int i = 0; i < 8; i++)
			{
				string key = array[i].Substring(0, 4);
				if (dict.ContainsKey(key))
				{
					array2[i] = dict[key].GetIndex();
				}
			}
			return array2;
		}

		private async UniTaskVoid FadeAsync(Transform car, (string key, string path, Transform trans) tuple, CancellationToken ct = default(CancellationToken))
		{
			_ = 1;
			try
			{
				List<GameObject> child = new List<GameObject>();
				GameObject[] objs = new GameObject[2];
				Renderer newRenderer = new Renderer();
				SellCarValues sCValues = car.GetComponent<SellCarValues>();
				foreach (Transform item in car)
				{
					if (item.name.Contains(tuple.key))
					{
						child.Add(item.gameObject);
					}
				}
				if (child == null)
				{
					return;
				}
				string path = tuple.path.Split('.')[0];
				AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(path + ".prefab").BindTo(base.gameObject);
				GameObject objectToSpawn = null;
				await asyncOperationHandle.ToUniTask(null, PlayerLoopTiming.Update, ct);
				if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
				{
					objectToSpawn = asyncOperationHandle.Result;
				}
				else
				{
					Debug.LogError("Can't load objectToSpawn in KitController:" + path);
				}
				for (int i = 0; i < 2; i++)
				{
					objs[i] = UnityEngine.Object.Instantiate(objectToSpawn, car);
					if (tuple.key == "hair")
					{
						objs[i].transform.name = "h" + objs[i].transform.name;
					}
					else if (tuple.key == "rair")
					{
						objs[i].transform.name = "r" + objs[i].transform.name;
					}
					bodyKitValues component = objs[i].GetComponent<bodyKitValues>();
					Renderer component2 = objs[i].GetComponent<Renderer>();
					if (tuple.trans == null)
					{
						objs[i].transform.localPosition = sCValues.pos;
						objs[i].transform.localRotation = Quaternion.Euler(sCValues.rot);
						objs[i].transform.localScale = car.transform.localScale;
					}
					else
					{
						objs[i].transform.localPosition = tuple.trans.localPosition + sCValues.pos;
						objs[i].transform.localRotation = Quaternion.Euler(tuple.trans.localRotation.eulerAngles + sCValues.rot);
						objs[i].transform.localScale = tuple.trans.localScale;
						float z = objs[i].transform.localPosition.z * Mathf.Cos(sCValues.rot.x * (float)Math.PI / 180f);
						float num = objs[i].transform.localPosition.z * Mathf.Sin(sCValues.rot.x * (float)Math.PI / 180f);
						objs[i].transform.localPosition = new Vector3(objs[i].transform.localPosition.x, objs[i].transform.localPosition.y - num, z);
					}
					try
					{
						newRenderer = ((!(component2 == null)) ? component2 : component.render);
					}
					catch (Exception arg)
					{
						Debug.LogWarning($"There are no Renderer or BodyKitValues on this body kit: {objectToSpawn}. Exception: {arg}");
						foreach (GameObject item2 in child)
						{
							UnityEngine.Object.Destroy(item2);
						}
						UnityEngine.Object.Destroy(objs[0]);
						return;
					}
					if (component != null)
					{
						objs[i].transform.localPosition += component.localPosition;
					}
				}
				Color color = newRenderer.material.color;
				newRenderer.material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
				newRenderer.material.color = color;
				foreach (GameObject item3 in child)
				{
					UnityEngine.Object.Destroy(item3);
				}
				List<Material> materials = new List<Material>();
				foreach (Transform item4 in objs[1].transform)
				{
					try
					{
						item4.GetComponent<Renderer>().material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
						materials.Add(item4.GetComponent<Renderer>().material);
					}
					catch (Exception message)
					{
						Debug.LogWarning(message);
					}
				}
				Color color2 = newRenderer.material.color;
				while (color2.a > 0.01f)
				{
					if (color2.a > 0.01f)
					{
						foreach (Material item5 in materials)
						{
							Color color3 = item5.color;
							color3.a /= 1f + 3f * Time.deltaTime;
							item5.color = color3;
						}
						color2.a /= 1f + 3f * Time.deltaTime;
					}
					newRenderer.material.color = color2;
					await UniTask.WaitForEndOfFrame(this, ct);
				}
				UnityEngine.Object.Destroy(objs[1]);
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void SortSpoiler()
		{
		}
	}
}

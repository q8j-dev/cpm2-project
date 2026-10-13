using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEngine;
using UnityEngine.Scripting;

namespace DebugPagesUi
{
	[Preserve]
	public class DebugPageController : MonoBehaviour
	{
		private const string RootPageName = "ROOT";

		private const string PagePrefix = "Page_";

		private const string ButtonPrefix = "Button_";

		private const string GroupPrefix = "Group_";

		private const string IntPrefix = "Int_";

		private const string FloatPrefix = "Float_";

		private const string StringPrefix = "String_";

		private const string BoolPrefix = "Bool_";

		private readonly Dictionary<string, HashSet<string>> _pages = new Dictionary<string, HashSet<string>>();

		private readonly Dictionary<string, object> _objects = new Dictionary<string, object>();

		private readonly Dictionary<string, bool> _boolCache = new Dictionary<string, bool>();

		public readonly Dictionary<string, (DebugPageBase, int)> Cells = new Dictionary<string, (DebugPageBase, int)>();

		[Preserve]
		public DebugPageController()
		{
		}

		private void Start()
		{
			FillMethods().Forget();
		}

		private async UniTaskVoid FillMethods()
		{
			Dictionary<string, List<(string, Action)>> groupedButtons = new Dictionary<string, List<(string, Action)>>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type[] types = assemblies[i].GetTypes();
				for (int j = 0; j < types.Length; j++)
				{
					MemberInfo[] members = types[j].GetMembers(BindingFlags.Static | BindingFlags.Public);
					foreach (MemberInfo memberInfo in members)
					{
						MethodInfo methodInfo = memberInfo as MethodInfo;
						if ((object)methodInfo != null)
						{
							try
							{
								if (methodInfo.GetCustomAttribute(typeof(DebugButtonAttribute)) is DebugButtonAttribute { Path: var text })
								{
									if (text.EndsWith("/"))
									{
										text += methodInfo.Name;
									}
									AddButton(text, () =>
									{
										methodInfo.Invoke(null, null);
									});
								}
								if (methodInfo.GetCustomAttribute(typeof(DebugGroupedButtonAttribute)) is DebugGroupedButtonAttribute { Path: var path } debugGroupedButtonAttribute)
								{
									if (!groupedButtons.ContainsKey(path))
									{
										groupedButtons[path] = new List<(string, Action)>();
									}
									groupedButtons[path].Add((debugGroupedButtonAttribute.Name ?? methodInfo.Name, () =>
									{
										methodInfo.Invoke(null, null);
									}));
								}
								if (methodInfo.GetCustomAttribute(typeof(DebugBoolHandlerAttribute)) is DebugBoolHandlerAttribute { Path: var text2 } debugBoolHandlerAttribute)
								{
									if (text2.EndsWith("/"))
									{
										text2 += methodInfo.Name;
									}
									_boolCache[methodInfo.Name] = debugBoolHandlerAttribute.DefaultValue;
									AddBool(text2, () => _boolCache[methodInfo.Name], (bool b) =>
									{
										_boolCache[methodInfo.Name] = b;
										methodInfo.Invoke(null, new object[1] { b });
									});
								}
								if (methodInfo.GetCustomAttribute(typeof(DebugIntHandlerAttribute)) is DebugIntHandlerAttribute { Path: var text3 })
								{
									if (text3.EndsWith("/"))
									{
										text3 += methodInfo.Name;
									}
									AddInt(text3, (int num) =>
									{
										methodInfo.Invoke(null, new object[1] { num });
									});
								}
								if (methodInfo.GetCustomAttribute(typeof(DebugStringHandlerAttribute)) is DebugStringHandlerAttribute { Path: var text4 })
								{
									if (text4.EndsWith("/"))
									{
										text4 += methodInfo.Name;
									}
									AddString(text4, (string text6) =>
									{
										methodInfo.Invoke(null, new object[1] { text6 });
									});
								}
							}
							catch (Exception)
							{
							}
							continue;
						}
						FieldInfo fieldInfo = memberInfo as FieldInfo;
						if ((object)fieldInfo != null && fieldInfo.GetCustomAttribute(typeof(DebugBoolValueAttribute)) is DebugBoolValueAttribute { Path: var text5 })
						{
							if (text5.EndsWith("/"))
							{
								text5 += fieldInfo.Name;
							}
							AddBool(text5, () => (bool)fieldInfo.GetValue(null), (bool b) =>
							{
								fieldInfo.SetValue(null, b);
							});
						}
					}
				}
				await UniTask.NextFrame();
			}
			foreach (string key in groupedButtons.Keys)
			{
				AddGroup(key, groupedButtons[key]);
			}
			DebugPage orCreateInitialPage = DebugSheet.Instance.GetOrCreateInitialPage();
			InitPage(orCreateInitialPage, "Page_ROOT");
		}

		private static string ObjKey(string pageKey, string entry)
		{
			return pageKey + "\u0001" + entry;
		}

		private void AddButton(string path, Action action)
		{
			FillPath(path, out var splitPath, out var index);
			string text = "Page_" + splitPath[index];
			string text2 = "Button_" + splitPath[index + 1];
			_pages[text].Add(text2);
			_objects[ObjKey(text, text2)] = action;
		}

		private void AddGroup(string path, List<(string, Action)> actions)
		{
			FillPath(path, out var splitPath, out var index);
			string text = "Page_" + splitPath[index];
			string text2 = "Group_" + splitPath[index + 1];
			_pages[text].Add(text2);
			_objects[ObjKey(text, text2)] = actions;
		}

		private void AddInt(string path, Action<int> action)
		{
			FillPath(path, out var splitPath, out var index);
			string text = "Page_" + splitPath[index];
			string text2 = "Int_" + splitPath[index + 1];
			_pages[text].Add(text2);
			_objects[ObjKey(text, text2)] = action;
		}

		private void AddFloat(string path, Action<float> action)
		{
			FillPath(path, out var splitPath, out var index);
			string text = "Page_" + splitPath[index];
			string text2 = "Float_" + splitPath[index + 1];
			_pages[text].Add(text2);
			_objects[ObjKey(text, text2)] = action;
		}

		private void AddString(string path, Action<string> action, int priority = 0)
		{
			FillPath(path, out var splitPath, out var index);
			string text = "Page_" + splitPath[index];
			string text2 = "String_" + splitPath[index + 1];
			_pages[text].Add(text2);
			_objects[ObjKey(text, text2)] = action;
		}

		private void AddBool(string path, Func<bool> defaultValue, Action<bool> action)
		{
			FillPath(path, out var splitPath, out var index);
			string text = "Page_" + splitPath[index];
			string text2 = "Bool_" + splitPath[index + 1];
			_pages[text].Add(text2);
			_objects[ObjKey(text, text2)] = action;
			_objects[ObjKey(text, text2) + "_DefaultValue"] = defaultValue;
		}

		private void FillPath(string path, out string[] splitPath, out int index)
		{
			path = "ROOT/" + path;
			splitPath = path.Split('/');
			index = 0;
			while (index < splitPath.Length - 2)
			{
				if (!_pages.ContainsKey("Page_" + splitPath[index]))
				{
					_pages["Page_" + splitPath[index]] = new HashSet<string>();
				}
				_pages["Page_" + splitPath[index]].Add("Page_" + splitPath[index + 1]);
				index++;
			}
			if (!_pages.ContainsKey("Page_" + splitPath[index]))
			{
				_pages["Page_" + splitPath[index]] = new HashSet<string>();
			}
		}

		private void InitPage(DebugPage page, string pageName)
		{
			if (!_pages.TryGetValue(pageName, out var value))
			{
				return;
			}
			List<string> list = value.ToList();
			list.Sort();
			foreach (string e in list)
			{
				string objKey = ObjKey(pageName, e);
				if (e.StartsWith("Button_"))
				{
					string text = e.Substring("Button_".Length);
					Action clicked = (Action)_objects[objKey];
					page.AddButton(text, null, null, null, null, null, showAllow: false, clicked);
				}
				else if (e.StartsWith("String_"))
				{
					string placeholder = e.Substring("String_".Length);
					page.AddSearchField(placeholder, null, (Action<string>)_objects[objKey]);
				}
				else if (e.StartsWith("Int_"))
				{
					string title = e.Substring("Int_".Length);
					int item = page.AddSearchField(title, null, (string s) =>
					{
						if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
						{
							((Action<int>)_objects[objKey])(result);
						}
						else
						{
							Debug.LogWarning("DebugPage: '" + s + "' is not a valid int for '" + title + "'");
						}
					});
					Cells[title] = (page, item);
				}
				else if (e.StartsWith("Float_"))
				{
					string title2 = e.Substring("Float_".Length);
					page.AddSearchField(title2, null, (string s) =>
					{
						if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
						{
							((Action<float>)_objects[objKey])(result);
						}
						else
						{
							Debug.LogWarning("DebugPage: '" + s + "' is not a valid float for '" + title2 + "'");
						}
					});
				}
				else if (e.StartsWith("Bool_"))
				{
					string text2 = e.Substring("Bool_".Length);
					bool value2 = ((Func<bool>)_objects[objKey + "_DefaultValue"])();
					Action<bool> valueChanged = (Action<bool>)_objects[objKey];
					page.AddSwitch(value2, text2, null, null, null, null, null, valueChanged);
				}
				else if (e.StartsWith("Page_"))
				{
					string text3 = e.Substring("Page_".Length);
					page.AddPageLinkButton(text3, null, null, null, null, null, null, ((string pageId, DebugPage page) s) =>
					{
						InitPage(s.page, e);
					});
				}
				else
				{
					if (!e.StartsWith("Group_"))
					{
						continue;
					}
					ButtonCollectionCellModel buttonCollectionCellModel = new ButtonCollectionCellModel();
					foreach (var item4 in (List<(string, Action)>)_objects[objKey])
					{
						string item2 = item4.Item1;
						Action item3 = item4.Item2;
						CollectionButtonModel collectionButtonModel = new CollectionButtonModel
						{
							Text = item2
						};
						collectionButtonModel.Clicked += item3;
						buttonCollectionCellModel.Buttons.Add(collectionButtonModel);
					}
					page.AddButtonCollection(buttonCollectionCellModel);
				}
			}
		}
	}
}

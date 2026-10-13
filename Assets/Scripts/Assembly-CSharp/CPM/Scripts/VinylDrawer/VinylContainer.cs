using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace CPM.Scripts.VinylDrawer
{
	public class VinylContainer : MonoBehaviour
	{
		[SerializeField]
		private VinylDrawer _vinylDrawer;

		private Dictionary<VinylsType, VinylsContext> _vinylsHolders = new Dictionary<VinylsType, VinylsContext>(0);

		public int ChildCount => base.transform.childCount;

		[Inject]
		public void Construct(VinylDrawer vinylDrawer)
		{
			_vinylDrawer = vinylDrawer;
		}

		public RenderTexture AddContext(VinylsType type)
		{
			if (_vinylsHolders.TryGetValue(type, out var value))
			{
				return value.renderTexture;
			}
			string text = type.ToString();
			VinylsContext vinylsContext = new VinylsContext
			{
				parent = new GameObject(text).transform,
				renderTexture = _vinylDrawer.CreateNewRenderTexture(text ?? ""),
				planeTex = null,
				planeColor = Color.clear
			};
			Transform obj = vinylsContext.parent.transform;
			obj.SetParent(base.transform);
			obj.localPosition = Vector3.zero;
			if (type == VinylsType.PlateVinyls)
			{
				vinylsContext.planeColor = Color.white;
				vinylsContext.planeTex = Texture2D.whiteTexture;
			}
			_vinylDrawer.PlaneMat.mainTexture = vinylsContext.planeTex;
			_vinylDrawer.PlaneMat.color = vinylsContext.planeColor;
			_vinylsHolders.Add(type, vinylsContext);
			return vinylsContext.renderTexture;
		}

		public void AddVinyl(GameObject vinyl, VinylsType type)
		{
			AddContext(type);
			vinyl.transform.parent = _vinylsHolders[type].parent;
		}

		public void ChangeContext(VinylsType type)
		{
			foreach (KeyValuePair<VinylsType, VinylsContext> vinylsHolder in _vinylsHolders)
			{
				if (vinylsHolder.Value != null && (bool)vinylsHolder.Value.parent)
				{
					vinylsHolder.Value.parent.gameObject.SetActive(type == vinylsHolder.Key);
				}
			}
			if (!_vinylsHolders.ContainsKey(type))
			{
				_vinylDrawer.SetCamState(isActive: false);
				return;
			}
			_vinylDrawer.ChangeCamTargetTex(_vinylsHolders[type].renderTexture);
			_vinylDrawer.ChangePlaneState(_vinylsHolders[type].planeTex, _vinylsHolders[type].planeColor);
		}

		public void DeleteContext(VinylsType type)
		{
			if (_vinylsHolders.ContainsKey(type))
			{
				VinylsContext vinylsContext = _vinylsHolders[type];
				if ((bool)vinylsContext.parent)
				{
					Object.Destroy(vinylsContext.parent.gameObject);
				}
				_vinylsHolders.Remove(type);
			}
		}

		public Transform GetVinylsParent(VinylsType type)
		{
			return _vinylsHolders[type].parent;
		}

		public RenderTexture GetRenderTexForVinylType(VinylsType type)
		{
			AddContext(type);
			return _vinylsHolders[type].renderTexture;
		}

		public bool DoesContextExist(VinylsType type)
		{
			return _vinylsHolders.ContainsKey(type);
		}

		public bool TryGetContext(VinylsType type, out VinylsContext context)
		{
			return _vinylsHolders.TryGetValue(type, out context);
		}

		public void ForceClean()
		{
			for (int num = base.transform.childCount - 1; num >= 0; num--)
			{
				Object.Destroy(base.transform.GetChild(num).gameObject);
			}
		}
	}
}

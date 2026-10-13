using UnityEngine;
using UnityEngine.Scripting;

namespace CPM.Scripts.VinylDrawer
{
	[Preserve]
	public class VinylDrawer : MonoBehaviour
	{
		[SerializeField]
		private Camera _camera;

		[SerializeField]
		private GameObject _vinylPrefab;

		[SerializeField]
		private VinylContainer _container;

		[SerializeField]
		private Material _planeMaterial;

		private PlayerDatas.VinylsDataUtility _vinylsDataUtility;

		public Camera Camera => _camera;

		public Material PlaneMat => _planeMaterial;

		public int ContainerChildCount => _container.ChildCount;

		public Vector3 Position { get; private set; }

		private void Awake()
		{
			Position = base.transform.position;
			if (!_planeMaterial)
			{
				return;
			}
			Material planeMaterial = _planeMaterial;
			_planeMaterial = new Material(planeMaterial);
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer.sharedMaterial == planeMaterial)
				{
					meshRenderer.sharedMaterial = _planeMaterial;
				}
			}
		}

		private void OnDestroy()
		{
			if ((bool)_planeMaterial)
			{
				Object.Destroy(_planeMaterial);
			}
		}

		private void Start()
		{
			if (_container == null)
			{
				_container = GetComponentInChildren<VinylContainer>();
			}
		}

		public RenderTexture CreateVinylContext(VinylsType type)
		{
			return _container.AddContext(type);
		}

		public void DeleteVinylContext(VinylsType type)
		{
			_container.DeleteContext(type);
		}

		public GameObject CreateVinyl(VinylsType type)
		{
			GameObject gameObject = Object.Instantiate(_vinylPrefab);
			_container.AddVinyl(gameObject, type);
			return gameObject;
		}

		public void FocusOnVinyls(VinylsType vinylsType)
		{
			_container.ChangeContext(vinylsType);
		}

		public Transform GetVinylsParent(VinylsType type)
		{
			return _container.GetVinylsParent(type);
		}

		public void SetCamState(bool isActive)
		{
			_camera.enabled = isActive;
		}

		public void ChangeCamTargetTex(RenderTexture renderTexture)
		{
			_camera.targetTexture = renderTexture;
		}

		public RenderTexture GetRenderTexture(VinylsType type)
		{
			return _container.GetRenderTexForVinylType(type);
		}

		public void ChangePlaneState(Texture texture, Color color)
		{
			_planeMaterial.mainTexture = texture;
			_planeMaterial.color = color;
		}

		public RenderTexture CreateNewRenderTexture(string renderTexName = "")
		{
			return new RenderTexture(3000, 3000, 0, RenderTextureFormat.ARGB32)
			{
				useMipMap = false,
				name = renderTexName
			};
		}

		public void TakeSnapshot()
		{
			_camera.Render();
		}

		public bool DoesContextExist(VinylsType type)
		{
			return _container.DoesContextExist(type);
		}

		public bool TryGetContext(VinylsType type, out VinylsContext context)
		{
			return _container.TryGetContext(type, out context);
		}

		public void ForceCleanContainer()
		{
			_container.ForceClean();
		}
	}
}

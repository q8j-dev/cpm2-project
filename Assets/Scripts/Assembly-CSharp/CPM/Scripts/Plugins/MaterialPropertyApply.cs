using UnityEngine;

namespace CPM.Scripts.Plugins
{
	public class MaterialPropertyApply : MonoBehaviour
	{
		[SerializeField]
		private NewMatProps _newMatProps = default;

		private MaterialPropertyBlock _block;

		private SkinnedMeshRenderer _renderer;

		private int _mainTex;

		private void OnValidate()
		{
			SetProps();
		}

		private void Awake()
		{
			SetProps();
		}

		private void SetProps()
		{
			if (TryGetComponent<SkinnedMeshRenderer>(out _renderer))
			{
				SerializeIDs();
				if (_block == null)
				{
					_block = new MaterialPropertyBlock();
				}
				_block.Clear();
				_block.SetVector(_mainTex, new Vector4(_newMatProps.scaleX, _newMatProps.scaleY, _newMatProps.offsetX, _newMatProps.offsetY));
				if (_renderer != null && _renderer.sharedMaterials != null)
				{
					_renderer.SetPropertyBlock(_block, _newMatProps.materialID);
				}
			}
		}

		private void SerializeIDs()
		{
			_mainTex = Shader.PropertyToID("_MainTex");
		}
	}
}

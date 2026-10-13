using UnityEngine;

namespace CPM.Scripts.Plugins
{
	[RequireComponent(typeof(MeshRenderer))]
	public class MaskBlendPropertyApply : MonoBehaviour
	{
		[SerializeField]
		private MatProps[] perMaterialProps = new MatProps[0];

		private MaterialPropertyBlock _block;

		private MeshRenderer _renderer;

		private int _mainColorID;

		private int _rColorID;

		private int _gColorID;

		private int _bColorID;

		private int _aColorID;

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
			if (!TryGetComponent<MeshRenderer>(out _renderer))
			{
				return;
			}
			SerializeIDs();
			if (_block == null)
			{
				_block = new MaterialPropertyBlock();
			}
			MatProps[] array = perMaterialProps;
			for (int i = 0; i < array.Length; i++)
			{
				MatProps matProps = array[i];
				_block.Clear();
				_block.SetColor(_mainColorID, matProps.mainColor);
				_block.SetColor(_rColorID, matProps.rColor);
				_block.SetColor(_gColorID, matProps.gColor);
				_block.SetColor(_bColorID, matProps.bColor);
				_block.SetColor(_aColorID, matProps.aColor);
				if (_renderer != null && _renderer.sharedMaterials != null && _renderer.sharedMaterials[matProps.materialID].shader.name == "CPM/MaskBlend")
				{
					_renderer.SetPropertyBlock(_block, matProps.materialID);
				}
			}
		}

		private void SerializeIDs()
		{
			_mainColorID = Shader.PropertyToID("_Color");
			_rColorID = Shader.PropertyToID("_ColorR");
			_gColorID = Shader.PropertyToID("_ColorG");
			_bColorID = Shader.PropertyToID("_ColorB");
			_aColorID = Shader.PropertyToID("_ColorA");
		}
	}
}

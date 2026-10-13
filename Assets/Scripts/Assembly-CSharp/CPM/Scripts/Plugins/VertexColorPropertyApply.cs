using System;
using UnityEngine;

namespace CPM.Scripts.Plugins
{
	[RequireComponent(typeof(MeshRenderer))]
	public class VertexColorPropertyApply : MonoBehaviour
	{
		[Serializable]
		private struct MatProps
		{
			public int materialID;

			public Color r1Color;

			public Color r09Color;

			public Color r08Color;

			public Color r07Color;

			public Color r06Color;

			public Color r05Color;

			public Color r04Color;

			public Color r03Color;

			public Color r02Color;

			public Color r01Color;
		}

		[SerializeField]
		private MatProps[] perMaterialProps = new MatProps[0];

		private MaterialPropertyBlock _block;

		private MeshRenderer _renderer;

		private int _r1ColorId;

		private int _r09ColorId;

		private int _r08ColorId;

		private int _r07ColorId;

		private int _r06ColorId;

		private int _r05ColorId;

		private int _r04ColorId;

		private int _r03ColorId;

		private int _r02ColorId;

		private int _r01ColorId;

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
				_block.SetColor(_r1ColorId, matProps.r1Color);
				_block.SetColor(_r09ColorId, matProps.r09Color);
				_block.SetColor(_r08ColorId, matProps.r08Color);
				_block.SetColor(_r07ColorId, matProps.r07Color);
				_block.SetColor(_r06ColorId, matProps.r06Color);
				_block.SetColor(_r05ColorId, matProps.r05Color);
				_block.SetColor(_r04ColorId, matProps.r04Color);
				_block.SetColor(_r03ColorId, matProps.r03Color);
				_block.SetColor(_r02ColorId, matProps.r02Color);
				_block.SetColor(_r01ColorId, matProps.r01Color);
				if (_renderer != null && _renderer.sharedMaterials != null && _renderer.sharedMaterials[matProps.materialID].shader.name == "CPM/VertexColor")
				{
					_renderer.SetPropertyBlock(_block, matProps.materialID);
				}
			}
		}

		private void SerializeIDs()
		{
			_r1ColorId = Shader.PropertyToID("_Color0");
			_r09ColorId = Shader.PropertyToID("_Color1");
			_r08ColorId = Shader.PropertyToID("_Color2");
			_r07ColorId = Shader.PropertyToID("_Color3");
			_r06ColorId = Shader.PropertyToID("_Color4");
			_r05ColorId = Shader.PropertyToID("_Color5");
			_r04ColorId = Shader.PropertyToID("_Color6");
			_r03ColorId = Shader.PropertyToID("_Color7");
			_r02ColorId = Shader.PropertyToID("_Color8");
			_r01ColorId = Shader.PropertyToID("_Color9");
		}
	}
}

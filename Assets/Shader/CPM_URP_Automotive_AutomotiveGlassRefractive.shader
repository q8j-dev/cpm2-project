Shader "CPM/URP/Automotive/AutomotiveGlassRefractive" {
	Properties {
		[NoScaleOffset] _Transmission ("Transmission", 2D) = "white" {}
		_Color ("Color", Vector) = (0,0,0,0)
		[NoScaleOffset] _NormalMask ("NormalMask", 2D) = "white" {}
		_RefractionPower ("RefractionPower", Range(-0.1, 0.1)) = 0
		_GeometryRefraction ("GeometryRefraction", Range(0, 1)) = 0
		[Toggle(_DEPTHFILTERING)] _DEPTHFILTERING ("DepthFiltering", Float) = 0
		[Toggle(_GLASSTHICKNESS)] _GLASSTHICKNESS ("GlassThickness", Float) = 1
		_Thickness ("Thickness", Range(0, 1)) = 0
		_ThicknessNormalPower ("ThicknessNormalPower", Range(0, 1)) = 1
		_Thickness_Power ("Thickness Power", Range(0, 1)) = 1
		_StencilRef ("_StencilRef", Float) = 1
		_FakeBottomAO ("FakeBottomAO", Range(0, 1)) = 0
		_VertAO ("VertAO", Range(0, 1)) = 1
		_MaskValue ("Dirt Amount", Range(0, 1)) = 0
		_MaskAddTex ("Dirt Albedo", 2D) = "white" {}
		_MaskColor ("Dirt color", Vector) = (0.9058824,0.7529412,0.5176471,1)
		[ToggleUI] _USEDIRT ("UseDirt", Float) = 0
		[Toggle(_EMISSION)] _EMISSION ("Emission", Float) = 0
		[NoScaleOffset] _EmissionMap ("Emission ID Map", 2D) = "black" {}
		_RainPower ("RainPower", Range(0, 1)) = 1
		_DRL_Power ("DRL Power", Range(0, 1)) = 1
		_DRL_MaskID ("DRL MaskID", Float) = 0
		[HDR] _DRL_Color ("DRL Color", Vector) = (1,1,1,0)
		_LBL_Power ("LBL Power", Range(0, 1)) = 1
		_LBL_MaskID ("LBL MaskID", Float) = 0
		[HDR] _LBL_Color ("LBL Color", Vector) = (1,1,1,0)
		_HBL_Power ("HBL Power", Range(0, 1)) = 1
		_HBL_MaskID ("HBL MaskID", Float) = 0
		[HDR] _HBL_Color ("HBL Color", Vector) = (1,1,1,0)
		_LTL_Power_0 ("LTL Power (0)", Range(0, 1)) = 1
		_LTL_MaskID ("LTL MaskID", Float) = 0
		[HDR] _LTL_Color ("LTL Color", Vector) = (1,1,1,0)
		_RTL_Power_0 ("RTL Power (0)", Range(0, 1)) = 1
		_RTL_MaskID ("RTL MaskID", Float) = 0
		[HDR] _RTL_Color ("RTL Color", Vector) = (1,1,1,0)
		_PTL_Power_0 ("PTL Power (0)", Range(0, 1)) = 1
		_PTL_MaskID ("PTL MaskID", Float) = 0
		[HDR] _PTL_Color ("PTL Color", Vector) = (1,1,1,0)
		_BL_Power_0 ("BL Power (0)", Range(0, 1)) = 1
		_BL_MaskID ("BL MaskID", Float) = 0
		[HDR] _BL_Color ("BL Color", Vector) = (1,1,1,0)
		_RL_Power ("RL Power", Range(0, 1)) = 1
		_RL_MaskID ("RL MaskID", Float) = 0
		[HDR] _RL_Color_0 ("RL Color (0)", Vector) = (1,1,1,0)
		[NoScaleOffset] _AutomotiveRainGatedCustomFunction_c53087ed45864de2bfb6e53caabc2edd_StillDropsTex_12_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _AutomotiveRainGatedCustomFunction_c53087ed45864de2bfb6e53caabc2edd_RainDropsTex_13_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _AutomotiveRainGatedCustomFunction_c53087ed45864de2bfb6e53caabc2edd_RainStreaksTex_14_Texture2D ("Texture2D", 2D) = "white" {}
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}
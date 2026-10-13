Shader "Shader Graphs/Sprite-Unlit-ExpCompensated"
{
	Properties
	{
		[NoScaleOffset] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint (Color)", Color) = (1,1,1,0)
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	SubShader
	{
		Tags { "QUEUE" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "ShaderGraphShader" = "true" "ShaderGraphTargetId" = "UniversalSpriteUnlitSubTarget" "UniversalMaterialType" = "Unlit" }
		Pass
		{
			Name "Sprite Unlit"
			Tags { "LIGHTMODE" = "Universal2D" }
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			float4 _GlobalsPad1[48];
			float2 _GlobalExposureWB;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[2];
			float4 _RendererColor;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float4 _Color : packoffset(c1);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1, out float4 o_INTERP2 : INTERP2, out float3 o_INTERP3 : INTERP3)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_INTERP3 = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				o_INTERP1 = _RendererColor.xyzw * i_COLOR0.xyzw;
				r0 = asuint(_Color.xyzw * i_COLOR0.xyzw);
				o_INTERP2.xyz = asfloat(r0.xyz) / _GlobalExposureWB.xxx;
				o_INTERP2.w = asfloat(r0.w);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, float4 i_INTERP2 : INTERP2, float3 i_INTERP3 : INTERP3, out float4 o_SV_TARGET0 : SV_TARGET0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0 = asuint(_MainTex.SampleBias(sampler_MainTex, i_INTERP0.xy, _GlobalMipBias.x));
				r0 = asuint(i_INTERP2.xyzw * asfloat(r0));
				b1.x = (0.0 == asfloat(r0.w));
				if (b1.x) discard;
				o_SV_TARGET0 = i_INTERP1.xyzw * asfloat(r0);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "Sprite Unlit"
			Tags { "LIGHTMODE" = "UniversalForward" }
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			float4 _GlobalsPad1[48];
			float2 _GlobalExposureWB;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[2];
			float4 _RendererColor;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float4 _Color : packoffset(c1);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1, out float4 o_INTERP2 : INTERP2, out float3 o_INTERP3 : INTERP3)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_INTERP3 = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				o_INTERP1 = _RendererColor.xyzw * i_COLOR0.xyzw;
				r0 = asuint(_Color.xyzw * i_COLOR0.xyzw);
				o_INTERP2.xyz = asfloat(r0.xyz) / _GlobalExposureWB.xxx;
				o_INTERP2.w = asfloat(r0.w);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, float4 i_INTERP2 : INTERP2, float3 i_INTERP3 : INTERP3, out float4 o_SV_TARGET0 : SV_TARGET0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0 = asuint(_MainTex.SampleBias(sampler_MainTex, i_INTERP0.xy, _GlobalMipBias.x));
				r0 = asuint(i_INTERP2.xyzw * asfloat(r0));
				b1.x = (0.0 == asfloat(r0.w));
				if (b1.x) discard;
				o_SV_TARGET0 = i_INTERP1.xyzw * asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}

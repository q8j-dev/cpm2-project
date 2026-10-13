Shader "Shader Graphs/Skidmarks"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,0)
		_BaseMap ("BaseMap", 2D) = "white" {}
		[NoScaleOffset] [Normal] _NormalMap ("NormalMap", 2D) = "bump" {}
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		_Metalness ("Metalness", Range(0, 1)) = 0
		_Contrast ("Contrast", Float) = 3
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	SubShader
	{
		Tags { "DisableBatching" = "False" "QUEUE" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "ShaderGraphShader" = "true" "ShaderGraphTargetId" = "UniversalUnlitSubTarget" "UniversalMaterialType" = "Unlit" }
		Pass
		{
			Name "Universal Forward"
			Blend DstColor Zero, Zero One
			Cull Back
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DIRLIGHTMAP_COMBINED
			#pragma multi_compile __ FOG_EXP
			#pragma multi_compile __ LIGHTMAP_ON

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[6];
			float4 _MainLightPosition;
			float4 _GlobalsPad1[69];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1, out float4 o_INTERP2 : INTERP2, out float4 o_INTERP3 : INTERP3, out float3 o_INTERP4 : INTERP4, out float3 o_INTERP5 : INTERP5)
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
				o_INTERP3.xyz = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_TANGENT0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_TANGENT0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_TANGENT0.zzz + asfloat(r0.xyz));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP0.xyz = asfloat(r0.xyz) * asfloat(r0.www);
				o_INTERP0.w = i_TANGENT0.w;
				o_INTERP1 = i_TEXCOORD0.xyzw;
				o_INTERP2 = i_COLOR0.xyzw;
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				r0.w = asuint(dot(-_MainLightPosition.xyz, -_MainLightPosition.xyz));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.xyz = asuint(-_MainLightPosition.xyz * asfloat(r0.www));
				o_INTERP3.w = dot(asfloat(r0.xyz), -asfloat(r1.xyz));
				o_INTERP4 = asfloat(r0.xyz);
				o_INTERP5 = -asfloat(r1.xyz);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			cbuffer UnityPerDraw : register(b1)
			{
				float4 unity_WorldTransformParams : packoffset(c9);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float4 _BaseMap_ST : packoffset(c1);
				float _Contrast : packoffset(c5);
			}
			Texture2D<float4> _BaseMap : register(t0);
			Texture2D<float4> _NormalMap : register(t1);
			SamplerState sampler_BaseMap : register(s0);
			SamplerState sampler_NormalMap : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, float4 i_INTERP2 : INTERP2, float4 i_INTERP3 : INTERP3, float3 i_INTERP4 : INTERP4, float3 i_INTERP5 : INTERP5, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				b0.x = (0.0 < i_INTERP0.w);
				b0.y = (unity_WorldTransformParams.w >= 0.0);
				r0.xy = b0.xy ? uint2(0x3f800000u, 0x3f800000u) : uint2(0xbf800000u, 0xbf800000u);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r0.yzw = asuint(i_INTERP4.zxy * i_INTERP0.yzx);
				r0.yzw = asuint(i_INTERP4.yzx * i_INTERP0.zxy + -asfloat(r0.yzw));
				r0.xyz = asuint(asfloat(r0.xxx) * asfloat(r0.yzw));
				r0.w = asuint(dot(i_INTERP4.xyz, i_INTERP4.xyz));
				r0.w = asuint(sqrt(asfloat(r0.w)));
				r0.w = asuint(1.0 / asfloat(r0.w));
				r0.xyz = asuint(asfloat(r0.www) * asfloat(r0.xyz));
				r1.xy = asuint(float2(1.0, 0.100000001) * i_INTERP1.xy);
				r2 = asuint(_NormalMap.SampleBias(sampler_NormalMap, asfloat(r1.xy), _GlobalMipBias.x));
				r1.xy = asuint(asfloat(r1.xy) * _BaseMap_ST.xy + _BaseMap_ST.zw);
				r1 = asuint(_BaseMap.SampleBias(sampler_BaseMap, asfloat(r1.xy), _GlobalMipBias.x));
				r2.x = asuint(asfloat(r2.w) * asfloat(r2.x));
				r2.xy = asuint(asfloat(r2.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r1.y = asuint(dot(asfloat(r2.xy), asfloat(r2.xy)));
				r1.y = asuint(min(1.0, asfloat(r1.y)));
				r1.xy = asuint(float2(1.0, 1.0) + -asfloat(r1.wy));
				r1.y = asuint(sqrt(asfloat(r1.y)));
				r2.z = asuint(max(1.00000002e-16, asfloat(r1.y)));
				r1.y = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r1.y = asuint(rsqrt(asfloat(r1.y)));
				r1.yzw = asuint(asfloat(r2.xyz) * asfloat(r1.yyy));
				r0.xyz = asuint(asfloat(r1.zzz) * asfloat(r0.xyz));
				r2.xyz = asuint(i_INTERP0.xyz * asfloat(r0.www));
				r3.xyz = asuint(i_INTERP4.xyz * asfloat(r0.www));
				r0.xyz = asuint(asfloat(r1.yyy) * asfloat(r2.xyz) + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r1.www) * asfloat(r3.xyz) + asfloat(r0.xyz));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				r0.x = asuint(dot(asfloat(r0.xyz), i_INTERP5.xyz));
				r0.x = asuint(i_INTERP3.w + -asfloat(r0.x));
				r0.x = asuint(max(-1.0, asfloat(r0.x)));
				r0.x = asuint(min(1.0, asfloat(r0.x)));
				r0.x = asuint(1.0 + asfloat(r0.x));
				r0.x = asuint(log2(asfloat(r0.x)));
				r0.x = asuint(_Contrast * asfloat(r0.x));
				r0.x = asuint(exp2(asfloat(r0.x)));
				r0.x = asuint(asfloat(r0.x) + -asfloat(r1.x));
				r0.x = asuint(i_INTERP2.z * asfloat(r0.x) + asfloat(r1.x));
				r0.x = asuint(-1.0 + asfloat(r0.x));
				o_SV_Target0.xyz = i_INTERP2.www * asfloat(r0.xxx) + float3(1.0, 1.0, 1.0);
				o_SV_Target0.w = 1.0;
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "DepthNormalsOnly"
			Tags { "LIGHTMODE" = "DepthNormalsOnly" }
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_INTERP0 : INTERP0)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0);
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP0 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_INTERP0 : INTERP0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.x = asuint(dot(i_INTERP0.xyz, i_INTERP0.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				o_SV_Target0.xyz = i_INTERP0.xyz * asfloat(r0.xxx);
				o_SV_Target0.w = 0.0;
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}

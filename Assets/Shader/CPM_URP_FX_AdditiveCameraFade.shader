Shader "CPM/URP/FX/AdditiveCameraFade"
{
	Properties
	{
		[MainTexture] _BaseMap ("BaseMap", 2D) = "white" {}
		[MainColor] _Color ("Color", Color) = (1,1,1,0)
		_FadeDistance ("FadeDistance", Float) = 1
		_AlphaClip ("AlphaClip", Float) = 0
		[HideInInspector] _CastShadows ("_CastShadows", Float) = 1
		[HideInInspector] _Surface ("_Surface", Float) = 1
		[HideInInspector] _Blend ("_Blend", Float) = 2
		[HideInInspector] _SrcBlend ("_SrcBlend", Float) = 1
		[HideInInspector] _DstBlend ("_DstBlend", Float) = 0
		[ToggleUI] [HideInInspector] _ZWrite ("_ZWrite", Float) = 0
		[HideInInspector] _ZWriteControl ("_ZWriteControl", Float) = 0
		[HideInInspector] _ZTest ("_ZTest", Float) = 4
		[HideInInspector] _Cull ("_Cull", Float) = 2
		[HideInInspector] _AlphaToMask ("_AlphaToMask", Float) = 0
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
			Blend [_SrcBlend] [_DstBlend], [_SrcBlend] [_DstBlend]
			Cull [_Cull]
			ZTest [_ZTest]
			ZWrite [_ZWrite]
			AlphaToMask [_AlphaToMask]
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DIRLIGHTMAP_COMBINED
			#pragma multi_compile __ FOG_EXP
			#pragma multi_compile __ LIGHTMAP_ON
			#pragma multi_compile __ _SURFACE_TYPE_TRANSPARENT

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float3 o_INTERP1 : INTERP1, out float3 o_INTERP2 : INTERP2)
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
				o_INTERP1 = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP2 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if (defined(DIRLIGHTMAP_COMBINED) && defined(FOG_EXP) && defined(LIGHTMAP_ON) && defined(_SURFACE_TYPE_TRANSPARENT)) || (defined(DIRLIGHTMAP_COMBINED) && defined(LIGHTMAP_ON) && defined(_SURFACE_TYPE_TRANSPARENT)) || (defined(FOG_EXP) && defined(_SURFACE_TYPE_TRANSPARENT)) || defined(_SURFACE_TYPE_TRANSPARENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[15];
			float4 _ProjectionParams;
			float4 _GlobalsPad4[42];
			float4x4 unity_MatrixV;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _BaseMap_ST : packoffset(c1);
				float4 _Color : packoffset(c2);
				float _FadeDistance : packoffset(c3);
			}
			Texture2D<float4> _BaseMap : register(t0);
			SamplerState sampler_BaseMap : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float3 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(unity_MatrixV._m21 * i_INTERP1.y);
				r0.x = asuint(unity_MatrixV._m20 * i_INTERP1.x + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m22 * i_INTERP1.z + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 + asfloat(r0.x));
				r0.x = asuint(-_ProjectionParams.y + -asfloat(r0.x));
				r0.y = asuint(max(_FadeDistance, 0.00100000005));
				r0.x = asuint(saturate(asfloat(r0.x) / asfloat(r0.y)));
				r0.yz = asuint(i_INTERP0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw);
				r1 = asuint(_BaseMap.SampleBias(sampler_BaseMap, asfloat(r0.yz), _GlobalMipBias.x));
				r1 = asuint(_Color.xyzw * asfloat(r1));
				o_SV_Target0.w = asfloat(r1.w) * asfloat(r0.x);
				o_SV_Target0.xyz = asfloat(r1.xyz);
			}
			#elif (defined(DIRLIGHTMAP_COMBINED) && defined(FOG_EXP) && defined(LIGHTMAP_ON)) || (defined(DIRLIGHTMAP_COMBINED) && defined(LIGHTMAP_ON)) || defined(FOG_EXP) || 1
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _BaseMap_ST : packoffset(c1);
				float4 _Color : packoffset(c2);
			}
			Texture2D<float4> _BaseMap : register(t0);
			SamplerState sampler_BaseMap : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float3 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.xy = asuint(i_INTERP0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw);
				r0 = asuint(_BaseMap.SampleBias(sampler_BaseMap, asfloat(r0.xy), _GlobalMipBias.x));
				o_SV_Target0.xyz = _Color.xyz * asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#endif
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "DepthOnly"
			Tags { "LIGHTMODE" = "DepthOnly" }
			ColorMask 8
			Cull [_Cull]
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
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float3 o_INTERP1 : INTERP1)
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
				o_INTERP1 = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				o_INTERP0 = i_TEXCOORD0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float3 i_INTERP1 : INTERP1, out float4 o_SV_TARGET0 : SV_TARGET0)
			{
				o_SV_TARGET0 = i_SV_POSITION0.zzzz;
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "DepthNormalsOnly"
			Tags { "LIGHTMODE" = "DepthNormalsOnly" }
			Cull [_Cull]
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ _SURFACE_TYPE_TRANSPARENT

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float3 o_INTERP1 : INTERP1, out float3 o_INTERP2 : INTERP2)
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
				o_INTERP1 = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP2 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float3 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.x = asuint(dot(i_INTERP2.xyz, i_INTERP2.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				o_SV_Target0.xyz = i_INTERP2.xyz * asfloat(r0.xxx);
				o_SV_Target0.w = 0.0;
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "ShadowCaster"
			Tags { "LIGHTMODE" = "SHADOWCASTER" }
			ColorMask 0
			Cull [_Cull]
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			float4 _GlobalsPad1[47];
			float4 _ShadowBias;
			float4 _GlobalsPad2[2];
			float3 _LightDirection;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float3 o_INTERP1 : INTERP1, out float3 o_INTERP2 : INTERP2)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				r0.w = asuint(saturate(dot(_LightDirection.xyz, asfloat(r0.xyz))));
				r0.w = asuint(1.0 + -asfloat(r0.w));
				r0.w = asuint(_ShadowBias.y * asfloat(r0.w));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r1.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r1.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r1.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r1.xyz));
				r2.xyz = asuint(_LightDirection.xyz * _ShadowBias.xxx + asfloat(r1.xyz));
				o_INTERP1 = asfloat(r1.xyz);
				r1.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www) + asfloat(r2.xyz));
				o_INTERP2 = asfloat(r0.xyz);
				r0 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r1.yyyy));
				r0 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r1.xxxx) + asfloat(r0));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r1.zzzz) + asfloat(r0));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0));
				o_SV_POSITION0.z = min(1.0, asfloat(r0.z));
				o_SV_POSITION0.xyw = asfloat(r0.xyw);
				o_INTERP0 = i_TEXCOORD0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float3 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_TARGET0 : SV_TARGET0)
			{
				o_SV_TARGET0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}

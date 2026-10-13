Shader "Shader Graphs/Raindrops"
{
	Properties
	{
		[NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
		_RaindropsScale ("RaindropsScale", Float) = 15
		_BaseColor ("BaseColor", Color) = (1,1,1,0.3333333)
		_Speed ("Speed", Vector) = (0.3,-1,0,0)
		_DepthFadeStart ("DepthFadeStart", Float) = 0
		_DepthFadeRange ("DepthFadeRange", Float) = 0
		_AmbientBlend ("AmbientBlend", Float) = 0.5
		_Refract ("Refract", Float) = 0.005
		_LayerScale1 ("LayerScale1", Float) = 1
		_LayerTiling1 ("LayerTiling1", Vector) = (1,1,0,0)
		_LayerScale2 ("LayerScale2", Float) = 2
		_LayerTiling2 ("LayerTiling2", Vector) = (1,1,0,0)
		_LayerScale3 ("LayerScale3", Float) = 3
		_LayerTiling3 ("LayerTiling3", Vector) = (1,1,0,0)
		[NoScaleOffset] [NonModifiableTextureData] _SampleTexture2D_e984f10a7a1140a6b9392d6254a23038_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
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
			Blend SrcAlpha One, One One
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
			float4 _GlobalsPad0[4];
			float4 _ScaledScreenParams;
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[13];
			float4 _TimeParameters;
			float4 _GlobalsPad4[1];
			float4 _ProjectionParams;
			float4 _GlobalsPad5[1];
			float4 _ZBufferParams;
			float4 unity_OrthoParams;
			float4 _GlobalsPad6[29];
			float4 unity_AmbientSky;
			float4 _GlobalsPad7[9];
			float4x4 unity_MatrixV;
			float4 _GlobalsPad8[12];
			float4x4 unity_MatrixInvVP;
			float4 _GlobalsPad9[44];
			float _RainConeFade;
			float _RainIntensity;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _RaindropsScale : packoffset(c1);
				float2 _LayerTiling3 : packoffset(c1.y);
				float2 _LayerTiling2 : packoffset(c2);
				float4 _BaseColor : packoffset(c3);
				float2 _Speed : packoffset(c4);
				float _DepthFadeStart : packoffset(c4.z);
				float _DepthFadeRange : packoffset(c4.w);
				float _LayerScale2 : packoffset(c6);
				float _LayerScale3 : packoffset(c6.y);
				float _LayerScale1 : packoffset(c6.z);
				float2 _LayerTiling1 : packoffset(c7);
				float _AmbientBlend : packoffset(c7.z);
			}
			Texture2D<float4> _CameraDepthTexture : register(t0);
			Texture2D<float4> _SampleTexture2D_e984f10a7a1140a6b9392d6254a23038_Texture_1_Texture2D : register(t1);
			Texture2D<float4> _MainTex : register(t2);
			SamplerState sampler_CameraDepthTexture : register(s0);
			SamplerState sampler_SampleTexture2D_e984f10a7a1140a6b9392d6254a23038_Texture_1_Texture2D : register(s1);
			SamplerState sampler_MainTex : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float3 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				bool4 b1;
				r0.x = asuint(unity_WorldToObject._m11 * i_INTERP1.y);
				r0.x = asuint(unity_WorldToObject._m10 * i_INTERP1.x + asfloat(r0.x));
				r0.x = asuint(unity_WorldToObject._m12 * i_INTERP1.z + asfloat(r0.x));
				r0.x = asuint(unity_WorldToObject._m13 + asfloat(r0.x));
				r0.y = asuint(unity_MatrixV._m21 * i_INTERP1.y);
				r0.y = asuint(unity_MatrixV._m20 * i_INTERP1.x + asfloat(r0.y));
				r0.y = asuint(unity_MatrixV._m22 * i_INTERP1.z + asfloat(r0.y));
				r0.y = asuint(unity_MatrixV._m23 + asfloat(r0.y));
				b0.z = (_ProjectionParams.x < 0.0);
				r0.w = asuint(_ScaledScreenParams.y + -i_SV_POSITION0.y);
				r1.y = b0.z ? r0.w : asuint(i_SV_POSITION0.y);
				r1.x = asuint(i_SV_POSITION0.x);
				r1.xy = asuint(asfloat(r1.xy) / _ScaledScreenParams.xy);
				r2.xyz = asuint(-_BaseColor.xyz + unity_AmbientSky.xyz);
				r2.xyz = asuint(float3(_AmbientBlend, _AmbientBlend, _AmbientBlend) * asfloat(r2.xyz) + _BaseColor.xyz);
				b0.z = (1.0 == unity_OrthoParams.w);
				[branch] if (b0.z)
				{
					r1.z = asuint(1.0 + -asfloat(r1.y));
					r3 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r1.xz), _GlobalMipBias.x));
					r0.zw = asuint(asfloat(r1.xz) * float2(2.0, 2.0) + float2(-1.0, -1.0));
					r4 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r0.wwww));
					r4 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r0.zzzz) + asfloat(r4));
					r3 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r3.xxxx) + asfloat(r4));
					r3 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r3));
					r3.xyz = asuint(asfloat(r3.xyz) / asfloat(r3.www));
					r0.z = asuint(unity_MatrixV._m21 * asfloat(r3.y));
					r0.z = asuint(unity_MatrixV._m20 * asfloat(r3.x) + asfloat(r0.z));
					r0.z = asuint(unity_MatrixV._m22 * asfloat(r3.z) + asfloat(r0.z));
					r0.z = asuint(unity_MatrixV._m23 + asfloat(r0.z));
					r0.z = asuint(abs(asfloat(r0.z)));
				}
				else
				{
					r1.w = asuint(1.0 + -asfloat(r1.y));
					r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r1.xw), _GlobalMipBias.x));
					r0.w = asuint(_ZBufferParams.z * asfloat(r1.x) + _ZBufferParams.w);
					r0.z = asuint(1.0 / asfloat(r0.w));
				}
				r0.y = asuint(asfloat(r0.z) + asfloat(r0.y));
				r0.y = asuint(-_DepthFadeStart + asfloat(r0.y));
				r0.z = asuint(1.0 / _DepthFadeRange);
				r0.y = asuint(saturate(asfloat(r0.y) * asfloat(r0.z)));
				r0.z = asuint(asfloat(r0.y) * -2.0 + 3.0);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.y));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y));
				r0.yzw = asuint(asfloat(r2.xyz) * asfloat(r0.yyy));
				r1.xy = asuint(float2(_RainIntensity, _RainIntensity) * float2(-0.400000006, 3.0) + float2(0.5, -2.0));
				r1.zw = asuint(_LayerTiling1.xy * float2(_LayerScale1, _LayerScale1));
				r1.zw = asuint(float2(_RaindropsScale, _RaindropsScale) * asfloat(r1.zw));
				r2.xy = asuint(_Speed.xy * _TimeParameters.xx);
				r1.zw = asuint(i_INTERP0.xy * asfloat(r1.zw) + asfloat(r2.xy));
				r3 = asuint(_MainTex.SampleBias(sampler_MainTex, asfloat(r1.zw), _GlobalMipBias.x).wxyz);
				r1.zw = asuint(float2(_LayerScale2, _LayerScale2) * _LayerTiling2.xy);
				r1.zw = asuint(float2(_RaindropsScale, _RaindropsScale) * asfloat(r1.zw));
				r1.zw = asuint(i_INTERP0.xy * asfloat(r1.zw) + asfloat(r2.xy));
				r4 = asuint(_MainTex.SampleBias(sampler_MainTex, asfloat(r1.zw), _GlobalMipBias.x).wxyz);
				r1.zw = asuint(float2(_LayerScale3, _LayerScale3) * _LayerTiling3.xy);
				r1.zw = asuint(float2(_RaindropsScale, _RaindropsScale) * asfloat(r1.zw));
				r1.zw = asuint(i_INTERP0.xy * asfloat(r1.zw) + asfloat(r2.xy));
				r2 = asuint(_MainTex.SampleBias(sampler_MainTex, asfloat(r1.zw), _GlobalMipBias.x).wxyz);
				r3.x = asuint(saturate(asfloat(r3.x)));
				r4.x = asuint(saturate(asfloat(r4.x)));
				r2.x = asuint(saturate(asfloat(r2.x)));
				r1.z = asuint(asfloat(r4.x) + asfloat(r3.x));
				r1.z = asuint(asfloat(r1.z) + asfloat(r2.x));
				r1.z = asuint(min(1.0, asfloat(r1.z)));
				b1.x = (asfloat(r1.z) >= asfloat(r1.x));
				r1.x = b1.x ? 0x3f800000u : 0u;
				r1.z = asuint(0.550000012 * _TimeParameters.x);
				r1.zw = asuint(i_INTERP0.xy * float2(-1.20000005, -1.39999998) + asfloat(r1.zz));
				r2 = asuint(_SampleTexture2D_e984f10a7a1140a6b9392d6254a23038_Texture_1_Texture2D.SampleBias(sampler_SampleTexture2D_e984f10a7a1140a6b9392d6254a23038_Texture_1_Texture2D, asfloat(r1.zw), _GlobalMipBias.x));
				r1.yzw = asuint(saturate(asfloat(r2.xyz) * asfloat(r1.yyy)));
				r1.xyz = asuint(asfloat(r1.xxx) + asfloat(r1.yzw));
				o_SV_Target0.xyz = asfloat(r1.xyz) * asfloat(r0.yzw);
				r0.xy = asuint(float2(-20.0, 20.0) + asfloat(r0.xx));
				r0.xy = asuint(saturate(float2(-0.0407331996, 0.0407331996) * asfloat(r0.xy)));
				r0.zw = asuint(asfloat(r0.xy) * float2(-2.0, -2.0) + float2(3.0, 3.0));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.xy));
				r0.y = asuint(1.0 - asfloat(r0.w) * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.z) * asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) * asfloat(r1.x));
				r0.x = asuint(_BaseColor.w * asfloat(r0.x));
				o_SV_Target0.w = _RainConeFade * asfloat(r0.x);
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
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}

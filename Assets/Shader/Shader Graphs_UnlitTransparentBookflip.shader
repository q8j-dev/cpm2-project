Shader "Shader Graphs/UnlitTransparentBookflip"
{
	Properties
	{
		[NoScaleOffset] _Texture ("Texture", 2D) = "white" {}
		_Width ("Width", Float) = 1
		_Height ("Height", Float) = 2
		_Speed ("Speed", Float) = 0
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
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[13];
			float4 _TimeParameters;
			cbuffer UnityPerMaterial : register(b1)
			{
				float _Height : packoffset(c1);
				float _Width : packoffset(c1.y);
				float _Speed : packoffset(c1.z);
			}
			Texture2D<float4> _Texture : register(t0);
			SamplerState sampler_Texture : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float3 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.x = asuint(_Speed * _TimeParameters.x);
				r0.x = asuint(floor(asfloat(r0.x)));
				r0.x = asuint(9.99999975e-06 + asfloat(r0.x));
				r0.y = asuint(_Width * _Height);
				r0.x = asuint(asfloat(r0.x) / asfloat(r0.y));
				b0.z = (asfloat(r0.x) >= -asfloat(r0.x));
				r0.x = asuint(frac(abs(asfloat(r0.x))));
				r0.x = asuint(b0.z ? asfloat(r0.x) : -asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r0.x = asuint(floor(asfloat(r0.x)));
				r0.y = asuint(0.5 + asfloat(r0.x));
				r0.zw = asuint(float2(1.0, 1.0) / float2(_Width, _Height));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z));
				r0.y = asuint(floor(asfloat(r0.y)));
				r1.x = asuint(asfloat(r0.x) - _Width * asfloat(r0.y));
				r0.x = asuint(1.0 + asfloat(r0.y));
				r1.y = asuint(_Height + -asfloat(r0.x));
				r0.xy = asuint(i_INTERP0.xy + asfloat(r1.xy));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zw));
				r0 = asuint(_Texture.SampleBias(sampler_Texture, asfloat(r0.xy), _GlobalMipBias.x));
				o_SV_Target0.xyz = asfloat(r0.xyz);
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
		Pass
		{
			Name "ShadowCaster"
			Tags { "LIGHTMODE" = "SHADOWCASTER" }
			ColorMask 0
			Cull Back
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

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_INTERP0 : INTERP0)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r0.xyz = asuint(_LightDirection.xyz * _ShadowBias.xxx + asfloat(r0.xyz));
				r1.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r1.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r1.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www));
				r0.w = asuint(saturate(dot(_LightDirection.xyz, asfloat(r1.xyz))));
				r0.w = asuint(1.0 + -asfloat(r0.w));
				r0.w = asuint(_ShadowBias.y * asfloat(r0.w));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www) + asfloat(r0.xyz));
				o_INTERP0 = asfloat(r1.xyz);
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0));
				o_SV_POSITION0.z = min(1.0, asfloat(r0.z));
				o_SV_POSITION0.xyw = asfloat(r0.xyw);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_INTERP0 : INTERP0, out float4 o_SV_TARGET0 : SV_TARGET0)
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

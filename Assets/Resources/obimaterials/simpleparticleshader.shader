Shader "Obi/Simple Particles"
{
	Properties
	{
		_Color ("Particle color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Pass
		{
			Name "ParticleFwdBase"
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Geometry" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DIRECTIONAL
			#pragma multi_compile __ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile __ LIGHTPROBE_SH
			#pragma multi_compile __ SHADOWS_SCREEN
			#pragma multi_compile __ SHADOWS_SHADOWMASK
			#pragma multi_compile __ VERTEXLIGHT_ON

			#if defined(SHADER_STAGE_VERTEX)
			#if (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(SHADOWS_SCREEN) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN)) || (defined(DIRECTIONAL) && defined(SHADOWS_SCREEN) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(SHADOWS_SCREEN))
			float _RadiusScale;
			float _GlobalsPad0;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[6];
			float4 _Color;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ProjectionParams : packoffset(c5);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
			}
			cbuffer UnityPerDraw : register(b3)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Position0 : SV_Position, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float3 o_TEXCOORD1 : TEXCOORD1, out float4 o_TEXCOORD4 : TEXCOORD4)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				r0 = asuint(unity_MatrixV._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_MatrixV._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_MatrixV._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_MatrixV._m03_m13_m23_m33 * i_POSITION0.wwww + asfloat(r0));
				r1.x = asuint(_RadiusScale * i_TEXCOORD0.w);
				r2.xy = asuint(i_NORMAL0.xy);
				r2.zw = uint2(0u, 0u);
				r0 = asuint(asfloat(r2) * asfloat(r1.xxxx) + asfloat(r0));
				r1 = asuint(glstate_matrix_projection._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(glstate_matrix_projection._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(glstate_matrix_projection._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(glstate_matrix_projection._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1));
				o_SV_Position0 = asfloat(r0);
				o_COLOR0 = _Color.xyzw * i_COLOR0.xyzw;
				o_TEXCOORD0 = i_NORMAL0.xy * float2(0.5, 0.5) + float2(0.5, 0.5);
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m10_m10_m10);
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m00_m00_m00 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m20_m20_m20 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m30_m30_m30 + asfloat(r1.xyz));
				r2.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m11_m11_m11);
				r2.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m01_m01_m01 + asfloat(r2.xyz));
				r2.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m21_m21_m21 + asfloat(r2.xyz));
				r2.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m31_m31_m31 + asfloat(r2.xyz));
				r3.xyz = asuint(unity_WorldToObject._m01_m11_m21 * _WorldSpaceLightPos0.yyy);
				r3.xyz = asuint(unity_WorldToObject._m00_m10_m20 * _WorldSpaceLightPos0.xxx + asfloat(r3.xyz));
				r3.xyz = asuint(unity_WorldToObject._m02_m12_m22 * _WorldSpaceLightPos0.zzz + asfloat(r3.xyz));
				r3.xyz = asuint(unity_WorldToObject._m03_m13_m23 * _WorldSpaceLightPos0.www + asfloat(r3.xyz));
				r2.xyz = asuint(asfloat(r3.yyy) * asfloat(r2.xyz));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r3.xxx) + asfloat(r2.xyz));
				r2.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m12_m12_m12);
				r2.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m02_m02_m02 + asfloat(r2.xyz));
				r2.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m22_m22_m22 + asfloat(r2.xyz));
				r2.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m32_m32_m32 + asfloat(r2.xyz));
				o_TEXCOORD1 = asfloat(r2.xyz) * asfloat(r3.zzz) + asfloat(r1.xyz);
				r0.y = asuint(_ProjectionParams.x * asfloat(r0.y));
				r1.xzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.xwy));
				o_TEXCOORD4.zw = asfloat(r0.zw);
				o_TEXCOORD4.xy = asfloat(r1.xw) + asfloat(r1.zz);
			}
			#elif (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SHADOWMASK)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH)) || (defined(DIRECTIONAL) && defined(VERTEXLIGHT_ON)) || defined(DIRECTIONAL)
			float _RadiusScale;
			float _GlobalsPad0;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[6];
			float4 _Color;
			cbuffer UnityLighting : register(b1)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Position0 : SV_Position, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float3 o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(unity_MatrixV._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_MatrixV._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_MatrixV._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_MatrixV._m03_m13_m23_m33 * i_POSITION0.wwww + asfloat(r0));
				r1.x = asuint(_RadiusScale * i_TEXCOORD0.w);
				r2.xy = asuint(i_NORMAL0.xy);
				r2.zw = uint2(0u, 0u);
				r0 = asuint(asfloat(r2) * asfloat(r1.xxxx) + asfloat(r0));
				r1 = asuint(glstate_matrix_projection._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(glstate_matrix_projection._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(glstate_matrix_projection._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_Position0 = glstate_matrix_projection._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				o_COLOR0 = _Color.xyzw * i_COLOR0.xyzw;
				o_TEXCOORD0 = i_NORMAL0.xy * float2(0.5, 0.5) + float2(0.5, 0.5);
				r0.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m10_m10_m10);
				r0.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m00_m00_m00 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m20_m20_m20 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m30_m30_m30 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m11_m11_m11);
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m01_m01_m01 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m21_m21_m21 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m31_m31_m31 + asfloat(r1.xyz));
				r2.xyz = asuint(unity_WorldToObject._m01_m11_m21 * _WorldSpaceLightPos0.yyy);
				r2.xyz = asuint(unity_WorldToObject._m00_m10_m20 * _WorldSpaceLightPos0.xxx + asfloat(r2.xyz));
				r2.xyz = asuint(unity_WorldToObject._m02_m12_m22 * _WorldSpaceLightPos0.zzz + asfloat(r2.xyz));
				r2.xyz = asuint(unity_WorldToObject._m03_m13_m23 * _WorldSpaceLightPos0.www + asfloat(r2.xyz));
				r1.xyz = asuint(asfloat(r2.yyy) * asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r2.xxx) + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m12_m12_m12);
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m02_m02_m02 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m22_m22_m22 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m32_m32_m32 + asfloat(r1.xyz));
				o_TEXCOORD1 = asfloat(r1.xyz) * asfloat(r2.zzz) + asfloat(r0.xyz);
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN)) || (defined(DIRECTIONAL) && defined(SHADOWS_SCREEN))
			float4 _GlobalsPad0[8];
			float4 _LightColor0;
			cbuffer UnityPerFrame : register(b1)
			{
				float4 glstate_lightmodel_ambient : packoffset(c0);
			}

			void frag(float4 i_SV_Position0 : SV_Position, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.x = asuint(dot(asfloat(r0.xy), asfloat(r0.xy)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				r1.z = asuint(sqrt(asfloat(r0.x)));
				if (b0.y) discard;
				r0.x = asuint(dot(i_TEXCOORD1.xyz, i_TEXCOORD1.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r0.xyz = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				r1.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.x = asuint(saturate(dot(asfloat(r1.xyz), asfloat(r0.xyz))));
				r0.yzw = asuint(glstate_lightmodel_ambient.xyz + glstate_lightmodel_ambient.xyz);
				r0.xyz = asuint(_LightColor0.xyz * asfloat(r0.xxx) + asfloat(r0.yzw));
				o_SV_Target0.xyz = i_COLOR0.xyz * asfloat(r0.xyz);
				o_SV_Target0.w = i_COLOR0.w;
			}
			#elif (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SHADOWMASK)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH)) || defined(DIRECTIONAL)
			float4 _GlobalsPad0[8];
			float4 _LightColor0;
			cbuffer UnityPerFrame : register(b1)
			{
				float4 glstate_lightmodel_ambient : packoffset(c0);
			}

			void frag(float4 i_SV_Position0 : SV_Position, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.x = asuint(dot(asfloat(r0.xy), asfloat(r0.xy)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				r1.z = asuint(sqrt(asfloat(r0.x)));
				if (b0.y) discard;
				r0.x = asuint(dot(i_TEXCOORD1.xyz, i_TEXCOORD1.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r0.xyz = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				r1.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.x = asuint(saturate(dot(asfloat(r1.xyz), asfloat(r0.xyz))));
				r0.yzw = asuint(glstate_lightmodel_ambient.xyz + glstate_lightmodel_ambient.xyz);
				r0.xyz = asuint(_LightColor0.xyz * asfloat(r0.xxx) + asfloat(r0.yzw));
				o_SV_Target0.xyz = i_COLOR0.xyz * asfloat(r0.xyz);
				o_SV_Target0.w = i_COLOR0.w;
			}
			#endif
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "ShadowCaster"
			Tags { "LIGHTMODE" = "SHADOWCASTER" "SHADOWSUPPORT" = "true" }
			Cull Back
			ZTest LEqual
			ZWrite On
			Offset 1, 1
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ SHADOWS_CUBE
			#pragma multi_compile __ SHADOWS_DEPTH

			#if defined(SHADER_STAGE_VERTEX)
			float _RadiusScale;
			cbuffer UnityPerFrame : register(b1)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Position0 : SV_Position, out float3 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(unity_MatrixV._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_MatrixV._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_MatrixV._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_MatrixV._m03_m13_m23_m33 * i_POSITION0.wwww + asfloat(r0));
				r1.xy = asuint(i_NORMAL0.xy);
				r1.zw = uint2(0u, 0u);
				r2.x = asuint(_RadiusScale * i_TEXCOORD0.w);
				r0 = asuint(asfloat(r1) * asfloat(r2.xxxx) + asfloat(r0));
				o_TEXCOORD0.z = asfloat(r2.x);
				r1 = asuint(glstate_matrix_projection._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(glstate_matrix_projection._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(glstate_matrix_projection._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_Position0 = glstate_matrix_projection._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				o_TEXCOORD0.xy = i_NORMAL0.xy * float2(0.5, 0.5) + float2(0.5, 0.5);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_Position0 : SV_Position, float3 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.x = asuint(dot(asfloat(r0.xy), asfloat(r0.xy)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Diffuse"
}

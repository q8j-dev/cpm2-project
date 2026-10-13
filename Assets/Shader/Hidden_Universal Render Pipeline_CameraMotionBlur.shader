Shader "Hidden/Universal Render Pipeline/CameraMotionBlur"
{
	Properties
	{
		_MaskCenter ("Radial Mask Center (UV)", Vector) = (0.5,0.5,0,0)
		_MaskRadius ("Radial Mask Radius", Float) = 0.25
		_MaskFeather ("Radial Mask Feather", Float) = 0.1
	}
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			Name "Camera Motion Blur - Low Quality"
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ _POSTFX_STENCIL_MASK

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[127];
			float4 _BlitScaleBias;

			void vert(uint i_SV_VertexID0 : SV_VertexID, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(asint(asfloat(i_SV_VertexID0)) << 1);
				r0.x = 2u & r0.x;
				r0.w = 2u & asuint(asfloat(i_SV_VertexID0));
				r0.xy = asuint(float2(r0.xw));
				r1.xy = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.z = asuint(1.0 + -asfloat(r0.y));
				o_TEXCOORD0.xy = asfloat(r0.xz) * _BlitScaleBias.xy + _BlitScaleBias.zw;
				o_SV_POSITION0.xy = asfloat(r1.xy);
				o_TEXCOORD0.zw = asfloat(r1.xy) * float2(0.5, 0.5) + float2(0.5, 0.5);
				o_SV_POSITION0.zw = float2(1.0, 1.0);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(_POSTFX_STENCIL_MASK)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[74];
			float4x4 unity_MatrixInvVP;
			float4 _GlobalsPad4[48];
			float4x4 _ViewProjM;
			float4x4 _PrevViewProjM;
			float _Intensity;
			float _Clamp;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _SourceSize;
			float2 _MaskCenter;
			float _MaskRadius;
			float _MaskFeather;
			Texture2D<float4> _BlitTexture : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			Texture2D<float4> _PostFxStencilMask : register(t2);
			SamplerState sampler_PointClamp : register(s0);
			SamplerState sampler_LinearClamp : register(s1);
			SamplerState sampler_CameraDepthTexture : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r1 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r0.yyyy));
				r0 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r0));
				r0 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r0));
				r0.xyz = asuint(asfloat(r0.xyz) / asfloat(r0.www));
				r1.xyz = asuint(_PrevViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r1.xyz = asuint(_PrevViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m03_m13_m33 + asfloat(r1.xyz));
				r1.xy = asuint(asfloat(r1.xy) / asfloat(r1.zz));
				r2.xyz = asuint(_ViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r0.xyw = asuint(_ViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r2.xyz));
				r0.xyz = asuint(_ViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(_ViewProjM._m03_m13_m33 + asfloat(r0.xyz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				r0.xy = asuint(asfloat(r1.xy) + -asfloat(r0.xy));
				r0.z = asuint(-asfloat(r0.y));
				r0.y = asuint(dot(asfloat(r0.xz), asfloat(r0.xz)));
				r0.y = asuint(sqrt(asfloat(r0.y)));
				r0.w = asuint(1.0 / asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.xz) * asfloat(r0.ww));
				r0.w = asuint(min(_Clamp, asfloat(r0.y)));
				b0.y = (0.0 < asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.ww) * asfloat(r0.xz));
				r0.xy = b0.yy ? r0.xz : uint2(0u, 0u);
				r0.xy = asuint(float2(_Intensity, _Intensity) * asfloat(r0.xy));
				r0.z = asuint(_SourceSize.y / _SourceSize.x);
				r1.yz = asuint(-_MaskCenter.xy + i_TEXCOORD0.xy);
				r1.x = asuint(asfloat(r1.y) * asfloat(r0.z));
				r0.z = asuint(dot(asfloat(r1.xz), asfloat(r1.xz)));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.z = asuint(-_MaskRadius + asfloat(r0.z));
				r0.w = asuint(max(9.99999975e-06, _MaskFeather));
				r0.w = asuint(1.0 / asfloat(r0.w));
				r0.z = asuint(saturate(asfloat(r0.z) * asfloat(r0.w)));
				r0.w = asuint(asfloat(r0.z) * -2.0 + 3.0);
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.z));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r1 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0.z = asuint(1.0 + -asfloat(r1.x));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.zw = asuint(_SourceSize.xy * i_TEXCOORD0.xy);
				r0.z = asuint(dot(asfloat(r0.zw), float2(0.0671105608, 0.00583714992)));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.z = asuint(52.9829178 * asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.w = asuint(0.25 * asfloat(r0.z));
				r1.xy = asuint(float2(-0.5, 1.0) + asfloat(r0.zz));
				r0.zw = asuint(asfloat(r0.ww) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r2 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r4 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r2.yzw = asuint(asfloat(r4.xyz) + -asfloat(r3.xyz));
				r2.xyz = asuint(asfloat(r2.xxx) * asfloat(r2.yzw) + asfloat(r3.xyz));
				r3 = asuint(float4(0.5, 0.5, 1.5, 1.5) + -asfloat(r1.xxxx));
				r0.z = asuint(0.25 * asfloat(r1.y));
				r0.zw = asuint(asfloat(r0.zz) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r1 = asuint(float4(0.25, 0.25, 0.25, 0.25) * asfloat(r3));
				r1 = asuint(i_TEXCOORD0.xyxy - asfloat(r1) * asfloat(r0.xyxy));
				r3 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r1.xy), _GlobalMipBias.x));
				r5 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r1.xy), _GlobalMipBias.x));
				r3.yzw = asuint(-asfloat(r5.xyz) + asfloat(r4.xyz));
				r3.xyz = asuint(asfloat(r3.xxx) * asfloat(r3.yzw) + asfloat(r5.xyz));
				r2.xyz = asuint(asfloat(r3.xyz) + asfloat(r2.xyz));
				r3 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r1.zw), _GlobalMipBias.x));
				r1 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r1.zw), _GlobalMipBias.x));
				r3.yzw = asuint(asfloat(r4.xyz) + -asfloat(r1.xyz));
				r1.xyz = asuint(asfloat(r3.xxx) * asfloat(r3.yzw) + asfloat(r1.xyz));
				r1.xyz = asuint(asfloat(r2.xyz) + asfloat(r1.xyz));
				r2 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r0 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r0.yzw = asuint(asfloat(r4.xyz) + -asfloat(r2.xyz));
				r0.xyz = asuint(asfloat(r0.xxx) * asfloat(r0.yzw) + asfloat(r2.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) + asfloat(r0.xyz));
				o_SV_Target0.xyz = float3(0.25, 0.25, 0.25) * asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#elif 1
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[74];
			float4x4 unity_MatrixInvVP;
			float4 _GlobalsPad4[48];
			float4x4 _ViewProjM;
			float4x4 _PrevViewProjM;
			float _Intensity;
			float _Clamp;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _SourceSize;
			float2 _MaskCenter;
			float _MaskRadius;
			float _MaskFeather;
			Texture2D<float4> _BlitTexture : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			SamplerState sampler_PointClamp : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r1 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r0.yyyy));
				r0 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r0));
				r0 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r0));
				r0.xyz = asuint(asfloat(r0.xyz) / asfloat(r0.www));
				r1.xyz = asuint(_PrevViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r1.xyz = asuint(_PrevViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m03_m13_m33 + asfloat(r1.xyz));
				r1.xy = asuint(asfloat(r1.xy) / asfloat(r1.zz));
				r2.xyz = asuint(_ViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r0.xyw = asuint(_ViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r2.xyz));
				r0.xyz = asuint(_ViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(_ViewProjM._m03_m13_m33 + asfloat(r0.xyz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				r0.xy = asuint(asfloat(r1.xy) + -asfloat(r0.xy));
				r0.z = asuint(-asfloat(r0.y));
				r0.y = asuint(dot(asfloat(r0.xz), asfloat(r0.xz)));
				r0.y = asuint(sqrt(asfloat(r0.y)));
				r0.w = asuint(1.0 / asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.xz) * asfloat(r0.ww));
				r0.w = asuint(min(_Clamp, asfloat(r0.y)));
				b0.y = (0.0 < asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.ww) * asfloat(r0.xz));
				r0.xy = b0.yy ? r0.xz : uint2(0u, 0u);
				r0.xy = asuint(float2(_Intensity, _Intensity) * asfloat(r0.xy));
				r0.z = asuint(_SourceSize.y / _SourceSize.x);
				r1.yz = asuint(-_MaskCenter.xy + i_TEXCOORD0.xy);
				r1.x = asuint(asfloat(r1.y) * asfloat(r0.z));
				r0.z = asuint(dot(asfloat(r1.xz), asfloat(r1.xz)));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.z = asuint(-_MaskRadius + asfloat(r0.z));
				r0.w = asuint(max(9.99999975e-06, _MaskFeather));
				r0.w = asuint(1.0 / asfloat(r0.w));
				r0.z = asuint(saturate(asfloat(r0.z) * asfloat(r0.w)));
				r0.w = asuint(asfloat(r0.z) * -2.0 + 3.0);
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.z));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.zw = asuint(_SourceSize.xy * i_TEXCOORD0.xy);
				r0.z = asuint(dot(asfloat(r0.zw), float2(0.0671105608, 0.00583714992)));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.z = asuint(52.9829178 * asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.w = asuint(0.25 * asfloat(r0.z));
				r1.xy = asuint(float2(-0.5, 1.0) + asfloat(r0.zz));
				r0.zw = asuint(asfloat(r0.ww) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r2 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r3 = asuint(float4(0.5, 0.5, 1.5, 1.5) + -asfloat(r1.xxxx));
				r0.z = asuint(0.25 * asfloat(r1.y));
				r0.zw = asuint(asfloat(r0.zz) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r1 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r3 = asuint(float4(0.25, 0.25, 0.25, 0.25) * asfloat(r3));
				r0 = asuint(i_TEXCOORD0.xyxy - asfloat(r3) * asfloat(r0.xyxy));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.xy), _GlobalMipBias.x));
				r0 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r2.xyz = asuint(asfloat(r3.xyz) + asfloat(r2.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r0.xyz) + asfloat(r1.xyz));
				o_SV_Target0.xyz = float3(0.25, 0.25, 0.25) * asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#endif
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "Camera Motion Blur - Medium Quality"
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ _POSTFX_STENCIL_MASK

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[127];
			float4 _BlitScaleBias;

			void vert(uint i_SV_VertexID0 : SV_VertexID, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(asint(asfloat(i_SV_VertexID0)) << 1);
				r0.x = 2u & r0.x;
				r0.w = 2u & asuint(asfloat(i_SV_VertexID0));
				r0.xy = asuint(float2(r0.xw));
				r1.xy = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.z = asuint(1.0 + -asfloat(r0.y));
				o_TEXCOORD0.xy = asfloat(r0.xz) * _BlitScaleBias.xy + _BlitScaleBias.zw;
				o_SV_POSITION0.xy = asfloat(r1.xy);
				o_TEXCOORD0.zw = asfloat(r1.xy) * float2(0.5, 0.5) + float2(0.5, 0.5);
				o_SV_POSITION0.zw = float2(1.0, 1.0);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(_POSTFX_STENCIL_MASK)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[74];
			float4x4 unity_MatrixInvVP;
			float4 _GlobalsPad4[48];
			float4x4 _ViewProjM;
			float4x4 _PrevViewProjM;
			float _Intensity;
			float _Clamp;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _SourceSize;
			float2 _MaskCenter;
			float _MaskRadius;
			float _MaskFeather;
			Texture2D<float4> _BlitTexture : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			Texture2D<float4> _PostFxStencilMask : register(t2);
			SamplerState sampler_PointClamp : register(s0);
			SamplerState sampler_LinearClamp : register(s1);
			SamplerState sampler_CameraDepthTexture : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r1 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r0.yyyy));
				r0 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r0));
				r0 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r0));
				r0.xyz = asuint(asfloat(r0.xyz) / asfloat(r0.www));
				r1.xyz = asuint(_PrevViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r1.xyz = asuint(_PrevViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m03_m13_m33 + asfloat(r1.xyz));
				r1.xy = asuint(asfloat(r1.xy) / asfloat(r1.zz));
				r2.xyz = asuint(_ViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r0.xyw = asuint(_ViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r2.xyz));
				r0.xyz = asuint(_ViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(_ViewProjM._m03_m13_m33 + asfloat(r0.xyz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				r0.xy = asuint(asfloat(r1.xy) + -asfloat(r0.xy));
				r0.z = asuint(-asfloat(r0.y));
				r0.y = asuint(dot(asfloat(r0.xz), asfloat(r0.xz)));
				r0.y = asuint(sqrt(asfloat(r0.y)));
				r0.w = asuint(1.0 / asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.xz) * asfloat(r0.ww));
				r0.w = asuint(min(_Clamp, asfloat(r0.y)));
				b0.y = (0.0 < asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.ww) * asfloat(r0.xz));
				r0.xy = b0.yy ? r0.xz : uint2(0u, 0u);
				r0.xy = asuint(float2(_Intensity, _Intensity) * asfloat(r0.xy));
				r0.z = asuint(_SourceSize.y / _SourceSize.x);
				r1.yz = asuint(-_MaskCenter.xy + i_TEXCOORD0.xy);
				r1.x = asuint(asfloat(r1.y) * asfloat(r0.z));
				r0.z = asuint(dot(asfloat(r1.xz), asfloat(r1.xz)));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.z = asuint(-_MaskRadius + asfloat(r0.z));
				r0.w = asuint(max(9.99999975e-06, _MaskFeather));
				r0.w = asuint(1.0 / asfloat(r0.w));
				r0.z = asuint(saturate(asfloat(r0.z) * asfloat(r0.w)));
				r0.w = asuint(asfloat(r0.z) * -2.0 + 3.0);
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.z));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r1 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0.z = asuint(1.0 + -asfloat(r1.x));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.zw = asuint(_SourceSize.xy * i_TEXCOORD0.xy);
				r0.z = asuint(dot(asfloat(r0.zw), float2(0.0671105608, 0.00583714992)));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.z = asuint(52.9829178 * asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.w = asuint(0.166666672 * asfloat(r0.z));
				r1.xyz = asuint(float3(-0.5, 1.0, 2.0) + asfloat(r0.zzz));
				r0.zw = asuint(asfloat(r0.ww) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r2 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r4 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r2.yzw = asuint(asfloat(r4.xyz) + -asfloat(r3.xyz));
				r2.xyz = asuint(asfloat(r2.xxx) * asfloat(r2.yzw) + asfloat(r3.xyz));
				r3.xyz = asuint(float3(0.5, 1.5, 2.5) + -asfloat(r1.xxx));
				r1 = asuint(float4(0.166666672, 0.166666672, 0.166666672, 0.166666672) * asfloat(r1.yyzz));
				r1 = asuint(asfloat(r1) * asfloat(r0.xyxy) + i_TEXCOORD0.xyxy);
				r3.xyz = asuint(float3(0.166666672, 0.166666672, 0.166666672) * asfloat(r3.xyz));
				r5 = asuint(i_TEXCOORD0.xyxy - asfloat(r3.xxyy) * asfloat(r0.xyxy));
				r0.xy = asuint(i_TEXCOORD0.xy - asfloat(r3.zz) * asfloat(r0.xy));
				r3 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r5.xy), _GlobalMipBias.x));
				r6 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r5.xy), _GlobalMipBias.x));
				r3.yzw = asuint(-asfloat(r6.xyz) + asfloat(r4.xyz));
				r3.xyz = asuint(asfloat(r3.xxx) * asfloat(r3.yzw) + asfloat(r6.xyz));
				r2.xyz = asuint(asfloat(r3.xyz) + asfloat(r2.xyz));
				r3 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r5.zw), _GlobalMipBias.x));
				r5 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r5.zw), _GlobalMipBias.x));
				r3.yzw = asuint(-asfloat(r5.xyz) + asfloat(r4.xyz));
				r3.xyz = asuint(asfloat(r3.xxx) * asfloat(r3.yzw) + asfloat(r5.xyz));
				r2.xyz = asuint(asfloat(r3.xyz) + asfloat(r2.xyz));
				r3 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r1.xy), _GlobalMipBias.x));
				r5 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r1.xy), _GlobalMipBias.x));
				r3.yzw = asuint(-asfloat(r5.xyz) + asfloat(r4.xyz));
				r3.xyz = asuint(asfloat(r3.xxx) * asfloat(r3.yzw) + asfloat(r5.xyz));
				r2.xyz = asuint(asfloat(r3.xyz) + asfloat(r2.xyz));
				r3 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r0.xy), _GlobalMipBias.x));
				r0 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.xy), _GlobalMipBias.x));
				r3.yzw = asuint(asfloat(r4.xyz) + -asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r3.xxx) * asfloat(r3.yzw) + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) + asfloat(r0.xyz));
				r2 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r1.zw), _GlobalMipBias.x));
				r1 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r1.zw), _GlobalMipBias.x));
				r1.yzw = asuint(asfloat(r4.xyz) + -asfloat(r2.xyz));
				r1.xyz = asuint(asfloat(r1.xxx) * asfloat(r1.yzw) + asfloat(r2.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) + asfloat(r0.xyz));
				o_SV_Target0.xyz = float3(0.166666672, 0.166666672, 0.166666672) * asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#elif 1
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[74];
			float4x4 unity_MatrixInvVP;
			float4 _GlobalsPad4[48];
			float4x4 _ViewProjM;
			float4x4 _PrevViewProjM;
			float _Intensity;
			float _Clamp;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _SourceSize;
			float2 _MaskCenter;
			float _MaskRadius;
			float _MaskFeather;
			Texture2D<float4> _BlitTexture : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			SamplerState sampler_PointClamp : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r1 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r0.yyyy));
				r0 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r0));
				r0 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r0));
				r0.xyz = asuint(asfloat(r0.xyz) / asfloat(r0.www));
				r1.xyz = asuint(_PrevViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r1.xyz = asuint(_PrevViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m03_m13_m33 + asfloat(r1.xyz));
				r1.xy = asuint(asfloat(r1.xy) / asfloat(r1.zz));
				r2.xyz = asuint(_ViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r0.xyw = asuint(_ViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r2.xyz));
				r0.xyz = asuint(_ViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(_ViewProjM._m03_m13_m33 + asfloat(r0.xyz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				r0.xy = asuint(asfloat(r1.xy) + -asfloat(r0.xy));
				r0.z = asuint(-asfloat(r0.y));
				r0.y = asuint(dot(asfloat(r0.xz), asfloat(r0.xz)));
				r0.y = asuint(sqrt(asfloat(r0.y)));
				r0.w = asuint(1.0 / asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.xz) * asfloat(r0.ww));
				r0.w = asuint(min(_Clamp, asfloat(r0.y)));
				b0.y = (0.0 < asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.ww) * asfloat(r0.xz));
				r0.xy = b0.yy ? r0.xz : uint2(0u, 0u);
				r0.xy = asuint(float2(_Intensity, _Intensity) * asfloat(r0.xy));
				r0.z = asuint(_SourceSize.y / _SourceSize.x);
				r1.yz = asuint(-_MaskCenter.xy + i_TEXCOORD0.xy);
				r1.x = asuint(asfloat(r1.y) * asfloat(r0.z));
				r0.z = asuint(dot(asfloat(r1.xz), asfloat(r1.xz)));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.z = asuint(-_MaskRadius + asfloat(r0.z));
				r0.w = asuint(max(9.99999975e-06, _MaskFeather));
				r0.w = asuint(1.0 / asfloat(r0.w));
				r0.z = asuint(saturate(asfloat(r0.z) * asfloat(r0.w)));
				r0.w = asuint(asfloat(r0.z) * -2.0 + 3.0);
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.z));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.zw = asuint(_SourceSize.xy * i_TEXCOORD0.xy);
				r0.z = asuint(dot(asfloat(r0.zw), float2(0.0671105608, 0.00583714992)));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.z = asuint(52.9829178 * asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.w = asuint(0.166666672 * asfloat(r0.z));
				r1.xyz = asuint(float3(-0.5, 1.0, 2.0) + asfloat(r0.zzz));
				r0.zw = asuint(asfloat(r0.ww) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r2 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r3.xyz = asuint(float3(0.5, 1.5, 2.5) + -asfloat(r1.xxx));
				r1 = asuint(float4(0.166666672, 0.166666672, 0.166666672, 0.166666672) * asfloat(r1.yyzz));
				r1 = asuint(asfloat(r1) * asfloat(r0.xyxy) + i_TEXCOORD0.xyxy);
				r3.xyz = asuint(float3(0.166666672, 0.166666672, 0.166666672) * asfloat(r3.xyz));
				r4 = asuint(i_TEXCOORD0.xyxy - asfloat(r3.xxyy) * asfloat(r0.xyxy));
				r0.xy = asuint(i_TEXCOORD0.xy - asfloat(r3.zz) * asfloat(r0.xy));
				r0 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.xy), _GlobalMipBias.x));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r4.xy), _GlobalMipBias.x));
				r4 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r4.zw), _GlobalMipBias.x));
				r2.xyz = asuint(asfloat(r3.xyz) + asfloat(r2.xyz));
				r2.xyz = asuint(asfloat(r2.xyz) + asfloat(r4.xyz));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r1.xy), _GlobalMipBias.x));
				r1 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r1.zw), _GlobalMipBias.x));
				r2.xyz = asuint(asfloat(r3.xyz) + asfloat(r2.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r0.xyz) + asfloat(r1.xyz));
				o_SV_Target0.xyz = float3(0.166666672, 0.166666672, 0.166666672) * asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#endif
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "Camera Motion Blur - High Quality"
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ _POSTFX_STENCIL_MASK

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[127];
			float4 _BlitScaleBias;

			void vert(uint i_SV_VertexID0 : SV_VertexID, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(asint(asfloat(i_SV_VertexID0)) << 1);
				r0.x = 2u & r0.x;
				r0.w = 2u & asuint(asfloat(i_SV_VertexID0));
				r0.xy = asuint(float2(r0.xw));
				r1.xy = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.z = asuint(1.0 + -asfloat(r0.y));
				o_TEXCOORD0.xy = asfloat(r0.xz) * _BlitScaleBias.xy + _BlitScaleBias.zw;
				o_SV_POSITION0.xy = asfloat(r1.xy);
				o_TEXCOORD0.zw = asfloat(r1.xy) * float2(0.5, 0.5) + float2(0.5, 0.5);
				o_SV_POSITION0.zw = float2(1.0, 1.0);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(_POSTFX_STENCIL_MASK)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[74];
			float4x4 unity_MatrixInvVP;
			float4 _GlobalsPad4[48];
			float4x4 _ViewProjM;
			float4x4 _PrevViewProjM;
			float _Intensity;
			float _Clamp;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _SourceSize;
			float2 _MaskCenter;
			float _MaskRadius;
			float _MaskFeather;
			Texture2D<float4> _BlitTexture : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			Texture2D<float4> _PostFxStencilMask : register(t2);
			SamplerState sampler_PointClamp : register(s0);
			SamplerState sampler_LinearClamp : register(s1);
			SamplerState sampler_CameraDepthTexture : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				uint4 r7;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r1 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r0.yyyy));
				r0 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r0));
				r0 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r0));
				r0.xyz = asuint(asfloat(r0.xyz) / asfloat(r0.www));
				r1.xyz = asuint(_PrevViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r1.xyz = asuint(_PrevViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m03_m13_m33 + asfloat(r1.xyz));
				r1.xy = asuint(asfloat(r1.xy) / asfloat(r1.zz));
				r2.xyz = asuint(_ViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r0.xyw = asuint(_ViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r2.xyz));
				r0.xyz = asuint(_ViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(_ViewProjM._m03_m13_m33 + asfloat(r0.xyz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				r0.xy = asuint(asfloat(r1.xy) + -asfloat(r0.xy));
				r0.z = asuint(-asfloat(r0.y));
				r0.y = asuint(dot(asfloat(r0.xz), asfloat(r0.xz)));
				r0.y = asuint(sqrt(asfloat(r0.y)));
				r0.w = asuint(1.0 / asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.xz) * asfloat(r0.ww));
				r0.w = asuint(min(_Clamp, asfloat(r0.y)));
				b0.y = (0.0 < asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.ww) * asfloat(r0.xz));
				r0.xy = b0.yy ? r0.xz : uint2(0u, 0u);
				r0.xy = asuint(float2(_Intensity, _Intensity) * asfloat(r0.xy));
				r0.z = asuint(_SourceSize.y / _SourceSize.x);
				r1.yz = asuint(-_MaskCenter.xy + i_TEXCOORD0.xy);
				r1.x = asuint(asfloat(r1.y) * asfloat(r0.z));
				r0.z = asuint(dot(asfloat(r1.xz), asfloat(r1.xz)));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.z = asuint(-_MaskRadius + asfloat(r0.z));
				r0.w = asuint(max(9.99999975e-06, _MaskFeather));
				r0.w = asuint(1.0 / asfloat(r0.w));
				r0.z = asuint(saturate(asfloat(r0.z) * asfloat(r0.w)));
				r0.w = asuint(asfloat(r0.z) * -2.0 + 3.0);
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.z));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r1 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0.z = asuint(1.0 + -asfloat(r1.x));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.zw = asuint(_SourceSize.xy * i_TEXCOORD0.xy);
				r0.z = asuint(dot(asfloat(r0.zw), float2(0.0671105608, 0.00583714992)));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.z = asuint(52.9829178 * asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.w = asuint(0.125 * asfloat(r0.z));
				r1 = asuint(float4(-0.5, 1.0, 2.0, 3.0) + asfloat(r0.zzzz));
				r0.zw = asuint(asfloat(r0.ww) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r2 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r4 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r2.yzw = asuint(asfloat(r4.xyz) + -asfloat(r3.xyz));
				r2.xyz = asuint(asfloat(r2.xxx) * asfloat(r2.yzw) + asfloat(r3.xyz));
				r3 = asuint(float4(0.5, 1.5, 2.5, 3.5) + -asfloat(r1.xxxx));
				r1.xyz = asuint(float3(0.125, 0.125, 0.125) * asfloat(r1.yzw));
				r3 = asuint(float4(0.125, 0.125, 0.125, 0.125) * asfloat(r3));
				r5 = asuint(i_TEXCOORD0.xyxy - asfloat(r3.xxyy) * asfloat(r0.xyxy));
				r3 = asuint(i_TEXCOORD0.xyxy - asfloat(r3.zzww) * asfloat(r0.xyxy));
				r6 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r5.xy), _GlobalMipBias.x));
				r7 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r5.xy), _GlobalMipBias.x));
				r6.yzw = asuint(-asfloat(r7.xyz) + asfloat(r4.xyz));
				r6.xyz = asuint(asfloat(r6.xxx) * asfloat(r6.yzw) + asfloat(r7.xyz));
				r2.xyz = asuint(asfloat(r6.xyz) + asfloat(r2.xyz));
				r6 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r5.zw), _GlobalMipBias.x));
				r5 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r5.zw), _GlobalMipBias.x));
				r6.yzw = asuint(-asfloat(r5.xyz) + asfloat(r4.xyz));
				r5.xyz = asuint(asfloat(r6.xxx) * asfloat(r6.yzw) + asfloat(r5.xyz));
				r2.xyz = asuint(asfloat(r5.xyz) + asfloat(r2.xyz));
				r5 = asuint(asfloat(r1.xxyy) * asfloat(r0.xyxy) + i_TEXCOORD0.xyxy);
				r0.xy = asuint(asfloat(r1.zz) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r1 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r5.xy), _GlobalMipBias.x));
				r6 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r5.xy), _GlobalMipBias.x));
				r1.yzw = asuint(-asfloat(r6.xyz) + asfloat(r4.xyz));
				r1.xyz = asuint(asfloat(r1.xxx) * asfloat(r1.yzw) + asfloat(r6.xyz));
				r1.xyz = asuint(asfloat(r2.xyz) + asfloat(r1.xyz));
				r2 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r3.xy), _GlobalMipBias.x));
				r6 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r3.xy), _GlobalMipBias.x));
				r2.yzw = asuint(-asfloat(r6.xyz) + asfloat(r4.xyz));
				r2.xyz = asuint(asfloat(r2.xxx) * asfloat(r2.yzw) + asfloat(r6.xyz));
				r1.xyz = asuint(asfloat(r2.xyz) + asfloat(r1.xyz));
				r2 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r5.zw), _GlobalMipBias.x));
				r5 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r5.zw), _GlobalMipBias.x));
				r2.yzw = asuint(-asfloat(r5.xyz) + asfloat(r4.xyz));
				r2.xyz = asuint(asfloat(r2.xxx) * asfloat(r2.yzw) + asfloat(r5.xyz));
				r1.xyz = asuint(asfloat(r2.xyz) + asfloat(r1.xyz));
				r2 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r3.zw), _GlobalMipBias.x));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r3.zw), _GlobalMipBias.x));
				r2.yzw = asuint(asfloat(r4.xyz) + -asfloat(r3.xyz));
				r2.xyz = asuint(asfloat(r2.xxx) * asfloat(r2.yzw) + asfloat(r3.xyz));
				r1.xyz = asuint(asfloat(r2.xyz) + asfloat(r1.xyz));
				r2 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.xy), _GlobalMipBias.x));
				r0 = asuint(_PostFxStencilMask.SampleBias(sampler_LinearClamp, asfloat(r0.xy), _GlobalMipBias.x));
				r0.yzw = asuint(asfloat(r4.xyz) + -asfloat(r2.xyz));
				r0.xyz = asuint(asfloat(r0.xxx) * asfloat(r0.yzw) + asfloat(r2.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) + asfloat(r0.xyz));
				o_SV_Target0.xyz = float3(0.125, 0.125, 0.125) * asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#elif 1
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[74];
			float4x4 unity_MatrixInvVP;
			float4 _GlobalsPad4[48];
			float4x4 _ViewProjM;
			float4x4 _PrevViewProjM;
			float _Intensity;
			float _Clamp;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _SourceSize;
			float2 _MaskCenter;
			float _MaskRadius;
			float _MaskFeather;
			Texture2D<float4> _BlitTexture : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			SamplerState sampler_PointClamp : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r1 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r0.yyyy));
				r0 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r0));
				r0 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r0));
				r0.xyz = asuint(asfloat(r0.xyz) / asfloat(r0.www));
				r1.xyz = asuint(_PrevViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r1.xyz = asuint(_PrevViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjM._m03_m13_m33 + asfloat(r1.xyz));
				r1.xy = asuint(asfloat(r1.xy) / asfloat(r1.zz));
				r2.xyz = asuint(_ViewProjM._m01_m11_m31 * asfloat(r0.yyy));
				r0.xyw = asuint(_ViewProjM._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r2.xyz));
				r0.xyz = asuint(_ViewProjM._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(_ViewProjM._m03_m13_m33 + asfloat(r0.xyz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				r0.xy = asuint(asfloat(r1.xy) + -asfloat(r0.xy));
				r0.z = asuint(-asfloat(r0.y));
				r0.y = asuint(dot(asfloat(r0.xz), asfloat(r0.xz)));
				r0.y = asuint(sqrt(asfloat(r0.y)));
				r0.w = asuint(1.0 / asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.xz) * asfloat(r0.ww));
				r0.w = asuint(min(_Clamp, asfloat(r0.y)));
				b0.y = (0.0 < asfloat(r0.y));
				r0.xz = asuint(asfloat(r0.ww) * asfloat(r0.xz));
				r0.xy = b0.yy ? r0.xz : uint2(0u, 0u);
				r0.xy = asuint(float2(_Intensity, _Intensity) * asfloat(r0.xy));
				r0.z = asuint(_SourceSize.y / _SourceSize.x);
				r1.yz = asuint(-_MaskCenter.xy + i_TEXCOORD0.xy);
				r1.x = asuint(asfloat(r1.y) * asfloat(r0.z));
				r0.z = asuint(dot(asfloat(r1.xz), asfloat(r1.xz)));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.z = asuint(-_MaskRadius + asfloat(r0.z));
				r0.w = asuint(max(9.99999975e-06, _MaskFeather));
				r0.w = asuint(1.0 / asfloat(r0.w));
				r0.z = asuint(saturate(asfloat(r0.z) * asfloat(r0.w)));
				r0.w = asuint(asfloat(r0.z) * -2.0 + 3.0);
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.z));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.zw = asuint(_SourceSize.xy * i_TEXCOORD0.xy);
				r0.z = asuint(dot(asfloat(r0.zw), float2(0.0671105608, 0.00583714992)));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.z = asuint(52.9829178 * asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.w = asuint(0.125 * asfloat(r0.z));
				r1 = asuint(float4(-0.5, 1.0, 2.0, 3.0) + asfloat(r0.zzzz));
				r0.zw = asuint(asfloat(r0.ww) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r2 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.zw), _GlobalMipBias.x));
				r3 = asuint(float4(0.5, 1.5, 2.5, 3.5) + -asfloat(r1.xxxx));
				r1.xyz = asuint(float3(0.125, 0.125, 0.125) * asfloat(r1.yzw));
				r3 = asuint(float4(0.125, 0.125, 0.125, 0.125) * asfloat(r3));
				r4 = asuint(i_TEXCOORD0.xyxy - asfloat(r3.xxyy) * asfloat(r0.xyxy));
				r3 = asuint(i_TEXCOORD0.xyxy - asfloat(r3.zzww) * asfloat(r0.xyxy));
				r5 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r4.xy), _GlobalMipBias.x));
				r4 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r4.zw), _GlobalMipBias.x));
				r2.xyz = asuint(asfloat(r5.xyz) + asfloat(r2.xyz));
				r2.xyz = asuint(asfloat(r2.xyz) + asfloat(r4.xyz));
				r4 = asuint(asfloat(r1.xxyy) * asfloat(r0.xyxy) + i_TEXCOORD0.xyxy);
				r0.xy = asuint(asfloat(r1.zz) * asfloat(r0.xy) + i_TEXCOORD0.xy);
				r0 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r0.xy), _GlobalMipBias.x));
				r1 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r4.xy), _GlobalMipBias.x));
				r4 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r4.zw), _GlobalMipBias.x));
				r1.xyz = asuint(asfloat(r2.xyz) + asfloat(r1.xyz));
				r2 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r3.xy), _GlobalMipBias.x));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, asfloat(r3.zw), _GlobalMipBias.x));
				r1.xyz = asuint(asfloat(r2.xyz) + asfloat(r1.xyz));
				r1.xyz = asuint(asfloat(r1.xyz) + asfloat(r4.xyz));
				r1.xyz = asuint(asfloat(r1.xyz) + asfloat(r3.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) + asfloat(r0.xyz));
				o_SV_Target0.xyz = float3(0.125, 0.125, 0.125) * asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}

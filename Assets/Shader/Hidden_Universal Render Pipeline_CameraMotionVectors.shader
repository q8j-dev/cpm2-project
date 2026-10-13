Shader "Hidden/Universal Render Pipeline/CameraMotionVectors"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			Name "Camera Motion Vectors"
			Cull Off
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)

			void vert(uint i_SV_VertexID0 : SV_VertexID, out float4 o_SV_POSITION0 : SV_POSITION)
			{
				uint4 r0;
				r0.x = asuint(asint(asfloat(i_SV_VertexID0)) << 1);
				r0.x = 2u & r0.x;
				r0.z = 2u & asuint(asfloat(i_SV_VertexID0));
				r0.xy = asuint(float2(r0.xz));
				o_SV_POSITION0.xy = asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0);
				o_SV_POSITION0.zw = float2(1.0, 1.0);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[4];
			float4 _ScaledScreenParams;
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[74];
			float4x4 unity_MatrixInvVP;
			float4 _GlobalsPad4[3];
			float4x4 _PrevViewProjMatrix;
			float4x4 _NonJitteredViewProjMatrix;
			Texture2D<float4> _CameraDepthTexture : register(t0);
			SamplerState sampler_CameraDepthTexture : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xy = asuint(i_SV_POSITION0.xy / _ScaledScreenParams.xy);
				r0.zw = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r0.xy), _GlobalMipBias.x));
				r2 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r0.wwww));
				r0 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r0.zzzz) + asfloat(r2));
				r0 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r0));
				o_SV_Depth0 = asfloat(r1.x);
				r0 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r0));
				r0.xyz = asuint(asfloat(r0.xyz) / asfloat(r0.www));
				r1.xyz = asuint(_PrevViewProjMatrix._m01_m11_m31 * asfloat(r0.yyy));
				r1.xyz = asuint(_PrevViewProjMatrix._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjMatrix._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r1.xyz));
				r1.xyz = asuint(_PrevViewProjMatrix._m03_m13_m33 + asfloat(r1.xyz));
				r0.w = asuint(1.0 / asfloat(r1.z));
				r1.xy = asuint(asfloat(r1.xy) * asfloat(r0.ww));
				r2.xyz = asuint(_NonJitteredViewProjMatrix._m01_m11_m31 * asfloat(r0.yyy));
				r0.xyw = asuint(_NonJitteredViewProjMatrix._m00_m10_m30 * asfloat(r0.xxx) + asfloat(r2.xyz));
				r0.xyz = asuint(_NonJitteredViewProjMatrix._m02_m12_m32 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(_NonJitteredViewProjMatrix._m03_m13_m33 + asfloat(r0.xyz));
				r0.z = asuint(1.0 / asfloat(r0.z));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz) + -asfloat(r1.xy));
				o_SV_Target0.xy = float2(0.5, -0.5) * asfloat(r0.xy);
				o_SV_Target0.zw = float2(0.0, 0.0);
			}
			#endif
			ENDHLSL
		}
	}
}

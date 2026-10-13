Shader "Hidden/Universal Render Pipeline/Scaling Setup"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			Name "ScalingSetup"
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ _FXAA
			#pragma multi_compile __ _FXAA_AND_GAMMA_20
			#pragma multi_compile __ _GAMMA_20

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[127];
			float4 _BlitScaleBias;

			void vert(uint i_SV_VertexID0 : SV_VertexID, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				r0.x = asuint(asint(asfloat(i_SV_VertexID0)) << 1);
				r0.x = 2u & r0.x;
				r0.w = 2u & asuint(asfloat(i_SV_VertexID0));
				r0.xy = asuint(float2(r0.xw));
				o_SV_POSITION0.xy = asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0);
				r0.z = asuint(1.0 + -asfloat(r0.y));
				o_TEXCOORD0 = asfloat(r0.xz) * _BlitScaleBias.xy + _BlitScaleBias.zw;
				o_SV_POSITION0.zw = float2(1.0, 1.0);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(_FXAA)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[126];
			float4 _SourceSize;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_PointClamp : register(s0);
			SamplerState sampler_LinearClamp : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				bool4 b0;
				bool4 b1;
				r0 = asuint(_SourceSize.xyxy * i_TEXCOORD0.xyxy);
				r0 = asuint(int4(asfloat(r0)));
				r1 = asuint(int4(-1, -1, 1, -1) + asint(r0.zwzw));
				r0 = asuint(int4(-1, 1, 1, 1) + asint(r0));
				r0 = asuint(float4(asint(r0)));
				r0 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r0)));
				r1 = asuint(float4(asint(r1)));
				r1 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r1)));
				r2 = asuint(float4(-1.0, -1.0, -1.0, -1.0) + _SourceSize.xyxy);
				r1 = asuint(min(asfloat(r2), asfloat(r1)));
				r0 = asuint(min(asfloat(r2), asfloat(r0)));
				r0 = asuint(int4(asfloat(r0.zwxy)));
				r1 = asuint(int4(asfloat(r1.zwxy)));
				r2.xy = r1.zw;
				r2.zw = uint2(0u, 0u);
				r2 = asuint(_BlitTexture.Load(int3(asint(r2.xy), asint(r2.w))));
				r2.xyz = asuint(saturate(asfloat(r2.xyz)));
				r2.x = asuint(dot(asfloat(r2.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r3.xy = r0.zw;
				r3.zw = uint2(0u, 0u);
				r3 = asuint(_BlitTexture.Load(int3(asint(r3.xy), asint(r3.w))));
				r3.xyz = asuint(saturate(asfloat(r3.xyz)));
				r2.y = asuint(dot(asfloat(r3.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r2.z = asuint(asfloat(r2.x) + asfloat(r2.y));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(_BlitTexture.Load(int3(asint(r1.xy), asint(r1.w))));
				r1.xyz = asuint(saturate(asfloat(r1.xyz)));
				r1.x = asuint(dot(asfloat(r1.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r0.zw = uint2(0u, 0u);
				r0 = asuint(_BlitTexture.Load(int3(asint(r0.xy), asint(r0.w))));
				r0.xyz = asuint(saturate(asfloat(r0.xyz)));
				r0.x = asuint(dot(asfloat(r0.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r0.y = asuint(asfloat(r1.x) + asfloat(r0.x));
				r3.yw = asuint(asfloat(r2.zz) + -asfloat(r0.yy));
				r0.y = asuint(asfloat(r2.x) + asfloat(r1.x));
				r0.z = asuint(asfloat(r2.y) + asfloat(r0.x));
				r0.z = asuint(asfloat(r0.y) + -asfloat(r0.z));
				r0.y = asuint(asfloat(r0.y) + asfloat(r2.y));
				r0.y = asuint(asfloat(r0.y) + asfloat(r0.x));
				r0.y = asuint(0.03125 * asfloat(r0.y));
				r0.y = asuint(max(0.0078125, asfloat(r0.y)));
				r0.w = asuint(min(abs(asfloat(r0.z)), abs(asfloat(r3.w))));
				r3.xz = asuint(-asfloat(r0.zz));
				r0.y = asuint(asfloat(r0.w) + asfloat(r0.y));
				r0.y = asuint(1.0 / asfloat(r0.y));
				r3 = asuint(asfloat(r3) * asfloat(r0.yyyy));
				r3 = asuint(max(float4(-8.0, -8.0, -8.0, -8.0), asfloat(r3)));
				r3 = asuint(min(float4(8.0, 8.0, 8.0, 8.0), asfloat(r3)));
				r3 = asuint(_SourceSize.zwzw * asfloat(r3));
				r4 = asuint(asfloat(r3.zwzw) * float4(-0.5, -0.5, -0.166666672, -0.166666672) + i_TEXCOORD0.xyxy);
				r3 = asuint(asfloat(r3) * float4(0.166666672, 0.166666672, 0.5, 0.5) + i_TEXCOORD0.xyxy);
				r5 = asuint(_BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r4.xy), _GlobalMipBias.x));
				r4 = asuint(_BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r4.zw), _GlobalMipBias.x));
				r4.xyz = asuint(saturate(asfloat(r4.xyz)));
				r5.xyz = asuint(saturate(asfloat(r5.xyz)));
				r6 = asuint(_BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r3.zw), _GlobalMipBias.x));
				r3 = asuint(_BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r3.xy), _GlobalMipBias.x));
				r3.xyz = asuint(saturate(asfloat(r3.xyz)));
				r0.yzw = asuint(asfloat(r4.xyz) + asfloat(r3.xyz));
				r6.xyz = asuint(saturate(asfloat(r6.xyz)));
				r1.yzw = asuint(asfloat(r6.xyz) + asfloat(r5.xyz));
				r1.yzw = asuint(float3(0.25, 0.25, 0.25) * asfloat(r1.yzw));
				r1.yzw = asuint(asfloat(r0.yzw) * float3(0.25, 0.25, 0.25) + asfloat(r1.yzw));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.yzw));
				r2.z = asuint(dot(asfloat(r1.yzw), float3(0.212672904, 0.715152204, 0.0721750036)));
				r2.w = asuint(min(asfloat(r1.x), asfloat(r2.y)));
				r1.x = asuint(max(asfloat(r1.x), asfloat(r2.y)));
				r1.x = asuint(max(asfloat(r1.x), asfloat(r0.x)));
				r0.x = asuint(min(asfloat(r2.w), asfloat(r0.x)));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r3.xyz = asuint(saturate(asfloat(r3.xyz)));
				r2.y = asuint(dot(asfloat(r3.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r2.w = asuint(min(asfloat(r2.y), asfloat(r2.x)));
				r2.x = asuint(max(asfloat(r2.y), asfloat(r2.x)));
				r1.x = asuint(max(asfloat(r2.x), asfloat(r1.x)));
				b1.x = (asfloat(r1.x) < asfloat(r2.z));
				r0.x = asuint(min(asfloat(r2.w), asfloat(r0.x)));
				b0.x = (asfloat(r2.z) < asfloat(r0.x));
				b0.x = b0.x || b1.x;
				o_SV_Target0.xyz = b0.xxx ? asfloat(r0.yzw) : asfloat(r1.yzw);
				o_SV_Target0.w = 1.0;
			}
			#elif defined(_GAMMA_20)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_PointClamp : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				o_SV_Target0.xyz = sqrt(asfloat(r0.xyz));
				o_SV_Target0.w = 1.0;
			}
			#elif defined(_FXAA_AND_GAMMA_20)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[126];
			float4 _SourceSize;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_PointClamp : register(s0);
			SamplerState sampler_LinearClamp : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				bool4 b0;
				bool4 b1;
				r0 = asuint(_SourceSize.xyxy * i_TEXCOORD0.xyxy);
				r0 = asuint(int4(asfloat(r0)));
				r1 = asuint(int4(-1, -1, 1, -1) + asint(r0.zwzw));
				r0 = asuint(int4(-1, 1, 1, 1) + asint(r0));
				r0 = asuint(float4(asint(r0)));
				r0 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r0)));
				r1 = asuint(float4(asint(r1)));
				r1 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r1)));
				r2 = asuint(float4(-1.0, -1.0, -1.0, -1.0) + _SourceSize.xyxy);
				r1 = asuint(min(asfloat(r2), asfloat(r1)));
				r0 = asuint(min(asfloat(r2), asfloat(r0)));
				r0 = asuint(int4(asfloat(r0.zwxy)));
				r1 = asuint(int4(asfloat(r1.zwxy)));
				r2.xy = r1.zw;
				r2.zw = uint2(0u, 0u);
				r2 = asuint(_BlitTexture.Load(int3(asint(r2.xy), asint(r2.w))));
				r2.xyz = asuint(saturate(asfloat(r2.xyz)));
				r2.x = asuint(dot(asfloat(r2.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r3.xy = r0.zw;
				r3.zw = uint2(0u, 0u);
				r3 = asuint(_BlitTexture.Load(int3(asint(r3.xy), asint(r3.w))));
				r3.xyz = asuint(saturate(asfloat(r3.xyz)));
				r2.y = asuint(dot(asfloat(r3.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r2.z = asuint(asfloat(r2.x) + asfloat(r2.y));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(_BlitTexture.Load(int3(asint(r1.xy), asint(r1.w))));
				r1.xyz = asuint(saturate(asfloat(r1.xyz)));
				r1.x = asuint(dot(asfloat(r1.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r0.zw = uint2(0u, 0u);
				r0 = asuint(_BlitTexture.Load(int3(asint(r0.xy), asint(r0.w))));
				r0.xyz = asuint(saturate(asfloat(r0.xyz)));
				r0.x = asuint(dot(asfloat(r0.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r0.y = asuint(asfloat(r1.x) + asfloat(r0.x));
				r3.yw = asuint(asfloat(r2.zz) + -asfloat(r0.yy));
				r0.y = asuint(asfloat(r2.x) + asfloat(r1.x));
				r0.z = asuint(asfloat(r2.y) + asfloat(r0.x));
				r0.z = asuint(asfloat(r0.y) + -asfloat(r0.z));
				r0.y = asuint(asfloat(r0.y) + asfloat(r2.y));
				r0.y = asuint(asfloat(r0.y) + asfloat(r0.x));
				r0.y = asuint(0.03125 * asfloat(r0.y));
				r0.y = asuint(max(0.0078125, asfloat(r0.y)));
				r0.w = asuint(min(abs(asfloat(r0.z)), abs(asfloat(r3.w))));
				r3.xz = asuint(-asfloat(r0.zz));
				r0.y = asuint(asfloat(r0.w) + asfloat(r0.y));
				r0.y = asuint(1.0 / asfloat(r0.y));
				r3 = asuint(asfloat(r3) * asfloat(r0.yyyy));
				r3 = asuint(max(float4(-8.0, -8.0, -8.0, -8.0), asfloat(r3)));
				r3 = asuint(min(float4(8.0, 8.0, 8.0, 8.0), asfloat(r3)));
				r3 = asuint(_SourceSize.zwzw * asfloat(r3));
				r4 = asuint(asfloat(r3.zwzw) * float4(-0.5, -0.5, -0.166666672, -0.166666672) + i_TEXCOORD0.xyxy);
				r3 = asuint(asfloat(r3) * float4(0.166666672, 0.166666672, 0.5, 0.5) + i_TEXCOORD0.xyxy);
				r5 = asuint(_BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r4.xy), _GlobalMipBias.x));
				r4 = asuint(_BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r4.zw), _GlobalMipBias.x));
				r4.xyz = asuint(saturate(asfloat(r4.xyz)));
				r5.xyz = asuint(saturate(asfloat(r5.xyz)));
				r6 = asuint(_BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r3.zw), _GlobalMipBias.x));
				r3 = asuint(_BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r3.xy), _GlobalMipBias.x));
				r3.xyz = asuint(saturate(asfloat(r3.xyz)));
				r0.yzw = asuint(asfloat(r4.xyz) + asfloat(r3.xyz));
				r6.xyz = asuint(saturate(asfloat(r6.xyz)));
				r1.yzw = asuint(asfloat(r6.xyz) + asfloat(r5.xyz));
				r1.yzw = asuint(float3(0.25, 0.25, 0.25) * asfloat(r1.yzw));
				r1.yzw = asuint(asfloat(r0.yzw) * float3(0.25, 0.25, 0.25) + asfloat(r1.yzw));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.yzw));
				r2.z = asuint(dot(asfloat(r1.yzw), float3(0.212672904, 0.715152204, 0.0721750036)));
				r2.w = asuint(min(asfloat(r1.x), asfloat(r2.y)));
				r1.x = asuint(max(asfloat(r1.x), asfloat(r2.y)));
				r1.x = asuint(max(asfloat(r1.x), asfloat(r0.x)));
				r0.x = asuint(min(asfloat(r2.w), asfloat(r0.x)));
				r3 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r3.xyz = asuint(saturate(asfloat(r3.xyz)));
				r2.y = asuint(dot(asfloat(r3.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r2.w = asuint(min(asfloat(r2.y), asfloat(r2.x)));
				r2.x = asuint(max(asfloat(r2.y), asfloat(r2.x)));
				r1.x = asuint(max(asfloat(r2.x), asfloat(r1.x)));
				b1.x = (asfloat(r1.x) < asfloat(r2.z));
				r0.x = asuint(min(asfloat(r2.w), asfloat(r0.x)));
				b0.x = (asfloat(r2.z) < asfloat(r0.x));
				b0.x = b0.x || b1.x;
				r0.xyz = b0.xxx ? r0.yzw : r1.yzw;
				o_SV_Target0.xyz = sqrt(asfloat(r0.xyz));
				o_SV_Target0.w = 1.0;
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}

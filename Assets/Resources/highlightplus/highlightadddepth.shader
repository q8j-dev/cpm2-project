Shader "HighlightPlus/Geometry/JustDepth"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Pass
		{
			ColorMask 0
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			cbuffer UnityPerDraw
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}

			cbuffer UnityPerFrame
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)));
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				return float4(0.0, 0.0, 0.0, 0.0);
			}
			ENDHLSL
		}
	}
}

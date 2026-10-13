Shader "HighlightPlus/Geometry/SeeThroughOccluder"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "QUEUE" = "Transparent+100" "RenderType" = "Transparent" }
		Pass
		{
			ColorMask 0
			Cull Back
			ZTest LEqual
			ZWrite Off
			Offset -1, -1
			Stencil { Ref 2 ReadMask 255 WriteMask 255 Comp Always Pass DecrWrap Fail Keep ZFail Keep }
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

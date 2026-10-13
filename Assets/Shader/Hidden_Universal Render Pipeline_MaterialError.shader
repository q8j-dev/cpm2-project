Shader "Hidden/Universal Render Pipeline/MaterialError"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _GlobalsPadding[76];
			float4x4 unity_MatrixVP;

			cbuffer UnityPerDraw
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
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
				return float4(1.0, 0.0, 1.0, 1.0);
			}
			ENDHLSL
		}
	}
}

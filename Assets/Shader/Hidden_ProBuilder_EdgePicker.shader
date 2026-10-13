Shader "Hidden/ProBuilder/EdgePicker"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "ALWAYS" "ProBuilderPicker" = "EdgePass" }
		Pass
		{
			Name "Edges"
			Cull Off
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			cbuffer UnityPerCamera
			{
				float4 unity_OrthoParams : packoffset(c8);
			}

			cbuffer UnityPerDraw
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}

			cbuffer UnityPerFrame
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = float4(mul(unity_MatrixV, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0))).xyz, 1.0);
				o.pos.xyz *= lerp(0.99, 1.0, unity_OrthoParams.w);
				o.pos.z *= lerp(1.0, 0.95, unity_OrthoParams.w);
				o.pos = mul(glstate_matrix_projection, o.pos);
				o.color = v.color;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				return i.color;
			}
			ENDHLSL
		}
	}
}

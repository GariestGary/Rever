
Shader "ShaderMan/Gaussian Blur"
{

	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct VertexInput
			{
				fixed4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
				fixed4 tangent : TANGENT;
				fixed3 normal : NORMAL;
				//VertexInput
			};


			struct VertexOutput
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				//VertexOutput
			};

			//Variables
			float4 _iMouse;
			sampler2D _MainTex;

			#ifdef GL_ES
				precision mediump fixed;
			#endif

			fixed normpdf(in fixed x, in fixed sigma)
			{
				return 0.39894 * exp(-0.5 * x * x / (sigma * sigma)) / sigma;
			}

			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				//VertexFactory
				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{

				fixed4 c = tex2D(_MainTex, i.uv / 1);
				if (i.uv.x < _iMouse.x)
				{
					return c;
				}
				return c;
			}

			ENDCG
		}
	}
}


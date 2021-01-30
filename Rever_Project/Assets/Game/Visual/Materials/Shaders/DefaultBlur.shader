// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DefaultBlur"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_KernelSize("Kernel Size (N)", Range(1,100)) = 3
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
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

			//Vertical Blur
			Pass{
				CGPROGRAM
				//include useful shader functions
				#include "UnityCG.cginc"

				//define vertex and fragment shader
				#pragma vertex vert
				#pragma fragment frag

				#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH
				#pragma shader_feature GAUSS

				//texture and transforms of the texture
				sampler2D _MainTex;
				float _KernelSize;
				float2 _MainTex_TexelSize;
				float4 _Color;

				//the object data that's put into the vertex shader
				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				//the data that's used to generate fragments and can be read by the fragment shader
				struct v2f {
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				//the vertex shader
				v2f vert(appdata v) {
					v2f o;
					//convert the vertex positions from object space to clip space so they can be rendered
					o.position = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				//the fragment shader
				fixed4 frag(v2f IN) : SV_TARGET
				{
					float4 sum = 0;

					float upper = ((_KernelSize - 1) / 2);
					float lower = -upper;

					for (float x = lower; x <= upper; ++x)
					{
						for (float y = lower; y <= upper; ++y)
						{
							fixed2 offset = fixed2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);

							float4 c = tex2D(_MainTex, IN.uv + offset);
							c.rgb *= c.a;

							sum += c;
						}
					}

					sum /= (_KernelSize * _KernelSize);
					return sum * _Color;
				}

				


				ENDCG
			}
		}
}
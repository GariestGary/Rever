Shader "Custom/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size("warp amount", Range(0,10)) = 1
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
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Size;
            float _Lock = 0;
            float4 _Out = 0;

            fixed4 frag(v2f IN) : SV_Target
            {   
                if (_Lock > 0)
                {
                    float4 sum = 0;
                    float avg = 0;

                    for (float i = -_Size; i <= _Size; i++)
                    {
                        for (float j = -_Size; j <= _Size; j++)
                        {
                            float4 c = tex2D(_MainTex, IN.uv + (float2(i, j) / _MainTex_TexelSize.zw));
                            c.rgb *= c.a;
                            sum += c;
                            avg++;
                        }
                    }

                    _Out = sum / avg;
                    _Lock = -1;
                    return _Out;
                }
                else
                {
                    return _Out;
                }
            }
            ENDCG
        }
    }
}

Shader "Unlit/waterShader"
{
    Properties
    {
        _Color ("Color", Color) = (0,0.5,1,0.5)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _WaveHeight("Wave Height", Float) = 0.1
        _WaveSpeed("Wave Speed", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveHeight;
            float _WaveSpeed;
            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                float wave = sin(v.vertex.x * 2 + _Time.y * _WaveSpeed) * _WaveHeight;
                v.vertex.y += wave;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}

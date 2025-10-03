Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Bright Radius", Range(0, 1)) = 0.3
        _Smoothness ("Fade Width", Range(0.01, 1)) = 0.2
        _Darkness ("Darkness Amount", Range(0, 1)) = 0.7
        _FogColor ("Fog Color", Color) = (0, 0, 0, 1)

        _Stencil ("Stencil ID", Float) = 1
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 2
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        //_ColorMask ("Color Mask", Float) = 15

    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 100

        Pass
        {
            Name "Default"
            Tags { "LightMode"="Always" }

            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref 1
                Comp Equal
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Radius;
            float _Smoothness;
            float _Darkness;
            float4 _FogColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);

                // 부드러운 전이 영역
                float fade = smoothstep(_Radius, _Radius + _Smoothness, dist);

                // 경계 선명도를 더 부드럽게 (곡선 적용)
                fade = pow(fade, 1.5);

                // 텍스처와 어둠 색상 혼합
                float4 tex = tex2D(_MainTex, i.uv);
                float4 fog = lerp(tex, _FogColor, _Darkness);

                return lerp(tex, fog, fade);
            }
            ENDCG
        }
    }
}

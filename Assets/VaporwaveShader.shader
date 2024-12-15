Shader "Custom/VaporwaveShader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _GridColor ("Grid Color", Color) = (1,1,1,1)
        _GridSpacing ("Grid Spacing", Float) = 1.0
    }
    SubShader {
        Tags { "Queue" = "Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        float4 _GridColor;
        float _GridSpacing;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            float gridValue = floor(IN.uv_MainTex.x * _GridSpacing) - floor(IN.uv_MainTex.y * _GridSpacing);
            gridValue = step(0.5, gridValue);
            o.Albedo = lerp(c.rgb, _GridColor.rgb, gridValue);
        }
        ENDCG
    }
    FallBack "Diffuse"
}

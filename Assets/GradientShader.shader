Shader "Custom/GradientShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,0,0,1)
        _Color2 ("Color 2", Color) = (0,1,0,1)
        _Color3 ("Color 3", Color) = (0,0,1,1)
        _Radius ("Radius", Range(0,1)) = 0.5
        _GradientOrigin ("Gradient Origin", Vector) = (0.5, 0.5, 0, 0)
        _Spread ("Spread", Range(0.1, 10.0)) = 1.0
        _SoftEdge ("Soft Edge", Range(0.0, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            // Enable alpha blending 
            Blend SrcAlpha OneMinusSrcAlpha 
            // ZWrite Off
            // ZTest Always

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color1; 
            float4 _Color2; 
            float4 _Color3; 
            float _Radius; 
            float4 _GradientOrigin; 
            float _Spread;
            float _SoftEdge;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 origin = _GradientOrigin.xy;
                float dist = distance(i.uv, origin) * _Spread; 
                fixed4 color = lerp(_Color1, _Color2, smoothstep(0.0, _Radius, dist));
                color = lerp(color, _Color3, smoothstep(_Radius, 1.0, dist)); 
                
                // Sample the texture and blend with the edge transparency 
                fixed4 texColor = tex2D(_MainTex, i.uv); 
                float edgeFactor = smoothstep(0.5, 0.5 - _SoftEdge, length(i.uv - float2(0.5, 0.5))); 
                fixed4 finalColor = texColor * color * edgeFactor; 

                finalColor.a *= edgeFactor;
                
                return finalColor;
                
                
                // return color * tex2D(_MainTex, i.uv);
                // float2 center = float2(0.5, 0.5);
                // float dist = distance(i.uv, center);
                // fixed4 color = lerp(_Color1, _Color2, smoothstep(0.0, _Radius, dist));
                // color = lerp(color, _Color3, smoothstep(_Radius, 1.0, dist));
                // return color;
            }
            ENDCG
        }
    }
    FallBack "Transparent"
}

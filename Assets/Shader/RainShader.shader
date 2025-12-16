Shader "Custom/RandomCircleShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _CircleColor ("Circle Color", Color) = (1,1,1,1)
        _Radius ("Circle Radius", Float) = 0.01
        _Spacing ("Circle Spacing", Float) = 0.02
        _Seed ("Random Seed", Float) = 0
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float _Radius;
            float _Spacing;
            float _Seed;
            
            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 _CircleColor;
            
            fixed4 frag(v2f i) : SV_Target {
                float2 center = float2(0.5, 0.5);
                float2 diff = i.uv - center;
                float distance = length(diff);
                
                float2 gridPos = floor(i.uv / _Spacing);
                float2 circleCenter = gridPos * _Spacing + _Spacing / 2.0;
                
                float randomOffsetX = frac(sin(dot(gridPos, float2(12.9898,78.233))) * 43758.5453 + _Seed) * _Spacing;
                float randomOffsetY = frac(cos(dot(gridPos, float2(4.5678,9.1234))) * 23421.631 + _Seed) * _Spacing;
                
                circleCenter += float2(randomOffsetX, randomOffsetY);
                
                float gridDistance = length(i.uv - circleCenter);
                
                if (gridDistance < _Radius) {
                    return _CircleColor;
                } else {
                    return float4(0, 0, 0, 0);
                }
            }
            ENDCG
        }
    }
}

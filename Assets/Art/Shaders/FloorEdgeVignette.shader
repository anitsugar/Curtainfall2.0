Shader "Unlit/FloorEdgeVignette"
{
    Properties
    {
        [MainColor] _Color("Tint (Alpha = Strength)", Color) = (0,0,0,0.35)
        _EdgeWidth("Edge Width (0..0.5)", Range(0,0.5)) = 0.08
        _EdgeSoft("Edge Softness (0..0.5)", Range(0,0.5)) = 0.06
    }

        SubShader
    {
        Tags { "Queue" = "Transparent"
               "RenderType" = "Transparent"
               "IgnoreProjector" = "True"
               "RenderPipeline" = "UniversalPipeline" }

        Cull Off
        ZWrite On
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Vignette"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _Color;
            float  _EdgeWidth;
            float  _EdgeSoft;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0; // expects 0..1 across the quad
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;                  // keep UV0 for rectangular fade
                return o;
            }

            // Returns 0 at edges, ~0.5 at center
            float edgeDistance(float2 uv)
            {
                float2 minToEdge = min(uv, 1.0 - uv);
                return min(minToEdge.x, minToEdge.y);
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Distance from the nearest border (0 at the border, 0.5 center)
                float d = edgeDistance(i.uv);

            // Feather: 0 near edge, 1 inside
            float feather = smoothstep(_EdgeWidth, _EdgeWidth + _EdgeSoft, d);

            half4 col = _Color;
            col.a *= saturate(feather);   // strong in center, fades to 0 at edges
            return col;
        }
        ENDHLSL
    }
    }
}
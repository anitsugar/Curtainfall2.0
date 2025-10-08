Shader "Unlit/SpriteUnlitZDepth"
{
    Properties
    {
        [MainTexture] _MainTex("Sprite", 2D) = "white" {}
        _Tint("Tint", Color) = (1,1,1,1)
        _AlphaClip("Alpha Clip (0=Off)", Range(0,1)) = 0.3
    }
        SubShader
        {
            Tags { "Queue" = "AlphaTest"
                   "RenderType" = "TransparentCutout"
                   "IgnoreProjector" = "True"
                   "RenderPipeline" = "UniversalPipeline" }

            Cull Off
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                Name "UnlitCutoutZ"
                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
                float4 _Tint; float _AlphaClip;

                struct A { float4 pos: POSITION; float2 uv: TEXCOORD0; float4 col: COLOR; };
                struct V { float4 pos: SV_POSITION; float2 uv: TEXCOORD0; float4 col: COLOR; };

                V vert(A i) { V o; o.pos = TransformObjectToHClip(i.pos.xyz); o.uv = i.uv; o.col = i.col; return o; }

                half4 frag(V i) : SV_Target
                {
                    half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * _Tint;
                    // Write depth and color only where alpha is solid enough
                    clip(c.a - _AlphaClip);
                    return c;
                }
                ENDHLSL
            }
        }
}

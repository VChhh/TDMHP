Shader "Unlit/CheckerboardURP"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0.9,0.9,0.9,1)
        _ColorB ("Color B", Color) = (0.2,0.2,0.2,1)
        _Scale  ("Scale (tiles)", Float) = 8
        _MainTex ("Texture (multiplies colors)", 2D) = "white" {}
    }

    SubShader
    {
        // OPAQUE URP UNLIT
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"     = "Opaque"
            "Queue"          = "Geometry"
        }

        Cull Back
        ZWrite On
        Blend Off

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4  _ColorA;
                half4  _ColorB;
                float4 _MainTex_ST;
                float  _Scale;
            CBUFFER_END

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float s = max(_Scale, 0.0001);
                float2 uvScaled = IN.uv * s;

                float parity = fmod(floor(uvScaled.x) + floor(uvScaled.y), 2.0);
                half4 col = (parity < 0.5) ? _ColorA : _ColorB;

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return col * tex;
            }
            ENDHLSL
        }
    }
}

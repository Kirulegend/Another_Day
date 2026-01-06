Shader "Custom/BatteryFill_DoubleSided"
{
    Properties
    {
        _FillColor("Fill Color", Color) = (1, 1, 1, 1)
        _FillLevel("Fill Level", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        // --- THIS LINE ENABLES DOUBLE-SIDED RENDERING ---
        Cull Off 
        // ------------------------------------------------

        Pass
        {
            Name "Unlit"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionOS   : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _FillColor;
                float _FillLevel;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionOS = IN.positionOS.xyz;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float yPos = IN.positionOS.y;
                float remappedFill = -0.5 + (_FillLevel - 0.0) * (0.5 - (-0.5)) / (1.0 - 0.0);

                float alphaMask = step(yPos, remappedFill);

                // Discard pixels below the fill line
                if (alphaMask < 0.5) discard;

                return _FillColor;
            }
            ENDHLSL
        }
    }
}
Shader "Custom/LiquidFill_WithCap"
{
    Properties
    {
        _FillColor ("Batter Color", Color) = (1, 0.5, 0, 1)
        _EmptyColor ("Inner Container Color", Color) = (0.2, 0.2, 0.2, 1)
        _FillLevel ("Fill Level", Range(-0.5, 0.5)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        // Critical: Disable culling so we can color the "inside" of the mesh
        Cull Off 

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 positionOS : TEXCOORD0; };

            CBUFFER_START(UnityPerMaterial)
                float4 _FillColor;
                float4 _EmptyColor;
                float _FillLevel;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionOS = input.positionOS.xyz; 
                return output;
            }

            half4 frag(Varyings input, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                // Mask logic: Is the current pixel below our fill height?
                bool isBelowFill = input.positionOS.y < _FillLevel;

                if (isFrontFace)
                {
                    // Exterior of the bowl: Color depends on if we are below the line
                    return isBelowFill ? _FillColor : _EmptyColor;
                }
                else
                {
                    // Interior/Hollow part:
                    // If we are below the fill line, color it as the "Top" (Cap) of the liquid
                    if (isBelowFill) return _FillColor;
                    
                    // If we are above the fill line, color it as the bowl's inner wall
                    return _EmptyColor;
                }
            }
            ENDHLSL
        }
    }
}
Shader "EQ/EQPixelPerfect"
{
    Properties
    {
        _TargetColor("TargetColor", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        Fog { Mode off }

        Color[_TargetColor]

        Pass {

        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}

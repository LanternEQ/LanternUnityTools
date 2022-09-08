#ifndef UNIVERSAL_UNLIT_INPUT_INCLUDED
#define UNIVERSAL_UNLIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
float4 _BaseColor;
float2 _UVPan;
 float4 _ColorMin;
 float4 _ColorMax;
 float _HeightMin;
 float _HeightMax;
CBUFFER_END

#endif

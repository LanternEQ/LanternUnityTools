#ifndef UNIVERSAL_SIMPLE_LIT_PASS_INCLUDED
#define UNIVERSAL_SIMPLE_LIT_PASS_INCLUDED

#include "EQLighting.hlsl"

struct Attributes
{
    float4 positionOS    : POSITION;
    float3 normalOS      : NORMAL;
    float4 tangentOS     : TANGENT;
    float2 texcoord      : TEXCOORD0;
    float2 lightmapUV    : TEXCOORD1;
    float4 colorOS       : COLOR; // Lantern: Vertex colors
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv                       : TEXCOORD0;
    float3 posWS                    : TEXCOORD2; // xyz: posWS
    float3  normal                  : TEXCOORD3;
    float3 viewDir                  : TEXCOORD4;
    half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light
    float3 light                    : COLOR;
    half3 mainLightData             : TEXCOORD8;

    #ifdef _ENVIRONMENTREFLECTIONS_MIRROR
    float4 positionSS               : TEXCOORD9;
    #endif

    float4 positionCS               : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// LANTERN
half3 _DayNightColor;
half3 _VisionColor;
half3 _AmbientLight;

///////////////////////////////////////////////////////////////////////////////
//                  Vertex and Fragment functions                            //
///////////////////////////////////////////////////////////////////////////////

// Used in Standard (Simple Lighting) shader
Varyings LitPassVertexSimple(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);

    half3 directionalLight = GetMainEQLight(vertexInput.positionWS, normalInput.normalWS);

    #if _SCREEN_SPACE_OCCLUSION
    //directionalLight = half3(0.0f, 0.0f, 0.0f);
    #endif

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.posWS.xyz = vertexInput.positionWS;
    output.positionCS = vertexInput.positionCS;

    // Lantern: Instead of calculating the fog here, we pass it to the fragment shader for PPF
    output.fogFactorAndVertexLight = half4(vertexInput.positionCS.z, vertexLight);
    output.mainLightData = directionalLight;
    
    // Lantern: Light calculation
    half ambientLight = input.colorOS.a;
    half3 vertexColorLight = input.colorOS.rgb;

    // Lantern: Dynamic object contribution
    // Any vertex color lit object will have a _DynamicSunlight value of -1f
    // We need to check for this here because by default the vertexColorLight will default to vec(1.0f, 1.0f, 1.0f)
    if(_DynamicSunlight >= 0.0f)
    {
        ambientLight = _DynamicSunlight;
        vertexColorLight = half3(0.0f, 0.0f, 0.0f);
    }

    half3 coloredAmbientLight = max(ambientLight * _DayNightColor.rgb, _VisionColor.rgb);
    half3 mixedDirectionalLight = directionalLight * ambientLight;
    half3 realtimeLight = vertexLight;
    output.light = clamp(mixedDirectionalLight + coloredAmbientLight + vertexColorLight + realtimeLight + _AmbientLight, 0, 1);

    // DEBUG LIGHTING

    // Vertex colors (baked lighting)
    //output.light = half4(vertexColorLight, 1.0f);

    // Ambient light
    //output.light = half4(ambientLight, ambientLight, ambientLight, 1.0f);

    // Colored ambient light
    //output.light = half4(coloredAmbientLight, 1.0f);

    // Directional light
    //output.light = half4(directionalLight, 1.0f);

    // Mixed directional light (Directional light * ambient light)
    //output.light = half4(mixedDirectionalLight, 1.0f);

    // Mixed directional light + vertex colors
    //output.light = half4(mixedDirectionalLight + vertexColorLight, 1.0f);

    // Vertex light (realtime)
    //output.light = half4(realtimeLight, 1.0f);

#ifdef _ENVIRONMENTREFLECTIONS_MIRROR
    output.positionSS = ComputeScreenPos(output.positionCS);
#endif

    return output;
}

half4 LitPassFragmentSimple(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    float2 uv = input.uv;
    half4 diffuseAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    half3 diffuse = diffuseAlpha.rgb * _BaseColor.rgb;
    half alpha = diffuseAlpha.a * _BaseColor.a;
    AlphaDiscard(alpha, _Cutoff);

    #if _SCREEN_SPACE_OCCLUSION
        diffuse = ApplyAmbientOcclusion(diffuse, alpha, GetNormalizedScreenSpaceUV(input.positionCS), NormalizeNormalPerPixel(input.normal));
    #endif

    half4 mixedColor = half4(input.light * diffuse, 1.0f);
    mixedColor.a = alpha;
 
    // DEBUG LIGHTING
    //mixedColor = half4(input.light, 1.0f);

    //#if defined(_ENABLE_FOG)
        half fogFactor = ComputeFogFactor(input.fogFactorAndVertexLight.x);
        mixedColor.rgb = MixFog(mixedColor.rgb, fogFactor);
    //#endif

    return mixedColor;
}

#endif

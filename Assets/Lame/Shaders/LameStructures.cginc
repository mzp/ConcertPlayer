#ifndef LAME_STRUCTURES
  #define LAME_STRUCTURES
  #include "AutoLight.cginc"
  
  struct appdata
  {
    float4 vertex: POSITION;
    float2 texcoord: TEXCOORD0;
    float2 texcoord1: TEXCOORD1;
    float2 texcoord2: TEXCOORD2;
    float3 normal: NORMAL;
    float4 tangent: TANGENT;
    UNITY_VERTEX_INPUT_INSTANCE_ID
  };

  #ifdef _GEOMETRY
    struct v2g
    {
      float3 vertex: OBJPOS;
      float2 rawUV: TEXCOORD0;
      float2 lameColorMapUV: TEXCOORD1;
      float2 lameStrengthMapUV: TEXCOORD2;
      float3 normal: NORMAL;
      UNITY_VERTEX_INPUT_INSTANCE_ID
      UNITY_LIGHTING_COORDS(3, 4)//4,4
    };

    struct dummy
    {
      float3 vertex: VERTEX;
    };

    struct fragIn
    {
      float4 pos: SV_POSITION;
      float2 uv: TEXCOORD0;
      float2 clipUV: TEXCOORD1;
      float3 lameDirAndAtten: LAMEDIRANDATTEN;
      uint clipTexId: CLIPTEXID;
      float3 wPos: WORLDPOS;
      uint2 lameID : LAMEID;
      UNITY_VERTEX_INPUT_INSTANCE_ID
      #ifdef _FORWARDBASE
        float3 ambient: AMBIENT;
      #endif
      UNITY_LIGHTING_COORDS(2, 3)//4,4
    };
  #else
    struct fragIn
    {
      float4 pos: SV_POSITION;
      float2 uv: TEXCOORD0;
      float2 lameUV: TEXCOORD1;
      float2 lameColorMapUV: TEXCOORD2;
      float2 lameStrengthMapUV: TEXCOORD3;
      float3 oPos: OBJPOS;
      float3 normal: NORMAL;
      float3 tangent: TANGENT;
      float3 binormal: BINORMAL;
      UNITY_VERTEX_INPUT_INSTANCE_ID
      #ifdef _FORWARDBASE
        float3 ambient: AMBIENT;
      #endif
      UNITY_LIGHTING_COORDS(4, 5)
    };
  #endif
#endif
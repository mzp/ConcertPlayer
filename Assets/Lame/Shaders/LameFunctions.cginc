#ifndef LAME_FUNCTIONS
  #define LAME_FUNCTIONS
  #include "Lighting.cginc"
  #include "Noise.cginc"
  #include "Color.cginc"
UNITY_INSTANCING_BUFFER_START(PropsLameFunc)
// UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
  UNITY_DEFINE_INSTANCED_PROP(int, _LameSpace)
  #ifdef _LAMESPACE_TEXTURE
  //sampler2D _LameDirTex;
  UNITY_DEFINE_INSTANCED_PROP(float4, _LameDirTex_ST)
  #endif
  //sampler2D _LameStrengthMap;
  UNITY_DEFINE_INSTANCED_PROP(float4, _LameStrengthMap_ST)
  //sampler2D _LameColorMap;
  UNITY_DEFINE_INSTANCED_PROP(float4, _LameColorMap_ST)
  UNITY_DEFINE_INSTANCED_PROP(float4, _LameColor)
  UNITY_DEFINE_INSTANCED_PROP(uint,   _RandomColor)//bool
  UNITY_DEFINE_INSTANCED_PROP(float, _RandomColorBlend)
  UNITY_DEFINE_INSTANCED_PROP(float, _LameParallaxDepth)
  UNITY_DEFINE_INSTANCED_PROP(float, _LameAmount)
  UNITY_DEFINE_INSTANCED_PROP(float, _LameJitter)
  UNITY_DEFINE_INSTANCED_PROP(float, _LameEmissive)
  UNITY_DEFINE_INSTANCED_PROP(uint, _Animate)//bool
  UNITY_DEFINE_INSTANCED_PROP(float, _AnimationSpeed)

  UNITY_DEFINE_INSTANCED_PROP(uint, _NoLightLame)//bool
  UNITY_DEFINE_INSTANCED_PROP(float3, _FixedLightDir)
  UNITY_DEFINE_INSTANCED_PROP(float4, _FixedLightColor)
  UNITY_DEFINE_INSTANCED_PROP(int, _LightDirSpace)
    
  #ifdef _ALPHABLEND_ON 
  UNITY_DEFINE_INSTANCED_PROP(uint, _IndependLameAlpha)//bool
  #endif

  #ifndef _GEOMETRY
  UNITY_DEFINE_INSTANCED_PROP(float, _LameScale)
  #endif

  #ifdef _GEOMETRY
  UNITY_DEFINE_INSTANCED_PROP(float, _LameAttenuation)
  #endif
UNITY_INSTANCING_BUFFER_END(PropsLameFunc)

  #ifdef _LAMESPACE_TEXTURE
  sampler2D _LameDirTex;
  #endif
  sampler2D _LameStrengthMap;
  sampler2D _LameColorMap;
  

  float3 rotate(float3 pos, float3 rot)
  {
    float3 rotS = sin(rot);
    float3 rotC = cos(rot);
    float3x3 rotmat = float3x3(
      rotC.y * rotC.z + rotS.x * rotS.y * rotS.z, rotC.z * rotS.x * rotS.y - rotC.y * rotS.z, rotC.x * rotS.y,
      rotC.x * rotS.z, rotC.x * rotC.z, -rotS.x,
      rotC.y * rotS.x * rotS.z - rotC.z * rotS.y, rotC.y * rotC.z * rotS.x + rotS.y * rotS.z, rotC.x * rotC.y
    );
    return mul(rotmat, pos);
  }
  
  float3 rate(float3 n, float3 rangeMin, float3 rangeMax){
    return (n - rangeMin) / (rangeMax - rangeMin);
  }

  float3 remap(float3 n, float3 srcMin, float3 srcMax, float3 dstMin, float3 dstMax){
    return lerp(dstMin, dstMax, rate(n, srcMin, srcMax));
  }

  float3 randomBrightColor(float3 seed)
  {
    float3 randomColor = rand3D(seed*100) * 0.5 + 0.5;
    randomColor.b = 1;//HSVのVを1に。明るく。
    float3 rgb = HSVtoRGB(randomColor);
    float l = length(rgb);
    rgb /= l;
    rgb *= 1.7320508;
    return rgb;
  }
  
  #ifndef _GEOMETRY
    //tnb is world
    float4 lame(float3 oPos, float2 lameUV, float2 lameColorMapUV, float2 lameStrengthMapUV, float3 normal, float3x3 wToTMat, float3 vDir, float mainAlpha, float minimalLight, float unity_light_atten, float3 ambient)
    {
      float3 innerPos = oPos;
      float3 nearCellPos = 0;
      #ifdef _LAMESPACE_LOCAL3D
        float3 oRayDir = normalize(ObjSpaceViewDir(float4(oPos, 1)));
        float paraDepth = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameParallaxDepth);
        innerPos += oRayDir * paraDepth;
        float lScale = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameScale);
        nearCellPos = getNearCellPos(innerPos * lScale);
      #endif
      float3 innerWPos = mul(unity_ObjectToWorld, float4(innerPos, 1));
      
      #if defined(_LAMESPACE_UV3D) || defined(_LAMESPACE_UV2D)
        float3 tanSpaceVDir = normalize(mul(wToTMat, vDir));
        float paraDepth = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameParallaxDepth);
        float3 parallaxOffset = -tanSpaceVDir * paraDepth;
        float lScale = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameScale);
        lameUV += parallaxOffset.xy * lScale;
        float2 lStrenMapST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameStrengthMap_ST);
        float2 lColMapST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameColorMap_ST);
        lameStrengthMapUV += parallaxOffset * lStrenMapST;
        lameColorMapUV += parallaxOffset * lColMapST.xy;
        #ifdef _LAMESPACE_UV2D
          nearCellPos = float3(getNearCellPos(lameUV), 0);
        #else
          nearCellPos = getNearCellPos(float3(lameUV, parallaxOffset.z));
        #endif
      #endif

      #ifdef _LAMESPACE_TEXTURE
        float3 tanSpaceVDir = normalize(mul(wToTMat, vDir));
        float paraDepth = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameParallaxDepth);
        float3 parallaxOffset = -tanSpaceVDir * paraDepth;
        float2 lDirTexST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameDirTex_ST).xy;
        lameUV += parallaxOffset.xy * lDirTexST.xy;
        float2 lStrenMapST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameStrengthMap_ST);
        float2 lColMapST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameColorMap_ST);
        lameStrengthMapUV += parallaxOffset * lStrenMapST;
        lameColorMapUV += parallaxOffset * lColMapST.xy;
        nearCellPos = tex2D(_LameDirTex, lameUV) + floor(lameUV).xyx;
      #endif
      
      float3 lameDir = normalize(rand3D(nearCellPos));
      // #ifdef _LAMESPACE_LOCAL3D
      // lameDir   = normalize(curlNoise(innerPos * _LameScale));
      // #endif
      bool animate = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _Animate) == 1;
      float animSpeed = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _AnimationSpeed);
      if (animate) lameDir = rotate(lameDir, float3(17, 37, 49) * _Time.x * animSpeed);
      float lJitter = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameJitter);
      lameDir = normalize(lerp(normal, lameDir, lJitter));
      float3 innerLDir = normalize(UnityWorldSpaceLightDir(innerWPos));
      
      float3 lightColor = _LightColor0;
      bool noLightLame = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _NoLightLame) == 1;
      if(length(lightColor) <= 0 && noLightLame){
        lightColor = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _FixedLightColor);
        float3 fixedlDir = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _FixedLightDir);
        innerLDir = normalize(fixedlDir);
        uint lDirSpace = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LightDirSpace);
        if(lDirSpace == 0) innerLDir = normalize(UnityObjectToWorldDir(innerLDir));
      }
      float3 innerVDir = normalize(UnityWorldSpaceViewDir(innerWPos));
      float3 innerRDir = normalize(reflect(-innerVDir, lameDir));
      float innerRdotL = dot(innerRDir, innerLDir);

      float lAmount = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameAmount);
      float lameStrength = tex2D(_LameStrengthMap, lameStrengthMapUV) * pow(saturate(innerRdotL), 1.0 / (lAmount + 0.000001));
      bool randomizeCol = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _RandomColor) == 1;
      float4 lameColor = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameColor);
      float randColBlend = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _RandomColorBlend);
      if (randomizeCol) lameColor.rgb *= lerp(1, randomBrightColor(nearCellPos), randColBlend);
      float4 lameColMap = tex2D(_LameColorMap, lameColorMapUV);
      float lEmissive = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameEmissive);
      #ifdef _FORWARDBASE
      float lameIntensity = unity_light_atten * lEmissive * lameStrength;
      #else
      float lameIntensity = unity_light_atten * lameStrength * lEmissive;
      #endif

      // #ifndef _FORWARDBASE
      //   minimalLight = 0;
      // #endif
    
      float3 lameCol = lameColMap * lameColor * (max(lightColor, minimalLight)+ambient);
      fixed4 col = 1;
      col.rgb = lameCol * lameIntensity;
      
      #ifdef _ALPHABLEND_ON
        float lameAlpha = lameColMap.a * lameStrength * lameColor.a;
        bool independLameAlpha = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _IndependLameAlpha) == 1;
        if(!independLameAlpha) lameAlpha *= mainAlpha;
        col.a = saturate(lameAlpha * length(lameCol) * lEmissive);
      #endif
      
      return col;
    }
  #endif

  #ifndef _STANDARD
  //スタンダードをベースにした処理。非staticなオブジェクト想定。
  inline half3 VertexGIForward(float3 posWorld, half3 normalWorld)
  {
      half3 ambient = 0;
      #ifdef UNITY_SHOULD_SAMPLE_SH
          #ifdef VERTEXLIGHT_ON
              // Approximated illumination from non-important point lights
              ambient.rgb = Shade4PointLights (
                  unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                  unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                  unity_4LightAtten0, posWorld, normalWorld);
          #endif

          ambient.rgb = ShadeSHPerVertex (normalWorld, ambient.rgb);
      #endif

      return ambient;
  }
  #endif
#endif
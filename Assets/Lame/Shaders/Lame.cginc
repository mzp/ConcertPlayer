#ifndef LAME
  #define LAME
  #include "LameStructures.cginc"
  #include "LameFunctions.cginc"
  #include "Lighting.cginc"
  #include "AutoLight.cginc"
//
  UNITY_INSTANCING_BUFFER_START(PropsLame)
    #ifndef _STANDARD
      UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
      #ifndef _GEOMETRY
        UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
      #endif
    #endif

    #ifdef _GEOMETRY
      UNITY_DEFINE_INSTANCED_PROP(uint2, _ClipTexDivide)
      UNITY_DEFINE_INSTANCED_PROP(float, _ClipThreshold)
      UNITY_DEFINE_INSTANCED_PROP(float, _LameNormalOffset)
      UNITY_DEFINE_INSTANCED_PROP(float, _LameNormalOffsetRandom)

      UNITY_DEFINE_INSTANCED_PROP(float, _LameSize)
      UNITY_DEFINE_INSTANCED_PROP(float, _MinimalAlpha)
    #endif

    #ifndef _GEOMETRY
      #ifndef _UNLIT
        UNITY_DEFINE_INSTANCED_PROP(float,  _SpecPower)
        UNITY_DEFINE_INSTANCED_PROP(float, _ReflectionBalance)
      #endif
    #endif

    UNITY_DEFINE_INSTANCED_PROP(float, _MinimalBrightness)
    UNITY_DEFINE_INSTANCED_PROP(uint, _IndependLameRGB)//bool
    UNITY_DEFINE_INSTANCED_PROP(float, _LightAttenuationInfluence)

    UNITY_DEFINE_INSTANCED_PROP(uint, _NoLightIndependLameRGB)//bool
  UNITY_INSTANCING_BUFFER_END(PropsLame)

  #ifndef _STANDARD
     sampler2D _MainTex;
    #ifndef _GEOMETRY
      sampler2D _BumpMap;
    #endif
  #endif

  #ifdef _GEOMETRY
    sampler2D _LameClipTex;
  #endif



  #ifdef _GEOMETRY
    v2g vert(appdata v)
    {
      v2g o;
      UNITY_INITIALIZE_OUTPUT(v2g, o);
      UNITY_SETUP_INSTANCE_ID(v);
      UNITY_TRANSFER_INSTANCE_ID(v, o);
      o.vertex = v.vertex;
      o.rawUV = v.texcoord;
      float4 lColMapST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameColorMap_ST);
      o.lameColorMapUV = v.texcoord * lColMapST.xy + lColMapST.zw;
      float4 lStrenMapST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameStrengthMap_ST);
      o.lameStrengthMapUV = v.texcoord * lStrenMapST.xy + lStrenMapST.zw; 
      o.normal = v.normal;
      return o;
    }
  #elif !defined(_STANDARD)
    fragIn vert(appdata v)
    {
      fragIn o;
      UNITY_INITIALIZE_OUTPUT(fragIn, o);
      UNITY_SETUP_INSTANCE_ID(v);
      UNITY_TRANSFER_INSTANCE_ID(v, o);
      o.pos = UnityObjectToClipPos(v.vertex);
      #ifdef  LAMETEXTUREGEN
      o.uv = v.texcoord;
      #else
      float4 mainTexST = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _MainTex_ST);
      o.uv = v.texcoord * mainTexST.xy + mainTexST.zw;
      #endif
      #ifndef _LAMESPACE_TEXTURE
      float lScale = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameScale);
      o.lameUV = v.texcoord * lScale;
      #else
      float4 lDirTexST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameDirTex_ST);
      o.lameUV = v.texcoord * lDirTexST.xy + lDirTexST.zw;
      #endif
      float4 lColMapST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameColorMap_ST);
      float4 lStrenMapST = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameStrengthMap_ST);
      o.lameColorMapUV = v.texcoord * lColMapST.xy + lColMapST.zw;
      o.lameStrengthMapUV = v.texcoord * lStrenMapST.xy + lStrenMapST.zw;
      o.oPos = v.vertex;
      o.normal = UnityObjectToWorldNormal(v.normal);
      o.tangent = UnityObjectToWorldNormal(normalize(v.tangent.xyz));
      o.binormal = normalize(cross(o.normal, o.tangent) * v.tangent.w);
      #ifdef _FORWARDBASE
        o.ambient = VertexGIForward(mul(unity_ObjectToWorld, v.vertex), o.normal);
      #endif
      UNITY_TRANSFER_LIGHTING(o, v.texcoord);
      return o;
    }
  #endif
  
  #ifdef _GEOMETRY
    [maxvertexcount(36)]
    void geom(triangle v2g input[3], uint pid: SV_PRIMITIVEID, inout TriangleStream < fragIn > stream)
    {
      UNITY_SETUP_INSTANCE_ID(input[0]);
      float lAmount = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameAmount);
      float lNormalOffset = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _LameNormalOffset);
      float lNormalOffsetRand = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _LameNormalOffsetRandom);
      bool animate = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _Animate);
      uint2 clipTexDivide = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _ClipTexDivide);
      float animSpeed = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _AnimationSpeed);
      float lJitter = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameJitter);
      bool noLightLame = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _NoLightLame);
      float4 fixedLightCol = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _FixedLightColor);
      float3 fixedLightDir = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _FixedLightDir);
      uint lDirSpace = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LightDirSpace);
      float lSize = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _LameSize);
      
      for (int lameId = 0; lameId < 9 * lAmount; lameId ++)
      {
        float3 random = normalize(rand3D(float2(pid, lameId)));
        float3 weight = random * random;
        float3 centerObjPos = mul(weight, float3x3(input[0].vertex, input[1].vertex, input[2].vertex));//objpos
        float3 objNormal = normalize(mul(weight, float3x3(input[0].normal, input[1].normal, input[2].normal)));
        centerObjPos += objNormal * (lNormalOffset +abs(random.y) * lNormalOffsetRand);
        float3 centerWPos = mul(unity_ObjectToWorld, float4(centerObjPos, 1));
        float3 geomNormal = normalize(UnityObjectToWorldNormal(objNormal));
        float2 centerRawUV = input[0].rawUV * weight.x + input[1].rawUV * weight.y + input[2].rawUV * weight.z;
        float2 centerColorMapUV = input[0].lameColorMapUV * weight.x + input[1].lameColorMapUV * weight.y + input[2].lameColorMapUV * weight.z;
        float2 centerStrengthMapUV = input[0].lameStrengthMapUV * weight.x + input[1].lameStrengthMapUV * weight.y + input[2].lameStrengthMapUV * weight.z;
        
        float3 lameDir = objNormal;
        float3 rot = UNITY_TWO_PI;
        if (animate) rot += 10 * _Time.x * animSpeed;
        lameDir = rotate(lameDir, random * rot);
        lameDir = normalize(lerp(objNormal, lameDir, lJitter));
        float3 worldTangent = normalize(UnityObjectToWorldNormal(cross(objNormal, random)));
        float3 worldBinormal = normalize(UnityObjectToWorldNormal(cross(lameDir, worldTangent)));
        float3 worldNromal = normalize(UnityObjectToWorldNormal(lameDir));
        uint clipTexId = (abs(random.x) * clipTexDivide.x * clipTexDivide.y) % (clipTexDivide.x * clipTexDivide.y);
        //ラメの計算
        float3 lDir = UnityWorldSpaceLightDir(centerWPos);
        float4 lightColor = _LightColor0;
        if (length(lightColor) <= 0 && noLightLame)
        {
          lightColor = fixedLightCol;
          lDir = normalize(fixedLightDir);
          if(lDirSpace == 0) lDir = normalize(UnityObjectToWorldDir(lDir));
        }
        #ifdef _FORWARDBASE
          float3 ambient = VertexGIForward(centerWPos, worldNromal);
        #endif
        
        float3 vDir = UnityWorldSpaceViewDir(centerWPos);
        if(dot(worldNromal, vDir) < 0) worldNromal *= -1;
        float3 rDir = normalize(reflect(-vDir, worldNromal));
        float RdotL = saturate(dot(rDir, lDir));
        float lAtten = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameAttenuation);
        float lameAttenuation = max(0.0001, pow(RdotL * 0.5 + 0.5, 1.0 / (1 - lAtten + 0.000001)));
        //lameCol.a = saturate(lameColMap.a * lameStrength * _LameColor.a * length(lameCol.rgb));
        //??????
        // bool randCol = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _RandomColor) == 1;
        // float4 lameCol = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _LameColor);
        // if (randCol) lameCol.rgb *= lerp(1, randomBrightColor(float3(pid, lameId, 0)), _RandomColorBlend);
        //quadを打つループ
        for (uint i = 0; i < 4; i ++)
        {
          int x = i % 2 == 0?0: 1;
          int y = i / 2 == 0?0: 1;
          float2 localVertePos = float2(x, y) * 2 - 1;
          float size = 0.01;
          localVertePos *= lSize;
          float3 offset = float3(worldTangent * localVertePos.x + worldBinormal * localVertePos.y);
          float3 vertWorldPos = mul(unity_ObjectToWorld, float4(centerObjPos, 1));// + offset;
          fragIn o;
          UNITY_INITIALIZE_OUTPUT(fragIn, o);
          o.pos = mul(UNITY_MATRIX_VP, float4(vertWorldPos + offset, 1));
          o.clipUV = float2(x, y);
          o.uv = centerRawUV;
          o.lameDirAndAtten = worldNromal * (lameAttenuation + 0.0001);//lengthにattenを乗せる
          o.clipTexId = clipTexId;
          o.lameID = uint2(pid, lameId);
          o.wPos = vertWorldPos;
          #ifdef _FORWARDBASE
            o.ambient = ambient;
          #endif
          dummy v;
          v.vertex = mul(unity_WorldToObject, float4(vertWorldPos + offset, 1));//UNITY_TRANSFER_LIGHTINGのために"v.vertex"を用意
          UNITY_TRANSFER_LIGHTING(o, o.uv);
          TRANSFER_VERTEX_TO_FRAGMENT(o);
          stream.Append(o);
        }
        stream.RestartStrip();
      }
    }
  #elif defined(LAMETEXTUREGEN)
    [maxvertexcount(3)]
    void geom_unwrapUV(triangle fragIn input[3], inout TriangleStream < fragIn > stream)
    {
      UNITY_SETUP_INSTANCE_ID(input[0]);
      [unroll]
      for(int i = 0; i < 3; i++)
      {
        float3 wPos = float3(input[i].uv-0.5,0);
        input[i].pos = mul(UNITY_MATRIX_VP, float4(wPos,1));
        stream.Append(input[i]);
      }
    }
  #endif

  #ifdef _GEOMETRY
    fixed4 frag(fragIn i, bool isFrontFace: SV_ISFRONTFACE): SV_Target
    {
      UNITY_SETUP_INSTANCE_ID(i);
      uint2 clipTexDiv = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _ClipTexDivide);
      uint id = i.clipTexId;
      uint texnum = clipTexDiv.x * clipTexDiv.y;
      uint2 id2d = uint2(id % (float)clipTexDiv.x, id / (float)clipTexDiv.x);
      float2 widthHeight = float2(1.0 / (float)clipTexDiv.x, 1.0 / (float)clipTexDiv.y);
      float2 uv = i.clipUV * widthHeight + (float2)id2d * widthHeight;
      float4 clipCol = tex2D(_LameClipTex, uv);
      float clipThreshold = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _ClipThreshold);
      if (clipCol.r < clipThreshold) return 0;
      
      float3 worldNromal = normalize(i.lameDirAndAtten);
      float lameAtten = length(i.lameDirAndAtten);
      //LightColor
      bool noLameLight = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _NoLightLame) == 1;
      bool fixedLightCol = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _FixedLightColor);
      float4 lightColor = _LightColor0;
      if (length(lightColor) <= 0 && noLameLight) lightColor = fixedLightCol;
      #ifdef _FORWARDBASE
        float3 ambient = ShadeSHPerPixel(worldNromal, i.ambient, i.wPos);
      #else
        float3 ambient = 0;
      #endif
      
      float lameStrength = tex2D(_LameStrengthMap, uv) * lameAtten;
      bool randCol = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _RandomColor) == 1;
      float randomColBlend = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _RandomColorBlend);
      float4 lameColor = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameColor);
      if(randCol) lameColor.rgb *= lerp(1, randomBrightColor(float3(i.lameID, 0)), randomColBlend);
      float4 lameColMap = tex2D(_LameColorMap, uv);
      UNITY_LIGHT_ATTENUATION(atten, i, i.wPos);

      #ifdef _FORWARDBASE
        float lightAttenInflu = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _LightAttenuationInfluence);
        atten = lerp(1, atten, lightAttenInflu);//嘘つき
      #else
        atten = atten;
      #endif

      //lameCol
      float lEmissice = UNITY_ACCESS_INSTANCED_PROP(PropsLameFunc, _LameEmissive);
      #ifdef _FORWARDBASE
        float lameIntensity = atten * lEmissice * lameStrength;
      #else
        float lameIntensity = atten * lameStrength * lEmissice;
      #endif

      float minBright = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _MinimalBrightness);
      float3 lameCol = lameColMap * lameColor * (max(lightColor, minBright) + ambient);
      
      float4 col = 0;
      float mainAlpha = lameColMap.a;
      col.rgb = lameCol * lameIntensity;
      float lameAlpha = lameColMap.a * lameStrength * lameColor.a * length(lameCol.rgb);
      lameAlpha *= mainAlpha;
      col.a = saturate(lameAlpha * lEmissice);
      
      float minAlpha = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _MinimalAlpha);
      col.a = max(minAlpha, col.a);
      return col;
    }
  #elif !defined(_STANDARD)
    fixed4 frag(fragIn i, bool isFrontFace: SV_ISFRONTFACE): SV_Target
    {
      UNITY_SETUP_INSTANCE_ID(i);
      float flip = isFrontFace?1: - 1;
      float3 oPos = i.oPos;
      float3 wPos = mul(unity_ObjectToWorld, float4(oPos, 1));
      float3 geomNormal = normalize(i.normal) * flip;
      float3 tangent = normalize(i.tangent);
      float3 binormal = normalize(i.binormal);
      float3x3 worldToTangent = float3x3(tangent, binormal, geomNormal);
      float3 normal = mul(UnpackNormal(tex2D(_BumpMap, i.uv)), worldToTangent);
      float3 vDir = normalize(UnityWorldSpaceViewDir(wPos));
      float4 mainTexCol = tex2D(_MainTex, i.uv);
      float4 color = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _Color);
      float mainAlpha = mainTexCol.a * color.a;
      #ifdef _FORWARDBASE
        float3 ambient = ShadeSHPerPixel(normal, i.ambient, wPos);
      #endif
      
      #ifndef _UNLIT
        float3 lDir = normalize(UnityWorldSpaceLightDir(wPos));  
        float3 rDir = normalize(reflect(-vDir, normal));
        float  NdotL = dot(normal, lDir);
        float  NdotV = dot(normal, vDir);
        float  RdotL = dot(rDir, lDir);

        UNITY_LIGHT_ATTENUATION(atten, i, wPos);
        #ifdef _FORWARDBASE
          //Directrional Lightのシャドウと減衰を無視
          float lightAttenInflu = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _LightAttenuationInfluence);
          float lameLightAtten = lerp(1, atten, lightAttenInflu);
        #else
          float lameLightAtten = atten;
        #endif

        float specPow = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _SpecPower);
        float reflectBalance = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _ReflectionBalance);
        float3 albedo = mainTexCol;
        float  diffuse = (NdotL * 0.5 + 0.5);
        float  specular = pow(saturate(RdotL), 1.0 / (specPow + 0.000001));//0除算回避
        float3 reflectCol = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, rDir);
        float3 c = lerp(albedo * color, reflectCol, reflectBalance);
        float minLightBright = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _MinimalBrightness);
        #ifdef _FORWARDBASE 
          float3 light = max(minLightBright, diffuse * _LightColor0 * atten) + ambient;
          float3 baseCol = c * light;
        #else
          float3 baseCol = c * diffuse * _LightColor0 * atten;
        #endif
        float3 specularCol = lerp(albedo * color, 1, pow(reflectBalance, 0.4)) * _LightColor0 * atten;

        fixed4 col = 1;
        col.rgb = saturate(baseCol + specular * specularCol);

      #else
        //defined UNLIT
        fixed4 col = mainTexCol * color;
        float lameLightAtten = 1;
      #endif
      
      #ifdef _FORWARDBASE
        float minBright = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _MinimalBrightness);
        float4 lameColor = lame(oPos, i.lameUV, i.lameColorMapUV, i.lameStrengthMapUV, normal, worldToTangent, vDir, mainAlpha, minBright, lameLightAtten, ambient);
      #else
        float4 lameColor = lame(oPos, i.lameUV, i.lameColorMapUV, i.lameStrengthMapUV, normal, worldToTangent, vDir, mainAlpha, 0, lameLightAtten, 0);
      #endif

      #ifdef _LAMEONLYPASS
        return lameColor;
      #endif

      bool noLight = length(_LightColor0.rgb) <= 0;
      bool independLameRGB = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _IndependLameRGB) == 1;
      bool noLightIndependLameRGB = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _NoLightIndependLameRGB) == 1;
      if (!noLight && !independLameRGB) lameColor.rgb *= col.rgb;
      if ( noLight && !noLightIndependLameRGB) lameColor.rgb *= col.rgb;
      col.rgb += lameColor.rgb;
      #ifdef _ALPHABLEND_ON
        col.a = saturate(mainAlpha + lameColor.a);
        if(length(col.rgb) < 0 || col.a <= 0) discard;
      #endif

      return col;
    }
  #endif
  //Lame Texture Gen
  #ifdef LAMETEXTUREGEN
  bool _Gen2DFlag;
  fixed4 frag_lameTex(fragIn i): SV_Target
  {
    float3 oPos = i.oPos;
    float3 innerPos = oPos;
    float2 uv = i.lameUV * _LameScale;
    float3 nearCellPos = 0;
    if(_Gen2DFlag) innerPos = float3(uv,0);
    //#ifdef _LAMESPACE_LOCAL3D
        //float3 oRayDir = normalize(ObjSpaceViewDir(float4(oPos, 1)));
        //innerPos += oRayDir * _LameParallaxDepth;
        nearCellPos = getNearCellPos(innerPos * _LameScale);
    //#endif
    float3 output = rand3D(getNearCellPos(innerPos * _LameScale)) * 0.5 + 0.5;
    return float4(output, 1);
  }
  #endif

  //Shadow Pass
  struct fragIn_shadow
  {
    float4 pos: SV_POSITION;
    #ifdef _ALPHABLEND_ON
      float2 uv: TEXCOORD0;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
  };

  #ifndef _STANDARD
    fragIn_shadow vert_shadow(appdata_base v)
    {
      fragIn_shadow o;
      UNITY_SETUP_INSTANCE_ID(v);
      UNITY_TRANSFER_INSTANCE_ID(v, o);
      o.pos = UnityClipSpaceShadowCasterPos(v.vertex, v.normal);
      o.pos = UnityApplyLinearShadowBias(o.pos);
      #ifdef _ALPHABLEND_ON
        float4 mainTexST = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _MainTex_ST);
        o.uv = v.texcoord * mainTexST.xy + mainTexST.zw;
      #endif
      return o;
    }

    // in
    #ifdef _ALPHABLEND_ON
      sampler3D   _DitherMaskLOD;
    #endif
    
    float4 frag_shadow(fragIn_shadow i): SV_Target
    {
      UNITY_SETUP_INSTANCE_ID(i);
      #ifdef _ALPHABLEND_ON
        float4 color = UNITY_ACCESS_INSTANCED_PROP(PropsLame, _Color);
        float alpha = tex2D(_MainTex, i.uv).a * color.a;
        half alphaRef = tex3D(_DitherMaskLOD, float3(i.pos.xy * 0.25, alpha * 0.9375)).a;
        clip(alphaRef - 0.01);
        clip(alphaRef - 0.01);
      #endif
      return 0;
    }
  #endif  
#endif
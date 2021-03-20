Shader "Lame/Geometry"
{
  Properties
  {
    [Header(Lame)]
    _LameClipTex ("Lame Clip Tex", 2D) = "white" { }
    _ClipTexDivide ("Lame Clip Tex Divide", vector) = (1,1,0,0)
    _ClipThreshold ("Lame Clip Threshold", Range(0,1)) = 0.5
    _LameNormalOffset ("Lame Normal Offset", float) = 0
    _LameNormalOffsetRandom ("Lame Normal Offset Random", float) = 0
    _LameStrengthMap ("Lame Strength Map", 2D) = "white" { }    
    _LameColorMap ("Lame Color Map", 2D) = "white" { }
    _LameColor ("Lame Color", color) = (1, 0, 1, 1)
    [Toggle]
    _RandomColor ("Random Color", int) = 0
    _RandomColorBlend ("Random Color Blend", Range(0,1)) = 1
    _MinimalAlpha ("Minimal Alpha", Range(0,1)) = 0.2
    _LameSize ("Lame Size", float) = 0.01
    _LameAmount ("Lame Amount", Range(0, 1)) = 0.1
    [PowerSlider(2)]
    _LameJitter ("Lame Jitter", Range(0, 1)) = 1
    [PowerSlider(3)]
    _LameAttenuation ("Lame Attenuation", Range(0, 1)) = 1
    _LameEmissive ("Lame Emissive", float) = 1
    [Toggle]
    _Animate ("Animate", int) = 1
    _AnimationSpeed ("Animation Speed", float) = 0.1
    
    [Header(VRC Options)]
    _MinimalBrightness ("Minimal Light", Range(0, 1)) = 0.4  
    [Header(When No Light)]
    [Toggle]
    _NoLightLame("Enable Lame", int) = 1
    _FixedLightDir ("Light Direction", vector) = (1,1,1,0)
    _FixedLightColor ("Light Color", color) = (0.4,0.4,0.4,1)
    [Enum(Local, 0, World, 1)]
    _LightDirSpace ("Light Space", int) = 1
    [Toggle]
    _NoLightIndependLameRGB ("Independ LameRGB (when no light)", int) = 1

    
    [Space(20)]
    [Enum(UnityEngine.Rendering.CullMode)]
    _Cull ("Cull", Float) = 0
  }
  SubShader
  {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Cull [_Cull]
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    Pass
    {
      Name "FORWARD"
      Tags { "LightMode" = "ForwardBase" }
      
      
      CGPROGRAM
      
      #pragma vertex vert
      #pragma geometry geom
      #pragma fragment frag
      #pragma multi_compile_fwdbase_fullshadows
      #pragma multi_compile_instancing
      #define _GEOMETRY
      #define _FORWARDBASE
      #include "UnityCG.cginc"
      #include "Lame.cginc"
      ENDCG
      
    }
    
    Pass
    {
      Name "FORWARD_DELTA"
      Tags { "LightMode" = "ForwardAdd" }
      Blend One One
      ZWrite Off
      ZTest LEqual
      
      CGPROGRAM
      
      #pragma vertex vert
      #pragma geometry geom
      #pragma fragment frag
      #pragma multi_compile_fwdadd_fullshadows
      #define _GEOMETRY
      #define _FORWARDADD
      #include "UnityCG.cginc"
      #include "Lame.cginc"
      ENDCG
      
    }
    // Pass
    // {
      //   Name "ShadowCaster"
      //   Tags { "LightMode" = "ShadowCaster" }
      //   ZWrite On ZTest LEqual
      
      //   CGPROGRAM
      //   #pragma vertex vert_shadow
      //   #pragma geometry geom_shadow
      //   #pragma fragment frag_shadow
      //   #pragma multi_compile_shadowcaster
      //   #define _GEOMETRY
      //   #include "UnityCG.cginc"
      //   #include "Lame.cginc"
      //   ENDCG
      // }
    }
  }

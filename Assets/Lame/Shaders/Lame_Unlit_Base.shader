Shader "Lame/Unlit/Opaque"
{
  Properties
  {
    [Header(Base)]
    _MainTex ("Texture", 2D) = "white" { }
    _Color ("Base Color", color) = (0.3, 0.3, 0.3, 1)
    [NoScaleOffset]
    _BumpMap ("Normal Map", 2D) = "bump" { }

    [Space(20)]
    [Header(Lame)]
    [KeywordEnum(UV2D, UV3D, Local3D, Texture)]
    _LameSpace ("Lame Space", int) = 0
    _LameDirTex ("Lame Direction Texture", 2D) = "white" { }
    _LameStrengthMap ("Lame Strength Map", 2D) = "white" { }
    _LameColorMap ("Lame Color Map", 2D) = "white" { }
    _LameColor ("Lame Color", color) = (1, 0, 1, 1)
    [Toggle]
    _IndependLameRGB ("Indipend Lame RGB From BaseRGB", int) = 0
    [Toggle]
    _RandomColor ("Random Color", int) = 0
    _RandomColorBlend ("Random Color Blend", Range(0,1)) = 1
    _LameScale ("Lame Scale", float) = 200
    _LameParallaxDepth ("Lame Parallax Depth", range(0, 0.1)) = 0.05
    [PowerSlider(3)]
    _LameAmount ("Lame Amount", Range(0, 1)) = 0.1
    [PowerSlider(2)]
    _LameJitter ("Lame Jitter", Range(0, 1)) = 1
    _LameEmissive ("Lame Emissive", float) = 1
    [Toggle]
    _Animate ("Animate", int) = 1
    _AnimationSpeed ("Animation Speed", float) = 0.1

    [Header(VRC Options)]
    [HideInInspector]
    _MinimalBrightness ("Minimal Light", Range(0, 1)) = 0.4
    [Header(Custom Lame Light)]
    [Toggle]
    [HideInInspector]
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
    Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
    Cull [_Cull]
    Pass
    {
      Name "FORWARD"
      Tags { "LightMode" = "ForwardBase" }
      
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fwdbase_fullshadows
      #pragma multi_compile_instancing
      #pragma shader_feature_local _LAMESPACE_UV2D _LAMESPACE_UV3D _LAMESPACE_LOCAL3D _LAMESPACE_TEXTURE
      #define _OPAQUE
      #define _FORWARDBASE
      #define _UNLIT
      #include "UnityCG.cginc"
      #include "Lame.cginc"
      ENDCG
    }
    // Pass
    // {
    //   Name "FORWARD_DELTA"
    //   Tags { "LightMode" = "ForwardAdd" }
    //   Blend One One
    //   ZWrite Off
    //   ZTest LEqual
      
    //   CGPROGRAM
    //   #pragma vertex vert
    //   #pragma fragment frag
    //   #pragma multi_compile_fwdadd_fullshadows
    //   #pragma shader_feature_local _LAMESPACE_UV2D _LAMESPACE_UV3D _LAMESPACE_LOCAL3D _LAMESPACE_TEXTURE
    //   #define _OPAQUE
    //   #define _FORWARDADD
    //   #include "UnityCG.cginc"
    //   #include "Lame.cginc"
    //   ENDCG
    // }
    Pass
    {
      Name "ShadowCaster"
      Tags { "LightMode" = "ShadowCaster" }
      ZWrite On ZTest LEqual
      
      CGPROGRAM
      #pragma vertex vert_shadow
      #pragma fragment frag_shadow
      #pragma multi_compile_shadowcaster
      #include "UnityCG.cginc"
      #include "Lame.cginc"
      ENDCG
    }
  }
  CustomEditor "LameShaderGUI"
}
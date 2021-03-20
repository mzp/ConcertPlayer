Shader "Lame/TextureGen"
{
    Properties
    {
        [Header(Lame)]
        _LameScale ("Lame Scale", float) = 200
        [Enum(2D, 0, 3D, 1)]
        _Gen2DFlag ("LameMapSpace", int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom_unwrapUV
            #pragma fragment frag_lameTex
            #define LAMETEXTUREGEN
            #include "UnityCG.cginc"
            #include "Lame.cginc"
            
            ENDCG
        }
    }
}

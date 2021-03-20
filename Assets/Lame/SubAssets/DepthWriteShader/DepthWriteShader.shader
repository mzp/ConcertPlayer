Shader "DepthWriteShader/Transparent2999"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent-1"}
        LOD 100

        Pass
		{
			Zwrite On
			ColorMask 0
			Lighting OFF
		}
    }
}

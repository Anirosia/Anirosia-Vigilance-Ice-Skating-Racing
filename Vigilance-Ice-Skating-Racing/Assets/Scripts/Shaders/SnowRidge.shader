Shader "Custom/Unlit/SnowRidge"
{
    Properties
    {
        _SnowColour("_SnowColour", COLOR) = (0.2,0.5,0.8,0)
        _BackgroundColour("Background Colour", COLOR) = (0.2,0.5,0.8,0)
        _Amplitude("_Amplitude", float) = 1.0
        _Iterations("_Iterations", float) = 1.0
        _VerticalOffset("_VerticalOffset", float) = 0.5
        _MinimumBackgroundHeight("_MinimumBackgroundHeight", float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct VectorInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VectorOutput
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float4 _SnowColour;
            float4 _BackgroundColour;
            float _Amplitude;
            float _Iterations;
            float _VerticalOffset;
            float _MinimumBackgroundHeight;

            VectorOutput vert (VectorInput v)
            {
                VectorOutput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            fixed4 frag (VectorOutput i) : SV_Target
            {
                float2 uv = i.uv;
                
                float3 colourA = _SnowColour.rgb;
                float3 colourB = _BackgroundColour.rgb;

                float t = 0;
                float amplitude = _Amplitude;
                float offset = 0;
                
                for (half iterationCount = 0; iterationCount < _Iterations; iterationCount++)
                {
                    t = smoothstep(_MinimumBackgroundHeight + cos(i.worldPos.y) * 0.05 , _VerticalOffset + sin(i.worldPos.y + offset) * amplitude, uv.x);
                    amplitude *= 0.5;
                    offset = 1 - amplitude;
                }
                
                
                float3 blend = lerp(colourA, colourB, t);

                return float4(blend, 1);
            }
            ENDCG
        }
    }
}
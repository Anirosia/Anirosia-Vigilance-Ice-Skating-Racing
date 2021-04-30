Shader "Custom/Unlit/SnowRidgeTexture"
{
    Properties
    {
        _SnowTex("Snow Texture", 2D) = "White" { }
        _BackgroundTex("Background Colour", 2D) = "Blue" { }
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

            sampler2D _SnowTex;
            float4 _SnowTex_ST;
            sampler2D _BackgroundTex;
            float _Amplitude;
            float _Iterations;
            float _VerticalOffset;
            float _MinimumBackgroundHeight;

            VectorOutput vert (VectorInput v)
            {
                VectorOutput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _SnowTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            fixed4 frag (VectorOutput IN) : SV_Target
            {
                float2 uv = IN.uv;

                float t = 0;
                float amplitude = _Amplitude;
                float offset = 0;

                fixed4 t1 = tex2D (_SnowTex, IN.uv);
			    fixed4 t2 = tex2D (_BackgroundTex, IN.uv);
                
                for (half iterationCount = 0; iterationCount < _Iterations; iterationCount++)
                {
                    t = smoothstep(_MinimumBackgroundHeight + cos(IN.worldPos.x) * 0.05,
                        _VerticalOffset + sin(IN.worldPos.x + offset) * amplitude,
                        uv.y);
                    amplitude *= 0.5;
                    offset = 1 - amplitude;
                }
                
                float3 blend = lerp(t2, t1, t);

                return float4(blend, 1);
            }
            ENDCG
        }
    }
}
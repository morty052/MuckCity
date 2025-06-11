Shader "Custom/SmoothNormalURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 300

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            struct VertexInput
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct VertexOutput
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            
            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.normalWS = normalize(TransformObjectToWorldNormal(v.normalOS));
                o.uv = v.uv;
                return o;
            }

            float4 frag (VertexOutput i) : SV_Target
            {
                float3 normalMap = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv).rgb;
                normalMap = normalMap * 2.0 - 1.0;
                
                float3 finalNormal = normalize(normalMap);
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                float NdotL = saturate(dot(finalNormal, float3(0,0,1))); // Simple lighting approximation
                return col * NdotL * _Smoothness; // Apply smoothness from properties
            }
            ENDHLSL
        }
    }
}
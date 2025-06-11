// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
// Shader "SyntyStudios/Zombies"
// {
// 	Properties
// 	{
// 		_Texture("Texture", 2D) = "white" {}
// 		_Blood("Blood", 2D) = "white" {}
// 		_BloodColor("BloodColor", Color) = (0.6470588,0.2569204,0.2569204,0)
// 		_BloodAmount("BloodAmount", Range( 0 , 1)) = 0
// 		_Emissive("Emissive", 2D) = "white" {}
// 		_EmissiveColor("Emissive Color", Color) = (0,0,0,0)
// 		[HideInInspector] _texcoord( "", 2D ) = "white" {}
// 		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
// 		[HideInInspector] __dirty( "", Int ) = 1
// 	}

// 	SubShader
// 	{
// 		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
// 		Cull Back
// 		CGPROGRAM
// 		#pragma target 3.0
// 		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
// 		struct Input
// 		{
// 			float2 uv_texcoord;
// 			float2 uv2_texcoord2;
// 		};

// 		uniform sampler2D _Texture;
// 		uniform float4 _Texture_ST;
// 		uniform float4 _BloodColor;
// 		uniform sampler2D _Blood;
// 		uniform float4 _Blood_ST;
// 		uniform float _BloodAmount;
// 		uniform sampler2D _Emissive;
// 		uniform float4 _Emissive_ST;
// 		uniform float4 _EmissiveColor;

// 		void surf( Input i , inout SurfaceOutputStandard o )
// 		{
// 			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
// 			float2 uv1_Blood = i.uv2_texcoord2 * _Blood_ST.xy + _Blood_ST.zw;
// 			float4 lerpResult33 = lerp( float4( 0,0,0,0 ) , tex2D( _Blood, uv1_Blood, float2( 0,0 ), float2( 0,0 ) ) , _BloodAmount);
// 			float4 lerpResult18 = lerp( tex2D( _Texture, uv_Texture ) , _BloodColor , lerpResult33);
// 			o.Albedo = lerpResult18.rgb;
// 			float2 uv_Emissive = i.uv_texcoord * _Emissive_ST.xy + _Emissive_ST.zw;
// 			o.Emission = ( tex2D( _Emissive, uv_Emissive ) * _EmissiveColor ).rgb;
// 			o.Alpha = 1;
// 		}

// 		ENDCG
// 	}
// 	Fallback "Diffuse"
// 	CustomEditor "ASEMaterialInspector"
// }
Shader "SyntyStudios/Zombies"
{
    Properties
    {
        _Texture("Texture", 2D) = "white" {}
        _Blood("Blood", 2D) = "white" {}
        _BloodColor("BloodColor", Color) = (0.6470588,0.2569204,0.2569204,0)
        _BloodAmount("BloodAmount", Range(0,1)) = 0
        _Emissive("Emissive", 2D) = "white" {}
        _EmissiveColor("Emissive Color", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry+0" }
        LOD 200
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            TEXTURE2D(_Texture); SAMPLER(sampler_Texture);
            TEXTURE2D(_Blood); SAMPLER(sampler_Blood);
            TEXTURE2D(_Emissive); SAMPLER(sampler_Emissive);

            float4 _Texture_ST;
            float4 _Blood_ST;
            float4 _BloodColor;
            float _BloodAmount;
            float4 _Emissive_ST;
            float4 _EmissiveColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _Texture);
                OUT.uv2 = TRANSFORM_TEX(IN.uv2, _Blood);
                return OUT;
            }

            // half4 frag(Varyings IN) : SV_Target
            // {
            //     // // Main texture
            //     // half4 baseCol = SAMPLE_TEXTURE2D(_Texture, sampler_Texture, IN.uv);

            //     // // Blood overlay
            //     // half4 bloodTex = SAMPLE_TEXTURE2D(_Blood, sampler_Blood, IN.uv2);
            //     // half4 bloodLerp = lerp(half4(0,0,0,0), bloodTex, _BloodAmount);
            //     // half4 finalCol = lerp(baseCol, _BloodColor, bloodLerp);

            //     // // Emissive
            //     // half4 emissiveTex = SAMPLE_TEXTURE2D(_Emissive, sampler_Emissive, IN.uv);
            //     // half3 emission = emissiveTex.rgb * _EmissiveColor.rgb;

            //     // // Simple Lambert lighting for demonstration
            //     // half3 normal = half3(0,0,1);
            //     // half3 lightDir = normalize(_MainLightPosition.xyz);
            //     // half NdotL = saturate(dot(normal, lightDir));
            //     // half3 litColor = finalCol.rgb * (_MainLightColor.rgb * NdotL + 0.1);

            //     // return half4(litColor + emission, 1.0);

				
            // }
						// ...existing code...
			half4 frag(Varyings IN) : SV_Target
			{
				// Main texture
				half4 baseCol = SAMPLE_TEXTURE2D(_Texture, sampler_Texture, IN.uv);
			
				// Blood overlay
				half4 bloodTex = SAMPLE_TEXTURE2D(_Blood, sampler_Blood, IN.uv2);
				half4 bloodLerp = lerp(half4(0,0,0,0), bloodTex, _BloodAmount);
				half4 finalCol = lerp(baseCol, _BloodColor, bloodLerp);
			
				// Emissive
				half4 emissiveTex = SAMPLE_TEXTURE2D(_Emissive, sampler_Emissive, IN.uv);
				half3 emission = emissiveTex.rgb * _EmissiveColor.rgb;
			
				// No lighting or normal code
				return half4(finalCol.rgb + emission, 1.0);
			}
			// ...existing code...
            ENDHLSL
        }
    }
    CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.SamplerNode;19;80.94751,-201.1474;Inherit;True;Property;_Blood;Blood;1;0;Create;True;0;0;0;False;0;False;-1;None;a72a3ad428c5c7940bea83cb341584a4;True;1;False;white;Auto;False;Object;-1;Derivative;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;78.87256,-2.177094;Float;False;Property;_BloodAmount;BloodAmount;3;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;77.88784,77.59879;Inherit;True;Property;_Emissive;Emissive;4;0;Create;True;0;0;0;False;0;False;-1;None;b29e8dbf7796f684b93a9054132b92bf;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;29;80.6876,274.6689;Float;False;Property;_EmissiveColor;Emissive Color;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.1323529,0.1323529,0.1323529,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;33;531.371,-224.244;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;20;79.94751,-378.1474;Float;False;Property;_BloodColor;BloodColor;2;0;Create;True;0;0;0;False;0;False;0.6470588,0.2569204,0.2569204,0;0.4191176,0.08320715,0.08320715,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;77.10994,-571.7598;Inherit;True;Property;_Texture;Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;f0ee41be7c1898f4ba67b333f7330d46;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;18;770.9475,-550.1474;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;532.8228,178.174;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1015.966,-555.1995;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SyntyStudios/Zombies;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;33;1;19;0
WireConnection;33;2;22;0
WireConnection;18;0;7;0
WireConnection;18;1;20;0
WireConnection;18;2;33;0
WireConnection;28;0;27;0
WireConnection;28;1;29;0
WireConnection;0;0;18;0
WireConnection;0;2;28;0
ASEEND*/
//CHKSM=A2758A78FFBDF757454FCFF867D51F0542A4D04D
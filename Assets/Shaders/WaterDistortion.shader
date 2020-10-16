// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WaterDistortion"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_Float0("Float 0", Float) = 0.025
		_Float3("Float 0", Float) = 0.025
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _MainTex;
		uniform float _Float0;
		uniform float _Float3;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 break19_g8 = float2( -0.5,0.5 );
			float mulTime55 = _Time.y * 8.0;
			float temp_output_1_0_g8 = ( ( i.uv_texcoord.y * 32.0 ) + mulTime55 );
			float sinIn7_g8 = sin( temp_output_1_0_g8 );
			float sinInOffset6_g8 = sin( ( temp_output_1_0_g8 + 1.0 ) );
			float lerpResult20_g8 = lerp( break19_g8.x , break19_g8.y , frac( ( sin( ( ( sinIn7_g8 - sinInOffset6_g8 ) * 91.2228 ) ) * 43758.55 ) ));
			float2 break19_g7 = float2( -0.5,0.5 );
			float mulTime60 = _Time.y * 8.0;
			float temp_output_1_0_g7 = ( ( i.uv_texcoord.x * 32.0 ) + mulTime60 );
			float sinIn7_g7 = sin( temp_output_1_0_g7 );
			float sinInOffset6_g7 = sin( ( temp_output_1_0_g7 + 1.0 ) );
			float lerpResult20_g7 = lerp( break19_g7.x , break19_g7.y , frac( ( sin( ( ( sinIn7_g7 - sinInOffset6_g7 ) * 91.2228 ) ) * 43758.55 ) ));
			float4 appendResult35 = (float4(( ( lerpResult20_g8 + sinIn7_g8 ) * _Float0 ) , ( ( lerpResult20_g7 + sinIn7_g7 ) * _Float3 ) , 0.0 , 0.0));
			float2 uv_TexCoord42 = i.uv_texcoord + appendResult35.xy;
			float4 tex2DNode13 = tex2D( _MainTex, uv_TexCoord42 );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			c.rgb = ( tex2DNode13 * ase_lightAtten * ase_lightColor ).rgb;
			c.a = tex2DNode13.a;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
0;48;1352;659;3041.007;548.7037;4.009783;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;59;-1720.648,-804.0923;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-1718.668,-393.1076;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-1669.972,-658.323;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;32;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-1667.992,-247.3383;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;32;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;55;-1698.392,-101.7384;Inherit;False;1;0;FLOAT;8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;60;-1700.372,-512.723;Inherit;False;1;0;FLOAT;8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;62;-1444.372,-544.723;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-1442.392,-133.7383;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;58;-1226.825,-26.19395;Inherit;False;Noise Sine Wave;-1;;8;a6eff29f739ced848846e3b648af87bd;0;2;1;FLOAT;0;False;2;FLOAT2;-0.5,0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-1145.517,182.0148;Inherit;False;Property;_Float0;Float 0;1;0;Create;True;0;0;False;0;False;0.025;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-1147.498,-228.9703;Inherit;False;Property;_Float3;Float 0;2;0;Create;True;0;0;False;0;False;0.025;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;64;-1228.805,-436.1788;Inherit;False;Noise Sine Wave;-1;;7;a6eff29f739ced848846e3b648af87bd;0;2;1;FLOAT;0;False;2;FLOAT2;-0.5,0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-982.6744,93.29227;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-984.6546,-317.6927;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-832.8699,-132.1171;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;-679.9482,-253.2324;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;74;222.7689,511.071;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightAttenuation;75;234.953,659.022;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-290.1208,189.7809;Inherit;True;Property;_MainTex;_MainTex;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;602.2178,514.5526;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1098.786,251.8214;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;WaterDistortion;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;61;0;59;1
WireConnection;53;0;47;2
WireConnection;62;0;61;0
WireConnection;62;1;60;0
WireConnection;54;0;53;0
WireConnection;54;1;55;0
WireConnection;58;1;54;0
WireConnection;64;1;62;0
WireConnection;52;0;58;0
WireConnection;52;1;56;0
WireConnection;65;0;64;0
WireConnection;65;1;63;0
WireConnection;35;0;52;0
WireConnection;35;1;65;0
WireConnection;42;1;35;0
WireConnection;13;1;42;0
WireConnection;76;0;13;0
WireConnection;76;1;75;0
WireConnection;76;2;74;0
WireConnection;0;9;13;4
WireConnection;0;13;76;0
ASEEND*/
//CHKSM=FBFCD4CF352AC3B6BEC93316479D0046189542B5
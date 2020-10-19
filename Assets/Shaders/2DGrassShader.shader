// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "2DGrassShader"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_LightYShift1("LightYShift", Float) = 1
		_Float0("Float 0", Range( 0 , 1)) = 0.1
		_LightZShift1("LightZShift", Float) = 1
		_NormalMap1("_NormalMap", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
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
		uniform float _LightYShift1;
		uniform float _LightZShift1;
		uniform float _NormalScale;
		uniform sampler2D _NormalMap1;

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
			float mulTime5 = _Time.y * 8.0;
			float mulTime29 = _Time.y * 7.0;
			float3 ase_worldPos = i.worldPos;
			float mulTime32 = _Time.y * 2.0;
			float4 appendResult15 = (float4(( i.uv_texcoord.y * ( ( sin( ( ( i.uv_texcoord.y * 4.0 ) + mulTime5 ) ) + sin( ( ( i.uv_texcoord.y * 8.0 ) + mulTime29 ) ) ) * _Float0 * ( ( sin( ( ( ase_worldPos.x * 0.25 ) + mulTime32 ) ) * 0.5 ) + 1.0 ) * ( ( sin( ( ( ase_worldPos.y * 0.5 ) + -_Time.y ) ) * 0.5 ) + 1.0 ) ) ) , 0.0 , 0.0 , 0.0));
			float2 uv_TexCoord16 = i.uv_texcoord + appendResult15.xy;
			float4 tex2DNode18 = tex2D( _MainTex, uv_TexCoord16 );
			float3 break70 = _WorldSpaceLightPos0.xyz;
			float3 appendResult80 = (float3(break70.x , ( _LightYShift1 + break70.y ) , ( -break70.y + _LightZShift1 )));
			float3 appendResult79 = (float3(ase_worldPos.x , ase_worldPos.y , -mul( unity_ObjectToWorld, float4(0,0,0,1) ).y));
			float3 ifLocalVar85 = 0;
			if( _WorldSpaceLightPos0.w > 0.0 )
				ifLocalVar85 = ( ( appendResult80 - appendResult79 ) * float3( 1,-1,1 ) );
			else if( _WorldSpaceLightPos0.w == 0.0 )
				ifLocalVar85 = ( _WorldSpaceLightPos0.xyz * float3( 1,-1,-1 ) );
			float dotResult87 = dot( ifLocalVar85 , UnpackScaleNormal( tex2D( _NormalMap1, uv_TexCoord16 ), _NormalScale ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			c.rgb = ( saturate( dotResult87 ) * ase_lightColor * ase_lightAtten * tex2DNode18 ).rgb;
			c.a = tex2DNode18.a;
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
		CGPROGRAM
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
7;7;1352;700;1801.102;-393.5657;1.381667;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;30;-2383.107,128.2214;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;46;-2383.473,370.8469;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;32;-2383.107,281.1346;Inherit;False;1;0;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-2161.748,190.8431;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;47;-2144.274,519.0468;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-2809.72,-575.8714;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2136.184,366.9211;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;5;-2533.432,-312.6881;Inherit;False;1;0;FLOAT;8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;29;-2504.232,-77.61279;Inherit;False;1;0;FLOAT;7;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-2519.515,-449.2968;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2496.502,-218.9748;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-1995.262,425.6279;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-2024.855,214.1442;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;41;-1842.51,337.1176;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;66;-2358.949,1431.171;Inherit;False;Constant;_Vector1;Vector 0;0;0;Create;True;0;0;False;0;False;0,0,0,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;33;-1845.728,205.4062;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode;68;-2399.169,1270.293;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-2281.927,-412.1235;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;67;-2435.841,1089.069;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-2274.491,-195.9418;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-2086.413,1410.091;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SinOpNode;21;-2035.988,-270.2664;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;70;-2101.57,940.0985;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1699.529,164.4261;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;25;-2041.703,-168.3746;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1674.026,301.2156;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-1486.895,186.6528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-1834.697,861.3265;Inherit;False;Property;_LightYShift1;LightYShift;1;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-2053.939,-66.12761;Inherit;False;Property;_Float0;Float 0;2;0;Create;True;0;0;False;0;False;0.1;0.013;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-1915.33,-227.1131;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;71;-1836.767,1044.192;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-1577.328,75.13913;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;73;-1931.39,1385.544;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;74;-1860.262,1143.858;Inherit;False;Property;_LightZShift1;LightZShift;3;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1411.386,-53.04071;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-1620.149,1066.054;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;77;-1615.473,1430.461;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-1618.174,885.9313;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;78;-1665.808,1253.607;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;79;-1427.923,1291.933;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;80;-1427.074,977.5083;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1725.704,-479.706;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;81;-1281.167,1138.202;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;15;-1443.759,-381.7742;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1213.961,-356.7913;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1070.833,1724.756;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,-1,-1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-1180.734,911.691;Inherit;False;Property;_NormalScale;Normal Scale;5;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;82;-762.1745,1021.991;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-1087.975,1142.975;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,-1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;86;-819.1422,789.1514;Inherit;True;Property;_NormalMap1;_NormalMap;4;0;Create;True;0;0;False;0;False;-1;None;c4c3570479af8c64e8c5cfdeb85d6c8b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;85;-702.0435,1219.976;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;87;-436.3741,1154.531;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;89;-795.9557,491.3544;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;18;-1044.088,45.5303;Inherit;True;Property;_MainTex;_MainTex;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;88;-808.3425,658.7278;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;90;-251.2245,1181.833;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-300.3014,474.424;Inherit;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;2DGrassShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;35;0;30;1
WireConnection;47;0;46;0
WireConnection;39;0;30;2
WireConnection;4;0;2;2
WireConnection;27;0;2;2
WireConnection;40;0;39;0
WireConnection;40;1;47;0
WireConnection;34;0;35;0
WireConnection;34;1;32;0
WireConnection;41;0;40;0
WireConnection;33;0;34;0
WireConnection;8;0;4;0
WireConnection;8;1;5;0
WireConnection;26;0;27;0
WireConnection;26;1;29;0
WireConnection;69;0;68;0
WireConnection;69;1;66;0
WireConnection;21;0;8;0
WireConnection;70;0;67;1
WireConnection;43;0;33;0
WireConnection;25;0;26;0
WireConnection;44;0;41;0
WireConnection;42;0;44;0
WireConnection;28;0;21;0
WireConnection;28;1;25;0
WireConnection;71;0;70;1
WireConnection;37;0;43;0
WireConnection;73;0;69;0
WireConnection;13;0;28;0
WireConnection;13;1;10;0
WireConnection;13;2;37;0
WireConnection;13;3;42;0
WireConnection;75;0;71;0
WireConnection;75;1;74;0
WireConnection;77;0;73;1
WireConnection;76;0;72;0
WireConnection;76;1;70;1
WireConnection;79;0;78;1
WireConnection;79;1;78;2
WireConnection;79;2;77;0
WireConnection;80;0;70;0
WireConnection;80;1;76;0
WireConnection;80;2;75;0
WireConnection;23;0;2;2
WireConnection;23;1;13;0
WireConnection;81;0;80;0
WireConnection;81;1;79;0
WireConnection;15;0;23;0
WireConnection;16;1;15;0
WireConnection;84;0;67;1
WireConnection;83;0;81;0
WireConnection;86;1;16;0
WireConnection;86;5;93;0
WireConnection;85;0;82;2
WireConnection;85;2;83;0
WireConnection;85;3;84;0
WireConnection;87;0;85;0
WireConnection;87;1;86;0
WireConnection;18;1;16;0
WireConnection;90;0;87;0
WireConnection;92;0;90;0
WireConnection;92;1;89;0
WireConnection;92;2;88;0
WireConnection;92;3;18;0
WireConnection;0;9;18;4
WireConnection;0;13;92;0
ASEEND*/
//CHKSM=BD4B3EC776AE5332E943EE1E3AF5296E1C5891D9
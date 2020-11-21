// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "2DLightingWater"
{
	Properties
	{
		_LightYShift("LightYShift", Float) = 1
		_LightZShift("LightZShift", Float) = 1
		_NormalMap("_NormalMap", 2D) = "white" {}
		_MainTex("_MainTex", 2D) = "white" {}
		_Submersion("Submersion", Range( -10 , 10)) = -10
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
		uniform float4 _MainTex_ST;
		uniform float _LightYShift;
		uniform float _LightZShift;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _Submersion;

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
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode25 = tex2D( _MainTex, uv_MainTex );
			float3 break5 = _WorldSpaceLightPos0.xyz;
			float3 appendResult15 = (float3(break5.x , ( _LightYShift + break5.y ) , ( -break5.y + _LightZShift )));
			float3 ase_worldPos = i.worldPos;
			float3 appendResult14 = (float3(ase_worldPos.x , ase_worldPos.y , -mul( unity_ObjectToWorld, float4(0,0,0,1) ).y));
			float3 ifLocalVar20 = 0;
			if( _WorldSpaceLightPos0.w > 0.0 )
				ifLocalVar20 = ( ( appendResult15 - appendResult14 ) * float3( 1,-1,1 ) );
			else if( _WorldSpaceLightPos0.w == 0.0 )
				ifLocalVar20 = ( _WorldSpaceLightPos0.xyz * float3( 1,-1,-1 ) );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float dotResult22 = dot( ifLocalVar20 , UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 color27 = IsGammaSpace() ? float4(0.257209,0.3447513,0.6415094,0) : float4(0.05381002,0.0973845,0.3691636,0);
			float4 ifLocalVar28 = 0;
			if( mul( float4( ase_worldPos , 0.0 ), unity_WorldToObject ).xyz.y > _Submersion )
				ifLocalVar28 = tex2DNode25;
			else if( mul( float4( ase_worldPos , 0.0 ), unity_WorldToObject ).xyz.y < _Submersion )
				ifLocalVar28 = ( tex2DNode25 * color27 );
			c.rgb = ( saturate( dotResult22 ) * ase_lightColor * ase_lightAtten * ifLocalVar28 ).rgb;
			c.a = tex2DNode25.a;
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
0;34;1353;673;1342.795;-400.9924;1.260393;True;True
Node;AmplifyShaderEditor.Vector4Node;1;-2658.78,192.0428;Inherit;False;Constant;_Vector0;Vector 0;0;0;Create;True;0;0;False;0;False;0,0,0,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldSpaceLightPos;2;-2735.672,-150.0593;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode;3;-2699,31.16489;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-2386.244,170.9628;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;5;-2401.401,-299.03;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;8;-2231.221,146.4161;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;9;-2160.094,-95.27039;Inherit;False;Property;_LightZShift;LightZShift;1;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;6;-2136.599,-194.9361;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-2134.528,-377.802;Inherit;False;Property;_LightYShift;LightYShift;0;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;12;-1915.305,191.3326;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;13;-1965.64,14.47889;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-1918.006,-353.1971;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-1919.981,-173.0742;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;14;-1727.756,52.80488;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;15;-1726.907,-261.6199;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;16;-1581,-100.9266;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;17;-1062.007,-217.1365;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1387.808,-96.1535;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,-1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1370.666,485.6282;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,-1,-1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;34;-490.7687,1123.197;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldToObjectMatrix;35;-474.3839,1048.835;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SamplerNode;21;-1020.643,297.2637;Inherit;True;Property;_NormalMap;_NormalMap;2;0;Create;True;0;0;False;0;False;-1;None;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;20;-1001.876,-19.15261;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;25;-839.3965,587.8554;Inherit;True;Property;_MainTex;_MainTex;3;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;27;-709.8407,795.2128;Inherit;False;Constant;_Color0;Color 0;4;0;Create;True;0;0;False;0;False;0.257209,0.3447513,0.6415094,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-239.9505,976.9926;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-457.5906,767.3175;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;22;-669.9067,163.702;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;37;-111.3905,970.6902;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;31;-524.2003,522.423;Inherit;False;Property;_Submersion;Submersion;4;0;Create;True;0;0;False;0;False;-10;0;-10;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;24;-514.6567,163.7045;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;33;-608.2446,420.5099;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;28;-226.2359,680.0812;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;23;-624.1351,307.9615;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-795.9716,979.2257;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-197.6899,250.1261;Inherit;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;119.5631,15.32859;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;2DLightingWater;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;3;0
WireConnection;4;1;1;0
WireConnection;5;0;2;1
WireConnection;8;0;4;0
WireConnection;6;0;5;1
WireConnection;12;0;8;1
WireConnection;11;0;7;0
WireConnection;11;1;5;1
WireConnection;10;0;6;0
WireConnection;10;1;9;0
WireConnection;14;0;13;1
WireConnection;14;1;13;2
WireConnection;14;2;12;0
WireConnection;15;0;5;0
WireConnection;15;1;11;0
WireConnection;15;2;10;0
WireConnection;16;0;15;0
WireConnection;16;1;14;0
WireConnection;18;0;16;0
WireConnection;19;0;2;1
WireConnection;20;0;17;2
WireConnection;20;2;18;0
WireConnection;20;3;19;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;32;0;25;0
WireConnection;32;1;27;0
WireConnection;22;0;20;0
WireConnection;22;1;21;0
WireConnection;37;0;36;0
WireConnection;24;0;22;0
WireConnection;28;0;37;1
WireConnection;28;1;31;0
WireConnection;28;2;25;0
WireConnection;28;4;32;0
WireConnection;26;0;24;0
WireConnection;26;1;23;0
WireConnection;26;2;33;0
WireConnection;26;3;28;0
WireConnection;0;9;25;4
WireConnection;0;13;26;0
ASEEND*/
//CHKSM=B65B965AF3912AC25CD7B73A031626F23A856E08
// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNB_Nature/SNB_Rocks"
{
	Properties
	{
		_DiffuseSmoothnessA("Diffuse, Smoothness (A)", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Float) = 0.8
		_SmoothnessStrength("Smoothness Strength", Range( 0 , 5)) = 0
		_EmissionStrength("Emission Strength", Float) = 0
		_RocksBrightness("Rocks Brightness", Range( 0 , 1)) = 0.5
		_RocksColorVariation("Rocks Color Variation", Color) = (0,0,0,0)
		_GrassAmount("Grass Amount", Range( 0 , 4)) = 0
		_GrassLevel("Grass Level", Float) = 5
		_GrassColor("Grass Color", Color) = (0.7009608,1,0.06132078,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _NormalStrength;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _RocksColorVariation;
		uniform float _RocksBrightness;
		uniform sampler2D _DiffuseSmoothnessA;
		uniform float4 _DiffuseSmoothnessA_ST;
		uniform float4 _GrassColor;
		uniform float _GrassLevel;
		uniform float _GrassAmount;
		uniform float _EmissionStrength;
		uniform float _SmoothnessStrength;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 tex2DNode1 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalStrength );
			o.Normal = tex2DNode1;
			float4 clampResult38 = clamp( _RocksColorVariation , float4( 0,0,0,0 ) , float4( 0.245283,0.245283,0.245283,0 ) );
			float clampResult16 = clamp( ( _RocksBrightness * 2.0 ) , 0.2 , 5.0 );
			float2 uv_DiffuseSmoothnessA = i.uv_texcoord * _DiffuseSmoothnessA_ST.xy + _DiffuseSmoothnessA_ST.zw;
			float4 tex2DNode2 = tex2D( _DiffuseSmoothnessA, uv_DiffuseSmoothnessA );
			float4 lerpResult47 = lerp( ( clampResult38 + ( clampResult16 * tex2DNode2 ) ) , _GrassColor , saturate( ( pow( (WorldNormalVector( i , tex2DNode1 )).y , _GrassLevel ) * _GrassAmount ) ));
			o.Albedo = lerpResult47.rgb;
			o.Emission = ( lerpResult47 * _EmissionStrength ).rgb;
			o.Smoothness = saturate( ( _SmoothnessStrength * tex2DNode2.a ) );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows dithercrossfade 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
-1920;47;1906;963;2018.856;417.1158;1.354568;True;True
Node;AmplifyShaderEditor.CommentaryNode;83;-2017.073,-289.3108;Float;False;578.0612;521.0447;;3;1;3;2;Base Textures;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;81;-1667.198,-903.9468;Float;False;1106.087;529.3005;;7;18;19;7;38;16;17;8;Color Variation;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-2007.297,43.07594;Float;False;Property;_NormalStrength;Normal Strength;3;0;Create;True;0;0;False;0;0.8;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;82;-1286.198,-142.6061;Float;False;796.7239;574.0007;Height blending;7;41;39;51;55;40;84;85;Grass;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1633.36,-630.8596;Float;False;Property;_RocksBrightness;Rocks Brightness;6;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1773.971,-4.918331;Float;True;Property;_Normal;Normal;2;1;[Normal];Create;True;0;0;False;0;None;4ab1a3671bb3c464fb5bb33ee846202c;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;39;-1259.721,78.6933;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;85;-1237.245,244.574;Float;False;Property;_GrassLevel;Grass Level;9;0;Create;True;0;0;False;0;5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1319.208,-630.7226;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1770.295,-219.3997;Float;True;Property;_DiffuseSmoothnessA;Diffuse, Smoothness (A);0;0;Create;True;0;0;False;0;None;7b1fe3c11d564de49a9b6ea9489ccacf;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;18;-1136.524,-839.4659;Float;False;Property;_RocksColorVariation;Rocks Color Variation;7;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;-1241.086,340.4883;Float;False;Property;_GrassAmount;Grass Amount;8;0;Create;True;0;0;False;0;0;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;16;-1157.23,-634.8467;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;84;-1030.839,82.93264;Float;False;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;38;-851.1448,-671.3923;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.245283,0.245283,0.245283,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-964.6199,-492.0813;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-883.0328,182.66;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-741.1794,-516.4659;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;55;-783.0206,-112.8873;Float;False;Property;_GrassColor;Grass Color;10;0;Create;True;0;0;False;0;0.7009608,1,0.06132078,0;0.5828111,0.735849,0.2603227,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;41;-716.0978,183.3092;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-1062.181,468.3788;Float;False;Property;_SmoothnessStrength;Smoothness Strength;4;0;Create;True;0;0;False;0;0;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;47;-354.6837,-205.6123;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-408.5539,151.7281;Float;False;Property;_EmissionStrength;Emission Strength;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-723.361,492.668;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-160.933,116.5643;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;90;-559.903,525.3595;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SNB_Nature/SNB_Rocks;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;1;Pragma;multi_compile _ LOD_FADE_CROSSFADE;False;;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;5;3;0
WireConnection;39;0;1;0
WireConnection;17;0;8;0
WireConnection;16;0;17;0
WireConnection;84;0;39;2
WireConnection;84;1;85;0
WireConnection;38;0;18;0
WireConnection;7;0;16;0
WireConnection;7;1;2;0
WireConnection;40;0;84;0
WireConnection;40;1;51;0
WireConnection;19;0;38;0
WireConnection;19;1;7;0
WireConnection;41;0;40;0
WireConnection;47;0;19;0
WireConnection;47;1;55;0
WireConnection;47;2;41;0
WireConnection;89;0;88;0
WireConnection;89;1;2;4
WireConnection;87;0;47;0
WireConnection;87;1;86;0
WireConnection;90;0;89;0
WireConnection;0;0;47;0
WireConnection;0;1;1;0
WireConnection;0;2;87;0
WireConnection;0;4;90;0
ASEEND*/
//CHKSM=48B70376AF4AEFC7149B91CFA8B2F9A050B47EF5
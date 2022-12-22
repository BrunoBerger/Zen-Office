// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNB_Nature/SNB_Rocks_Deferred"
{
	Properties
	{
		_DiffuseSmoothnessA("Diffuse, Smoothness (A)", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Float) = 0.8
		_EmissionStrength("Emission Strength", Float) = 0
		_RocksBrightness("Rocks Brightness", Range( 0 , 1)) = 0.5
		_RocksColorVariation("Rocks Color Variation", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NormalStrength;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _RocksColorVariation;
		uniform float _RocksBrightness;
		uniform sampler2D _DiffuseSmoothnessA;
		uniform float4 _DiffuseSmoothnessA_ST;
		uniform float _EmissionStrength;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalStrength );
			float4 clampResult38 = clamp( _RocksColorVariation , float4( 0,0,0,0 ) , float4( 0.245283,0.245283,0.245283,0 ) );
			float clampResult16 = clamp( ( _RocksBrightness * 2.0 ) , 0.2 , 5.0 );
			float2 uv_DiffuseSmoothnessA = i.uv_texcoord * _DiffuseSmoothnessA_ST.xy + _DiffuseSmoothnessA_ST.zw;
			float4 tex2DNode2 = tex2D( _DiffuseSmoothnessA, uv_DiffuseSmoothnessA );
			float4 temp_output_19_0 = ( clampResult38 + ( clampResult16 * tex2DNode2 ) );
			o.Albedo = temp_output_19_0.rgb;
			o.Emission = ( temp_output_19_0 * _EmissionStrength ).rgb;
			o.Smoothness = tex2DNode2.a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
7;1109;1906;1004;1979.518;791.5969;1.824061;True;False
Node;AmplifyShaderEditor.CommentaryNode;81;-1425.947,-495.5255;Float;False;1106.087;529.3005;;7;18;19;7;38;16;17;8;Color Variation;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1392.11,-222.4378;Float;False;Property;_RocksBrightness;Rocks Brightness;5;0;Create;True;0;0;False;0;0.5;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1077.959,-222.3008;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;83;-1375.436,114.9623;Float;False;578.0612;521.0447;;3;1;3;2;Base Textures;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;18;-895.2755,-431.0445;Float;False;Property;_RocksColorVariation;Rocks Color Variation;6;0;Create;True;0;0;False;0;0,0,0,0;0.2862742,0.2996424,0.6705883,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;16;-915.9814,-226.425;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1128.658,184.8734;Float;True;Property;_DiffuseSmoothnessA;Diffuse, Smoothness (A);0;0;Create;True;0;0;False;0;None;7b1fe3c11d564de49a9b6ea9489ccacf;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-723.3714,-83.6591;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1365.66,447.3488;Float;False;Property;_NormalStrength;Normal Strength;3;0;Create;True;0;0;False;0;0.8;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;38;-609.8962,-262.9706;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.245283,0.245283,0.245283,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1132.334,399.3547;Float;True;Property;_Normal;Normal;2;1;[Normal];Create;True;0;0;False;0;None;4ab1a3671bb3c464fb5bb33ee846202c;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-499.9307,-108.0438;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-456.5283,246.2853;Float;False;Property;_EmissionStrength;Emission Strength;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-208.9074,211.1216;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;86;-559.9816,114.4659;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SNB_Nature/SNB_Rocks_Deferred;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;1;Pragma;multi_compile _ LOD_FADE_CROSSFADE;False;;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;8;0
WireConnection;16;0;17;0
WireConnection;7;0;16;0
WireConnection;7;1;2;0
WireConnection;38;0;18;0
WireConnection;1;5;3;0
WireConnection;19;0;38;0
WireConnection;19;1;7;0
WireConnection;85;0;19;0
WireConnection;85;1;84;0
WireConnection;86;0;1;0
WireConnection;0;0;19;0
WireConnection;0;1;86;0
WireConnection;0;2;85;0
WireConnection;0;4;2;4
ASEEND*/
//CHKSM=52B5D571F465E1E9472537D4DDD051C1F5D33DA7
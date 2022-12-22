// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNB_Nature/SNB_Trees"
{
	Properties
	{
		_DiffuseSmoothnessA("Diffuse, Smoothness (A)", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Float) = 1
		_SmoothnessStrength("Smoothness Strength", Range( 0 , 5)) = 1
		_EmissionStrength("Emission Strength", Float) = 0
		_TrunkBrightness("Trunk Brightness", Range( 0 , 1)) = 0.5
		_TrunkColorVariation("Trunk Color Variation", Color) = (0,0,0,0)
		_HeightGradient("Height Gradient", Float) = 1.5
		_HeightStartGradient("Height Start Gradient", Float) = 1.25
		_HeightBrightness("Height Brightness", Range( 0 , 8)) = 0
		_WindTrunkAmplitude("Wind Trunk Amplitude", Range( 0 , 1)) = 0
		_WindTrunkSpeed("Wind Trunk Speed", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#pragma instancing_options procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON__
		#include "VS_indirect.cginc"
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float _WindTrunkSpeed;
		uniform float _WindTrunkAmplitude;
		uniform float _NormalStrength;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _HeightBrightness;
		uniform float _HeightStartGradient;
		uniform float _HeightGradient;
		uniform float4 _TrunkColorVariation;
		uniform float _TrunkBrightness;
		uniform sampler2D _DiffuseSmoothnessA;
		uniform float4 _DiffuseSmoothnessA_ST;
		uniform float _EmissionStrength;
		uniform float _SmoothnessStrength;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float temp_output_48_0 = ( _Time.y * ( 2.0 * _WindTrunkSpeed ) );
			float4 appendResult58 = (float4(( ( sin( temp_output_48_0 ) * _WindTrunkAmplitude ) * v.color.b ) , 0.0 , ( v.color.b * ( ( _WindTrunkAmplitude * 0.5 ) * cos( temp_output_48_0 ) ) ) , 0.0));
			v.vertex.xyz += appendResult58.rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalStrength );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 clampResult38 = clamp( _TrunkColorVariation , float4( 0,0,0,0 ) , float4( 0.3867925,0.3867925,0.3867925,0 ) );
			float clampResult16 = clamp( ( _TrunkBrightness * 2.0 ) , 0.5 , 5.0 );
			float2 uv_DiffuseSmoothnessA = i.uv_texcoord * _DiffuseSmoothnessA_ST.xy + _DiffuseSmoothnessA_ST.zw;
			float4 tex2DNode2 = tex2D( _DiffuseSmoothnessA, uv_DiffuseSmoothnessA );
			float4 temp_output_19_0 = ( clampResult38 + ( clampResult16 * tex2DNode2 ) );
			o.Albedo = ( ( _HeightBrightness * ( saturate( ( ( ase_vertex3Pos.y - _HeightStartGradient ) / _HeightGradient ) ) * temp_output_19_0 ) ) + temp_output_19_0 ).rgb;
			o.Emission = ( _EmissionStrength * tex2DNode2 ).rgb;
			o.Smoothness = saturate( ( tex2DNode2.a * _SmoothnessStrength ) );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
-1920;47;1906;963;1993.116;276.0834;1.3;True;True
Node;AmplifyShaderEditor.CommentaryNode;42;-1671.48,-435.0915;Float;False;1100.124;397.9584;;7;38;18;8;19;7;16;17;Color Variation;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;41;-1660.11,-1107.042;Float;False;1252.568;615.0753;Use these parameters in order create convincing dead trees;7;34;31;30;32;39;28;37;Height Color (Dead Trees);1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1655.018,-161.3838;Float;False;Property;_TrunkBrightness;Trunk Brightness;6;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;44;-2193.858,608.9498;Float;False;1821.23;666.407;Vertex offset using Blue Vertex Color channel;13;58;57;56;55;54;53;52;51;50;49;48;47;46;Wind Trunk;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;40;-1622.977,56.88641;Float;False;1429.833;501.3197;;6;59;60;1;3;2;63;Base Textures;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;37;-1527.079,-1001.581;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1351.109,-155.5394;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-2203.111,960.8234;Float;False;Property;_WindTrunkSpeed;Wind Trunk Speed;12;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1600.21,-805.5048;Float;False;Property;_HeightStartGradient;Height Start Gradient;9;0;Create;True;0;0;False;0;1.25;1.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;47;-2103.607,682.01;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1917.996,874.5693;Float;False;2;2;0;FLOAT;2;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1359.393,342.3377;Float;True;Property;_DiffuseSmoothnessA;Diffuse, Smoothness (A);0;0;Create;True;0;0;False;0;None;e5949f3f02e8cd24e97605b8c7ed5b30;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-1218.514,-822.0156;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1291.665,-654.8906;Float;False;Property;_HeightGradient;Height Gradient;8;0;Create;True;0;0;False;0;1.5;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;16;-1189.13,-159.6641;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;18;-1229.019,-372.6056;Float;False;Property;_TrunkColorVariation;Trunk Color Variation;7;0;Create;True;0;0;False;0;0,0,0,0;0.3018866,0.3018866,0.3018866,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1762.963,750.2173;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;38;-940.5663,-360.7372;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.3867925,0.3867925,0.3867925,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;30;-1069.336,-669.0164;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-919.6409,-160.6285;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1640.252,873.4512;Float;False;Property;_WindTrunkAmplitude;Wind Trunk Amplitude;11;0;Create;True;0;0;False;0;0;0.06;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;52;-1590.511,1098.281;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-1373.95,918.5912;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;31;-916.1373,-594.02;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;51;-1473.764,744.2002;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-696.2008,-185.0133;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-1224.985,1060.911;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-487.2887,-337.9709;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;54;-1095.376,839.2805;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-1293.263,715.699;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-697.0922,-650.8575;Float;False;Property;_HeightBrightness;Height Brightness;10;0;Create;True;0;0;False;0;0;0;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-1015.515,503.9165;Float;False;Property;_SmoothnessStrength;Smoothness Strength;4;0;Create;True;0;0;False;0;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-616.1148,204.292;Float;False;Property;_EmissionStrength;Emission Strength;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1616.573,173.2084;Float;False;Property;_NormalStrength;Normal Strength;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-898.8868,1075.036;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-851.804,729.9441;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-749.0156,389.5165;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-281.9474,-363.3468;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-337.3196,312.2039;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-138.6807,-212.959;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;58;-552.2635,984.865;Float;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;63;-542.3155,409.0166;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1357.06,129.6678;Float;True;Property;_Normal;Normal;2;1;[Normal];Create;True;0;0;False;0;None;06e6b2458a47542428bca89ee8a27e44;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SNB_Nature/SNB_Trees;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;4;Pragma;multi_compile _ LOD_FADE_CROSSFADE;False;;Pragma;instancing_options procedural:setup;False;;Pragma;multi_compile GPU_FRUSTUM_ON__;False;;Include;VS_indirect.cginc;False;;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;8;0
WireConnection;46;1;45;0
WireConnection;39;0;37;2
WireConnection;39;1;28;0
WireConnection;16;0;17;0
WireConnection;48;0;47;0
WireConnection;48;1;46;0
WireConnection;38;0;18;0
WireConnection;30;0;39;0
WireConnection;30;1;32;0
WireConnection;7;0;16;0
WireConnection;7;1;2;0
WireConnection;52;0;48;0
WireConnection;50;0;49;0
WireConnection;31;0;30;0
WireConnection;51;0;48;0
WireConnection;19;0;38;0
WireConnection;19;1;7;0
WireConnection;53;0;50;0
WireConnection;53;1;52;0
WireConnection;25;0;31;0
WireConnection;25;1;19;0
WireConnection;55;0;51;0
WireConnection;55;1;49;0
WireConnection;57;0;54;3
WireConnection;57;1;53;0
WireConnection;56;0;55;0
WireConnection;56;1;54;3
WireConnection;61;0;2;4
WireConnection;61;1;64;0
WireConnection;33;0;34;0
WireConnection;33;1;25;0
WireConnection;60;0;59;0
WireConnection;60;1;2;0
WireConnection;24;0;33;0
WireConnection;24;1;19;0
WireConnection;58;0;56;0
WireConnection;58;2;57;0
WireConnection;63;0;61;0
WireConnection;1;5;3;0
WireConnection;0;0;24;0
WireConnection;0;1;1;0
WireConnection;0;2;60;0
WireConnection;0;4;63;0
WireConnection;0;11;58;0
ASEEND*/
//CHKSM=C371E22FED25B6BE590CA5A70E3640E08F896261
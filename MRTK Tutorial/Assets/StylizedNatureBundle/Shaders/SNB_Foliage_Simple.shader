// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNB_Nature/SNB_Foliage_Simple"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_EmissionRMetallicGSmoothnessB("Emission (R), Metallic (G), Smoothness (B)", 2D) = "black" {}
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.75
		_NormalStrength("Normal Strength", Float) = 1
		_EmissionStrength("Emission Strength", Float) = 0
		_WindFoliageAmplitude("Wind Foliage Amplitude", Range( 0 , 1)) = 0
		_WindFoliageSpeed("Wind Foliage Speed", Range( 0 , 1)) = 0
		_WindTrunkAmplitude("Wind Trunk Amplitude", Range( 0 , 1)) = 0
		_WindTrunkSpeed("Wind Trunk Speed", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "DisableBatching" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#pragma instancing_options procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON__
		#include "VS_indirect.cginc"
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _WindTrunkSpeed;
		uniform float _WindTrunkAmplitude;
		uniform float _WindFoliageSpeed;
		uniform float _WindFoliageAmplitude;
		uniform float _NormalStrength;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _EmissionRMetallicGSmoothnessB;
		uniform float4 _EmissionRMetallicGSmoothnessB_ST;
		uniform float _EmissionStrength;
		uniform float _Cutoff = 0.75;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float temp_output_130_0 = ( _Time.y * ( 2.0 * _WindTrunkSpeed ) );
			float4 appendResult141 = (float4(( ( sin( temp_output_130_0 ) * _WindTrunkAmplitude ) * v.color.b ) , 0.0 , ( v.color.b * ( ( _WindTrunkAmplitude * 0.5 ) * cos( temp_output_130_0 ) ) ) , 0.0));
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult149 = (float4(ase_worldPos.x , ase_worldPos.y , ase_worldPos.z , 0.0));
			float2 panner93 = ( ( _Time.y * _WindFoliageSpeed ) * float2( 2,2 ) + appendResult149.xy);
			float simplePerlin2D101 = snoise( panner93 );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( appendResult141 + float4( ( simplePerlin2D101 * _WindFoliageAmplitude * ase_vertexNormal * v.color.r ) , 0.0 ) ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalStrength );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode36 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = tex2DNode36.rgb;
			float2 uv_EmissionRMetallicGSmoothnessB = i.uv_texcoord * _EmissionRMetallicGSmoothnessB_ST.xy + _EmissionRMetallicGSmoothnessB_ST.zw;
			float4 tex2DNode106 = tex2D( _EmissionRMetallicGSmoothnessB, uv_EmissionRMetallicGSmoothnessB );
			o.Emission = ( tex2DNode106.r * _EmissionStrength * tex2DNode36 ).rgb;
			o.Smoothness = tex2DNode106.b;
			o.Alpha = 1;
			clip( tex2DNode36.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
7;1109;1906;1004;2408.054;1842.431;2.107857;True;True
Node;AmplifyShaderEditor.CommentaryNode;144;-2070.754,-227.6897;Float;False;1821.23;666.407;Vertex offset using Blue Vertex Color channel;14;140;141;137;135;134;136;138;139;133;130;143;131;129;148;Wind Trunk;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;131;-2082.088,57.5998;Float;False;Property;_WindTrunkSpeed;Wind Trunk Speed;9;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-1794.891,37.92978;Float;False;2;2;0;FLOAT;2;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;102;-2066.462,526.8181;Float;False;1715.196;801.9376;Vertex offset using Red Vertex Color channel base on panning noise;11;97;149;85;99;132;101;98;93;95;94;96;Wind Foliage;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;129;-1980.502,-154.6292;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;85;-2018.262,682.03;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;94;-1985.927,927.2303;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-1953.716,1096.651;Float;False;Property;_WindFoliageSpeed;Wind Foliage Speed;7;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-1517.146,36.81174;Float;False;Property;_WindTrunkAmplitude;Wind Trunk Amplitude;8;0;Create;True;0;0;False;0;0;0.06;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-1639.858,-86.42235;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-1250.845,81.9518;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;140;-1467.405,261.6441;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;133;-1350.659,-92.43931;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;149;-1733.742,695.092;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1647.085,955.8643;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;136;-1005.819,4.40688;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-1103.646,250.7591;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-1170.158,-120.9403;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;93;-1382.269,706.54;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;2,2;False;1;FLOAT;0.1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;132;-979.3766,1155.101;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;-775.7828,238.3991;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-987.6646,898.9308;Float;False;Property;_WindFoliageAmplitude;Wind Foliage Amplitude;6;0;Create;True;0;0;False;0;0;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;101;-961.6227,642.655;Float;True;Simplex2D;1;0;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-728.6999,-106.6953;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;146;-1517.837,-966.3447;Float;False;1272.888;652.4158;;6;104;107;106;36;105;108;Base Textures;1,1,1,1;0;0
Node;AmplifyShaderEditor.NormalVertexDataNode;97;-988.5987,991.7465;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;36;-1224.826,-533.9968;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;fcbd578e80a32e8469fe6fecd607d8aa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;107;-800.1319,-605.7378;Float;False;Property;_EmissionStrength;Emission Strength;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;106;-1234.825,-739.4573;Float;True;Property;_EmissionRMetallicGSmoothnessB;Emission (R), Metallic (G), Smoothness (B);1;0;Create;True;0;0;False;0;None;b40e4f8e896e21344a3d5ca84d541ca5;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;141;-429.1591,148.227;Float;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-507.1464,875.573;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-1444.778,-858.068;Float;False;Property;_NormalStrength;Normal Strength;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;-549.8426,-635.8936;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;142;3.530522,171.5077;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;104;-1230.504,-917.5995;Float;True;Property;_NormalMap;Normal Map;2;1;[Normal];Create;True;0;0;False;0;None;fc84c738c0737df418cb63cacdcb9f84;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;353.0061,-358.2258;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;SNB_Nature/SNB_Foliage_Simple;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;7;Custom;0.75;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;3;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;4;Pragma;multi_compile _ LOD_FADE_CROSSFADE;False;;Pragma;instancing_options procedural:setup;False;;Pragma;multi_compile GPU_FRUSTUM_ON__;False;;Include;VS_indirect.cginc;False;;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;148;1;131;0
WireConnection;130;0;129;0
WireConnection;130;1;148;0
WireConnection;139;0;143;0
WireConnection;140;0;130;0
WireConnection;133;0;130;0
WireConnection;149;0;85;1
WireConnection;149;1;85;2
WireConnection;149;2;85;3
WireConnection;95;0;94;0
WireConnection;95;1;96;0
WireConnection;138;0;139;0
WireConnection;138;1;140;0
WireConnection;134;0;133;0
WireConnection;134;1;143;0
WireConnection;93;0;149;0
WireConnection;93;1;95;0
WireConnection;137;0;136;3
WireConnection;137;1;138;0
WireConnection;101;0;93;0
WireConnection;135;0;134;0
WireConnection;135;1;136;3
WireConnection;141;0;135;0
WireConnection;141;2;137;0
WireConnection;99;0;101;0
WireConnection;99;1;98;0
WireConnection;99;2;97;0
WireConnection;99;3;132;1
WireConnection;108;0;106;1
WireConnection;108;1;107;0
WireConnection;108;2;36;0
WireConnection;142;0;141;0
WireConnection;142;1;99;0
WireConnection;104;5;105;0
WireConnection;0;0;36;0
WireConnection;0;1;104;0
WireConnection;0;2;108;0
WireConnection;0;4;106;3
WireConnection;0;10;36;4
WireConnection;0;11;142;0
ASEEND*/
//CHKSM=980F4B6E487F022C5E89C6D10FCD43CC14726406
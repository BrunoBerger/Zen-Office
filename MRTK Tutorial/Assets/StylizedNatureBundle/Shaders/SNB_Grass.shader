// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNB_Nature/SNB_Grass"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.75
		_WindFoliageAmplitude("Wind Foliage Amplitude", Range( 0 , 1)) = 0
		_WindFoliageSpeed("Wind Foliage Speed", Range( 0 , 1)) = 0
		_WindTrunkAmplitude("Wind Trunk Amplitude", Range( 0 , 1)) = 0
		_WindTrunkSpeed("Wind Trunk Speed", Range( 0 , 1)) = 0
		_GrassColor("Grass Color", Color) = (0.5264154,0.7264151,0.2158686,0)
		_HeightColor("Height Color", Color) = (0.4464056,0.6981132,0.1350124,0)
		_HeightStartGradient("Height Start Gradient", Range( 0 , 1)) = 0.1
		_HeightGradient("Height Gradient", Range( 0 , 1)) = 0.3
		_FlowerMainColor01("Flower Main Color 01", Color) = (1,0.9637499,0.759434,0)
		_FlowerInsideColor01("Flower Inside Color 01", Color) = (1,0.7789562,0.1273585,0)
		_FlowerMainColor02("Flower Main Color 02", Color) = (1,0.703345,0.1556604,0)
		_FlowerInsideColor02("Flower Inside Color 02", Color) = (1,0.9507267,0.6084906,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "DisableBatching" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#pragma instancing_options procedural:setupScale
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
		uniform float4 _GrassColor;
		uniform float4 _HeightColor;
		uniform float _HeightStartGradient;
		uniform float _HeightGradient;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float4 _FlowerMainColor02;
		uniform float4 _FlowerMainColor01;
		uniform float4 _FlowerInsideColor02;
		uniform float4 _FlowerInsideColor01;
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
			float3 appendResult91 = (float3(ase_worldPos.x , ase_worldPos.y , ase_worldPos.z));
			float2 panner93 = ( ( _Time.y * _WindFoliageSpeed ) * float2( 2,2 ) + appendResult91.xy);
			float simplePerlin2D101 = snoise( panner93 );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( appendResult141 + float4( ( simplePerlin2D101 * _WindFoliageAmplitude * ase_vertexNormal * v.color.r ) , 0.0 ) ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float clampResult178 = clamp( _HeightStartGradient , 0.0 , 0.5 );
			float4 lerpResult195 = lerp( _GrassColor , _HeightColor , saturate( ( ( ase_vertex3Pos.y - clampResult178 ) / _HeightGradient ) ));
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode185 = tex2D( _TextureSample0, uv_TextureSample0 );
			float smoothstepResult209 = smoothstep( 0.73 , 0.75 , ( i.uv_texcoord.y + 0.3 ));
			float4 lerpResult194 = lerp( _FlowerMainColor02 , _FlowerMainColor01 , smoothstepResult209);
			float4 lerpResult199 = lerp( float4( 0,0,0,0 ) , lerpResult194 , tex2DNode185.b);
			float4 lerpResult196 = lerp( _FlowerInsideColor02 , _FlowerInsideColor01 , smoothstepResult209);
			float4 lerpResult197 = lerp( float4( 0,0,0,0 ) , lerpResult196 , tex2DNode185.r);
			o.Albedo = ( ( lerpResult195 * tex2DNode185.g ) + lerpResult199 + lerpResult197 ).rgb;
			o.Smoothness = 0.0;
			o.Alpha = 1;
			clip( tex2DNode185.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
-1920;47;1906;963;1653.835;470.1212;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;144;-2913.597,545.9066;Float;False;1821.23;666.407;Vertex offset using Blue Vertex Color channel;14;140;141;137;135;134;136;138;139;133;130;143;131;129;148;Wind Trunk;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;149;-2717.014,-1403.388;Float;False;1252.568;615.0753;;7;155;154;153;152;151;150;178;Height Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;131;-2924.931,831.1962;Float;False;Property;_WindTrunkSpeed;Wind Trunk Speed;5;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;151;-2751.54,-1104.126;Float;False;Property;_HeightStartGradient;Height Start Gradient;8;0;Create;True;0;0;False;0;0.1;0.23;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;129;-2823.345,618.9672;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-2637.734,811.5262;Float;False;2;2;0;FLOAT;2;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;102;-2909.305,1300.414;Float;False;1653.539;798.5751;Vertex offset using Red Vertex Color channel base on panning noise;11;97;85;101;91;99;132;98;93;95;94;96;Wind Foliage;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;181;-2756.402,-701.373;Float;False;1750.334;1123.101;;16;199;198;197;196;195;194;193;192;191;189;188;187;185;209;203;210;Grass & Flower Color Variation;1,1,1,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;178;-2468.101,-1100.698;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;150;-2500.565,-1341.262;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;203;-2590.816,127.4431;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-2482.701,687.1741;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-2753.813,1989.849;Float;False;Property;_WindFoliageSpeed;Wind Foliage Speed;3;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;94;-2786.024,1820.427;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;152;-2275.424,-1118.361;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;153;-2576.574,-951.2346;Float;False;Property;_HeightGradient;Height Gradient;9;0;Create;True;0;0;False;0;0.3;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;85;-2882.843,1437.402;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;143;-2359.99,810.4082;Float;False;Property;_WindTrunkAmplitude;Wind Trunk Amplitude;4;0;Create;True;0;0;False;0;0;0.06;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;133;-2193.502,681.1571;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;140;-2310.249,1035.24;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-2447.187,1849.061;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;91;-2554.636,1457.04;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-2093.688,855.5482;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;154;-2126.248,-965.3605;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;210;-2315.419,144.885;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;191;-2703.704,-655.7825;Float;False;Property;_GrassColor;Grass Color;6;0;Create;True;0;0;False;0;0.5264154,0.7264151,0.2158686,0;0.4392355,0.735849,0.2117299,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;187;-2702.605,-493.582;Float;False;Property;_HeightColor;Height Color;7;0;Create;True;0;0;False;0;0.4464056,0.6981132,0.1350124,0;0.7004818,0.8867924,0.4141152,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;189;-1752.851,222.1379;Float;False;Property;_FlowerInsideColor01;Flower Inside Color 01;11;0;Create;True;0;0;False;0;1,0.7789562,0.1273585,0;1,0.7789562,0.1273583,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;209;-2071.436,138.7519;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.73;False;2;FLOAT;0.75;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-1944.723,997.8702;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;192;-1757.406,50.76353;Float;False;Property;_FlowerInsideColor02;Flower Inside Color 02;13;0;Create;True;0;0;False;0;1,0.9507267,0.6084906,0;1,0.9507267,0.6084906,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;136;-1815.115,776.2373;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;155;-1973.048,-890.3631;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-2013.001,652.6562;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;193;-2047.968,-96.57793;Float;False;Property;_FlowerMainColor01;Flower Main Color 01;10;0;Create;True;0;0;False;0;1,0.9637499,0.759434,0;1,0.9637498,0.759434,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;188;-2049.1,-272.8226;Float;False;Property;_FlowerMainColor02;Flower Main Color 02;12;0;Create;True;0;0;False;0;1,0.703345,0.1556604,0;1,0.7033451,0.1556602,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;93;-2262.257,1405.227;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;2,2;False;1;FLOAT;0.1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalVertexDataNode;97;-1906.772,1772.697;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;195;-1469.011,-639.5245;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;185;-2141.88,-505.1411;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;9bf606c43682e9d488ea2aa0c944418f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;196;-1491.105,245.2079;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;194;-1748.592,-113.862;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-1571.543,666.9012;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;101;-1876.346,1396.915;Float;True;Simplex2D;1;0;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-1902.388,1653.194;Float;False;Property;_WindFoliageAmplitude;Wind Foliage Amplitude;2;0;Create;True;0;0;False;0;0;0.073;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;132;-1894.1,1909.365;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;-1618.626,1011.995;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;198;-1270.394,-484.4413;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;197;-1295.706,-219.1545;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;199;-1289.545,-366.302;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;211;-940.4134,-1031.312;Float;False;663;247;Do you want the normals to always be up ? PLug in Local Vertex Normal port;2;202;201;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;141;-1272.002,921.8229;Float;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-1421.867,1629.836;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;142;-710.1516,1070.944;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;200;-879.2684,-480.3698;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;157;-528.6384,-246.9677;Float;False;Constant;_Null;Null;15;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;201;-617.9302,-975.4223;Float;False;World;Object;True;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;202;-804.9299,-974.4223;Float;False;Constant;_Vector0;Vector 0;14;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-317.1068,-379.97;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;SNB_Nature/SNB_Grass;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.75;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;4;Pragma;multi_compile _ LOD_FADE_CROSSFADE;False;;Pragma;instancing_options procedural:setupScale;False;;Pragma;multi_compile GPU_FRUSTUM_ON__;False;;Include;VS_indirect.cginc;False;;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;148;1;131;0
WireConnection;178;0;151;0
WireConnection;130;0;129;0
WireConnection;130;1;148;0
WireConnection;152;0;150;2
WireConnection;152;1;178;0
WireConnection;133;0;130;0
WireConnection;140;0;130;0
WireConnection;95;0;94;0
WireConnection;95;1;96;0
WireConnection;91;0;85;1
WireConnection;91;1;85;2
WireConnection;91;2;85;3
WireConnection;139;0;143;0
WireConnection;154;0;152;0
WireConnection;154;1;153;0
WireConnection;210;0;203;2
WireConnection;209;0;210;0
WireConnection;138;0;139;0
WireConnection;138;1;140;0
WireConnection;155;0;154;0
WireConnection;134;0;133;0
WireConnection;134;1;143;0
WireConnection;93;0;91;0
WireConnection;93;1;95;0
WireConnection;195;0;191;0
WireConnection;195;1;187;0
WireConnection;195;2;155;0
WireConnection;196;0;192;0
WireConnection;196;1;189;0
WireConnection;196;2;209;0
WireConnection;194;0;188;0
WireConnection;194;1;193;0
WireConnection;194;2;209;0
WireConnection;135;0;134;0
WireConnection;135;1;136;3
WireConnection;101;0;93;0
WireConnection;137;0;136;3
WireConnection;137;1;138;0
WireConnection;198;0;195;0
WireConnection;198;1;185;2
WireConnection;197;1;196;0
WireConnection;197;2;185;1
WireConnection;199;1;194;0
WireConnection;199;2;185;3
WireConnection;141;0;135;0
WireConnection;141;2;137;0
WireConnection;99;0;101;0
WireConnection;99;1;98;0
WireConnection;99;2;97;0
WireConnection;99;3;132;1
WireConnection;142;0;141;0
WireConnection;142;1;99;0
WireConnection;200;0;198;0
WireConnection;200;1;199;0
WireConnection;200;2;197;0
WireConnection;201;0;202;0
WireConnection;0;0;200;0
WireConnection;0;4;157;0
WireConnection;0;10;185;4
WireConnection;0;11;142;0
ASEEND*/
//CHKSM=8F6CDEA47BCAC8605C0BC7D64676AD2AB123B991